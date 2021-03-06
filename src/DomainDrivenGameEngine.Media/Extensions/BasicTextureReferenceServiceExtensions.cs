﻿using System;
using System.Collections.ObjectModel;
using DomainDrivenGameEngine.Media.Models;
using DomainDrivenGameEngine.Media.Services;

namespace DomainDrivenGameEngine.Media.Extensions
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
        /// <returns>A <see cref="IMediaReference{Texture}"/> to the generated texture.</returns>
        public static IMediaReference<Texture> ReferenceSingleColorTexture(this IMediaReferenceService<Texture> textureReferenceService, byte red, byte green, byte blue)
        {
            if (textureReferenceService == null)
            {
                throw new ArgumentNullException(nameof(textureReferenceService));
            }

            return textureReferenceService.Reference(new Texture(1, 1, TextureFormat.Rgb24, new ReadOnlyCollection<byte>(new[] { red, green, blue })));
        }

        /// <summary>
        /// Creates and references a single color texture.
        /// </summary>
        /// <param name="textureReferenceService">The <see cref="IMediaReferenceService{Texture}"/> to reference the generated texture with.</param>
        /// <param name="red">The red value to apply to the texture.</param>
        /// <param name="green">The green value to apply to the texture.</param>
        /// <param name="blue">The blue value to apply to the texture.</param>
        /// <param name="alpha">The alpha value to apply to the texture.</param>
        /// <returns>A <see cref="IMediaReference{Texture}"/> to the generated texture.</returns>
        public static IMediaReference<Texture> ReferenceSingleColorTexture(this IMediaReferenceService<Texture> textureReferenceService, byte red, byte green, byte blue, byte alpha)
        {
            if (textureReferenceService == null)
            {
                throw new ArgumentNullException(nameof(textureReferenceService));
            }

            return textureReferenceService.Reference(new Texture(1, 1, TextureFormat.Rgba32, new ReadOnlyCollection<byte>(new[] { red, green, blue, alpha })));
        }
    }
}
