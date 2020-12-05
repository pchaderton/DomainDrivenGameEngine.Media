namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A reference to a piece of media.
    /// </summary>
    /// <typeparam name="TMedia">The type of media this is a reference for.</typeparam>
    internal class MediaReference<TMedia> : IMediaReference<TMedia>
        where TMedia : class, IMedia
    {
        /// <summary>
        /// A counter for keeping track of the next ID to use for a reference.
        /// </summary>
        private static int _referenceIdCounter = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaReference{TMedia}"/> class.
        /// </summary>
        internal MediaReference()
        {
            Id = ++_referenceIdCounter;
            ReferenceCount = 1;
        }

        /// <summary>
        /// Gets the ID of this reference.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets or sets the number of references being tracked for this reference.
        /// </summary>
        public int ReferenceCount { get; set; }

        /// <summary>
        /// Checks to see if this reference is equal to another object.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns><c>true</c> if this reference is equal to the other object.</returns>
        public override bool Equals(object obj)
        {
            var reference = obj as MediaReference<TMedia>;
            if (reference == null)
            {
                return false;
            }

            return Equals(reference);
        }

        /// <summary>
        /// Checks to see if this reference is equal to another reference.
        /// </summary>
        /// <param name="reference">The <see cref="MediaReference{TMedia}"/> to compare against.</param>
        /// <returns><c>true</c> if this reference is equal to the other reference.</returns>
        public bool Equals(MediaReference<TMedia> reference)
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
