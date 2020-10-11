using System.Numerics;

namespace DomainDriveGameEngine.Media.Models
{
    /// <summary>
    /// A struct describing a single vertex in a mesh.
    /// </summary>
    public struct Vertex
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> struct.
        /// </summary>
        /// <param name="position">The position of the vertex.</param>
        /// <param name="normal">The normal of the vertex.</param>
        /// <param name="tangent">The tangent of the vertex.</param>
        /// <param name="color">The color of the vertex.</param>
        /// <param name="textureCoordinate">The texture coordinate of the vertex.</param>
        public Vertex(Vector3 position, Vector3 normal, Vector3 tangent, VertexColor color, Vector2 textureCoordinate)
        {
            Position = position;
            Normal = normal;
            Tangent = tangent;
            Color = color;
            TextureCoordinate = textureCoordinate;
        }

        /// <summary>
        /// Gets the color of the vertex.
        /// </summary>
        public VertexColor Color { get; }

        /// <summary>
        /// Gets the normal of the vertex.
        /// </summary>
        public Vector3 Normal { get; }

        /// <summary>
        /// Gets the position of the vertex.
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        /// Gets the tangent of the vertex.
        /// </summary>
        public Vector3 Tangent { get; }

        /// <summary>
        /// Gets the texture coordinate of the vertex.
        /// </summary>
        public Vector2 TextureCoordinate { get; }
    }
}
