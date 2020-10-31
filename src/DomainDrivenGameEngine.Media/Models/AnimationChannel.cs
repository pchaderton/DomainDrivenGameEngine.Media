using System;
using System.Collections.Generic;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// An animation channel.
    /// </summary>
    public class AnimationChannel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationChannel"/> class.
        /// </summary>
        /// <param name="boneName">The name of the bone that this channel is applied to.</param>
        /// <param name="keyframes">The keyframes for this channel.</param>
        public AnimationChannel(string boneName, IReadOnlyCollection<AnimationChannelKeyFrame> keyframes)
        {
            BoneName = boneName ?? throw new ArgumentNullException(nameof(boneName));
            KeyFrames = keyframes ?? throw new ArgumentNullException(nameof(keyframes));
        }

        /// <summary>
        /// Gets the name of the bone that this channel is applied to.
        /// </summary>
        public string BoneName { get; }

        /// <summary>
        /// Gets the keyframes for this channel.
        /// </summary>
        public IReadOnlyCollection<AnimationChannelKeyFrame> KeyFrames { get; }
    }
}
