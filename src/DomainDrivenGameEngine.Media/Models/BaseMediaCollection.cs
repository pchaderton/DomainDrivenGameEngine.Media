using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// An abstract base class for all media objects that are collections.
    /// </summary>
    /// <typeparam name="TCollectionItem">The type of item in this collection.</typeparam>
    public abstract class BaseMediaCollection<TCollectionItem> : ReadOnlyCollection<TCollectionItem>, IMedia
    {
        /// <summary>
        /// The <see cref="Stream"/> used to read this media.
        /// </summary>
        private Stream _sourceStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMediaCollection{TCollectionItem}"/> class.
        /// </summary>
        /// <param name="items">The initial items in this collection.</param>
        /// <param name="sourceStream">Optional, the <see cref="Stream"/> used to read this media.</param>
        protected BaseMediaCollection(IList<TCollectionItem> items, Stream sourceStream = null)
            : base(items ?? throw new ArgumentNullException(nameof(items)))
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
