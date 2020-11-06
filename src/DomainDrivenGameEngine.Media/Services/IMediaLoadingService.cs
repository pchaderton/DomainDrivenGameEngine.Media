namespace DomainDrivenGameEngine.Media.Services
{
    /// <summary>
    /// An interface describing a service for loading referenced media.
    /// </summary>
    public interface IMediaLoadingService
    {
        /// <summary>
        /// Checks to see if this service is currently loading any media.
        /// </summary>
        /// <returns><c>true</c> if the service is currently loading media.</returns>
        bool IsLoadingMedia();

        /// <summary>
        /// Handles processing any loaded media into their final implementations, as well as cleaning up any unloaded implementations.
        /// </summary>
        /// <returns><c>true</c> if any media was processed.</returns>
        bool ProcessMediaImplementations();
    }
}
