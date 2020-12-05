using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using DomainDrivenGameEngine.Media.Models;

namespace DomainDrivenGameEngine.Media.Extensions
{
    /// <summary>
    /// Options for use in generating basic models.
    /// </summary>
    public class BasicModelOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicModelOptions"/> class.
        /// </summary>
        /// <param name="color">The color to apply to each vertex.</param>
        /// <param name="offset">The offset amount to shift the model vertices by.</param>
        /// <param name="textures">The textures to use for this basic model.</param>
        /// <param name="defaultBlendMode">The default blend mode to use for this basic model.</param>
        public BasicModelOptions(Color? color = null,
                                 Vector3? offset = null,
                                 IEnumerable<MeshTexture> textures = null,
                                 BlendMode defaultBlendMode = BlendMode.None)
        {
            if (textures != null && textures.Any(mt => mt.EmbeddedTextureIndex != null))
            {
                throw new ArgumentException($"Using {nameof(MeshTexture.EmbeddedTextureIndex)} is not supported when creating basic models.");
            }

            Color = color ?? new Color(1.0f, 1.0f, 1.0f, 1.0f);
            Offset = offset ?? Vector3.Zero;
            Textures = textures != null
                ? new ReadOnlyCollection<MeshTexture>(textures?.ToArray())
                : null;
            DefaultBlendMode = defaultBlendMode;
        }

        /// <summary>
        /// Gets the default blend mode to use for this basic model.
        /// </summary>
        public BlendMode DefaultBlendMode { get; }

        /// <summary>
        /// Gets the color to apply to each vertex.
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// Gets the offset amount to shift the model vertices by.
        /// </summary>
        public Vector3 Offset { get; }

        /// <summary>
        /// Gets the paths to the textures to use for this basic model.
        /// </summary>
        public ReadOnlyCollection<MeshTexture> Textures { get; }
    }
}
