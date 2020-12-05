using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DomainDrivenGameEngine.Media.IO;
using DomainDrivenGameEngine.Media.Loaders;
using DomainDrivenGameEngine.Media.Models;
using DomainDrivenGameEngine.Media.Readers;

namespace DomainDrivenGameEngine.Media.Services
{
    /// <summary>
    /// A service for referencing and loading media.
    /// </summary>
    /// <typeparam name="TMedia">The type of media this service loads into an implementation.</typeparam>
    /// <typeparam name="TMediaImplementation">The type of implementation that is loaded.</typeparam>
    public class MediaService<TMedia, TMediaImplementation> : IMediaStorageService<TMedia, TMediaImplementation>, IMediaReferenceLoadingService, IDisposable
        where TMedia : class, IMedia
        where TMediaImplementation : class, IMediaImplementation<TMedia>
    {
        /// <summary>
        /// The <see cref="IFileSystem"/> to use for accessing the file system.
        /// </summary>
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// A lookup of in-progress task cancellation token sources by reference ID.
        /// </summary>
        private readonly IDictionary<int, CancellationTokenSource> _inProgressTaskCancellationTokenSources;

        /// <summary>
        /// A lookup of in-progress tasks by reference ID, where each task is expected to return a KVP containing the path and media.
        /// </summary>
        private readonly IDictionary<int, Task<KeyValuePair<string, TMedia>[]>> _inProgressTasks;

        /// <summary>
        /// A lookup of loaded implementations by reference ID.
        /// </summary>
        private readonly IDictionary<int, TMediaImplementation> _loadedImplementationsByReferenceId;

        /// <summary>
        /// The loader which which loads and unloads the final implementation of read media.
        /// </summary>
        private readonly IMediaLoader<TMedia, TMediaImplementation> _mediaLoader;

        /// <summary>
        /// The collection of <see cref="IMediaReader{TMedia}"/> for reading media.
        /// </summary>
        private readonly IReadOnlyList<IMediaReader<TMedia>> _mediaReaders;

        /// <summary>
        /// Implementations that are no longer referenced and need to be unloaded.
        /// </summary>
        private readonly List<TMediaImplementation> _oldImplementations;

        /// <summary>
        /// A lookup of references by ID.
        /// </summary>
        private readonly IDictionary<int, MediaReference<TMedia>> _referencesById;

        /// <summary>
        /// A lookup of previously referenced media by their joined paths.
        /// </summary>
        private readonly IDictionary<string, MediaFileReference<TMedia>> _referencesByJoinedPaths;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaService{TMedia, TMediaImplementation}"/> class.
        /// </summary>
        /// <param name="mediaReaders">The collection of <see cref="IMediaReader{TMedia}"/> for reading media.</param>
        /// <param name="mediaLoader">The loader which which loads and unloads the final implementation of read media.</param>
        /// <param name="fileSystem">The <see cref="IFileSystem"/> to use for accessing the file system.</param>
        public MediaService(IMediaReader<TMedia>[] mediaReaders,
                            IMediaLoader<TMedia, TMediaImplementation> mediaLoader,
                            IFileSystem fileSystem)
        {
            _mediaReaders = mediaReaders ?? throw new ArgumentNullException(nameof(mediaReaders));
            _mediaLoader = mediaLoader ?? throw new ArgumentNullException(nameof(mediaLoader));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _inProgressTaskCancellationTokenSources = new Dictionary<int, CancellationTokenSource>();
            _inProgressTasks = new Dictionary<int, Task<KeyValuePair<string, TMedia>[]>>();
            _loadedImplementationsByReferenceId = new Dictionary<int, TMediaImplementation>();
            _oldImplementations = new List<TMediaImplementation>();
            _referencesById = new Dictionary<int, MediaReference<TMedia>>();
            _referencesByJoinedPaths = new Dictionary<string, MediaFileReference<TMedia>>();
        }

        /// <summary>
        /// Checks to see if this service has any media that is no longer being referenced that can be unloaded.
        /// </summary>
        /// <returns><c>true</c> if media can be unloaded.</returns>
        public bool CanUnloadMedia()
        {
            return _oldImplementations.Count > 0;
        }

        /// <summary>
        /// Disposes this service.
        /// </summary>
        public virtual void Dispose()
        {
            Reset();
        }

        /// <summary>
        /// Gets the implementation for the given reference.
        /// </summary>
        /// <param name="reference">The <see cref="IMediaReference{TMedia}"/> to get the implementation for.</param>
        /// <returns>The implementation, or null if the implementation is not ready for use yet.</returns>
        public TMediaImplementation GetImplementation(IMediaReference<TMedia> reference)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            if (!_loadedImplementationsByReferenceId.TryGetValue(reference.Id, out var implementation))
            {
                return null;
            }

            return implementation;
        }

