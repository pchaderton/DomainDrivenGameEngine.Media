using System.Collections.Generic;
using System.Linq;
using DomainDrivenGameEngine.Media.Models;

namespace DomainDrivenGameEngine.Media.Loaders
{
    /// <summary>
    /// A loader implementing <see cref="AnimationCollection"/> objects with no wrapper implementation.
    /// </summary>
    public class AnimationCollectionLoader : IMediaLoader<AnimationCollection, AnimationCollection>
    {
        /// <inheritdoc/>
        public bool IsSourceMediaRequired => false;

        /// <inheritdoc/>
        public bool IsMediaCountSupported(uint count)
        {
            return count > 0;
        }

        /// <inheritdoc/>
        public AnimationCollection Load(IReadOnlyList<AnimationCollection> media, IReadOnlyList<string> paths = null)
        {
            return new AnimationCollection(media.SelectMany(ac => ac).ToList());
        }

        /// <inheritdoc/>
        public void Unload(AnimationCollection implementation)
        {
        }
    }
}
