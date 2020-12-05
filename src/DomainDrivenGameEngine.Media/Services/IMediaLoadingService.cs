namespace DomainDrivenGameEngine.Media.Services
{
    /// <summary>
    /// An interface describing a service for loading and unloading media across all media services.
    /// </summary>
    public interface IMediaLoadingService
    {
        /// <summary>
        /// Checks to see if any media service has any media that can be unloaded.
        /// </summary>
        /// <returns><c>true</c> if any media can be unloaded.</returns>
        bool CanUnloadMedia();

        /// <summary>
        /// Checks to see if any media service is currently loading any media.
        /// </summary>
        /// <returns><c>true</c> if any media is currently being loaded.</returns>
        bool IsLoadingMedia();

        /// <summary>
        /// Handles loading any media that has finished being read into their final implementations across all media services.
        /// </summary>
        /// <returns><c>true</c> if any media was loaded.</returns>
        bool LoadMedia();

        /// <summary>
        /// Stops all in-progress media being loaded and unloads all loaded media across all media services.
        /// </summary>
        void Reset();

        /// <summary>
        /// Handles unloading any media that is no longer being referenced across all media services.
        /// </summary>
        /// <returns><c>true</c> if any media was unloaded.</returns>
        bool UnloadMedia();
    }
}
