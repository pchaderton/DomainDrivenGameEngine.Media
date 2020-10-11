using DomainDriveGameEngine.Media.Models;

namespace DomainDriveGameEngine.Media.Services
{
    /// <summary>
    /// An interface to a service for sourcing media, loading a media domain model using a given implementation.
    /// </summary>
    /// <typeparam name="TMediaType">The type of media this service loads.</typeparam>
    public interface IMediaSourceService<TMediaType>
        where TMediaType : class, IMedia
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
        /// <param name="path">The path to the media to load.</param>
        /// <returns>The loaded media object.</returns>
        TMediaType Load(string path);
    }
}
