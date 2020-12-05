namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// An interface to a reference to a piece of media.
    /// </summary>
    /// <typeparam name="TMedia">The type of media this is a reference for.</typeparam>
    public interface IMediaReference<TMedia>
        where TMedia : class, IMedia
    {
        /// <summary>
        /// Gets the ID of this reference.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the number of references being tracked for this reference.
        /// </summary>
        int ReferenceCount { get; }
    }
}
