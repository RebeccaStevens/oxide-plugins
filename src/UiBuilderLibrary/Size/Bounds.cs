using System;
using Oxide.Game.Rust.Cui;

// ReSharper disable UnusedMember.Global

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// The bounds of a rectangle.
    /// </summary>
    public partial class Bounds
    {
        private static readonly (double AsRelative, double AsAbsolute) ScreenWidth = (1d, 1280d);
        private static readonly (double AsRelative, double AsAbsolute) ScreenHeight = (1d, 720d);

        /// <summary>
        /// How far from the top of the parent container this is.
        /// </summary>
        public Value FromTop;

        /// <summary>
        /// How far from the right of the parent container this is.
        /// </summary>
        public Value FromRight;

        /// <summary>
        /// How far from the bottom of the parent container this is.
        /// </summary>
        public Value FromBottom;

        /// <summary>
        /// How far from the left of the parent container this is.
        /// </summary>
        public Value FromLeft;

        /// <summary>
        /// The pivot point of this element relative to the element's anchor.
        /// Defaults to (0.5, 0.5) (the center of the element).
        /// </summary>
        public (double X, double Y) PivotPoint;

        // TODO: Document rotation units (degrees, radians, etc.)
        /// <summary>
        /// The rotation of this element around its pivot point.<br />
        /// Defaults to 0.
        /// </summary>
        public float Rotation;

        /// <summary>
        /// Change the order of a GameObject within its parent's hierarchy.<br />
        /// Defaults to -1, which means it will use the default transform order.
        /// </summary>
        public int TransformIndex;

        /// <summary>
        /// Create a new bounds.
        /// </summary>
        public Bounds()
        {
            PivotPoint = (0.5, 0.5);
            Rotation = 0;
            TransformIndex = -1;
        }

        /// <summary>
        /// Set the values to the given source.
        /// </summary>
        /// <param name="source">The source to set the values of this to.</param>
        public void SetTo(Bounds source)
        {
            FromTop = source.FromTop;
            FromRight = source.FromRight;
            FromBottom = source.FromBottom;
            FromLeft = source.FromLeft;
            PivotPoint = source.PivotPoint;
            Rotation = source.Rotation;
            TransformIndex = source.TransformIndex;
        }

        /// <summary>
        /// Add two positions together.
        /// </summary>
        public static Bounds Add(Bounds a, Bounds b)
        {
            return new Bounds()
            {
                FromTop = a.FromTop + b.FromTop,
                FromRight = a.FromRight + b.FromRight,
                FromBottom = a.FromBottom + b.FromBottom,
                FromLeft = a.FromLeft + b.FromLeft,
            };
        }

        /// <summary>
        /// Add the given components to the position in the given direction.
        ///
        /// Note: This mutates the position.
        /// </summary>
        public Bounds Add(Direction direction, Value components)
        {
            switch (direction)
            {
                case Direction.Top:
                    FromTop += components;
                    break;
                case Direction.Right:
                    FromRight += components;
                    break;
                case Direction.Bottom:
                    FromBottom += components;
                    break;
                case Direction.Left:
                    FromLeft += components;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }

            return this;
        }

        /// <summary>
        /// Subtract a position from another.
        /// </summary>
        public static Bounds Subtract(Bounds a, Bounds b)
        {
            return new Bounds()
            {
                FromTop = a.FromTop - b.FromTop,
                FromRight = a.FromRight - b.FromRight,
                FromBottom = a.FromBottom - b.FromBottom,
                FromLeft = a.FromLeft - b.FromLeft,
            };
        }

        /// <summary>
        /// Subtract the given components to the position in the given direction.
        ///
        /// Note: This mutates the position.
        /// </summary>
        public Bounds Subtract(Direction direction, Value components)
        {
            switch (direction)
            {
                case Direction.Top:
                    FromTop -= components;
                    break;
                case Direction.Right:
                    FromRight -= components;
                    break;
                case Direction.Bottom:
                    FromBottom -= components;
                    break;
                case Direction.Left:
                    FromLeft -= components;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }

            return this;
        }

        /// <summary>
        /// Multiply a position by a scalar.
        /// </summary>
        public static Bounds Multiply(double scalar, Bounds value)
        {
            return new Bounds()
            {
                FromTop = scalar * value.FromTop,
                FromRight = scalar * value.FromRight,
                FromBottom = scalar * value.FromBottom,
                FromLeft = scalar * value.FromLeft,
            };
        }

        /// <summary>
        /// Divide a position by a scalar.
        /// </summary>
        public static Bounds Divide(Bounds value, double scalar)
        {
            return new Bounds()
            {
                FromTop = value.FromTop / scalar,
                FromRight = value.FromRight / scalar,
                FromBottom = value.FromBottom / scalar,
                FromLeft = value.FromLeft / scalar,
            };
        }

        /// <inheritdoc cref="Add(Bounds, Bounds)"/>
        public static Bounds operator +(Bounds a, Bounds b) => Add(a, b);

        /// <inheritdoc cref="Subtract(Bounds, Bounds)"/>
        public static Bounds operator -(Bounds a, Bounds b) => Subtract(a, b);

        /// <inheritdoc cref="Multiply(double, Bounds)"/>
        public static Bounds operator *(double scalar, Bounds value) => Multiply(scalar, value);

        /// <inheritdoc cref="Divide(Bounds, double)"/>
        public static Bounds operator /(Bounds value, double scalar) => Divide(value, scalar);

        /// <summary>
        /// Make this bounds to fill its parent.
        /// </summary>
        public Bounds Maximize()
        {
            FromTop = new(0, 0);
            FromRight = new(0, 0);
            FromBottom = new(0, 0);
            FromLeft = new(0, 0);
            return this;
        }

        /// <summary>
        /// Get the components of this position for the given direction.
        /// </summary>
        public Value GetValue(Direction direction)
        {
            return direction switch
            {
                Direction.Top => FromTop,
                Direction.Right => FromRight,
                Direction.Bottom => FromBottom,
                Direction.Left => FromLeft,
                _ => throw new ArgumentOutOfRangeException(nameof(direction)),
            };
        }

        /// <summary>
        /// Create a CuiRectTransformComponent from this position.
        ///
        /// Returns null if the bounds has no size.
        /// </summary>
        public CuiRectTransformComponent? CreateCuiRectTransformComponent(ElementState state)
        {
            var (topAnchor, topOffset) = FromTop.ToCuiPositionComponents(Direction.Top);
            var (rightAnchor, rightOffset) = FromRight.ToCuiPositionComponents(Direction.Right);
            var (bottomAnchor, bottomOffset) = FromBottom.ToCuiPositionComponents(Direction.Bottom);
            var (leftAnchor, leftOffset) = FromLeft.ToCuiPositionComponents(Direction.Left);

            var xAnchor = rightAnchor - leftAnchor;
            var xOffset = rightOffset - leftOffset;
            var yAnchor = topAnchor - bottomAnchor;
            var yOffset = topOffset - bottomOffset;

            if (xAnchor <= 0 && xOffset <= 0 || yAnchor <= 0 && yOffset <= 0)
                return null;

            return new CuiRectTransformComponent
            {
                AnchorMin = $"{leftAnchor} {bottomAnchor}",
                AnchorMax = $"{rightAnchor} {topAnchor}",
                OffsetMin = $"{leftOffset} {bottomOffset}",
                OffsetMax = $"{rightOffset} {topOffset}",
                Pivot = $"{PivotPoint.X} {PivotPoint.Y}",
                Rotation = Rotation,
                SetTransformIndex = TransformIndex,
            };
        }

        /// <summary>
        /// Get the size of the screen.
        /// </summary>
        /// <returns>The size of the screen in each axis both as a relative value and as an absolute value.</returns>
        public static ((double AsRelative, double AsAbsolute) Width, (double AsRelative, double AsAbsolute) Height)
            GetScreenSize()
        {
            return (ScreenWidth, ScreenHeight);
        }

        /// <summary>
        /// Get the size of the screen in the given axis.
        /// </summary>
        /// <param name="axis">The axis of the screen to get the size of.</param>
        /// <returns>The size of the screen in the given axis both as a relative value and as an absolute value.</returns>
        public static (double AsRelative, double AsAbsolute) GetScreenSize(Axis axis)
        {
            return axis switch
            {
                Axis.X => ScreenWidth,
                Axis.Y => ScreenHeight,
                _ => throw new ArgumentOutOfRangeException(nameof(axis)),
            };
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"(fromTop: {FromTop}, fromRight: {FromRight}, fromBottom: {FromBottom}, fromLeft: {FromLeft})";
        }
    }
}
