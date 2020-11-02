using System;
using System.Collections.Generic;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A struct describing a mesh in a model.
    /// </summary>
    public class Mesh
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mesh"/> class.
        /// </summary>
        /// <param name="vertices">The vertices of the mesh.</param>
        /// <param name="indices">The indices of the vertices for each triangle in the mesh.</param>
        /// <param name="meshTextures">The textures used by the mesh.</param>
        /// <param name="defaultBlendMode">The default blend mode to use when rendering this mesh.</param>
        public Mesh(IReadOnlyCollection<Vertex> vertices,
                    IReadOnlyCollection<uint> indices,
                    IReadOnlyCollection<MeshTexture> meshTextures,
                    BlendMode defaultBlendMode = BlendMode.None)
        {
            Vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
            Indices = indices ?? throw new ArgumentNullException(nameof(indices));
            MeshTextures = meshTextures ?? new MeshTexture[0];
            DefaultBlendMode = defaultBlendMode;
        }

        /// <summary>
        /// Gets the default blend mode to use when rendering this mesh.
        /// </summary>
        public BlendMode DefaultBlendMode { get; }

        /// <summary>
        /// Gets the indices of the vertices for each triangle in the mesh.
        /// </summary>
        public IReadOnlyCollection<uint> Indices { get; }

        /// <summary>
        /// Gets the textures used by the mesh.
        /// </summary>
        public IReadOnlyCollection<MeshTexture> MeshTextures { get; }

        /// <summary>
        /// Gets the vertices of the mesh.
        /// </summary>
        public IReadOnlyCollection<Vertex> Vertices { get; }
    }
}
