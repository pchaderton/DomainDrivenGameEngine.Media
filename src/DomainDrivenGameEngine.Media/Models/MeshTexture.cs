using System;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A texture referenced by a mesh.
    /// </summary>
    public class MeshTexture
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeshTexture"/> class.
        /// </summary>
        /// <param name="reference">The reference to the texture to use for this mesh texture.</param>
        /// <param name="embeddedTextureIndex">The index to the embedded texture to use for this mesh texture.</param>
        /// <param name="path">The path to the texture to use for this mesh texture.</param>
        /// <param name="usageType">The suggested usage type for this texture.</param>
        public MeshTexture(IMediaReference<Texture> reference = null,
                           uint? embeddedTextureIndex = null,
                           string path = null,
                           TextureUsageType usageType = TextureUsageType.Unknown)
        {
            if (string.IsNullOrWhiteSpace(path) && reference == null && embeddedTextureIndex == null)
            {
                throw new ArgumentException($"At least one {nameof(path)}, {nameof(reference)} or {nameof(embeddedTextureIndex)} is required.");
            }

            if (reference != null)
            {
                Reference = reference;
            }
            else if (embeddedTextureIndex != null)
            {
                EmbeddedTextureIndex = embeddedTextureIndex;
            }
            else
            {
                Path = path;
            }

            UsageType = usageType;
        }

        /// <summary>
        /// Gets the index to the embedded texture to use for this mesh texture.
        /// </summary>
        public uint? EmbeddedTextureIndex { get; }

        /// <summary>
        /// Gets the path to the texture to use for this mesh texture.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the reference to the texture to use for this mesh texture.
        /// </summary>
        public IMediaReference<Texture> Reference { get; }

        /// <summary>
        /// Gets the suggested usage type for this texture.
        /// </summary>
        public TextureUsageType UsageType { get; }
    }
}
