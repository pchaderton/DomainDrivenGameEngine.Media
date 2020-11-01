using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A collection of animation data.
    /// </summary>
    public class AnimationCollection : ReadOnlyCollection<Animation>, IMedia
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationCollection"/> class.
        /// </summary>
        /// <param name="animations">The animations in this animation set.</param>
        public AnimationCollection(IReadOnlyCollection<Animation> animations)
            : base(animations?.ToList() ?? throw new ArgumentNullException(nameof(animations)))
        {
        }
    }
}
