using System.IO;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// An abstract base class for all media objects.
    /// </summary>
    public abstract class BaseMedia : IMedia
    {
        /// <summary>
        /// The <see cref="Stream"/> used to read this media.
        /// </summary>
        private Stream _sourceStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMedia"/> class.
        /// </summary>
        /// <param name="sourceStream">Optional, the <see cref="Stream"/> used to read this media.</param>
        protected BaseMedia(Stream sourceStream = null)
        {
            _sourceStream = sourceStream;
        }

        /// <summary>
        /// Release all resources managed by this media.
        /// </summary>
        public virtual void Dispose()
        {
            if (_sourceStream == null)
            {
                return;
            }

            _sourceStream.Dispose();
            _sourceStream = null;
        }
    }
}
