using System;
using System.IO;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A piece of music.
    /// </summary>
    /// <remarks>
    /// <see cref="IDisposable"/> is not added here as the <see cref="Stream"/> is likely to be used in
    /// for gradually streaming music later on.  As such it falls to the implementation side of handling
    /// media to dispose the <see cref="Stream"/>.
    /// </remarks>
    public class Music : IMedia
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Music"/> class.
        /// </summary>
        /// <param name="channels">The number of channels the sound effect has.</param>
        /// <param name="sampleRate">The sample rate for the sound effect.</param>
        /// <param name="stream">The <see cref="Stream"/> for accessing the music.</param>
        public Music(int channels, int sampleRate, Stream stream)
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
            Stream = stream ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// Gets the number of channels the music has.
        /// </summary>
        public int Channels { get; }

        /// <summary>
        /// Gets the sample rate for the music.
        /// </summary>
        public int SampleRate { get; }

        /// <summary>
        /// Gets the stream for reading this music.
        /// </summary>
        public Stream Stream { get; }
    }
}
