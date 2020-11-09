using DrawingColor = System.Drawing.Color;

namespace DomainDrivenGameEngine.Media.Models
{
    /// <summary>
    /// A struct describing a color.
    /// </summary>
    public struct Color
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.
        /// </summary>
        /// <param name="red">Optional, the red value of the color.  Defaults to 1.0f.</param>
        /// <param name="green">Optional, the green value of the color.  Defaults to 1.0f.</param>
        /// <param name="blue">Optional, the blue value of the color.  Defaults to 1.0f.</param>
        /// <param name="alpha">Optional, the alpha value of the color.  Defaults to 1.0f.</param>
        public Color(float red = 1.0f, float green = 1.0f, float blue = 1.0f, float alpha = 1.0f)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        /// <summary>
        /// Gets the red value of the color.
        /// </summary>
        public float Red { get; }

        /// <summary>
        /// Gets the green value of the color.
        /// </summary>
        public float Green { get; }

        /// <summary>
        /// Gets the blue value of the color.
        /// </summary>
        public float Blue { get; }

        /// <summary>
        /// Gets the alpha value of the color.
        /// </summary>
        public float Alpha { get; }

        /// <summary>
        /// Creates a new <see cref="Color"/> based on a <see cref="DrawingColor"/> value.
        /// </summary>
        /// <param name="color">The <see cref="DrawingColor"/> value.</param>
        /// <returns>A <see cref="Color"/> value.</returns>
        public static Color FromDrawingColor(DrawingColor color)
        {
            return new Color(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
        }
    }
}
