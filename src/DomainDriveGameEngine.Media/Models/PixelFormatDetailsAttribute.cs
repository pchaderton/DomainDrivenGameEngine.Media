using System;

namespace DomainDriveGameEngine.Media.Models
{
    /// <summary>
    /// An attribute describing the details of a pixel format.
    /// </summary>
    public class PixelFormatDetailsAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PixelFormatDetailsAttribute"/> class.
        /// </summary>
        /// <param name="bytesPerPixel">The number of bytes per pixel for the related format.</param>
        public PixelFormatDetailsAttribute(int bytesPerPixel)
        {
            if (bytesPerPixel <= 0)
            {
                throw new ArgumentException($"A valid {nameof(bytesPerPixel)} are required.");
            }

            BytesPerPixel = bytesPerPixel;
        }

        /// <summary>
        /// Gets the number of bytes per pixel for the related format.
        /// </summary>
        public int BytesPerPixel { get; }

        /// <summary>
        /// Gets the details for a given pixel format.
        /// </summary>
        /// <param name="pixelFormat">The pixel format.</param>
        /// <returns>The details for the pixel format.</returns>
        public static PixelFormatDetailsAttribute GetPixelFormatDetails(PixelFormat pixelFormat)
        {
            var type = typeof(PixelFormat);
            var memberInfo = type.GetMember(pixelFormat.ToString());

            var attributes = memberInfo[0].GetCustomAttributes(typeof(PixelFormatDetailsAttribute), false);
            var attribute = attributes.Length > 0 ? attributes[0] as PixelFormatDetailsAttribute : null;
            if (attribute == null)
            {
                throw new Exception($"A {nameof(PixelFormatDetailsAttribute)} was not assigned to the {nameof(PixelFormat)} {pixelFormat.ToString()}.");
            }

            return attribute;
        }
    }
}
