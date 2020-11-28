using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// An animation channel.
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Channel"/> class.
        /// </summary>
        /// <param name="boneName">The name of the bone that this channel is applied to.</param>
        /// <param name="rotationKeyFrames">The keyframes for this channel.</param>
        /// <param name="offsetKeyFrames">The offset keyframes for this channel.</param>
        /// <param name="scaleKeyFrames">The scaling keyframes for this channel.</param>
        public Channel(string boneName,
                       ReadOnlyCollection<KeyFrame<Quaternion>> rotationKeyFrames,
                       ReadOnlyCollection<KeyFrame<Vector3>> offsetKeyFrames,
                       ReadOnlyCollection<KeyFrame<Vector3>> scaleKeyFrames)
        {
            BoneName = boneName ?? throw new ArgumentNullException(nameof(boneName));
            RotationKeyFrames = rotationKeyFrames ?? throw new ArgumentNullException(nameof(rotationKeyFrames));
            OffsetKeyFrames = offsetKeyFrames ?? throw new ArgumentNullException(nameof(offsetKeyFrames));
            ScaleKeyFrames = scaleKeyFrames ?? throw new ArgumentNullException(nameof(scaleKeyFrames));
        }

        /// <summary>
        /// Gets the name of the bone that this channel is applied to.
        /// </summary>
        public string BoneName { get; }

        /// <summary>
        /// Gets the offset keyframes for this channel.
        /// </summary>
        public IReadOnlyList<KeyFrame<Vector3>> OffsetKeyFrames { get; }

        /// <summary>
        /// Gets the rotation keyframes for this channel.
        /// </summary>
        public IReadOnlyList<KeyFrame<Quaternion>> RotationKeyFrames { get; }

        /// <summary>
        /// Gets the scale keyframes for this channel.
        /// </summary>
        public IReadOnlyList<KeyFrame<Vector3>> ScaleKeyFrames { get; }

        /// <summary>
        /// Gets the offset value at the specified time.
        /// </summary>
        /// <param name="animationTime">The animation time to get the offset for.</param>
        /// <param name="interpolate">Optional, defaults to <c>true</c>.  When <c>true</c>, smoothly interpolates between frames.</param>
        /// <returns>The offset value at the specified time.</returns>
        public Vector3 GetOffsetAtTime(double animationTime, bool interpolate = true)
        {
            GetKeyFramesAtTime(OffsetKeyFrames, animationTime, out var keyFrame, out var nextKeyFrame);

            if (!interpolate)
            {
                return animationTime >= nextKeyFrame.TimeInSeconds
                    ? nextKeyFrame.Value
                    : keyFrame.Value;
            }

            var lerpAmount = GetLerpAmount(keyFrame.TimeInSeconds, nextKeyFrame.TimeInSeconds, animationTime);

            return Vector3.Lerp(keyFrame.Value, nextKeyFrame.Value, lerpAmount);
        }

        /// <summary>
        /// Gets the rotation value at the specified time.
        /// </summary>
        /// <param name="animationTime">The animation time to get the rotation for.</param>
        /// <param name="interpolate">Optional, defaults to <c>true</c>.  When <c>true</c>, smoothly interpolates between frame.</param>
        /// <returns>The rotation value at the specified time.</returns>
        public Quaternion GetRotationAtTime(double animationTime, bool interpolate = true)
        {
            GetKeyFramesAtTime(RotationKeyFrames, animationTime, out var keyFrame, out var nextKeyFrame);

            if (!interpolate)
            {
                return keyFrame.Value;
            }

            var lerpAmount = GetLerpAmount(keyFrame.TimeInSeconds, nextKeyFrame.TimeInSeconds, animationTime);

            return Quaternion.Slerp(keyFrame.Value, nextKeyFrame.Value, lerpAmount);
        }

        /// <summary>
        /// Gets the scale value at the specified time.
        /// </summary>
        /// <param name="animationTime">The animation time to get the scale for.</param>
        /// <param name="interpolate">Optional, defaults to <c>true</c>.  When <c>true</c>, smoothly interpolates between frame.</param>
        /// <returns>The scale value at the specified time.</returns>
        public Vector3 GetScaleAtTime(double animationTime, bool interpolate = true)
        {
            GetKeyFramesAtTime(ScaleKeyFrames, animationTime, out var keyFrame, out var nextKeyFrame);

            if (!interpolate)
            {
                return keyFrame.Value;
            }

            var lerpAmount = GetLerpAmount(keyFrame.TimeInSeconds, nextKeyFrame.TimeInSeconds, animationTime);

            return Vector3.Lerp(keyFrame.Value, nextKeyFrame.Value, lerpAmount);
        }

        /// <summary>
        /// Gets the set of active keyframes at the given time.
        /// </summary>
        /// <typeparam name="TValue">The type of keyframe value to get.</typeparam>
        /// <param name="keyFrames">The keyframes to read from.</param>
        /// <param name="animationTime">The animation time.</param>
        /// <param name="startKeyFrame">The output starting keyframe.</param>
        /// <param name="endKeyFrame">The output ending keyframe.</param>
        private void GetKeyFramesAtTime<TValue>(IReadOnlyList<KeyFrame<TValue>> keyFrames,
                                                double animationTime,
                                                out KeyFrame<TValue> startKeyFrame,
                                                out KeyFrame<TValue> endKeyFrame)
            where TValue : struct
        {
            startKeyFrame = null;
            endKeyFrame = keyFrames[0];
            for (var i = 0; i < keyFrames.Count; i++)
            {
                startKeyFrame = endKeyFrame;
                endKeyFrame = keyFrames[i];
                if (endKeyFrame.TimeInSeconds > animationTime)
                {
                    break;
                }
            }

            if (startKeyFrame == null)
            {
                startKeyFrame = endKeyFrame;
            }
        }

        /// <summary>
        /// Gets the lerp amount of a given time between a start and end time.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="currentTime">The current time to get the lerp amount for.</param>
        /// <returns>The lerp amount.</returns>
        private float GetLerpAmount(double startTime, double endTime, double currentTime)
        {
            if (startTime == endTime)
            {
                return 0.0f;
            }

            return (float)((currentTime - startTime) / (endTime - startTime));
        }
    }
}
