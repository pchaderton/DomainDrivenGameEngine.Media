using System.Numerics;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// An animation channel keyframe.
    /// </summary>
    public class AnimationChannelKeyFrame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationChannelKeyFrame"/> class.
        /// </summary>
        /// <param name="rotation">The rotation of this keyframe.</param>
        /// <param name="offset">The offset of this keyframe.</param>
        /// <param name="scale">The scale of this keyframe.</param>
        public AnimationChannelKeyFrame(Quaternion? rotation, Vector3? offset, Vector3? scale)
        {
            Rotation = rotation ?? Quaternion.Identity;
            Offset = offset ?? Vector3.Zero;
            Scale = scale ?? Vector3.Zero;
        }

        /// <summary>
        /// Gets the offset of this keyframe.
        /// </summary>
        public Vector3 Offset { get; }

        /// <summary>
        /// Gets the rotation of this keyframe.
        /// </summary>
        public Quaternion Rotation { get; }

        /// <summary>
        /// Gets the scale of this keyframe.
        /// </summary>
        public Vector3 Scale { get; }
    }
}
