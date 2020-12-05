namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// The available texture formats.
    /// </summary>
    public enum TextureFormat
    {
        /// <summary>
        /// An uncompressed three channel texture format, where each pixel includes a red, green and blue channel, each channel using one unsigned byte.
        /// </summary>
        Rgb24,

        /// <summary>
        /// An uncompressed four channel texture format, where each pixel includes a red, green, blue and alpha channel, each channel using one unsigned byte.
        /// </summary>
        Rgba32,
    }
}
