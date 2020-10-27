using System;
using System.Collections.Generic;
using System.IO;
using DomainDrivenGameEngine.Media.Models;

namespace DomainDrivenGameEngine.Media.Services
{
    /// <summary>
    /// A base service for sourcing media through a <see cref="Stream"/> object, loading it into a domain media model.
    /// </summary>
    /// <remarks>
    /// If the isDisposeAfterLoadEnabled constructor parameter is set to false, it is up to either the source or the
    /// implementation to handle disposing of the loaded file stream (unless an exception is thrown, at which point
    /// the stream will be disposed of automatically).
    /// </remarks>
    /// <typeparam name="TMediaType">The type of media this service loads.</typeparam>
    public abstract class BaseStreamMediaSourceService<TMediaType> : BaseMediaSourceService<TMediaType>
        where TMediaType : class, IMedia
    {
        /// <summary>
        /// The service to use for reading file streams.
        /// </summary>
        private readonly IFileStreamService _fileStreamService;

        /// <summary>
        /// A flag which defines if disposing of a file stream after loading is enabled or not.
        /// </summary>
        private readonly bool _isDisposeAfterLoadEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseStreamMediaSourceService{TMediaType}"/> class.
        /// </summary>
        /// <param name="extensions">The extensions supported by this source service.</param>
        /// <param name="fileStreamService">The service to use for reading file streams.</param>
        /// <param name="isDisposeAfterLoadEnabled">Defaults to true.  If true, disposes of the file stream after a load is finished.</param>
        protected BaseStreamMediaSourceService(IReadOnlyCollection<string> extensions,
                                               IFileStreamService fileStreamService,
                                               bool isDisposeAfterLoadEnabled = true)
            : base(extensions)
        {
            _fileStreamService = fileStreamService ?? throw new ArgumentNullException(nameof(fileStreamService));
            _isDisposeAfterLoadEnabled = isDisposeAfterLoadEnabled;
        }

        /// <inheritdoc/>
        public override TMediaType Load(string path)
        {
            if (_isDisposeAfterLoadEnabled)
            {
                // Automatically dispose of the stream after loading is complete.
                using (var stream = _fileStreamService.OpenFileStream(path))
                {
                    return Load(stream, path);
                }
            }
            else
            {
                // Don't dispose of the stream here unless an exception is thrown by the implemented load method.
                var stream = _fileStreamService.OpenFileStream(path);
                try
                {
                    return Load(stream, path);
                }
                catch
                {
                    stream.Dispose();
                    throw;
                }
            }
        }

        /// <summary>
        /// Loads a given piece of media from the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load media from.</param>
        /// <param name="path">The path used to generate the <see cref="Stream"/>.</param>
        /// <returns>The loaded media object.</returns>
        public abstract TMediaType Load(Stream stream, string path);
    }
}
