using System.IO;

namespace DomainDrivenGameEngine.Media.Services
{
    /// <summary>
    /// An interface to a service for accessing files.
    /// </summary>
    public interface IFileAccessService
    {
        /// <summary>
        /// Checks to see if a file exists or not.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the file exists.</returns>
        bool DoesFileExist(string path);

        /// <summary>
        /// Gets the file extension for a given path.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The file extension, including prefixed '.'.</returns>
        string GetFileExtension(string path);

        /// <summary>
        /// Gets the fully qualified path for a given input path.
        /// </summary>
        /// <param name="path">The input path to fully qualify.</param>
        /// <returns>The fully qualified path.</returns>
        string GetFullyQualifiedPath(string path);

        /// <summary>
        /// Gets the fully qualified relative path of one file compared to another.
        /// </summary>
        /// <param name="path">The original file path.</param>
        /// <param name="relativePath">The relative path in comparison to the original file path.</param>
        /// <returns>The path to the file relative to the original path.</returns>
        string GetFullyQualifiedRelativePath(string path, string relativePath);

        /// <summary>
        /// Checks to see if a path is fully qualified.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the path is fully qualified.</returns>
        bool IsPathFullyQualified(string path);

        /// <summary>
        /// Opens a file stream to a given path.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>A <see cref="Stream"/> for streaming the contents of the file.</returns>
        Stream OpenFile(string path);
    }
}
