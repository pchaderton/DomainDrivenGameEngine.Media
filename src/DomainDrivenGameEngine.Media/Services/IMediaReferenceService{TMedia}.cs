using DomainDrivenGameEngine.Media.Models;

namespace DomainDrivenGameEngine.Media.Services
{
    /// <summary>
    /// An interface to a service for tracking references to media being used in the business logic of the application.
    /// </summary>
    /// <typeparam name="TMedia">The type of media this is a reference service for.</typeparam>
    public interface IMediaReferenceService<TMedia>
        where TMedia : class, IMedia
    {
        /// <summary>
        /// References a piece of media.  If this is the first time referencing a given piece of media, lists the reference to be loaded.
        /// </summary>
        /// <param name="paths">One or more strings containing the file paths to reference.</param>
        /// <returns>A <see cref="IMediaFileReference{TMedia}"/> object which refers to the media at the specified paths.</returns>
        IMediaFileReference<TMedia> Reference(params string[] paths);

        /// <summary>
        /// References a provided piece of media and lists it to be loaded.
        /// </summary>
        /// <param name="media">The media to reference.</param>
        /// <returns>A <see cref="IMediaReference{TMedia}"/> object which refers to the media.</returns>
        IMediaReference<TMedia> Reference(params TMedia[] media);

        /// <summary>
        /// Unreferences a previously retrieved reference.  If no references remain, lists the reference to be unloaded.
        /// </summary>
        /// <param name="reference">The <see cref="IMediaReference{TMedia}"/> to unreference.</param>
        void Unreference(IMediaReference<TMedia> reference);
    }
}
