using System;
using System.Collections.Generic;

namespace DomainDriveGameEngine.Media.Models
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
        /// <param name="texturePaths">Paths to any textures for the mesh.</param>
        public Mesh(IReadOnlyCollection<Vertex> vertices, IReadOnlyCollection<uint> indices, IReadOnlyCollection<string> texturePaths)
        {
            Vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
            Indices = indices ?? throw new ArgumentNullException(nameof(indices));
            TexturePaths = texturePaths ?? throw new ArgumentNullException(nameof(texturePaths));
        }

        /// <summary>
        /// Gets the indices of the vertices for each triangle in the mesh.
        /// </summary>
        public IReadOnlyCollection<uint> Indices { get; }

        /// <summary>
        /// Gets or the paths to any textures for the mesh.
        /// </summary>
        public IReadOnlyCollection<string> TexturePaths { get; }

        /// <summary>
        /// Gets the vertices of the mesh.
        /// </summary>
        public IReadOnlyCollection<Vertex> Vertices { get; }
    }
}
