using System;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A shader.
    /// </summary>
    public class Shader : IMedia
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Shader"/> class.
        /// </summary>
        /// <param name="source">The source needed to compile the shader.</param>
        public Shader(string source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        /// <summary>
        /// Gets the sources needed to compile the shader.
        /// </summary>
        public string Source { get; }
    }
}
