using System.IO;

namespace DomainDrivenGameEngine.Media.Services
{
    /// <summary>
    /// An interface to a service for streaming files.
    /// </summary>
    public interface IFileStreamService
    {
        /// <summary>
        /// Opens a file stream to a given path.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>A <see cref="Stream"/> for streaming the contents of the file.</returns>
        Stream OpenFileStream(string path);
    }
}
