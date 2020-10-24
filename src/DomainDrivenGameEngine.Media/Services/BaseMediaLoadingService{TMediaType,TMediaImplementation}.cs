using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DomainDrivenGameEngine.Media.Models;

namespace DomainDrivenGameEngine.Media.Services
{
    /// <summary>
    /// A service for loading referenced media for a specific implementation.
    /// </summary>
    /// <typeparam name="TMediaType">The type of media this service loads into an implementation.</typeparam>
    /// <typeparam name="TMediaImplementation">The type of implementation that is loaded.</typeparam>
    public abstract class BaseMediaLoadingService<TMediaType, TMediaImplementation> : IMediaLoadingService<TMediaType, TMediaImplementation>, IDisposable
        where TMediaType : class, IMedia
        where TMediaImplementation : class, IMediaImplementation
    {
        /// <summary>
        /// Gets the number of paths and/or media this loading service expects to receive when a caller references it.
        /// </summary>
        private readonly HashSet<uint> _expectedCountLookup;

        /// <summary>
        /// A lookup of in-progress task cancellation token sources by reference ID.
        /// </summary>
        private readonly IDictionary<int, CancellationTokenSource> _inProgressTaskCancellationTokenSources;

        /// <summary>
        /// A lookup of in-progress tasks by reference ID.
        /// </summary>
        private readonly IDictionary<int, Task<TMediaType[]>> _inProgressTasks;

        /// <summary>
        /// A lookup of loaded implementations by reference ID.
        /// </summary>
        private readonly IDictionary<int, TMediaImplementation> _loadedImplementationsByReferenceId;

        /// <summary>
        /// The services to use for sourcing referenced media.
        /// </summary>
        private readonly IReadOnlyCollection<IMediaSourceService<TMediaType>> _mediaSourceServices;

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
        private readonly IDictionary<string, MediaFileReference<TMediaType>> _referencesByJoinedPaths;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMediaLoadingService{TMediaType, TMediaImplementation}"/> class.
        /// </summary>
        /// <param name="mediaSourceServices">The services to use for sourcing referenced media.</param>
        /// <param name="supportedPathCounts">The number of path counts that this service can support loading.</param>
        protected BaseMediaLoadingService(IReadOnlyCollection<IMediaSourceService<TMediaType>> mediaSourceServices, IReadOnlyCollection<uint> supportedPathCounts = null)
        {
            _mediaSourceServices = mediaSourceServices ?? throw new ArgumentNullException(nameof(mediaSourceServices));
            _expectedCountLookup = (supportedPathCounts ?? new uint[] { 1 }).ToHashSet();
            _inProgressTaskCancellationTokenSources = new Dictionary<int, CancellationTokenSource>();
            _inProgressTasks = new Dictionary<int, Task<TMediaType[]>>();
            _loadedImplementationsByReferenceId = new Dictionary<int, TMediaImplementation>();
            _oldImplementations = new List<TMediaImplementation>();
            _referenceCountsByReferenceId = new Dictionary<int, int>();
            _referencesByJoinedPaths = new Dictionary<string, MediaFileReference<TMediaType>>();
        }

        /// <summary>
        /// Disposes this service.
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var oldImplementation in _oldImplementations)
            {
                UnloadImplementation(oldImplementation);
            }

            foreach (var loadedImplementationByKvp in _loadedImplementationsByReferenceId)
            {
                UnloadImplementation(loadedImplementationByKvp.Value);
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
        /// <param name="reference">The <see cref="IMediaReference{TMediaType}"/> to get the implementation for.</param>
        /// <returns>The implementation, or null if the implementation is not ready for use yet.</returns>
        public TMediaImplementation GetImplementation(IMediaReference<TMediaType> reference)
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
                    var loadedMedia = inProgressTaskKvp.Value.GetAwaiter().GetResult();
                    _loadedImplementationsByReferenceId[inProgressTaskKvp.Key] = LoadImplementation(media: loadedMedia);
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
                UnloadImplementation(oldImplementation);
                mediaImplementationsProcessed = true;
            }

            _oldImplementations.Clear();

            return mediaImplementationsProcessed;
        }

        /// <summary>
        /// References a piece of media.  If this is the first time referencing a given piece of media, lists the reference to be loaded.
        /// </summary>
        /// <param name="paths">One or more strings containing the file paths to reference.</param>
        /// <returns>A <see cref="IMediaFileReference{TMediaType}"/> object which refers to the media at the specified paths.</returns>
        public IMediaFileReference<TMediaType> Reference(params string[] paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException(nameof(paths));
            }

            if (!_expectedCountLookup.Contains((uint)paths.Length))
            {
                throw new ArgumentException($"Attempting to reference an unexpected number of {nameof(paths)}: {paths.Length}.");
            }

            var joinedPaths = MediaFileReference<TMediaType>.GetJoinedReferencePaths(paths);
            if (_referencesByJoinedPaths.TryGetValue(joinedPaths, out var existingReference))
            {
                _referenceCountsByReferenceId[existingReference.Id]++;
                return existingReference;
            }

            var reference = new MediaFileReference<TMediaType>(paths);
            _referencesByJoinedPaths.Add(joinedPaths, reference);
            _referenceCountsByReferenceId.Add(reference.Id, 1);

            var cancellationTokenSource = new CancellationTokenSource();

            var tasks = new List<Task<TMediaType>>();
            foreach (var path in paths)
            {
                var extension = Path.GetExtension(path);
                var source = _mediaSourceServices.FirstOrDefault(mss => mss.IsExtensionSupported(extension));

                tasks.Add(Task.Run(() =>
                {
                    return source.Load(path);
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
        /// <returns>A <see cref="IMediaReference{TMediaType}"/> object which refers to the media.</returns>
        public IMediaReference<TMediaType> Reference(params TMediaType[] media)
        {
            if (media == null)
            {
                throw new ArgumentNullException(nameof(media));
            }

            if (!_expectedCountLookup.Contains((uint)media.Length))
            {
                throw new ArgumentException($"Attempting to reference an unexpected number of {nameof(media)}: {media.Length}.");
            }

            var reference = new MediaReference<TMediaType>();
            _referenceCountsByReferenceId.Add(reference.Id, 1);

            // Load the media as a task so that they can be processed at the same time as every other piece of media.
            var wrapperTask = Task.FromResult(media);

            _inProgressTasks.Add(reference.Id, wrapperTask);

            return reference;
        }

        /// <summary>
        /// Unreferences a previously retrieved reference.  If no references remain, lists the reference to be unloaded.
        /// </summary>
        /// <param name="reference">The <see cref="IMediaReference{TMediaType}"/> to unreference.</param>
        public void Unreference(IMediaReference<TMediaType> reference)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            if (!_referenceCountsByReferenceId.TryGetValue(reference.Id, out var currentReferenceCount))
            {
                throw new ArgumentException($"Reference {reference.Id} is no longer being tracked by this {nameof(IMediaReferenceService<TMediaType>)} and cannot be unreferenced");
            }

            var newReferenceCount = currentReferenceCount - 1;
            if (newReferenceCount == 0)
            {
                var fileReference = reference as IMediaFileReference<TMediaType>;
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

        /// <summary>
        /// Loads the sourced media into the specific implementation.
        /// </summary>
        /// <param name="media">The loaded media.</param>
        /// <returns>The processed implementation.</returns>
        protected abstract TMediaImplementation LoadImplementation(params TMediaType[] media);

        /// <summary>
        /// Unloads a previously loaded implementation.
        /// </summary>
        /// <param name="implementation">The implementation to unload.</param>
        protected abstract void UnloadImplementation(TMediaImplementation implementation);
    }
}
