using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A model.
    /// </summary>
    public class Model : BaseMedia
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class.
        /// </summary>
        /// <param name="meshes">The meshes in the model.</param>
        /// <param name="embeddedTextures">Optional, the embedded textures in the model, to be referenced by nested meshes.</param>
        /// <param name="skeletonRoot">Optional, the skeleton root for the model.</param>
        /// <param name="embeddedAnimationCollection">Optional, the animations embedded in the model.</param>
        /// <param name="sourceStream">The source <see cref="Stream"/> used to read this model.</param>
        public Model(ReadOnlyCollection<Mesh> meshes,
                     ReadOnlyCollection<Texture> embeddedTextures = null,
                     Bone skeletonRoot = null,
                     AnimationCollection embeddedAnimationCollection = null,
                     Stream sourceStream = null)
            : base(sourceStream)
        {
            Meshes = meshes ?? throw new ArgumentNullException(nameof(meshes));
            EmbeddedTextures = embeddedTextures ?? new ReadOnlyCollection<Texture>(new Texture[0]);
            SkeletonRoot = skeletonRoot;
            EmbeddedAnimationCollection = embeddedAnimationCollection ?? new AnimationCollection(new Animation[0]);
        }

        /// <summary>
        /// Gets the animations embedded in the model.
        /// </summary>
        public AnimationCollection EmbeddedAnimationCollection { get; }

        /// <summary>
        /// Gets the embedded textures in the model, to be used by nested meshes.
        /// </summary>
        public IReadOnlyList<Texture> EmbeddedTextures { get; }

        /// <summary>
        /// Gets the meshes in the model.
        /// </summary>
        public IReadOnlyList<Mesh> Meshes { get; }

        /// <summary>
        /// Gets the skeleton root for the model.
        /// </summary>
        public Bone SkeletonRoot { get; }
    }
}
