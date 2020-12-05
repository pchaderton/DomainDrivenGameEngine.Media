using System;
using System.Collections.Generic;
using System.IO;

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
        public Texture(int width, int height, TextureFormat format, IReadOnlyList<byte> bytes, Stream sourceStream = null)
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

            Width = width;
            Height = height;
            Format = format;
            Bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
        }

        /// <summary>
        /// Gets the bytes representing this texture.
        /// </summary>
        public IReadOnlyList<byte> Bytes { get; }

        /// <summary>
        /// Gets the format of the pixels in the texture.
        /// </summary>
        public TextureFormat Format { get; }

        /// <summary>
        /// Gets the height of the texture in pixels.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the width of the texture in pixels.
        /// </summary>
        public int Width { get; }
    }
}
