using System;
using DomainDrivenGameEngine.Media.Models;

namespace DomainDrivenGameEngine.Media.Services.Extensions
{
    /// <summary>
    /// Extensions methods for generating and referencing basic textures via a <see cref="IMediaReferenceService{Texture}"/>.
    /// </summary>
    public static class BasicTextureReferenceServiceExtensions
    {
        /// <summary>
        /// Creates and references a single color texture.
        /// </summary>
        /// <param name="textureReferenceService">The <see cref="IMediaReferenceService{Texture}"/> to reference the generated texture with.</param>
        /// <param name="red">The red value to apply to the texture.</param>
        /// <param name="green">The green value to apply to the texture.</param>
        /// <param name="blue">The blue value to apply to the texture.</param>
        /// <returns>A <see cref="IReference{Texture}"/> to the generated texture.</returns>
        public static IReference<Texture> ReferenceSingleColorTexture(this IMediaReferenceService<Texture> textureReferenceService, byte red, byte green, byte blue)
        {
            if (textureReferenceService == null)
            {
                throw new ArgumentNullException(nameof(textureReferenceService));
            }

            return textureReferenceService.Reference(new Texture(1, 1, PixelFormat.Rgb8, new[] { red, green, blue }));
        }

        /// <summary>
        /// Creates and references a single color texture.
        /// </summary>
        /// <param name="textureReferenceService">The <see cref="IMediaReferenceService{Texture}"/> to reference the generated texture with.</param>
        /// <param name="red">The red value to apply to the texture.</param>
        /// <param name="green">The green value to apply to the texture.</param>
        /// <param name="blue">The blue value to apply to the texture.</param>
        /// <param name="alpha">The alpha value to apply to the texture.</param>
        /// <returns>A <see cref="IReference{Texture}"/> to the generated texture.</returns>
        public static IReference<Texture> ReferenceSingleColorTexture(this IMediaReferenceService<Texture> textureReferenceService, byte red, byte green, byte blue, byte alpha)
        {
            if (textureReferenceService == null)
            {
                throw new ArgumentNullException(nameof(textureReferenceService));
            }

            return textureReferenceService.Reference(new Texture(1, 1, PixelFormat.Rgba8, new[] { red, green, blue, alpha }));
        }
    }
}
