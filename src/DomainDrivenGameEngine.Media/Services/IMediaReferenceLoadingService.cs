namespace DomainDrivenGameEngine.Media.Services
{
    /// <summary>
    /// An interface describing a service for loading referenced media.
    /// </summary>
    public interface IMediaReferenceLoadingService
    {
        /// <summary>
        /// Checks to see if this service has any media that is no longer being referenced that can be unloaded.
        /// </summary>
        /// <returns><c>true</c> if media can be unloaded.</returns>
        bool CanUnloadMedia();

        /// <summary>
        /// Checks to see if this service is currently loading any media.
        /// </summary>
        /// <returns><c>true</c> if the service is currently loading media.</returns>
        bool IsLoadingMedia();

        /// <summary>
        /// Handles loading any media that has finished being read into their final implementations.
        /// </summary>
        /// <returns><c>true</c> if any media was loaded.</returns>
        bool LoadReferencedMedia();

        /// <summary>
        /// Stops all in-progress media being loaded and unloads all loaded media.
        /// </summary>
        void Reset();

        /// <summary>
        /// Handles unloading any media that is no longer being referenced.
        /// </summary>
        /// <returns><c>true</c> if any media was unloaded.</returns>
        bool UnloadUnreferencedMedia();
    }
}
