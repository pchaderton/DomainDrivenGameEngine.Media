using DomainDrivenGameEngine.Media.Models;

namespace DomainDrivenGameEngine.Media.Services
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
        /// <param name="reference">The <see cref="IMediaReference{TMediaType}"/> to get the implementation for.</param>
        /// <returns>The implementation, or null if the implementation is not ready for use yet.</returns>
        TMediaImplementation GetImplementation(IMediaReference<TMediaType> reference);

        /// <summary>
        /// Handles processing any loaded media into their final implementations, as well as cleaning up any unloaded implementations.
        /// </summary>
        /// <returns><c>true</c> if any media was processed.</returns>
        bool ProcessMediaImplementations();
    }
}
