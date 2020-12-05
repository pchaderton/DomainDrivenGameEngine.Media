using System;
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
        public SoundEffect(int channels, int sampleRate, byte[] bytes, Stream sourceStream = null)
            : this(channels, sampleRate, new MemoryStream(bytes ?? throw new ArgumentNullException(nameof(bytes))), sourceStream)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundEffect"/> class.
        /// </summary>
        /// <param name="channels">The number of channels the sound effect has.</param>
        /// <param name="sampleRate">The sample rate for the sound effect.</param>
        /// <param name="stream">The <see cref="Stream"/> used to read the bytes of the sound effect.</param>
        /// <param name="sourceStream">The source <see cref="Stream"/> used to read this sound effect.</param>
        public SoundEffect(int channels, int sampleRate, Stream stream, Stream sourceStream = null)
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
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        /// <summary>
        /// Gets the number of channels the sound effect has.
        /// </summary>
        public int Channels { get; }

        /// <summary>
        /// Gets the sample rate for the sound effect.
        /// </summary>
        public int SampleRate { get; }

        /// <summary>
        /// Gets the <see cref="Stream"/> used to read the bytes of the sound effect.
        /// </summary>
        public Stream Stream { get; private set; }

        /// <summary>
        /// Release all resources managed by this sound effect.
        /// </summary>
        public override void Dispose()
        {
            if (Stream != null)
            {
                Stream.Dispose();
                Stream = null;
            }

            base.Dispose();
        }
    }
}
