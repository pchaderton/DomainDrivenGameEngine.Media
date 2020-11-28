using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// An animation for a <see cref="Model"/>.
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// A dictionary lookup of the channels in this animation keyed by name.
        /// </summary>
        private readonly Dictionary<string, Channel> _channelsByBoneName;

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
            _channelsByBoneName = Channels.GroupBy(c => c.BoneName).ToDictionary(g => g.Key, g => g.First());
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

        /// <summary>
        /// Gets the matrices to apply to a skeleton to render the animation at the specified time.
        /// </summary>
        /// <param name="skeletonRoot">The root skeleton <see cref="Bone"/>.</param>
        /// <param name="animationTime">The time of the animation to get matrices for.</param>
        /// <param name="interpolate">Optional, defaults to <c>true</c>.  When <c>true</c>, smoothly interpolates between frames.</param>
        /// <returns>A <see cref="List{Matrix4x4}"/> containing the matrices to apply to the skeleton.</returns>
        public List<Matrix4x4> GetAnimationMatricesAtTime(Bone skeletonRoot, double animationTime, bool interpolate = true)
        {
            var matrices = new List<Matrix4x4>();
            GenerateMatricesForAnimationAtTime(matrices, animationTime, skeletonRoot, Matrix4x4.Identity, interpolate);
            return matrices;
        }

        /// <summary>
        /// Tries to get a channel from the collection with the specified bone name.
        /// </summary>
        /// <param name="boneName">The bone name of the channel to get.</param>
        /// <param name="channel">The output channel.</param>
        /// <returns><c>true</c> if a channel with the bone name was found.</returns>
        public bool TryGetChannelByBoneName(string boneName, out Channel channel)
        {
            if (boneName == null)
            {
                throw new ArgumentNullException(nameof(boneName));
            }

            return _channelsByBoneName.TryGetValue(boneName, out channel);
        }

        /// <summary>
        /// Generates the matrices for an animation at the given time.
        /// </summary>
        /// <param name="matrices">The <see cref="List{Matrix4x4}"/> to add the resulting matrices to.</param>
        /// <param name="animationTime">The animation time to generate the matrices for.</param>
        /// <param name="currentBone">The bone to generate matrices for.</param>
        /// <param name="parentTransformationMatrix">The transformation matrix of the parent bone.</param>
        /// <param name="interpolate">A flag indicating if frames should be smoothly interpolated between.</param>
        private void GenerateMatricesForAnimationAtTime(List<Matrix4x4> matrices,
                                                        double animationTime,
                                                        Bone currentBone,
                                                        Matrix4x4 parentTransformationMatrix,
                                                        bool interpolate)
        {
            Matrix4x4 localTransformation = TryGetChannelByBoneName(currentBone.Name, out var channel)
                ? GenerateChannelTransformationMatrixAtTime(channel, animationTime, interpolate)
                : Matrix4x4.Identity;

            var toBoneSpaceMatrix = currentBone.WorldToBindMatrix;

            var finalTransformation = localTransformation * parentTransformationMatrix;

            matrices.Add(toBoneSpaceMatrix * finalTransformation);

            foreach (var childBone in currentBone.Children)
            {
                GenerateMatricesForAnimationAtTime(matrices, animationTime, childBone, finalTransformation, interpolate);
            }
        }

        /// <summary>
        /// Gets the transformation matrix for a given channel at the specified time.
        /// </summary>
        /// <param name="channel">The channel to get the matrix for.</param>
        /// <param name="animationTime">The time to get the matrix for.</param>
        /// <param name="interpolate">A flag indicating if frames should be smoothly interpolated between.</param>
        /// <returns>The transformation matrix for the given channel.</returns>
        private Matrix4x4 GenerateChannelTransformationMatrixAtTime(Channel channel, double animationTime, bool interpolate)
        {
            var offset = channel.GetOffsetAtTime(animationTime, interpolate);
            var rotation = channel.GetRotationAtTime(animationTime, interpolate);
            var scale = channel.GetScaleAtTime(animationTime, interpolate);

            return Matrix4x4.CreateScale(scale) *
                Matrix4x4.CreateFromQuaternion(rotation) *
                Matrix4x4.CreateTranslation(offset);
        }
    }
}
