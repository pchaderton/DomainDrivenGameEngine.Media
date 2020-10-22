using System;
using System.Collections.Generic;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A reference to a piece of media defined by one or more file paths.
    /// </summary>
    /// <typeparam name="TMediaType">The type of media this is a reference for.</typeparam>
    internal class MediaFileReference<TMediaType> : MediaReference<TMediaType>, IMediaFileReference<TMediaType>
        where TMediaType : class, IMedia
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaFileReference{TMediaType}"/> class.
        /// </summary>
        /// <param name="paths">The paths used to generate this reference.</param>
        internal MediaFileReference(IReadOnlyCollection<string> paths)
            : base()
        {
            Paths = paths ?? throw new ArgumentNullException(nameof(paths));
        }

        /// <summary>
        /// Gets the paths used to generate this reference.
        /// </summary>
        public IReadOnlyCollection<string> Paths { get; }

        /// <summary>
        /// Gets paths joined for comparing between references.
        /// </summary>
        /// <param name="paths">The paths to join.</param>
        /// <returns>A string containing the joined paths.</returns>
        public static string GetJoinedReferencePaths(IReadOnlyCollection<string> paths)
        {
            return string.Join(',', paths);
        }

        /// <summary>
        /// Checks to see if this reference is equal to another object.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns><c>true</c> if this reference is equal to the other object.</returns>
        public override bool Equals(object obj)
        {
            var reference = obj as MediaFileReference<TMediaType>;
            if (reference == null)
            {
                return false;
            }

            return Equals(reference);
        }

        /// <summary>
        /// Checks to see if this reference is equal to another reference.
        /// </summary>
        /// <param name="reference">The <see cref="MediaFileReference{TMediaType}"/> to compare against.</param>
        /// <returns><c>true</c> if this reference is equal to the other reference.</returns>
        public bool Equals(MediaFileReference<TMediaType> reference)
        {
            return Id == reference.Id && GetJoinedPaths() == reference.GetJoinedPaths();
        }

        /// <summary>
        /// Returns a hash code for this reference.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return $"{Id},{GetJoinedPaths()}".GetHashCode();
        }

        /// <summary>
        /// Gets the joined paths for this reference.
        /// </summary>
        /// <returns>A <see cref="string"/> containing the joined paths of this reference.</returns>
        public string GetJoinedPaths()
        {
            return GetJoinedReferencePaths(Paths);
        }
    }
}
