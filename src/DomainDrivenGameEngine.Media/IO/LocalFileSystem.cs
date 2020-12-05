using System.IO;

namespace DomainDrivenGameEngine.Media.IO
{
    /// <summary>
    /// A class for accessing files from the local file system.
    /// </summary>
    public class LocalFileSystem : IFileSystem
    {
        /// <summary>
        /// Checks to see if a file exists or not.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the file exists.</returns>
        public bool DoesFileExist(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Gets the file extension for a given path.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The file extension, including prefixed '.'.</returns>
        public string GetFileExtension(string path)
        {
            return Path.GetExtension(path);
        }

        /// <summary>
        /// Gets the fully qualified path for a given input path.
        /// </summary>
        /// <param name="path">The input path to fully qualify.</param>
        /// <returns>The fully qualified path.</returns>
        public string GetFullyQualifiedPath(string path)
        {
            return Path.GetFullPath(path);
        }

        /// <summary>
        /// Gets the fully qualified relative path of one file compared to another.
        /// </summary>
        /// <param name="path">The original file path.</param>
        /// <param name="relativePath">The relative path in comparison to the original file path.</param>
        /// <returns>The path to the file relative to the original path.</returns>
        public string GetFullyQualifiedRelativePath(string path, string relativePath)
        {
            var directory = Path.GetDirectoryName(path);
            var newPath = Path.Combine(directory, relativePath);
            return Path.GetFullPath(newPath);
        }

        /// <summary>
        /// Checks to see if a path is fully qualified.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the path is fully qualified.</returns>
        public bool IsPathFullyQualified(string path)
        {
            return Path.IsPathFullyQualified(path);
        }

        /// <summary>
        /// Opens a <see cref="Stream"/> to read a file from the file system.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>A <see cref="Stream"/> for streaming the contents of the file.</returns>
        public Stream OpenFile(string path)
        {
            return File.OpenRead(path);
        }
    }
}
