using System.IO;

namespace DomainDrivenGameEngine.Media.Services
{
    /// <summary>
    /// A service for streaming files from the file system.
    /// </summary>
    public class FileSystemStreamService : IFileStreamService
    {
        /// <summary>
        /// Opens a file stream to a given path.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>A <see cref="Stream"/> for streaming the contents of the file.</returns>
        public Stream OpenFileStream(string path)
        {
            return File.OpenRead(path);
        }
    }
}
