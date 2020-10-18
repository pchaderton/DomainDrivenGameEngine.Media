namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// An enum describing different blend modes available to pieces of media.
    /// </summary>
    public enum BlendMode
    {
        /// <summary>
        /// No blending is enabled.
        /// </summary>
        None,

        /// <summary>
        /// Overlay blending is enabled.
        /// </summary>
        Overlay,

        /// <summary>
        /// Addition blending is enabled.
        /// </summary>
        Addition,

        /// <summary>
        /// Multiply blending is enabled.
        /// </summary>
        Multiply,
    }
}
