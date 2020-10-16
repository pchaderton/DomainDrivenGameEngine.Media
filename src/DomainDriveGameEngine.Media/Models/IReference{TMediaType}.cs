using System.Collections.Generic;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// An interface to a reference to a piece of media.
    /// </summary>
    /// <typeparam name="TMediaType">The type of media this is a reference for.</typeparam>
    public interface IReference<TMediaType>
        where TMediaType : class, IMedia
    {
        /// <summary>
        /// Gets the ID of this reference.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the paths used to generate this reference.
        /// </summary>
        IReadOnlyCollection<string> Paths { get; }

        /// <summary>
        /// Gets the joined paths for this reference.
        /// </summary>
        /// <returns>A <see cref="string"/> containing the joined paths of this reference.</returns>
        string GetJoinedPaths();
    }
}
