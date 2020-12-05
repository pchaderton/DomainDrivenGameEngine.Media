using System;
using System.IO;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A shader.
    /// </summary>
    public class Shader : BaseMedia
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Shader"/> class.
        /// </summary>
        /// <param name="source">The source needed to compile the shader.</param>
        /// <param name="sourceStream">The source <see cref="Stream"/> used to read this shader.</param>
        public Shader(string source, Stream sourceStream = null)
            : base(sourceStream)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        /// <summary>
        /// Gets the source code needed to compile the shader.
        /// </summary>
        public string Source { get; }
    }
}
