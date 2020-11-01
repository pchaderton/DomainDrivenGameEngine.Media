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
        /// <param name="skeletonRoot">Optional, the skeleton root for the model.</param>
        /// <param name="animationCollection">Optional, the animations included with this model.</param>
        public Model(IReadOnlyCollection<Mesh> meshes,
                     IReadOnlyCollection<Texture> embeddedTextures = null,
                     Bone skeletonRoot = null,
                     AnimationCollection animationCollection = null)
        {
            Meshes = meshes ?? throw new ArgumentNullException(nameof(meshes));
            EmbeddedTextures = embeddedTextures;
            SkeletonRoot = skeletonRoot;
            AnimationCollection = animationCollection;
        }

        /// <summary>
        /// Gets the animations included with this model.
        /// </summary>
        public AnimationCollection AnimationCollection { get; }

        /// <summary>
        /// Gets the embedded textures in the model, to be used by nested meshes.
        /// </summary>
        public IReadOnlyCollection<Texture> EmbeddedTextures { get; }

        /// <summary>
        /// Gets the meshes in the model.
        /// </summary>
        public IReadOnlyCollection<Mesh> Meshes { get; }

        /// <summary>
        /// Gets the skeleton root for the model.
        /// </summary>
        public Bone SkeletonRoot { get; }
    }
}
