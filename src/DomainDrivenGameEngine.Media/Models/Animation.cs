using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// An animation for a <see cref="Model"/>.
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Animation"/> class.
        /// </summary>
        /// <param name="name">The name of this animation.</param>
        /// <param name="channels">The channels in this animation.</param>
        /// <param name="durationInSeconds">The duration of the animation in seconds.</param>
        public Animation(string name, ReadOnlyCollection<Channel> channels, double durationInSeconds)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Channels = channels ?? throw new ArgumentNullException(nameof(channels));
            DurationInSeconds = durationInSeconds;
        }

        /// <summary>
        /// Gets the channels in this animation.
        /// </summary>
        public IReadOnlyList<Channel> Channels { get; }

        /// <summary>
        /// Gets the duration of the animation in seconds.
        /// </summary>
        public double DurationInSeconds { get; }

        /// <summary>
        /// Gets the name of this animation.
        /// </summary>
        public string Name { get; }
    }
}
