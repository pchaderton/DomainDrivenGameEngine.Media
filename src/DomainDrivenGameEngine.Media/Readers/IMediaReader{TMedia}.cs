using System.IO;
using DomainDrivenGameEngine.Media.Models;

namespace DomainDrivenGameEngine.Media.Readers
{
    /// <summary>
    /// An interface to a class for reading media.
    /// </summary>
    /// <typeparam name="TMedia">The type of media this class reads.</typeparam>
    public interface IMediaReader<TMedia>
        where TMedia : class, IMedia
    {
        /// <summary>
        /// Checks to see if this reader supports loading the given extension.
        /// </summary>
        /// <param name="extension">The extension to check against, including the '.' prefix.</param>
        /// <returns><c>true</c> if this reader supports loading the given media.</returns>
        bool IsExtensionSupported(string extension);

        /// <summary>
        /// Reads a given piece of media from the provided <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> to the file to read.</param>
        /// <param name="path">The path to the file that is the source of the provided <see cref="Stream"/>.</param>
        /// <param name="extension">The extension of the source file.</param>
        /// <returns>The read media object.</returns>
        TMedia Read(Stream stream, string path, string extension);
    }
}
