namespace DomainDriveGameEngine.Media.Models
{
    /// <summary>
    /// A struct describing a color for a vertex.
    /// </summary>
    public struct VertexColor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexColor"/> struct.
        /// </summary>
        /// <param name="red">Optional, the red value of the vertex color.  Defaults to 1.0f.</param>
        /// <param name="green">Optional, the green value of the vertex color.  Defaults to 1.0f.</param>
        /// <param name="blue">Optional, the blue value of the vertex color.  Defaults to 1.0f.</param>
        /// <param name="alpha">Optional, the alpha value of the vertex color.  Defaults to 1.0f.</param>
        public VertexColor(float red = 1.0f, float green = 1.0f, float blue = 1.0f, float alpha = 1.0f)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        /// <summary>
        /// Gets the red value of the vertex color.
        /// </summary>
        public float Red { get; }

        /// <summary>
        /// Gets the green value of the vertex color.
        /// </summary>
        public float Green { get; }

        /// <summary>
        /// Gets the blue value of the vertex color.
        /// </summary>
        public float Blue { get; }

        /// <summary>
        /// Gets the alpha value of the vertex color.
        /// </summary>
        public float Alpha { get; }
    }
}
