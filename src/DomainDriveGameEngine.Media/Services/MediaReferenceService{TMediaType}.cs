using System;
using System.Collections.Generic;
using System.Linq;
using DomainDriveGameEngine.Media.Models;

namespace DomainDriveGameEngine.Media.Services
{
    /// <summary>
    /// A service for keeping track of references to media being used in the business logic of the application.
    /// </summary>
    /// <typeparam name="TMediaType">The type of media this is a reference service for.</typeparam>
    public class MediaReferenceService<TMediaType> : IMediaReferenceService<TMediaType>
        where TMediaType : class, IMedia
    {
        /// <summary>
        /// A list of newly requested references by the caller.
        /// </summary>
        private readonly IList<IReference<TMediaType>> _newReferences;

        /// <summary>
        /// A list of newly requested references by the caller that also have provided media.
        /// </summary>
        private readonly IList<Tuple<IReference<TMediaType>, TMediaType>> _newReferencesWithMedia;

        /// <summary>
        /// A list of references that are no longer being referenced by the caller.
        /// </summary>
        private readonly IList<IReference<TMediaType>> _oldReferences;

        /// <summary>
        /// A lookup of reference counts by reference ID.
        /// </summary>
        private readonly IDictionary<int, int> _referenceCountsByReferenceId;

        /// <summary>
        /// A lookup of previously referenced media by their joined paths.
        /// </summary>
        private readonly IDictionary<string, Reference<TMediaType>> _referencesByJoinedPaths;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaReferenceService{TMediaType}"/> class.
        /// </summary>
        public MediaReferenceService()
        {
            _newReferences = new List<IReference<TMediaType>>();
            _newReferencesWithMedia = new List<Tuple<IReference<TMediaType>, TMediaType>>();
            _oldReferences = new List<IReference<TMediaType>>();
            _referenceCountsByReferenceId = new Dictionary<int, int>();
            _referencesByJoinedPaths = new Dictionary<string, Reference<TMediaType>>();
        }

        /// <summary>
        /// References a piece of media.  If this is the first time referencing a given piece of media, lists the reference to be loaded.
        /// </summary>
        /// <param name="paths">One or more strings containing the file paths to reference.</param>
        /// <returns>A <see cref="Reference{TMediaType}"/> object which refers to the media at the specified paths.</returns>
        public IReference<TMediaType> Reference(params string[] paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException(nameof(paths));
            }

            if (paths.Length == 0)
            {
                throw new ArgumentException($"At least one valid entry in {nameof(paths)} is required.");
            }

            var joinedPaths = Reference<TMediaType>.GetJoinedReferencePaths(paths);
            if (_referencesByJoinedPaths.TryGetValue(joinedPaths, out var existingReference))
            {
                _referenceCountsByReferenceId[existingReference.Id]++;
                return existingReference;
            }

            var reference = new Reference<TMediaType>(paths);
            _referencesByJoinedPaths.Add(joinedPaths, reference);
            _referenceCountsByReferenceId.Add(reference.Id, 1);
            _newReferences.Add(reference);

            return reference;
        }

        /// <summary>
        /// References a provided piece of media and lists it to be loaded.
        /// </summary>
        /// <param name="media">The media to reference.</param>
        /// <returns>A <see cref="Reference{TMediaType}"/> object which refers to the media.</returns>
        public IReference<TMediaType> Reference(TMediaType media)
        {
            if (media == null)
            {
                throw new ArgumentNullException(nameof(media));
            }

            var reference = new Reference<TMediaType>(new string[] { });
            _referenceCountsByReferenceId.Add(reference.Id, 1);

            return reference;
        }

        /// <summary>
        /// Unreferences a previously retrieved reference.  If no references remain, lists the reference to be unloaded.
        /// </summary>
        /// <param name="reference">The <see cref="Reference{TMediaType}"/> to unreference.</param>
        public void Unreference(IReference<TMediaType> reference)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            if (!_referenceCountsByReferenceId.TryGetValue(reference.Id, out var currentReferenceCount))
            {
                throw new ArgumentException($"Reference {reference.Id} is no longer being tracked by the {nameof(MediaReferenceService<TMediaType>)}");
            }

            var newReferenceCount = currentReferenceCount - 1;
            if (newReferenceCount == 0)
            {
                _referencesByJoinedPaths.Remove(reference.GetJoinedPaths());
                _referenceCountsByReferenceId.Remove(reference.Id);
                _oldReferences.Add(reference);
                return;
            }

            _referenceCountsByReferenceId[reference.Id] = newReferenceCount;
        }

        /// <summary>
        /// Gets any newly requested references.  Clears the new reference list.
        /// </summary>
        /// <returns>The newly requested references.</returns>
        public IEnumerable<IReference<TMediaType>> GetNewReferences()
        {
            var newReferences = _newReferences.ToList();

            _newReferences.Clear();

            return newReferences;
        }

        /// <summary>
        /// Gets any newly requested references with provided media.  Clears the new reference list.
        /// </summary>
        /// <returns>The newly requested references.</returns>
        public IEnumerable<Tuple<IReference<TMediaType>, TMediaType>> GetNewReferencesWithMedia()
        {
            var newReferencesWithMedia = _newReferencesWithMedia.ToList();

            _newReferencesWithMedia.Clear();

            return newReferencesWithMedia;
        }

        /// <summary>
        /// Gets any old references that are no longer being referenced.  Clears the old reference list.
        /// </summary>
        /// <returns>The old references.</returns>
        public IEnumerable<IReference<TMediaType>> GetOldReferences()
        {
            var oldReferences = _oldReferences.ToList();

            _oldReferences.Clear();

            return oldReferences;
        }
    }
}
