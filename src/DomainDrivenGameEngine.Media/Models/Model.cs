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
        /// <param name="embeddedTextures">Optional, the embedded textures in the model, to be referenced by nested meshes.</param>
        public Model(IReadOnlyCollection<Mesh> meshes, IReadOnlyCollection<Texture> embeddedTextures = null)
        {
            Meshes = meshes ?? throw new ArgumentNullException(nameof(meshes));
            EmbeddedTextures = embeddedTextures ?? throw new ArgumentNullException(nameof(embeddedTextures));
        }

        /// <summary>
        /// Gets the embedded textures available for the model, to be used by nested meshes.
        /// </summary>
        public IReadOnlyCollection<Texture> EmbeddedTextures { get; }

        /// <summary>
        /// Gets the meshes in the model.
        /// </summary>
        public IReadOnlyCollection<Mesh> Meshes { get; }
    }
}
