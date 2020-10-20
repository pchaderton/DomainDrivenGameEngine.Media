using System;
using DomainDrivenGameEngine.Media.Models;

namespace DomainDrivenGameEngine.Media.Services
{
    /// <summary>
    /// An interface to a service for keeping track of references to media being used in the business logic of the application.
    /// </summary>
    /// <typeparam name="TMediaType">The type of media this is a reference service for.</typeparam>
    public interface IMediaReferenceService<TMediaType>
        where TMediaType : class, IMedia
    {
        /// <summary>
        /// References a piece of media.  If this is the first time referencing a given piece of media, lists the reference to be loaded.
        /// </summary>
        /// <param name="paths">One or more strings containing the file paths to reference.</param>
        /// <returns>A <see cref="IReference{TMediaType}"/> object which refers to the media at the specified paths.</returns>
        IReference<TMediaType> Reference(params string[] paths);

        /// <summary>
        /// References a provided piece of media and lists it to be loaded.
        /// </summary>
        /// <param name="media">The media to reference.</param>
        /// <returns>A <see cref="IReference{TMediaType}"/> object which refers to the media.</returns>
        IReference<TMediaType> Reference(params TMediaType[] media);

        /// <summary>
        /// Unreferences a previously retrieved reference.  If no references remain, lists the reference to be unloaded.
        /// </summary>
        /// <param name="reference">The <see cref="IReference{TMediaType}"/> to unreference.</param>
        void Unreference(IReference<TMediaType> reference);
    }
}
