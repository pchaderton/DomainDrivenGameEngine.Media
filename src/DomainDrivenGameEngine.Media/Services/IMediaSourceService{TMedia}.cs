using System.IO;
using DomainDrivenGameEngine.Media.Models;

namespace DomainDrivenGameEngine.Media.Services
{
    /// <summary>
    /// An interface to a service for sourcing media, loading a media domain model using a given implementation.
    /// </summary>
    /// <typeparam name="TMedia">The type of media this service loads.</typeparam>
    public interface IMediaSourceService<TMedia>
        where TMedia : class, IMedia
    {
        /// <summary>
        /// Checks to see if this source supports loading the given extension.
        /// </summary>
        /// <param name="extension">The extension to check against, including the '.' prefix.</param>
        /// <returns><c>true</c> if this service supports loading the given media.</returns>
        bool IsExtensionSupported(string extension);

        /// <summary>
        /// Loads a given piece of media from the specified path.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> to the file to load.</param>
        /// <param name="path">The path to the file that is the source of the provided <see cref="Stream"/>.</param>
        /// <param name="extension">The extension of the source file.</param>
        /// <returns>The loaded media object.</returns>
        TMedia Load(Stream stream, string path, string extension);
    }
}
