using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DomainDrivenGameEngine.Media.Models;

namespace DomainDrivenGameEngine.Media.Services
{
    /// <summary>
    /// A service for loading referenced media for a specific implementation.
    /// </summary>
    /// <typeparam name="TMedia">The type of media this service loads into an implementation.</typeparam>
    /// <typeparam name="TMediaImplementation">The type of implementation that is loaded.</typeparam>
    public class MediaLoadingService<TMedia, TMediaImplementation> : IMediaLoadingService<TMedia, TMediaImplementation>, IDisposable
        where TMedia : class, IMedia
        where TMediaImplementation : class, IMediaImplementation<TMedia>
    {
        /// <summary>
        /// The service to use for accessing files.
        /// </summary>
        private readonly IFileAccessService _fileAccessService;

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
        /// The service which generates the final implementation of loaded media.
        /// </summary>
        private readonly IMediaImplementationService<TMedia, TMediaImplementation> _mediaImplementationService;

        /// <summary>
        /// The services to use for sourcing referenced media.
        /// </summary>
        private readonly IReadOnlyCollection<IMediaSourceService<TMedia>> _mediaSourceServices;

        /// <summary>
        /// Implementations that are no longer referenced and need to be unloaded.
        /// </summary>
        private readonly List<TMediaImplementation> _oldImplementations;

        /// <summary>
        /// A lookup of reference counts by reference ID.
        /// </summary>
        private readonly IDictionary<int, int> _referenceCountsByReferenceId;

        /// <summary>
        /// A lookup of previously referenced media by their joined paths.
        /// </summary>
        private readonly IDictionary<string, MediaFileReference<TMedia>> _referencesByJoinedPaths;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaLoadingService{TMedia, TMediaImplementation}"/> class.
        /// </summary>
        /// <param name="mediaSourceServices">The services to use for sourcing referenced media.</param>
        /// <param name="mediaImplementationService">The service which generates the final implementation of loaded media.</param>
        /// <param name="fileAccessService">The service to use for accessing files.</param>
        protected MediaLoadingService(IReadOnlyCollection<IMediaSourceService<TMedia>> mediaSourceServices,
                                      IMediaImplementationService<TMedia, TMediaImplementation> mediaImplementationService,
                                      IFileAccessService fileAccessService)
        {
            _mediaSourceServices = mediaSourceServices ?? throw new ArgumentNullException(nameof(mediaSourceServices));
            _mediaImplementationService = mediaImplementationService ?? throw new ArgumentNullException(nameof(mediaImplementationService));
            _fileAccessService = fileAccessService ?? throw new ArgumentNullException(nameof(fileAccessService));
            _inProgressTaskCancellationTokenSources = new Dictionary<int, CancellationTokenSource>();
            _inProgressTasks = new Dictionary<int, Task<KeyValuePair<string, TMedia>[]>>();
            _loadedImplementationsByReferenceId = new Dictionary<int, TMediaImplementation>();
            _oldImplementations = new List<TMediaImplementation>();
            _referenceCountsByReferenceId = new Dictionary<int, int>();
            _referencesByJoinedPaths = new Dictionary<string, MediaFileReference<TMedia>>();
        }

        /// <summary>
        /// Disposes this service.
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var oldImplementation in _oldImplementations)
            {
                _mediaImplementationService.UnloadImplementation(oldImplementation);
            }

