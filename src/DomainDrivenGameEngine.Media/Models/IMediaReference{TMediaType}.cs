namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// An interface to a reference to a piece of media.
    /// </summary>
    /// <typeparam name="TMediaType">The type of media this is a reference for.</typeparam>
    public interface IMediaReference<TMediaType>
        where TMediaType : class, IMedia
    {
        /// <summary>
        /// Gets the ID of this reference.
        /// </summary>
        int Id { get; }
    }
}
