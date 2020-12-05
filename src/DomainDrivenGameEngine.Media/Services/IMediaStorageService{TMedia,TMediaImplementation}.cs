using DomainDrivenGameEngine.Media.Models;

namespace DomainDrivenGameEngine.Media.Services
{
    /// <summary>
    /// An interface describing a service for loading referenced media for a specific implementation.
    /// </summary>
    /// <typeparam name="TMedia">The type of media this service loads into an implementation.</typeparam>
    /// <typeparam name="TMediaImplementation">The type of implementation that is loaded.</typeparam>
    public interface IMediaStorageService<TMedia, TMediaImplementation> : IMediaReferenceService<TMedia>
        where TMedia : class, IMedia
        where TMediaImplementation : class, IMediaImplementation<TMedia>
    {
        /// <summary>
        /// Gets the implementation for the given reference.
        /// </summary>
        /// <param name="reference">The <see cref="IMediaReference{TMedia}"/> to get the implementation for.</param>
        /// <returns>The implementation, or null if the implementation is not ready for use yet.</returns>
        TMediaImplementation GetImplementation(IMediaReference<TMedia> reference);

        /// <summary>
        /// Tries to get the implementation for the given reference.
        /// </summary>
        /// <param name="reference">The <see cref="IMediaReference{TMedia}"/> to get the implementation for.</param>
        /// <param name="output">The output implementation, or <c>null</c> if the implementation is not ready for use yet.</param>
        /// <returns><c>true</c> if the implementation is ready for use.</returns>
        bool TryGetImplementation(IMediaReference<TMedia> reference, out TMediaImplementation output);
    }
}
