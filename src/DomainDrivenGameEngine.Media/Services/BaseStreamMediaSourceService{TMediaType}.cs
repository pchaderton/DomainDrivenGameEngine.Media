using System;
using System.Collections.Generic;
using System.IO;
using DomainDrivenGameEngine.Media.Models;

namespace DomainDrivenGameEngine.Media.Services
{
    /// <summary>
    /// A base service for sourcing media through a <see cref="Stream"/> object, loading it into a domain media model.
    /// </summary>
    /// <typeparam name="TMediaType">The type of media this service loads.</typeparam>
    public abstract class BaseStreamMediaSourceService<TMediaType> : BaseMediaSourceService<TMediaType>
        where TMediaType : class, IMedia
    {
        /// <summary>
        /// The service to use for reading file streams.
        /// </summary>
        private readonly IFileStreamService _fileStreamService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseStreamMediaSourceService{TMediaType}"/> class.
        /// </summary>
        /// <param name="extensions">The extensions supported by this source service.</param>
        /// <param name="fileStreamService">The service to use for reading file streams.</param>
        protected BaseStreamMediaSourceService(IReadOnlyCollection<string> extensions, IFileStreamService fileStreamService)
            : base(extensions)
        {
            _fileStreamService = fileStreamService ?? throw new ArgumentNullException(nameof(fileStreamService));
        }

        /// <inheritdoc/>
        public override TMediaType Load(string path)
        {
            using (var stream = _fileStreamService.OpenFileStream(path))
            {
                return Load(stream);
            }
        }

        /// <summary>
        /// Loads a given piece of media from the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load media from.</param>
        /// <returns>The loaded media object.</returns>
        public abstract TMediaType Load(Stream stream);
    }
}
