using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A piece of media representing a texture.
    /// </summary>
    public class Texture : IMedia
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class.
        /// </summary>
        /// <param name="width">The width of the texture in pixels.</param>
        /// <param name="height">The height of the texture in pixels.</param>
        /// <param name="pixelFormat">The pixel format used by the texture.</param>
        /// <param name="bytes">The bytes of the texture.</param>
        public Texture(int width, int height, PixelFormat pixelFormat, ReadOnlyCollection<byte> bytes)
        {
            if (width <= 0)
            {
                throw new ArgumentException($"A valid {nameof(width)} is required.");
            }

            if (height <= 0)
            {
                throw new ArgumentException($"A valid {nameof(height)} is required.");
            }

            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            var pixelFormatDetails = PixelFormatDetailsAttribute.GetPixelFormatDetails(pixelFormat);
            var expectedBytes = width * height * pixelFormatDetails.BytesPerPixel;
            if (bytes.Count != expectedBytes)
            {
                throw new ArgumentException($"Expected {expectedBytes} bytes, but only received {bytes.Count}.");
            }

            Width = width;
            Height = height;
            Format = pixelFormat;
            Bytes = bytes;
        }

        /// <summary>
        /// Gets the bytes of the texture.
        /// </summary>
        public IReadOnlyList<byte> Bytes { get; }

        /// <summary>
        /// Gets the format of the pixels in the texture.
        /// </summary>
        public PixelFormat Format { get; }

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