            foreach (var loadedImplementationByKvp in _loadedImplementationsByReferenceId)
            {
                _mediaImplementationService.UnloadImplementation(loadedImplementationByKvp.Value);
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
            _oldImplementations.Clear();
            _referenceCountsByReferenceId.Clear();
            _referencesByJoinedPaths.Clear();
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
        /// Handles processing any loaded media into their final implementations, as well as cleaning up any unloaded implementations.
        /// </summary>
        /// <returns><c>true</c> if any media was processed.</returns>
        public bool ProcessMediaImplementations()
        {
            var mediaImplementationsProcessed = false;
            var finishedReferenceIds = new List<int>();
            foreach (var inProgressTaskKvp in _inProgressTasks)
            {
                if (inProgressTaskKvp.Value.IsCompleted)
                {
                    var loadedMediaKvps = inProgressTaskKvp.Value.GetAwaiter().GetResult();
                    var loadedPaths = loadedMediaKvps.Select(kvp => kvp.Key).Where(path => !string.IsNullOrWhiteSpace(path)).ToArray();
                    var loadedMedia = loadedMediaKvps.Select(kvp => kvp.Value).ToArray();
                    var loadedImplementation = _mediaImplementationService.LoadImplementation(loadedMedia, loadedPaths.Length > 0 ? loadedPaths : null);
                    _loadedImplementationsByReferenceId[inProgressTaskKvp.Key] = loadedImplementation;
                    finishedReferenceIds.Add(inProgressTaskKvp.Key);
                    mediaImplementationsProcessed = true;
                }
            }

            foreach (var finishedReferenceId in finishedReferenceIds)
            {
                _inProgressTasks.Remove(finishedReferenceId);
            }

            foreach (var oldImplementation in _oldImplementations)
            {
                _mediaImplementationService.UnloadImplementation(oldImplementation);
                mediaImplementationsProcessed = true;
            }

            _oldImplementations.Clear();

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

            if (!_mediaImplementationService.IsMediaCountSupported((uint)paths.Length))
            {
                throw new ArgumentException($"Attempting to reference an unexpected number of {nameof(paths)}: {paths.Length}.");
            }

            var fullyQualifiedPaths = paths.Select(p => _fileAccessService.GetFullyQualifiedPath(p)).ToArray();
            var joinedPaths = MediaFileReference<TMedia>.GetJoinedReferencePaths(fullyQualifiedPaths);
            if (_referencesByJoinedPaths.TryGetValue(joinedPaths, out var existingReference))
            {
                _referenceCountsByReferenceId[existingReference.Id]++;
                return existingReference;
            }

            var reference = new MediaFileReference<TMedia>(fullyQualifiedPaths);
            _referencesByJoinedPaths.Add(joinedPaths, reference);
            _referenceCountsByReferenceId.Add(reference.Id, 1);

            var cancellationTokenSource = new CancellationTokenSource();

            var tasks = new List<Task<KeyValuePair<string, TMedia>>>();
            foreach (var path in fullyQualifiedPaths)
            {
                var extension = _fileAccessService.GetFileExtension(path);
                var source = _mediaSourceServices.FirstOrDefault(mss => mss.IsExtensionSupported(extension));

                tasks.Add(Task.Run(() =>
                {
                    if (!_fileAccessService.DoesFileExist(path))
                    {
                        var exception = new Exception($"File does not exist.");
                        exception.Data["Path"] = path;
                        throw exception;
                    }

                    var fileStream = _fileAccessService.OpenFile(path);

                    try
                    {
                        return new KeyValuePair<string, TMedia>(path, source.Load(fileStream, path, extension));
                    }
                    finally
                    {
                        if (!_mediaImplementationService.IsSourceStreamRequired)
                        {
                            fileStream.Dispose();
                        }
                    }
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

            if (!_mediaImplementationService.IsMediaCountSupported((uint)media.Length))
            {
                throw new ArgumentException($"Attempting to reference an unexpected number of {nameof(media)}: {media.Length}.");
            }

            var reference = new MediaReference<TMedia>();
            _referenceCountsByReferenceId.Add(reference.Id, 1);

            // Load the media as a task so that they can be processed at the same time as every other piece of media.
            var wrapperTask = Task.FromResult(media.Select(m => new KeyValuePair<string, TMedia>(string.Empty, m)).ToArray());

            _inProgressTasks.Add(reference.Id, wrapperTask);

            return reference;
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

            if (!_referenceCountsByReferenceId.TryGetValue(reference.Id, out var currentReferenceCount))
            {
                throw new ArgumentException($"Reference {reference.Id} is no longer being tracked by this {nameof(IMediaReferenceService<TMedia>)} and cannot be unreferenced");
            }

            var newReferenceCount = currentReferenceCount - 1;
            if (newReferenceCount == 0)
            {
                var fileReference = reference as IMediaFileReference<TMedia>;
                if (fileReference != null)
                {
                    _referencesByJoinedPaths.Remove(fileReference.GetJoinedPaths());
                }

                _referenceCountsByReferenceId.Remove(reference.Id);

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

                return;
            }

            _referenceCountsByReferenceId[reference.Id] = newReferenceCount;
        }
    }
}
