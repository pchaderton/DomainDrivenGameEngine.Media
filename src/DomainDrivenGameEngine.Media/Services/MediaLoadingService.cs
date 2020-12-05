using System;
using System.Linq;

namespace DomainDrivenGameEngine.Media.Services
{
    /// <summary>
    /// A service for loading and unloading media across all media services.
    /// </summary>
    public class MediaLoadingService : IMediaLoadingService, IDisposable
    {
        /// <summary>
        /// The <see cref="IMediaReferenceLoadingService"/> objects to load and unload media with.
        /// </summary>
        private readonly IMediaReferenceLoadingService[] _mediaReferenceLoadingServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaLoadingService"/> class.
        /// </summary>
        /// <param name="mediaReferenceLoadingServices">The <see cref="IMediaReferenceLoadingService"/> objects to load and unload media with.</param>
        public MediaLoadingService(IMediaReferenceLoadingService[] mediaReferenceLoadingServices)
        {
            _mediaReferenceLoadingServices = mediaReferenceLoadingServices ?? throw new ArgumentNullException(nameof(mediaReferenceLoadingServices));
        }

        /// <summary>
        /// Checks to see if any media service has any media that can be unloaded.
        /// </summary>
        /// <returns><c>true</c> if any media can be unloaded.</returns>
        public bool CanUnloadMedia()
        {
            return _mediaReferenceLoadingServices.Any(s => s.CanUnloadMedia());
        }

        /// <summary>
        /// Disposes this service.
        /// </summary>
        public virtual void Dispose()
        {
            Reset();
        }

        /// <summary>
        /// Checks to see if any media service is currently loading any media.
        /// </summary>
        /// <returns><c>true</c> if any media is currently being loaded.</returns>
        public bool IsLoadingMedia()
        {
            return _mediaReferenceLoadingServices.Any(s => s.IsLoadingMedia());
        }

        /// <summary>
        /// Handles loading any media that has finished being read into their final implementations across all media services.
        /// </summary>
        /// <returns><c>true</c> if any media was loaded.</returns>
        public bool LoadMedia()
        {
            var mediaLoaded = false;
            foreach (var mediaReferenceLoadingService in _mediaReferenceLoadingServices)
            {
                mediaLoaded |= mediaReferenceLoadingService.LoadReferencedMedia();
            }

            return mediaLoaded;
        }

        /// <summary>
        /// Stops all in-progress media being loaded and unloads all loaded media across all media services.
        /// </summary>
        public void Reset()
        {
            foreach (var mediaReferenceLoadingService in _mediaReferenceLoadingServices)
            {
                mediaReferenceLoadingService.Reset();
            }
        }

        /// <summary>
        /// Handles unloading any media that is no longer being referenced across all media services.
        /// </summary>
        /// <returns><c>true</c> if any media was unloaded.</returns>
        public bool UnloadMedia()
        {
            var mediaUnloaded = false;
            foreach (var mediaReferenceLoadingService in _mediaReferenceLoadingServices)
            {
                mediaUnloaded |= mediaReferenceLoadingService.UnloadUnreferencedMedia();
            }

            return mediaUnloaded;
        }
    }
}
