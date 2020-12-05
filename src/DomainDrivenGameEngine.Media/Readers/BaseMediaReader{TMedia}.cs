using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DomainDrivenGameEngine.Media.Models;

namespace DomainDrivenGameEngine.Media.Readers
{
    /// <summary>
    /// An abstract base class for a class for reading media.
    /// </summary>
    /// <typeparam name="TMedia">The type of media this class reads.</typeparam>
    public abstract class BaseMediaReader<TMedia> : IMediaReader<TMedia>
        where TMedia : class, IMedia
    {
        /// <summary>
        /// A lookup of extensions that are supported by this service, including the '.' prefix.
        /// </summary>
        private readonly HashSet<string> _supportedExtensionLookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMediaReader{TMedia}"/> class.
        /// </summary>
        /// <param name="extensions">The extensions supported by this source service.</param>
        protected BaseMediaReader(IEnumerable<string> extensions)
        {
            var supportedExtensionLookup = extensions?.Select(ext => ext.ToLowerInvariant()).ToHashSet() ?? throw new ArgumentNullException(nameof(extensions));

            if (supportedExtensionLookup.Count == 0)
            {
                throw new ArgumentException($"At least one entry in {nameof(extensions)} is required.");
            }

            if (supportedExtensionLookup.Any(ext => !ext.StartsWith('.')))
            {
                throw new ArgumentException($"All entries in {nameof(extensions)} must start with a '.' character.");
            }

            _supportedExtensionLookup = supportedExtensionLookup;
        }

        /// <summary>
        /// Checks to see if this reader supports loading the given extension.
        /// </summary>
        /// <param name="extension">The extension to check against, including the '.' prefix.</param>
        /// <returns><c>true</c> if this reader supports loading the given media.</returns>
        public bool IsExtensionSupported(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException(nameof(extension));
            }

            if (string.IsNullOrWhiteSpace(extension) || !extension.StartsWith('.'))
            {
                throw new ArgumentException($"A valid {nameof(extension)} is required.");
            }

            return _supportedExtensionLookup.Contains(extension.ToLowerInvariant());
        }

        /// <summary>
        /// Reads a given piece of media from the provided <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> to the file to read.</param>
        /// <param name="path">The path to the file that is the source of the provided <see cref="Stream"/>.</param>
        /// <param name="extension">The extension of the source file.</param>
        /// <returns>The read media object.</returns>
        public abstract TMedia Read(Stream stream, string path, string extension);
    }
}
