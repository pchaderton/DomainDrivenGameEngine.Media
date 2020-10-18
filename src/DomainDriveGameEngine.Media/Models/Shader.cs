using System;
using System.Collections.Generic;

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
        /// <param name="sources">The sources needed to compile the shader.</param>
        public Shader(IReadOnlyCollection<string> sources)
        {
            Sources = sources ?? throw new ArgumentNullException(nameof(sources));
        }

        /// <summary>
        /// Gets the sources needed to compile the shader.
        /// </summary>
        public IReadOnlyCollection<string> Sources { get; }
    }
}
