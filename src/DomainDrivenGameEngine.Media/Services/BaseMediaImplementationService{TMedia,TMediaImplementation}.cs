using System.Collections.Generic;
using DomainDrivenGameEngine.Media.Models;

namespace DomainDrivenGameEngine.Media.Services
{
    /// <summary>
    /// An interface to a service for generating the final implementations of loaded media.
    /// </summary>
    /// <typeparam name="TMedia">The type of media this service converts into an implementation.</typeparam>
    /// <typeparam name="TMediaImplementation">The final media implementation.</typeparam>
    public abstract class BaseMediaImplementationService<TMedia, TMediaImplementation> : IMediaImplementationService<TMedia, TMediaImplementation>
        where TMedia : class, IMedia
        where TMediaImplementation : class, IMediaImplementation<TMedia>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMediaImplementationService{TMedia, TMediaImplementation}"/> class.
        /// </summary>
        /// <param name="isSourceStreamRequired">A value indicating whether this implementation service requires file streams to be maintained.</param>
        protected BaseMediaImplementationService(bool isSourceStreamRequired = false)
        {
            IsSourceStreamRequired = isSourceStreamRequired;
        }

        /// <summary>
        /// Gets a value indicating whether this implementation service requires file streams to be maintained.
        /// </summary>
        /// <remarks>
        /// Any implementation of this interface that returns <c>true</c> from this property should dispose streams themself.
        /// </remarks>
        public bool IsSourceStreamRequired { get; }

        /// <summary>
        /// Loads the sourced media into the specific implementation.
        /// </summary>
        /// <param name="media">The loaded media.</param>
        /// <param name="paths">Optional, the paths that were used to load the media.</param>
        /// <returns>The processed implementation.</returns>
        public abstract TMediaImplementation LoadImplementation(IReadOnlyCollection<TMedia> media, IReadOnlyCollection<string> paths = null);

        /// <summary>
        /// Unloads a previously loaded implementation.
        /// </summary>
        /// <param name="implementation">The implementation to unload.</param>
        public abstract void UnloadImplementation(TMediaImplementation implementation);
    }
}