        /// <summary>
        /// Checks to see if this service is currently loading any media.
        /// </summary>
        /// <returns><c>true</c> if the service is currently loading media.</returns>
        public bool IsLoadingMedia()
        {
            return _inProgressTasks.Count > 0;
        }

        /// <summary>
        /// Handles loading any media that has finished being read into their final implementations.
        /// </summary>
        /// <returns><c>true</c> if any media was loaded.</returns>
        public bool LoadReferencedMedia()
        {
            var mediaImplementationsProcessed = false;
            var finishedReferenceIds = new List<int>();
            foreach (var inProgressTaskKvp in _inProgressTasks)
            {
                if (inProgressTaskKvp.Value.IsCompleted)
                {
                    var readMediaKvp = inProgressTaskKvp.Value.GetAwaiter().GetResult();
                    var readPaths = readMediaKvp.Select(kvp => kvp.Key).Where(path => !string.IsNullOrWhiteSpace(path)).ToArray();
                    var readMedia = readMediaKvp.Select(kvp => kvp.Value).ToArray();

                    _loadedImplementationsByReferenceId[inProgressTaskKvp.Key] = LoadMediaImplementation(readMedia, readPaths);

                    finishedReferenceIds.Add(inProgressTaskKvp.Key);
                    mediaImplementationsProcessed = true;
                }
            }

            foreach (var finishedReferenceId in finishedReferenceIds)
            {
                _inProgressTasks.Remove(finishedReferenceId);
            }

            return mediaImplementationsProcessed;
        }

        /// <summary>
        /// References a piece of media.  If this is the first time referencing a given piece of media, lists the reference to be loaded.
        /// </summary>
        /// <param name="paths">One or more strings containing the file paths to reference.</param>
        /// <returns>A <see cref="IMediaFileReference{TMedia}"/> object which refers to the media at the specified paths.</returns>
        public IMediaFileReference<TMedia> Reference(params string[] paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException(nameof(paths));
            }

            if (!_mediaLoader.IsMediaCountSupported((uint)paths.Length))
            {
                throw new ArgumentException($"Attempting to reference an unexpected number of {nameof(paths)}: {paths.Length}.");
            }

            var fullyQualifiedPaths = paths.Select(p => _fileSystem.GetFullyQualifiedPath(p)).ToArray();
            var joinedPaths = MediaFileReference<TMedia>.GetJoinedReferencePaths(fullyQualifiedPaths);
            if (_referencesByJoinedPaths.TryGetValue(joinedPaths, out var existingReference))
            {
                existingReference.ReferenceCount++;
                return existingReference;
            }

            var reference = new MediaFileReference<TMedia>(new ReadOnlyCollection<string>(fullyQualifiedPaths));
            _referencesByJoinedPaths.Add(joinedPaths, reference);
            _referencesById.Add(reference.Id, reference);

            var cancellationTokenSource = new CancellationTokenSource();

            var tasks = new List<Task<KeyValuePair<string, TMedia>>>();
            foreach (var path in fullyQualifiedPaths)
            {
                var extension = _fileSystem.GetFileExtension(path);
                var reader = _mediaReaders.FirstOrDefault(mss => mss.IsExtensionSupported(extension));

                tasks.Add(Task.Run(() =>
                {
                    if (!_fileSystem.DoesFileExist(path))
                    {
                        var exception = new Exception($"File does not exist.");
                        exception.Data["Path"] = path;
                        throw exception;
                    }

                    var fileStream = _fileSystem.OpenFile(path);

                    return new KeyValuePair<string, TMedia>(path, reader.Read(fileStream, path, extension));
                }, cancellationTokenSource.Token));
            }

            var wrapperTask = Task.WhenAll(tasks);

            _inProgressTaskCancellationTokenSources.Add(reference.Id, cancellationTokenSource);
            _inProgressTasks.Add(reference.Id, wrapperTask);

