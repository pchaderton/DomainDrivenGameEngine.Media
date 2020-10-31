using System;
using System.Collections.Generic;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// An animation for a <see cref="Model"/>.
    /// </summary>
    public class Animation : IMedia
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Animation"/> class.
        /// </summary>
        /// <param name="name">The name of this animation.</param>
        /// <param name="channels">The channels in this animation.</param>
        /// <param name="framesPerSecond">The frame rate per second of this animation.  Defaults to 60 frames per second.</param>
        public Animation(string name, IReadOnlyCollection<AnimationChannel> channels, double framesPerSecond = 60.0)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Channels = channels ?? throw new ArgumentNullException(nameof(channels));
            FramesPerSecond = framesPerSecond;
        }

        /// <summary>
        /// Gets the channels in this animation.
        /// </summary>
        public IReadOnlyCollection<AnimationChannel> Channels { get; }

        /// <summary>
        /// Gets the frame rate per second of this animation.
        /// </summary>
        public double FramesPerSecond { get; }

        /// <summary>
        /// Gets the name of this animation.
        /// </summary>
        public string Name { get; }
    }
}
