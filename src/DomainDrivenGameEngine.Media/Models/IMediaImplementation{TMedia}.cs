using System;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// An implementation of a loaded piece of media.
    /// </summary>
    /// <typeparam name="TMedia">The type of media this is an implementation for.</typeparam>
    public interface IMediaImplementation<TMedia>
        where TMedia : class, IMedia
    {
    }
}
