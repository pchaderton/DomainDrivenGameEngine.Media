using System.Collections.Generic;
using System.Linq;
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
        /// A lookup to the number of paths and/or media this loading service expects to receive when a caller references it.
        /// </summary>
        private readonly HashSet<uint> _expectedCountLookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMediaImplementationService{TMedia, TMediaImplementation}"/> class.
        /// </summary>
        /// <param name="supportedPathCounts">The number of path counts that this service can support loading.  Defaults to 1.</param>
        /// <param name="isSourceStreamRequired">A value indicating whether this implementation service requires file streams to be maintained.</param>
        protected BaseMediaImplementationService(IEnumerable<uint> supportedPathCounts = null,
                                                 bool isSourceStreamRequired = false)
        {
            _expectedCountLookup = (supportedPathCounts ?? new uint[] { 1 }).ToHashSet();
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
        /// Checks to see if the specified count of media is supported.
        /// </summary>
        /// <param name="count">The amount of media to check.</param>
        /// <returns><c>true</c> if the service supports the specified count of media.</returns>
        public bool IsMediaCountSupported(uint count)
        {
            return _expectedCountLookup.Contains(count);
        }

        /// <summary>
        /// Loads the sourced media into the specific implementation.
        /// </summary>
        /// <param name="media">The loaded media.</param>
        /// <param name="paths">Optional, the paths that were used to load the media.</param>
        /// <returns>The processed implementation.</returns>
        public abstract TMediaImplementation LoadImplementation(IReadOnlyList<TMedia> media, IReadOnlyList<string> paths = null);

        /// <summary>
        /// Unloads a previously loaded implementation.
        /// </summary>
        /// <param name="implementation">The implementation to unload.</param>
        public abstract void UnloadImplementation(TMediaImplementation implementation);
    }
}
