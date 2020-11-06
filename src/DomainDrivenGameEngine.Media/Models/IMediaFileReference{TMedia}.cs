using System.Collections.Generic;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// An interface for a reference to a piece of media defined by one or more file paths.
    /// </summary>
    /// <typeparam name="TMedia">The type of media this is a reference for.</typeparam>
    public interface IMediaFileReference<TMedia> : IMediaReference<TMedia>
        where TMedia : class, IMedia
    {
        /// <summary>
        /// Gets the paths used to generate this reference.
        /// </summary>
        IReadOnlyList<string> Paths { get; }

        /// <summary>
        /// Gets the joined paths for this reference.
        /// </summary>
        /// <returns>A <see cref="string"/> containing the joined paths of this reference.</returns>
        string GetJoinedPaths();
    }
}
