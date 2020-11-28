using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A collection of animation data.
    /// </summary>
    public class AnimationCollection : ReadOnlyCollection<Animation>, IMedia, IMediaImplementation<AnimationCollection>
    {
        /// <summary>
        /// A dictionary lookup of animations in the collection keyed by name.
        /// </summary>
        private Dictionary<string, Animation> _animationsByName;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationCollection"/> class.
        /// </summary>
        /// <param name="animations">The animations in this animation set.</param>
        public AnimationCollection(IList<Animation> animations)
            : base(animations ?? throw new ArgumentNullException(nameof(animations)))
        {
            _animationsByName = animations.GroupBy(a => a.Name).ToDictionary(g => g.Key, g => g.First());
        }

        /// <summary>
        /// Tries to get an animation from the collection with the specified name.
        /// </summary>
        /// <param name="name">The name of the animation to get.</param>
        /// <param name="animation">The output animation.</param>
        /// <returns><c>true</c> if an animation with the name was found.</returns>
        public bool TryGetAnimationByName(string name, out Animation animation)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return _animationsByName.TryGetValue(name, out animation);
        }
    }
}
