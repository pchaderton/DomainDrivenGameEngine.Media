using System;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// Usage hints for how a model suggests a texture be used.
    /// </summary>
    [Flags]
    public enum TextureUsageType
    {
        /// <summary>
        /// Unknown usage type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// A diffuse map.
        /// </summary>
        Diffuse = 1 << 0,

        /// <summary>
        /// A normal map.
        /// </summary>
        Normal = 1 << 1,

        /// <summary>
        /// A specular map.
        /// </summary>
        Specular = 1 << 2,

        /// <summary>
        /// An ambient occlusion map.
        /// </summary>
        Ambient = 1 << 3,

        /// <summary>
        /// A height map.
        /// </summary>
        Height = 1 << 4,

        /// <summary>
        /// An albedo map.
        /// </summary>
        Albedo = 1 << 5,

        /// <summary>
        /// A metalness map.
        /// </summary>
        Metalness = 1 << 6,

        /// <summary>
        /// A roughness map.
        /// </summary>
        Roughness = 1 << 7,

        /// <summary>
        /// An emission map.
        /// </summary>
        Emission = 1 << 8,
    }
}
