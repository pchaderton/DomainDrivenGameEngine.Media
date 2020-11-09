using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A single vertex in a mesh.
    /// </summary>
    public class Vertex
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> class.
        /// </summary>
        /// <param name="position">The position of the vertex.</param>
        /// <param name="normal">The normal of the vertex.</param>
        /// <param name="tangent">The tangent of the vertex.</param>
        /// <param name="color">The color of the vertex.</param>
        /// <param name="textureCoordinate">The texture coordinate of the vertex.</param>
        /// <param name="boneIndices">Optional, the indices of the bones that affect this vertex.</param>
        /// <param name="boneWeights">Optional, the weights of the bones that affect this vertex.</param>
        public Vertex(Vector3 position,
                      Vector3 normal,
                      Vector3 tangent,
                      Color color,
                      Vector2 textureCoordinate,
                      ReadOnlyCollection<uint> boneIndices = null,
                      ReadOnlyCollection<float> boneWeights = null)
        {
            Position = position;
            Normal = normal;
            Tangent = tangent;
            Color = color;
            TextureCoordinate = textureCoordinate;
            BoneIndices = boneIndices ?? new ReadOnlyCollection<uint>(new uint[0]);
            BoneWeights = boneWeights ?? new ReadOnlyCollection<float>(new float[0]);
        }

        /// <summary>
        /// Gets the indices of the bones that affect this vertex.
        /// </summary>
        public IReadOnlyList<uint> BoneIndices { get; }

        /// <summary>
        /// Gets the weights of the bones that affect this vertex.
        /// </summary>
        public IReadOnlyList<float> BoneWeights { get; }

        /// <summary>
        /// Gets the color of the vertex.
        /// </summary>
        public Color Color { get; }

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
