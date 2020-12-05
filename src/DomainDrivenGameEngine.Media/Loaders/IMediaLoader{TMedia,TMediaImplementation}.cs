using System.Collections.Generic;
using DomainDrivenGameEngine.Media.Models;

namespace DomainDrivenGameEngine.Media.Loaders
{
    /// <summary>
    /// A class for loading media into a final implementation.
    /// </summary>
    /// <typeparam name="TMedia">The media type this loader loads.</typeparam>
    /// <typeparam name="TMediaImplementation">The final media implementation this loader loads.</typeparam>
    public interface IMediaLoader<TMedia, TMediaImplementation>
        where TMedia : class, IMedia
        where TMediaImplementation : class, IMediaImplementation<TMedia>
    {
        /// <summary>
        /// Gets a value indicating whether this loader requires the source media to be maintained after loading.
        /// </summary>
        /// <remarks>
        /// Any implementation of this interface that returns <c>true</c> from this property should dispose the source media object in the <see cref="Unload"/> method.
        /// </remarks>
        bool IsSourceMediaRequired { get; }

        /// <summary>
        /// Checks to see if the specified count of media is supported.
        /// </summary>
        /// <param name="count">The amount of media to check.</param>
        /// <returns><c>true</c> if this loader supports the specified count of media.</returns>
        bool IsMediaCountSupported(uint count);

        /// <summary>
        /// Loads the media into the specific implementation.
        /// </summary>
        /// <param name="media">The media to load.</param>
        /// <param name="paths">Optional, the paths that were used to read the media.</param>
        /// <returns>The loaded implementation.</returns>
        TMediaImplementation Load(IReadOnlyList<TMedia> media, IReadOnlyList<string> paths = null);

        /// <summary>
        /// Unloads a previously loaded implementation.
        /// </summary>
        /// <param name="implementation">The implementation to unload.</param>
        void Unload(TMediaImplementation implementation);
    }
}
