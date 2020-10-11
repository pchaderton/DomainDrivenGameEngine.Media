using System;
using System.Collections.Generic;

namespace DomainDriveGameEngine.Media.Models
{
    /// <summary>
    /// A reference to a piece of media.
    /// </summary>
    /// <typeparam name="TMediaType">The type of media this is a reference for.</typeparam>
    internal class Reference<TMediaType> : IReference<TMediaType>
        where TMediaType : class, IMedia
    {
        /// <summary>
        /// A counter for keeping track of the next ID to use for a reference.
        /// </summary>
        private static int _referenceIdCounter = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Reference{TMediaType}"/> class.
        /// </summary>
        internal Reference()
            : this(new string[] { })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Reference{TMediaType}"/> class.
        /// </summary>
        /// <param name="paths">The paths used to generate this reference.</param>
        internal Reference(IReadOnlyCollection<string> paths)
        {
            Paths = paths ?? throw new ArgumentNullException(nameof(paths));
            Id = ++_referenceIdCounter;
        }

        /// <summary>
        /// Gets the ID of this reference.
        /// </summary>
        public int Id { get; }

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
            var reference = obj as Reference<TMediaType>;
            if (reference == null)
            {
                return false;
            }

            return Equals(reference);
        }

        /// <summary>
        /// Checks to see if this reference is equal to another reference.
        /// </summary>
        /// <param name="reference">The <see cref="Reference{TMediaType}"/> to compare against.</param>
        /// <returns><c>true</c> if this reference is equal to the other reference.</returns>
        public bool Equals(Reference<TMediaType> reference)
        {
            return Id == reference.Id && GetJoinedPaths() == reference.GetJoinedPaths();
        }

        /// <summary>
        /// Returns a hash code for this reference.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return GetJoinedPaths().GetHashCode();
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
