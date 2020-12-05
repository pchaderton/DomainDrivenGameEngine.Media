using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A piece of media representing a texture.
    /// </summary>
    public class Texture : BaseMedia
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class.
        /// </summary>
        /// <param name="width">The width of the texture in pixels.</param>
        /// <param name="height">The height of the texture in pixels.</param>
        /// <param name="format">The format used by the texture.</param>
        /// <param name="bytes">The bytes of the texture.</param>
        /// <param name="sourceStream">The source <see cref="Stream"/> used to read this texture.</param>
        public Texture(int width, int height, TextureFormat format, ReadOnlyCollection<byte> bytes, Stream sourceStream = null)
            : this(width, height, format, new MemoryStream(bytes?.ToArray() ?? throw new ArgumentNullException(nameof(bytes))), sourceStream)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class.
        /// </summary>
        /// <param name="width">The width of the texture in pixels.</param>
        /// <param name="height">The height of the texture in pixels.</param>
        /// <param name="format">The format used by the texture.</param>
        /// <param name="stream">The <see cref="Stream"/> to use to read the bytes of the texture.</param>
        /// <param name="sourceStream">The source <see cref="Stream"/> used to read this texture.</param>
        public Texture(int width, int height, TextureFormat format, Stream stream, Stream sourceStream = null)
            : base(sourceStream)
        {
            if (width <= 0)
            {
                throw new ArgumentException($"A valid {nameof(width)} is required.");
            }

            if (height <= 0)
            {
                throw new ArgumentException($"A valid {nameof(height)} is required.");
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            Width = width;
            Height = height;
            Format = format;
            Stream = stream;
        }

        /// <summary>
        /// Gets the format of the pixels in the texture.
        /// </summary>
        public TextureFormat Format { get; }

        /// <summary>
        /// Gets the height of the texture in pixels.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the <see cref="Stream"/> to use for reading the texture.
        /// </summary>
        public Stream Stream { get; private set; }

        /// <summary>
        /// Gets the width of the texture in pixels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Release all resources managed by this texture.
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
