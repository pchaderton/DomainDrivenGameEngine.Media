namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// The supported pixel formats.
    /// </summary>
    public enum PixelFormat
    {
        /// <summary>
        /// A three channel pixel format including red, green and blue channels, using one unsigned byte per channel.
        /// </summary>
        [PixelFormatDetails(bytesPerPixel: 3)]
        Rgb8,

        /// <summary>
        /// A four channel pixel format including red, green, blue and alpha channels, using one unsigned byte per channel.
        /// </summary>
        [PixelFormatDetails(bytesPerPixel: 4)]
        Rgba8,
    }
}
