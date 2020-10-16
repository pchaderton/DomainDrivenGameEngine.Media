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
        /// A lookup of in-progress task cancellation token sources by reference ID.
        /// </summary>
        private readonly IDictionary<int, CancellationTokenSource> _inProgressTaskCancellationTokenSources;

        /// <summary>
        /// A lookup of in-progress tasks by reference ID.
        /// </summary>
        private readonly IDictionary<int, IReadOnlyCollection<Task<TMediaType>>> _inProgressTasks;

        /// <summary>
        /// A lookup of loaded implementations by reference ID.
        /// </summary>
        private readonly IDictionary<int, TMediaImplementation> _loadedImplementationsByReferenceId;

        /// <summary>
        /// The services to use for sourcing referenced media.
        /// </summary>
        private readonly IReadOnlyCollection<IMediaSourceService<TMediaType>> _mediaSourceServices;

        /// <summary>
        /// A lookup of reference counts by reference ID.
        /// </summary>
        private readonly IDictionary<int, int> _referenceCountsByReferenceId;

        /// <summary>
        /// A lookup of previously referenced media by their joined paths.
        /// </summary>
        private readonly IDictionary<string, Reference<TMediaType>> _referencesByJoinedPaths;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMediaLoadingService{TMediaType, TMediaImplementation}"/> class.
        /// </summary>
        /// <param name="mediaSourceServices">The services to use for sourcing referenced media.</param>
        protected BaseMediaLoadingService(IReadOnlyCollection<IMediaSourceService<TMediaType>> mediaSourceServices)
        {
            _mediaSourceServices = mediaSourceServices ?? throw new ArgumentNullException(nameof(mediaSourceServices));
            _inProgressTaskCancellationTokenSources = new Dictionary<int, CancellationTokenSource>();
            _inProgressTasks = new Dictionary<int, IReadOnlyCollection<Task<TMediaType>>>();
            _loadedImplementationsByReferenceId = new Dictionary<int, TMediaImplementation>();
            _referenceCountsByReferenceId = new Dictionary<int, int>();
            _referencesByJoinedPaths = new Dictionary<string, Reference<TMediaType>>();
        }

        /// <summary>
        /// Gets the number of paths this loading service expects to receive when a caller references a piece of media.
        /// </summary>
        protected abstract int ExpectedPathCount { get; }

        /// <summary>
        /// Disposes this service.
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var loadedImplementationByKvp in _loadedImplementationsByReferenceId)
            {
                loadedImplementationByKvp.Value.Dispose();
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
            _referenceCountsByReferenceId.Clear();
            _referencesByJoinedPaths.Clear();
        }

        /// <summary>
        /// Gets the implementation for the given reference.
        /// </summary>
        /// <param name="reference">The <see cref="IReference{TMediaType}"/> to get the implementation for.</param>
        /// <returns>The implementation, or null if the implementation is not ready for use yet.</returns>
        public TMediaImplementation GetImplementation(IReference<TMediaType> reference)
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
        /// Handles processing any loaded media into their final implementations.
        /// </summary>
        public void ProcessLoadedMedia()
        {
            var finishedReferenceIds = new List<int>();
            foreach (var inProgressTaskKvp in _inProgressTasks)
            {
                if (inProgressTaskKvp.Value.All(t => t.IsCompleted))
                {
                    var loadedMedia = inProgressTaskKvp.Value.Select(t => t.GetAwaiter().GetResult()).ToArray();
                    _loadedImplementationsByReferenceId[inProgressTaskKvp.Key] = ProcessMedia(media: loadedMedia);
                    finishedReferenceIds.Add(inProgressTaskKvp.Key);
                }
            }

            foreach (var finishedReferenceId in finishedReferenceIds)
            {
                _inProgressTasks.Remove(finishedReferenceId);
            }
        }

        /// <summary>
        /// References a piece of media.  If this is the first time referencing a given piece of media, lists the reference to be loaded.
        /// </summary>
        /// <param name="paths">One or more strings containing the file paths to reference.</param>
        /// <returns>A <see cref="Reference{TMediaType}"/> object which refers to the media at the specified paths.</returns>
        public IReference<TMediaType> Reference(params string[] paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException(nameof(paths));
            }

            if (paths.Length == ExpectedPathCount)
            {
                throw new ArgumentException($"Expected {ExpectedPathCount} {nameof(paths)}, but only received {paths.Length}.");
            }

            var joinedPaths = Reference<TMediaType>.GetJoinedReferencePaths(paths);
            if (_referencesByJoinedPaths.TryGetValue(joinedPaths, out var existingReference))
            {
                _referenceCountsByReferenceId[existingReference.Id]++;
                return existingReference;
            }

            var reference = new Reference<TMediaType>(paths);
            _referencesByJoinedPaths.Add(joinedPaths, reference);
            _referenceCountsByReferenceId.Add(reference.Id, 1);

            var cancellationTokenSource = new CancellationTokenSource();

            var tasks = new List<Task<TMediaType>>();
            foreach (var path in paths)
            {
                var extension = Path.GetExtension(path);
                var source = _mediaSourceServices.FirstOrDefault(mss => mss.IsExtensionSupported(extension));

                // TODO - Add and store cancellation token.
                tasks.Add(Task.Run(() =>
                {
                    return source.Load(path);
                }, cancellationTokenSource.Token));
            }

            _inProgressTaskCancellationTokenSources.Add(reference.Id, cancellationTokenSource);
            _inProgressTasks.Add(reference.Id, tasks);

            return reference;
        }

        /// <summary>
        /// References a provided piece of media and lists it to be loaded.
        /// </summary>
        /// <param name="media">The media to reference.</param>
        /// <returns>A <see cref="Reference{TMediaType}"/> object which refers to the media.</returns>
        public IReference<TMediaType> Reference(TMediaType media)
        {
            if (media == null)
            {
                throw new ArgumentNullException(nameof(media));
            }

            var reference = new Reference<TMediaType>(new string[] { });
            _referenceCountsByReferenceId.Add(reference.Id, 1);

            _loadedImplementationsByReferenceId[reference.Id] = ProcessMedia(media);

            return reference;
        }

        /// <summary>
        /// Unreferences a previously retrieved reference.  If no references remain, lists the reference to be unloaded.
        /// </summary>
        /// <param name="reference">The <see cref="Reference{TMediaType}"/> to unreference.</param>
        public void Unreference(IReference<TMediaType> reference)
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
                _referencesByJoinedPaths.Remove(reference.GetJoinedPaths());
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
                    oldImplementation.Dispose();
                    _loadedImplementationsByReferenceId.Remove(reference.Id);
                }

                return;
            }

            _referenceCountsByReferenceId[reference.Id] = newReferenceCount;
        }

        /// <summary>
        /// Converts the loaded media into the specific implementation.
        /// </summary>
        /// <param name="media">The loaded media.</param>
        /// <returns>The processed implementation.</returns>
        protected abstract TMediaImplementation ProcessMedia(params TMediaType[] media);
    }
}
