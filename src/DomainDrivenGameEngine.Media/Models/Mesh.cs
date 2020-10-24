using System;
using System.Collections.Generic;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A struct describing a mesh in a model.
    /// </summary>
    public struct Mesh
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mesh"/> struct.
        /// </summary>
        /// <param name="vertices">The vertices of the mesh.</param>
        /// <param name="indices">The indices of the vertices for each triangle in the mesh.</param>
        /// <param name="texturePaths">Paths to any textures for the mesh.  If this and textureReferences are provided, both must have the same number of elements.</param>
        /// <param name="textureReferences">Paths to any texture references for the mesh.  If this and texturePaths are provided, both must have the same number of elements.</param>
        /// <param name="defaultBlendMode">The default blend mode to use when rendering this mesh.</param>
        /// <param name="defaultShaderPaths">The paths to use for the default shader to use for rendering this mesh.</param>
        /// <param name="defaultShaderReference">The reference to the default shader to use for rendering this mesh.</param>
        public Mesh(IReadOnlyCollection<Vertex> vertices,
                    IReadOnlyCollection<uint> indices,
                    IReadOnlyCollection<string> texturePaths = null,
                    IReadOnlyCollection<IMediaReference<Texture>> textureReferences = null,
                    BlendMode defaultBlendMode = BlendMode.None,
                    IReadOnlyCollection<string> defaultShaderPaths = null,
                    IMediaReference<Shader> defaultShaderReference = null)
        {
            Vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
            Indices = indices ?? throw new ArgumentNullException(nameof(indices));

            if (texturePaths != null && textureReferences != null && texturePaths.Count != textureReferences.Count)
            {
                throw new ArgumentException($"If both {nameof(texturePaths)} and {nameof(textureReferences)} are provided, both must have the same number of elements.");
            }

            TexturePaths = texturePaths;
            TextureReferences = textureReferences;

            DefaultBlendMode = defaultBlendMode;
            DefaultShaderPaths = defaultShaderPaths;
            DefaultShaderReference = defaultShaderReference;
        }

        /// <summary>
        /// Gets the default blend mode to use when rendering this mesh.
        /// </summary>
        public BlendMode DefaultBlendMode { get; }

        /// <summary>
        /// Gets the paths to use for the default shader to use for rendering this mesh.
        /// </summary>
        public IReadOnlyCollection<string> DefaultShaderPaths { get; }

        /// <summary>
        /// Gets the reference to the default shader to use for rendering this mesh.
        /// </summary>
        public IMediaReference<Shader> DefaultShaderReference { get; }

        /// <summary>
        /// Gets the indices of the vertices for each triangle in the mesh.
        /// </summary>
        public IReadOnlyCollection<uint> Indices { get; }

        /// <summary>
        /// Gets the paths to any textures for the mesh.
        /// </summary>
        public IReadOnlyCollection<string> TexturePaths { get; }

        /// <summary>
        /// Gets the references to any textures for the mesh.
        /// </summary>
        public IReadOnlyCollection<IMediaReference<Texture>> TextureReferences { get; }

        /// <summary>
        /// Gets the vertices of the mesh.
        /// </summary>
        public IReadOnlyCollection<Vertex> Vertices { get; }
    }
}
