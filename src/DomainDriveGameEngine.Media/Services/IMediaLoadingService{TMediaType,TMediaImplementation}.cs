using DomainDriveGameEngine.Media.Models;

namespace DomainDriveGameEngine.Media.Services
{
    /// <summary>
    /// An interface describing a service for loading referenced media for a specific implementation.
    /// </summary>
    /// <typeparam name="TMediaType">The type of media this service loads into an implementation.</typeparam>
    /// <typeparam name="TMediaImplementation">The type of implementation that is loaded.</typeparam>
    public interface IMediaLoadingService<TMediaType, TMediaImplementation> : IMediaReferenceService<TMediaType>
        where TMediaType : class, IMedia
        where TMediaImplementation : class, IMediaImplementation
    {
        /// <summary>
        /// Gets the implementation for the given reference.
        /// </summary>
        /// <param name="reference">The <see cref="IReference{TMediaType}"/> to get the implementation for.</param>
        /// <returns>The implementation, or null if the implementation is not ready for use yet.</returns>
        TMediaImplementation GetImplementation(IReference<TMediaType> reference);

        /// <summary>
        /// Handles processing any loaded media into their final implementations.
        /// </summary>
        void ProcessLoadedMedia();
    }
}
