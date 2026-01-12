using System;
using Oxide.Game.Rust.Cui;

// ReSharper disable UnusedMember.Global

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// The bounds of a rectangle.<br />
    /// <br />
    /// Bound positions can be added, subtracted, multiplied and divided using the +, -, * and / operators.<br />
    /// Note: When using these operators, only the position is effected, rotation and other properties are not effected.
    /// </summary>
    public partial class Bounds
    {
        private static readonly double ScreenWidth = 1280d;
        private static readonly double ScreenHeight = 720d;

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
        /// The width of these bounds.
        /// </summary>
        public Value Width => Value.Full - FromLeft - FromRight;

        /// <summary>
        /// The height of these bounds.
        /// </summary>
        public Value Height => Value.Full - FromTop - FromBottom;

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
        public double Rotation;

        /// <summary>
        /// Create a new bounds.
        /// </summary>
        public Bounds()
        {
            PivotPoint = PositionAnchorToPointComponents(PositionAnchor.MiddleCenter);
            Rotation = 0;
        }

        /// <summary>
        /// Create a new bounds that is a copy of the given bounds.
        /// </summary>
        public Bounds(Bounds? source)
        {
            if (source != null)
                SetTo(source);
        }

        /// <summary>
        /// Set the values to the given source.
        /// </summary>
        /// <param name="source">The source to set the values of this to.</param>
        public Bounds SetTo(Bounds source)
        {
            FromTop = source.FromTop;
            FromRight = source.FromRight;
            FromBottom = source.FromBottom;
            FromLeft = source.FromLeft;
            PivotPoint = source.PivotPoint;
            Rotation = source.Rotation;
            return this;
        }

        /// <summary>
        /// Set the position of this to that of the given bounds.
        /// </summary>
        public Bounds SetPosition(Bounds source)
        {
            FromTop = source.FromTop;
            FromRight = source.FromRight;
            FromBottom = source.FromBottom;
            FromLeft = source.FromLeft;
            return this;
        }

        /// <summary>
        /// Set the rotation of this to the given value.
        /// </summary>
        public Bounds SetRotation(double rotation)
        {
            Rotation = rotation;
            return this;
        }

        /// <summary>
        /// Set the rotation of this to that of the given bounds.
        /// </summary>
        public Bounds SetRotation(Bounds source)
        {
            Rotation = source.Rotation;
            return this;
        }

        /// <summary>
        /// Set the pivot point of this to the given anchor.
        /// </summary>
        public Bounds SetPivotPoint(PositionAnchor anchor)
        {
            PivotPoint = PositionAnchorToPointComponents(anchor);
            return this;
        }

        /// <summary>
        /// Set the pivot point of this to the given position.
        /// </summary>
        public Bounds SetPivotPoint((double X, double Y) pivotPoint)
        {
            PivotPoint = pivotPoint;
            return this;
        }

        /// <summary>
        /// Set the pivot point of this to the given x and y values.
        /// </summary>
        public Bounds SetPivotPoint(double x, double y)
        {
            PivotPoint = (x, y);
            return this;
        }

        /// <summary>
        /// Set the pivot point of this to that of the given bounds.
        /// </summary>
        public Bounds SetPivotPoint(Bounds source)
        {
            PivotPoint = source.PivotPoint;
            return this;
        }

        /// <summary>
        /// Add the position of the given bounds to this.
        /// </summary>
        public Bounds AddPosition(Bounds other)
        {
            FromTop += other.FromTop;
            FromRight += other.FromRight;
            FromBottom += other.FromBottom;
            FromLeft += other.FromLeft;
            return this;
        }

        /// <summary>
        /// Add the given bound value to this in the given direction.
        /// </summary>
        public Bounds AddPosition(Direction direction, Value components)
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
        /// Add the given rotation to this.
        /// </summary>
        public Bounds AddRotation(double rotation)
        {
            Rotation += rotation;
            return this;
        }

        /// <summary>
        /// Add the rotation of the given bounds to this.
        /// </summary>
        public Bounds AddRotation(Bounds other)
        {
            Rotation += other.Rotation;
            return this;
        }

        /// <summary>
        /// Subtract the position of the given bounds from this.
        /// </summary>
        public Bounds SubtractPosition(Bounds other)
        {
            FromTop -= other.FromTop;
            FromRight -= other.FromRight;
            FromBottom -= other.FromBottom;
            FromLeft -= other.FromLeft;
            return this;
        }

        /// <summary>
        /// Subtract the given bound value from this in the given direction.
        /// </summary>
        public Bounds SubtractPosition(Direction direction, Value components)
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
        /// Subtract the given rotation from this.
        /// </summary>
        public Bounds SubtractRotation(double rotation)
        {
            Rotation -= rotation;
            return this;
        }

        /// <summary>
        /// Subtract the rotation of the given bounds from this.
        /// </summary>
        public Bounds SubtractRotation(Bounds other)
        {
            Rotation -= other.Rotation;
            return this;
        }

        /// <summary>
        /// Multiply the position of the given bounds by a scalar.
        /// </summary>
        public Bounds MultiplyPosition(double scalar)
        {
            FromTop *= scalar;
            FromRight *= scalar;
            FromBottom *= scalar;
            FromLeft *= scalar;
            return this;
        }

        /// <summary>
        /// Multiply the rotation of this bounds by a scalar.
        /// </summary>
        public Bounds MultiplyRotation(double scalar)
        {
            Rotation *= scalar;
            return this;
        }

        /// <summary>
        /// Divide the position of the given bounds by a scalar.
        /// </summary>
        public Bounds DividePosition(double scalar)
        {
            FromTop /= scalar;
            FromRight /= scalar;
            FromBottom /= scalar;
            FromLeft /= scalar;
            return this;
        }

        /// <summary>
        /// Divide the rotation of this bounds by a scalar.
        /// </summary>
        public Bounds DivideRotation(double scalar)
        {
            Rotation /= scalar;
            return this;
        }

        /// <summary>
        /// Maximize the position of this bounds.
        /// </summary>
        public Bounds MaximizePosition()
        {
            FromTop = Value.Zero;
            FromRight = Value.Zero;
            FromBottom = Value.Zero;
            FromLeft = Value.Zero;
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
        /// Does this bounds have a size greater than zero in both directions?
        /// </summary>
        public bool HasSize()
        {
            var top = FromTop.ToCuiPositionComponents(Direction.Top);
            var right = FromRight.ToCuiPositionComponents(Direction.Right);
            var bottom = FromBottom.ToCuiPositionComponents(Direction.Bottom);
            var left = FromLeft.ToCuiPositionComponents(Direction.Left);

            return HasSize(top, right, bottom, left);
        }

        /// <summary>
        /// Does this bounds have a size greater than zero in both directions?
        /// </summary>
        public static bool HasSize(
            (double topAnchor, double topOffset) top,
            (double rightAnchor, double rightOffset) right,
            (double bottomAnchor, double bottomOffset) bottom,
            (double leftAnchor, double leftOffset) left
        )
        {
            var (topAnchor, topOffset) = top;
            var (rightAnchor, rightOffset) = right;
            var (bottomAnchor, bottomOffset) = bottom;
            var (leftAnchor, leftOffset) = left;

            var xAnchor = rightAnchor - leftAnchor;
            var xOffset = rightOffset - leftOffset;
            var yAnchor = topAnchor - bottomAnchor;
            var yOffset = topOffset - bottomOffset;

            return !(xAnchor <= 0 && xOffset <= 0 || yAnchor <= 0 && yOffset <= 0);
        }

        /// <summary>
        /// Create a CuiRectTransformComponent from this position.
        ///
        /// Returns null if the bounds has no size.
        /// </summary>
        public CuiRectTransformComponent? CreateCuiRectTransformComponent(ElementState state)
        {
            var top = FromTop.ToCuiPositionComponents(Direction.Top);
            var right = FromRight.ToCuiPositionComponents(Direction.Right);
            var bottom = FromBottom.ToCuiPositionComponents(Direction.Bottom);
            var left = FromLeft.ToCuiPositionComponents(Direction.Left);

            if (!HasSize(top, right, bottom, left))
                return null;

            return new CuiRectTransformComponent
            {
                AnchorMin = $"{left.Anchor} {bottom.Anchor}",
                AnchorMax = $"{right.Anchor} {top.Anchor}",
                OffsetMin = $"{left.Offset} {bottom.Offset}",
                OffsetMax = $"{right.Offset} {top.Offset}",
                Pivot = $"{PivotPoint.X} {PivotPoint.Y}",
                Rotation = (float)Rotation,
            };
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"(fromTop: {FromTop}, fromRight: {FromRight}, fromBottom: {FromBottom}, fromLeft: {FromLeft})";
        }

        /// <summary>
        /// Add two positions together.
        /// </summary>
        public static Bounds AddPosition(Bounds a, Bounds b)
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
        /// Subtract a position from another.
        /// </summary>
        public static Bounds SubtractPosition(Bounds a, Bounds b)
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
        /// Multiply a position by a scalar.
        /// </summary>
        public static Bounds MultiplyPosition(double scalar, Bounds value)
        {
            return new Bounds()
            {
                FromTop = scalar * value.FromTop,
                FromRight = scalar * value.FromRight,
                FromBottom = scalar * value.FromBottom,
                FromLeft = scalar * value.FromLeft,
            };
        }

        /// <inheritdoc cref="MultiplyPosition(double, Bounds)"/>
        public static Bounds MultiplyPosition(Bounds value, double scalar) => MultiplyPosition(scalar, value);

        /// <summary>
        /// Divide a position by a scalar.
        /// </summary>
        public static Bounds DividePosition(Bounds value, double scalar)
        {
            return new Bounds()
            {
                FromTop = value.FromTop / scalar,
                FromRight = value.FromRight / scalar,
                FromBottom = value.FromBottom / scalar,
                FromLeft = value.FromLeft / scalar,
            };
        }

        /// <inheritdoc cref="AddPosition(Bounds, Bounds)"/>
        public static Bounds operator +(Bounds a, Bounds b) => AddPosition(a, b);

        /// <inheritdoc cref="SubtractPosition(Bounds, Bounds)"/>
        public static Bounds operator -(Bounds a, Bounds b) => SubtractPosition(a, b);

        /// <inheritdoc cref="MultiplyPosition(double, Bounds)"/>
        public static Bounds operator *(double scalar, Bounds value) => MultiplyPosition(scalar, value);

        /// <inheritdoc cref="MultiplyPosition(double, Bounds)"/>
        public static Bounds operator *(Bounds value, double scalar) => MultiplyPosition(value, scalar);

        /// <inheritdoc cref="DividePosition(Bounds, double)"/>
        public static Bounds operator /(Bounds value, double scalar) => DividePosition(value, scalar);

        /// <summary>
        /// Get the size of the screen.
        /// </summary>
        /// <returns>The screen size in pixels.</returns>
        public static (double Width, double Height) GetScreenSizeAsAbsolute()
        {
            return (ScreenWidth, ScreenHeight);
        }

        /// <summary>
        /// Get the size of the screen in the given axis.
        /// </summary>
        /// <param name="axis">The axis of the screen to get the size of.</param>
        /// <returns>The size of the screen in the given axis in pixels.</returns>
        public static double GetScreenSizeAsAbsolute(Axis axis)
        {
            return axis switch
            {
                Axis.X => ScreenWidth,
                Axis.Y => ScreenHeight,
                _ => throw new ArgumentOutOfRangeException(nameof(axis)),
            };
        }
    }
}
