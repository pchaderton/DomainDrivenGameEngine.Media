using System;
using System.Collections.Generic;
using System.IO;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A sound effect.
    /// </summary>
    public class SoundEffect : BaseMedia
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoundEffect"/> class.
        /// </summary>
        /// <param name="channels">The number of channels the sound effect has.</param>
        /// <param name="sampleRate">The sample rate for the sound effect.</param>
        /// <param name="bytes">The bytes for the sound effect.</param>
        /// <param name="sourceStream">The source <see cref="Stream"/> used to read this sound effect.</param>
        public SoundEffect(int channels, int sampleRate, IReadOnlyList<byte> bytes, Stream sourceStream = null)
            : base(sourceStream)
        {
            if (channels <= 0)
            {
                throw new ArgumentException($"A valid {nameof(channels)} is required.");
            }

            if (sampleRate <= 0)
            {
                throw new ArgumentException($"A valid {nameof(sampleRate)} is required.");
            }

            Channels = channels;
            SampleRate = sampleRate;
            Bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
        }

        /// <summary>
        /// Gets the bytes that represent this sound effect.
        /// </summary>
        public IReadOnlyList<byte> Bytes { get; }

        /// <summary>
        /// Gets the number of channels the sound effect has.
        /// </summary>
        public int Channels { get; }

        /// <summary>
        /// Gets the sample rate for the sound effect.
        /// </summary>
        public int SampleRate { get; }
    }
}
