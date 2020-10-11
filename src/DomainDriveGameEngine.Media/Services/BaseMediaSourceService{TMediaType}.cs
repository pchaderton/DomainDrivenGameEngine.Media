using System;
using System.Collections.Generic;
using System.Linq;
using DomainDriveGameEngine.Media.Models;

namespace DomainDriveGameEngine.Media.Services
{
    /// <summary>
    /// A base service for sourcing media, loading a media domain model using a given implementation.
    /// </summary>
    /// <typeparam name="TMediaType">The type of media this service loads.</typeparam>
    public abstract class BaseMediaSourceService<TMediaType> : IMediaSourceService<TMediaType>
        where TMediaType : class, IMedia
    {
        /// <summary>
        /// A lookup of extensions that are supported by this service, including the '.' prefix.
        /// </summary>
        private readonly HashSet<string> _supportedExtensionLookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMediaSourceService{TMediaType}"/> class.
        /// </summary>
        /// <param name="extensions">The extensions supported by this source service.</param>
        protected BaseMediaSourceService(IReadOnlyCollection<string> extensions)
        {
            if (extensions == null)
            {
                throw new ArgumentNullException(nameof(extensions));
            }

            if (extensions.Count == 0)
            {
                throw new ArgumentException($"At least one entry in {nameof(extensions)} is required.");
            }

            if (extensions.Any(ext => !ext.StartsWith('.')))
            {
                throw new ArgumentException($"All entries in {nameof(extensions)} must start with a '.' character.");
            }

            _supportedExtensionLookup = extensions.Select(ext => ext.ToLowerInvariant()).ToHashSet();
        }

        /// <summary>
        /// Checks to see if this source supports loading the given extension.
        /// </summary>
        /// <param name="extension">The extension to check against, including the '.' prefix.</param>
        /// <returns><c>true</c> if this service supports loading the given media.</returns>
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
        /// Loads a given piece of media from the specified path.
        /// </summary>
        /// <param name="path">The path to the media to load.</param>
        /// <returns>The loaded media object.</returns>
        public abstract TMediaType Load(string path);
    }
}
