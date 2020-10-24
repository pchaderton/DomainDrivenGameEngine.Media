namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A reference to a piece of media.
    /// </summary>
    /// <typeparam name="TMediaType">The type of media this is a reference for.</typeparam>
    internal class MediaReference<TMediaType> : IMediaReference<TMediaType>
        where TMediaType : class, IMedia
    {
        /// <summary>
        /// A counter for keeping track of the next ID to use for a reference.
        /// </summary>
        private static int _referenceIdCounter = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaReference{TMediaType}"/> class.
        /// </summary>
        internal MediaReference()
        {
            Id = ++_referenceIdCounter;
        }

        /// <summary>
        /// Gets the ID of this reference.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Checks to see if this reference is equal to another object.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns><c>true</c> if this reference is equal to the other object.</returns>
        public override bool Equals(object obj)
        {
            var reference = obj as MediaReference<TMediaType>;
            if (reference == null)
            {
                return false;
            }

            return Equals(reference);
        }

        /// <summary>
        /// Checks to see if this reference is equal to another reference.
        /// </summary>
        /// <param name="reference">The <see cref="MediaReference{TMediaType}"/> to compare against.</param>
        /// <returns><c>true</c> if this reference is equal to the other reference.</returns>
        public bool Equals(MediaReference<TMediaType> reference)
        {
            return Id == reference.Id;
        }

        /// <summary>
        /// Returns a hash code for this reference.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