            return reference;
        }

        /// <summary>
        /// References a provided piece of media and lists it to be loaded.
        /// </summary>
        /// <param name="media">The media to reference.</param>
        /// <returns>A <see cref="IMediaReference{TMedia}"/> object which refers to the media.</returns>
        public IMediaReference<TMedia> Reference(params TMedia[] media)
        {
            if (media == null)
            {
                throw new ArgumentNullException(nameof(media));
            }

            if (!_mediaLoader.IsMediaCountSupported((uint)media.Length))
            {
                throw new ArgumentException($"Attempting to reference an unexpected number of {nameof(media)}: {media.Length}.");
            }

            var reference = new MediaReference<TMedia>();
            _referencesById.Add(reference.Id, reference);

            _loadedImplementationsByReferenceId[reference.Id] = LoadMediaImplementation(media);

            return reference;
        }

        /// <summary>
        /// Stops all in-progress media being loaded and unloads all loaded media.
        /// </summary>
        public void Reset()
        {
            UnloadUnreferencedMedia();

            foreach (var loadedImplementationByKvp in _loadedImplementationsByReferenceId)
            {
                _mediaLoader.Unload(loadedImplementationByKvp.Value);
            }

            foreach (var inProgressTaskKvp in _inProgressTasks)
            {
                if (_inProgressTaskCancellationTokenSources.TryGetValue(inProgressTaskKvp.Key, out var cancellationTokenSource) && cancellationTokenSource.Token.CanBeCanceled)
                {
                    cancellationTokenSource.Cancel();
                }
            }

            _inProgressTaskCancellationTokenSources.Clear();
            _inProgressTasks.Clear();
            _loadedImplementationsByReferenceId.Clear();
            _referencesById.Clear();
            _referencesByJoinedPaths.Clear();
        }

        /// <summary>
        /// Tries to get the implementation for the given reference.
        /// </summary>
        /// <param name="reference">The <see cref="IMediaReference{TMedia}"/> to get the implementation for.</param>
        /// <param name="output">The output implementation, or <c>null</c> if the implementation is not ready for use yet.</param>
        /// <returns><c>true</c> if the implementation is ready for use.</returns>
        public bool TryGetImplementation(IMediaReference<TMedia> reference, out TMediaImplementation output)
        {
            output = GetImplementation(reference);
            return output != null;
        }

        /// <summary>
        /// Handles unloading any media that is no longer being referenced.
        /// </summary>
        /// <returns><c>true</c> if any media was unloaded.</returns>
        public bool UnloadUnreferencedMedia()
        {
            var mediaUnloaded = _oldImplementations.Count > 0;

            foreach (var oldImplementation in _oldImplementations)
            {
                _mediaLoader.Unload(oldImplementation);
            }

            _oldImplementations.Clear();

            return mediaUnloaded;
        }

        /// <summary>
        /// Unreferences a previously retrieved reference.  If no references remain, lists the reference to be unloaded.
        /// </summary>
        /// <param name="reference">The <see cref="IMediaReference{TMedia}"/> to unreference.</param>
        public void Unreference(IMediaReference<TMedia> reference)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            if (!_referencesById.TryGetValue(reference.Id, out var currentReference))
            {
                throw new ArgumentException($"Reference {reference.Id} is not being tracked by this {nameof(IMediaReferenceService<TMedia>)} and cannot be unreferenced.");
            }

            currentReference.ReferenceCount--;

            if (currentReference.ReferenceCount > 0)
            {
                return;
            }

            var fileReference = reference as IMediaFileReference<TMedia>;
            if (fileReference != null)
            {
                _referencesByJoinedPaths.Remove(fileReference.GetJoinedPaths());
            }

            _referencesById.Remove(reference.Id);

            if (_inProgressTasks.TryGetValue(reference.Id, out var inProgressTasks) &&
                _inProgressTaskCancellationTokenSources.TryGetValue(reference.Id, out var cancellationTokenSource) &&
                cancellationTokenSource.Token.CanBeCanceled)
            {
                cancellationTokenSource.Cancel();
                _inProgressTaskCancellationTokenSources.Remove(reference.Id);
                _inProgressTasks.Remove(reference.Id);
            }

            if (_loadedImplementationsByReferenceId.TryGetValue(reference.Id, out var oldImplementation))
            {
                _oldImplementations.Add(oldImplementation);
                _loadedImplementationsByReferenceId.Remove(reference.Id);
            }
        }

        /// <summary>
        /// Loads a piece of media and disposes of it afterwards if necessary.
        /// </summary>
        /// <param name="media">The media to load.</param>
        /// <param name="paths">The paths to the media that was read.</param>
        /// <returns>The loaded implementation.</returns>
        private TMediaImplementation LoadMediaImplementation(TMedia[] media, string[] paths = null)
        {
            var loadedImplementation = _mediaLoader.Load(media, paths != null && paths.Length > 0 ? paths : null);

            if (!_mediaLoader.IsSourceMediaRequired)
            {
                foreach (var mediaItem in media)
                {
                    mediaItem.Dispose();
                }
            }

            return loadedImplementation;
        }
    }
}
