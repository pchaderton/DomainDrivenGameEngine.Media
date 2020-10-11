using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DomainDriveGameEngine.Media.Models;

namespace DomainDriveGameEngine.Media.Services
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
        /// A lookup of in-progress tasks by reference ID.
        /// </summary>
        private IDictionary<int, IReadOnlyCollection<Task<TMediaType>>> _inProgressTasks;

        /// <summary>
        /// A lookup of loaded implementations by reference ID.
        /// </summary>
        private IDictionary<int, TMediaImplementation> _loadedImplementationsByReferenceId;

        /// <summary>
        /// The services to use for sourcing referenced media.
        /// </summary>
        private IReadOnlyCollection<IMediaSourceService<TMediaType>> _mediaSourceServices;

        /// <summary>
        /// The service to use for tracking referenced media.
        /// </summary>
        private IMediaReferenceService<TMediaType> _mediaReferenceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMediaLoadingService{TMediaType, TMediaImplementation}"/> class.
        /// </summary>
        /// <param name="mediaReferenceService">The service to use for tracking referenced media.</param>
        /// <param name="mediaSourceServices">The services to use for sourcing referenced media.</param>
        protected BaseMediaLoadingService(IMediaReferenceService<TMediaType> mediaReferenceService, IReadOnlyCollection<IMediaSourceService<TMediaType>> mediaSourceServices)
        {
            _mediaReferenceService = mediaReferenceService ?? throw new ArgumentNullException(nameof(mediaReferenceService));
            _mediaSourceServices = mediaSourceServices ?? throw new ArgumentNullException(nameof(mediaSourceServices));
            _inProgressTasks = new Dictionary<int, IReadOnlyCollection<Task<TMediaType>>>();
            _loadedImplementationsByReferenceId = new Dictionary<int, TMediaImplementation>();
        }

        /// <summary>
        /// Disposes this service.
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var loadedImplementationByKvp in _loadedImplementationsByReferenceId)
            {
                loadedImplementationByKvp.Value.Dispose();
            }

            _loadedImplementationsByReferenceId.Clear();

            _inProgressTasks.Clear();
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
        /// Handles processing any reference changes, queueing up background tasks to load media and process it into their final implementations.
        /// </summary>
        public void ProcessReferenceUpdates()
        {
            // Clean up any implementations that are no longer being referenced.
            var oldReferences = _mediaReferenceService.GetOldReferences();
            foreach (var oldReference in oldReferences)
            {
                if (_loadedImplementationsByReferenceId.TryGetValue(oldReference.Id, out var oldImplementation))
                {
                    oldImplementation.Dispose();
                    _loadedImplementationsByReferenceId.Remove(oldReference.Id);
                }
                else
                {
                    throw new Exception("Attempt to remove previously unloaded reference.");
                }
            }

            // Identify the sources used for newly requested references and queue up tasks to load them in the background.
            var newReferences = _mediaReferenceService.GetNewReferences();
            foreach (var newReference in newReferences)
            {
                var tasks = new List<Task<TMediaType>>();
                foreach (var path in newReference.Paths)
                {
                    var extension = Path.GetExtension(path);
                    var source = _mediaSourceServices.FirstOrDefault(mss => mss.IsExtensionSupported(extension));
                    tasks.Add(Task.Run(() =>
                    {
                        return source.Load(path);
                    }));

                    _inProgressTasks.Add(newReference.Id, tasks);
                }
            }

            // Immediately process any media that was referenced with pre-defined media.
            var newReferencesWithMedia = _mediaReferenceService.GetNewReferencesWithMedia();
            foreach (var newReferenceWithMedia in newReferencesWithMedia)
            {
                _loadedImplementationsByReferenceId[newReferenceWithMedia.Item1.Id] = ProcessMedia(newReferenceWithMedia.Item2);
            }

            // Finally, process any media for which the background tasks have finished running.
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
        /// Converts the loaded media into the specific implementation.
        /// </summary>
        /// <param name="media">The loaded media.</param>
        /// <returns>The processed implementation.</returns>
        protected abstract TMediaImplementation ProcessMedia(params TMediaType[] media);
    }
}
