using System;
using System.Collections.Generic;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A model.
    /// </summary>
    public class Model : IMedia
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class.
        /// </summary>
        /// <param name="meshes">The meshes in the model.</param>
        public Model(IReadOnlyCollection<Mesh> meshes)
        {
            Meshes = meshes ?? throw new ArgumentNullException(nameof(meshes));
        }

        /// <summary>
        /// Gets the meshes in the model.
        /// </summary>
        public IReadOnlyCollection<Mesh> Meshes { get; }
    }
}
