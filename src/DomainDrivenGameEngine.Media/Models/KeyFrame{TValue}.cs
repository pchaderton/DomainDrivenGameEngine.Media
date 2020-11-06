using System;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// An animation channel keyframe.
    /// </summary>
    /// <typeparam name="TValue">The type of value contained in this keyframe.</typeparam>
    public class KeyFrame<TValue>
        where TValue : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyFrame{TValue}"/> class.
        /// </summary>
        /// <param name="timeInSeconds">The time in seconds this keyframe is hit in a given channel.</param>
        /// <param name="value">The value of this keyframe.</param>
        public KeyFrame(double timeInSeconds, TValue value)
        {
            if (timeInSeconds < 0)
            {
                throw new ArgumentException($"A valid {nameof(timeInSeconds)} is required.");
            }

            TimeInSeconds = timeInSeconds;
            Value = value;
        }

        /// <summary>
        /// Gets the time in seconds this keyframe is hit in a given channel.
        /// </summary>
        public double TimeInSeconds { get; }

        /// <summary>
        /// Gets the value of this keyframe.
        /// </summary>
        public TValue Value { get; }
    }
}
