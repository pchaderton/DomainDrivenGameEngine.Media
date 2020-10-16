using System;

namespace DomainDriveGameEngine.Media.Models
{
    /// <summary>
    /// A composite media including all of the textures necessary to represent a cube map for use as an environment texture.
    /// </summary>
    public class CubeTexture : IMedia
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CubeTexture"/> class.
        /// </summary>
        /// <param name="rightTexture">The right texture.</param>
        /// <param name="leftTexture">The left texture.</param>
        /// <param name="topTexture">The top texture.</param>
        /// <param name="bottomTexture">The bottom texture.</param>
        /// <param name="frontTexture">The front texture.</param>
        /// <param name="backTexture">The back texture.</param>
        public CubeTexture(Texture rightTexture, Texture leftTexture, Texture topTexture, Texture bottomTexture, Texture frontTexture, Texture backTexture)
        {
            RightTexture = rightTexture ?? throw new ArgumentNullException(nameof(rightTexture));
            LeftTexture = leftTexture ?? throw new ArgumentNullException(nameof(leftTexture));
            TopTexture = topTexture ?? throw new ArgumentNullException(nameof(topTexture));
            BottomTexture = bottomTexture ?? throw new ArgumentNullException(nameof(bottomTexture));
            FrontTexture = frontTexture ?? throw new ArgumentNullException(nameof(frontTexture));
            BackTexture = backTexture ?? throw new ArgumentNullException(nameof(backTexture));
        }

        /// <summary>
        /// Gets the back texture.
        /// </summary>
        public Texture BackTexture { get; }

        /// <summary>
        /// Gets the bottom texture.
        /// </summary>
        public Texture BottomTexture { get; }

        /// <summary>
        /// Gets the front texture.
        /// </summary>
        public Texture FrontTexture { get; }

        /// <summary>
        /// Gets the left texture.
        /// </summary>
        public Texture LeftTexture { get; }

        /// <summary>
        /// Gets the right texture.
        /// </summary>
        public Texture RightTexture { get; }

        /// <summary>
        /// Gets the top texture.
        /// </summary>
        public Texture TopTexture { get; }
    }
}
