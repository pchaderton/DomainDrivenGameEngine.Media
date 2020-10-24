using System.Collections.Generic;
using System.Numerics;
using DomainDrivenGameEngine.Media.Models;

namespace DomainDrivenGameEngine.Media.Services.Extensions
{
    /// <summary>
    /// Options for use in generating basic models.
    /// </summary>
    public struct BasicModelOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicModelOptions"/> struct.
        /// </summary>
        /// <param name="color">The color to apply to each vertex.</param>
        /// <param name="offset">The offset amount to shift the model vertices by.</param>
        /// <param name="texturePaths">The paths to the textures to use for this basic model.</param>
        /// <param name="textureReferences">The references to the textures to use for this basic model.</param>
        /// <param name="defaultBlendMode">The default blend mode to use for this basic model.</param>
        /// <param name="defaultShaderPaths">The paths to use for generating the default shader to use for rendering this basic model.</param>
        /// <param name="defaultShaderReferences">The references to the default shaders to use for rendering this basic model.</param>
        public BasicModelOptions(VertexColor? color = null,
                                 Vector3? offset = null,
                                 IReadOnlyCollection<string> texturePaths = null,
                                 IReadOnlyCollection<IMediaReference<Texture>> textureReferences = null,
                                 BlendMode defaultBlendMode = BlendMode.None,
                                 IReadOnlyCollection<string> defaultShaderPaths = null,
                                 IReadOnlyCollection<IMediaReference<Shader>> defaultShaderReferences = null)
        {
            Color = color ?? new VertexColor(1.0f, 1.0f, 1.0f, 1.0f);
            Offset = offset ?? Vector3.Zero;
            TexturePaths = texturePaths;
            TextureReferences = textureReferences;
            DefaultBlendMode = defaultBlendMode;
            DefaultShaderPaths = defaultShaderPaths;
            DefaultShaderReferences = defaultShaderReferences;
        }

        /// <summary>
        /// Gets the default blend mode to use for this basic model.
        /// </summary>
        public BlendMode DefaultBlendMode { get; }

        /// <summary>
        /// Gets the paths to use for generating the default shader to use for rendering this basic model.
        /// </summary>
        public IReadOnlyCollection<string> DefaultShaderPaths { get; }

        /// <summary>
        /// Gets the references to the default shader to use for rendering this basic model.
        /// </summary>
        public IReadOnlyCollection<IMediaReference<Shader>> DefaultShaderReferences { get; }

        /// <summary>
        /// Gets the color to apply to each vertex.
        /// </summary>
        public VertexColor Color { get; }

        /// <summary>
        /// Gets the offset amount to shift the model vertices by.
        /// </summary>
        public Vector3 Offset { get; }

        /// <summary>
        /// Gets the paths to the textures to use for this basic model.
        /// </summary>
        public IReadOnlyCollection<string> TexturePaths { get; }

        /// <summary>
        /// Gets the references to the textures to use for this basic model.
        /// </summary>
        public IReadOnlyCollection<IMediaReference<Texture>> TextureReferences { get; }
    }
}
