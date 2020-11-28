using System.Collections.Generic;
using System.Linq;
using DomainDrivenGameEngine.Media.Models;

namespace DomainDrivenGameEngine.Media.Services
{
    /// <summary>
    /// An service for implementing <see cref="AnimationCollection"/> objects with no wrapper implementation.
    /// </summary>
    public class AnimationCollectionImplementationService : IMediaImplementationService<AnimationCollection, AnimationCollection>
    {
        /// <inheritdoc/>
        public bool IsSourceStreamRequired => false;

        /// <inheritdoc/>
        public bool IsMediaCountSupported(uint count)
        {
            return count > 0;
        }

        /// <inheritdoc/>
        public AnimationCollection LoadImplementation(IReadOnlyList<AnimationCollection> media, IReadOnlyList<string> paths = null)
        {
            return new AnimationCollection(media.SelectMany(ac => ac).ToList());
        }

        /// <inheritdoc/>
        public void UnloadImplementation(AnimationCollection implementation)
        {
        }
    }
}
