using System;

// ReSharper disable UnusedMember.Global

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// An size value in each of the two axes (x and y).
    /// </summary>
    public class AxesSizeValues
    {
        /// <summary>
        /// The context for the x value.
        /// </summary>
        internal readonly SizeContext XContext;

        /// <summary>
        /// The context for the y value.
        /// </summary>
        internal readonly SizeContext YContext;

        /// <summary>
        /// The x value.
        /// </summary>
        public Size X
        {
            get => XContext.Get();
            set => XContext.Set(value);
        }

        /// <summary>
        /// The y value.
        /// </summary>
        public Size Y
        {
            get => YContext.Get();
            set => YContext.Set(value);
        }

        /// <summary>
        /// Create a new axes size values.
        /// </summary>
        internal AxesSizeValues(string name, Element appliedTo, Size initialValue)
        {
            XContext = new SizeContext($"{name}.{Axis.X}", appliedTo, initialValue);
            YContext = new SizeContext($"{name}.{Axis.Y}", appliedTo, initialValue);
        }

        /// <summary>
        /// Get the size context for the given axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <returns>The context for the given axis.</returns>
        internal SizeContext this[Axis axis] => axis switch
        {
            Axis.X => XContext,
            Axis.Y => YContext,
            _ => throw new ArgumentOutOfRangeException(nameof(axis)),
        };

        /// <summary>
        /// Get the bounds of a rectangle that is inset by this.
        /// </summary>
        public Bounds GetInnerBounds(ElementState state)
        {
            var x = XContext.GetBoundsValue(state);
            var y = YContext.GetBoundsValue(state);
            return new Bounds()
            {
                FromTop = y,
                FromRight = x,
                FromBottom = y,
                FromLeft = x,
            };
        }

        /// <summary>
        /// Set all axis values to the given value.
        /// </summary>
        /// <param name="value">The value to set all axis values to.</param>
        public void SetSize(Size value)
        {
            X = value;
            Y = value;
        }

        /// <summary>
        /// Set the horizontal and vertical values to the given values.
        /// </summary>
        /// <param name="x">The value to set the horizontal values to.</param>
        /// <param name="y">The value to set the vertical values to.</param>
        public void SetSize(Size x, Size y)
        {
            X = y;
            Y = y;
        }
    }
}
