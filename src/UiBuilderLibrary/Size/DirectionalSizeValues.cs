using System;

// ReSharper disable UnusedMember.Global

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// An size value in each of the four directions (top, right, bottom, left).
    /// </summary>
    public class DirectionalSizeValues
    {
        /// <summary>
        /// The context for the top value.
        /// </summary>
        internal readonly SizeContext TopContext;

        /// <summary>
        /// The context for the right value.
        /// </summary>
        internal readonly SizeContext RightContext;

        /// <summary>
        /// The context for the bottom value.
        /// </summary>
        internal readonly SizeContext BottomContext;

        /// <summary>
        /// The context for the left value.
        /// </summary>
        internal readonly SizeContext LeftContext;

        /// <summary>
        /// The top value.
        /// </summary>
        public Size Top
        {
            get => TopContext.Get();
            set => TopContext.Set(value);
        }

        /// <summary>
        /// The right value.
        /// </summary>
        public Size Right
        {
            get => RightContext.Get();
            set => RightContext.Set(value);
        }

        /// <summary>
        /// The bottom value.
        /// </summary>
        public Size Bottom
        {
            get => BottomContext.Get();
            set => BottomContext.Set(value);
        }

        /// <summary>
        /// The left value.
        /// </summary>
        public Size Left
        {
            get => LeftContext.Get();
            set => LeftContext.Set(value);
        }

        /// <summary>
        /// Create a new directional size values.
        /// </summary>
        internal DirectionalSizeValues(string name, Element appliedTo, Size initialValue)
        {
            TopContext = new SizeContext($"{name}.{Direction.Top}", appliedTo, initialValue);
            RightContext = new SizeContext($"{name}.{Direction.Right}", appliedTo, initialValue);
            BottomContext = new SizeContext($"{name}.{Direction.Bottom}", appliedTo, initialValue);
            LeftContext = new SizeContext($"{name}.{Direction.Left}", appliedTo, initialValue);
        }

        /// <summary>
        /// Get the size context for the given direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <returns>The context for the given direction.</returns>
        internal SizeContext this[Direction direction] => direction switch
        {
            Direction.Top => TopContext,
            Direction.Bottom => BottomContext,
            Direction.Left => LeftContext,
            Direction.Right => RightContext,
            _ => throw new ArgumentOutOfRangeException(nameof(direction)),
        };

        /// <summary>
        /// Get the bounds of a rectangle that is inset by this.
        /// </summary>
        public Bounds GetInnerBounds(ElementState state)
        {
            // TODO: Is it worth caching this? Overhead vs new instance creations.
            return new Bounds()
            {
                FromTop = TopContext.GetBoundsValue(state),
                FromRight = RightContext.GetBoundsValue(state),
                FromBottom = BottomContext.GetBoundsValue(state),
                FromLeft = LeftContext.GetBoundsValue(state),
            };
        }

        /// <summary>
        /// Set all directional values to the given value.
        /// </summary>
        /// <param name="value">The value to set all directional values to.</param>
        public void SetSize(Size value)
        {
            Top = value;
            Right = value;
            Bottom = value;
            Left = value;
        }

        /// <summary>
        /// Set the horizontal and vertical values to the given values.
        /// </summary>
        /// <param name="x">The value to set the horizontal values to.</param>
        /// <param name="y">The value to set the vertical values to.</param>
        public void SetSize(Size x, Size y)
        {
            Top = y;
            Right = x;
            Bottom = y;
            Left = x;
        }

        /// <summary>
        /// Set each directional value to the given values.
        /// </summary>
        /// <param name="top">The value to set the top value to.</param>
        /// <param name="right">The value to set the right value to.</param>
        /// <param name="bottom">The value to set the bottom value to.</param>
        /// <param name="left">The value to set the left value to.</param>
        public void SetSize(Size top, Size right, Size bottom, Size left)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }
    }
}
