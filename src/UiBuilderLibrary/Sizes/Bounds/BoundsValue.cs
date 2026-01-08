using System;

// ReSharper disable UnusedMember.Global

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    public partial class Bounds
    {
        /// <summary>
        /// The components of a position.
        /// </summary>
        public readonly record struct Value(double Relative, double Absolute)
        {
            /// <summary>
            /// Take the complement of this component.
            /// </summary>
            /// <returns>100% - this</returns>
            public Value TakeComplement()
            {
                return new Value(1 - Relative, -Absolute);
            }

            /// <summary>
            /// Negate this component.
            /// </summary>
            /// <returns>0 - this</returns>
            public Value Negated()
            {
                return new Value(-Relative, -Absolute);
            }

            /// <summary>
            /// Get the anchor and offset values to use in a CuiRectTransformComponent.
            /// </summary>
            /// <param name="direction">The direction these components are in.</param>
            internal (double Anchor, double Offset) ToCuiPositionComponents(Direction direction)
            {
                return (ToCuiPositionAnchor(direction), ToCuiPositionOffset(direction));
            }

            private double ToCuiPositionAnchor(Direction direction)
            {
                var value = direction switch
                {
                    Direction.Top or Direction.Right => 1d - Relative,
                    Direction.Bottom or Direction.Left => Relative,
                    _ => throw new ArgumentOutOfRangeException(nameof(direction)),
                };
                return RoundAnchorValue(value);
            }

            private double ToCuiPositionOffset(Direction direction)
            {
                return direction switch
                {
                    Direction.Top or Direction.Right => -Absolute,
                    Direction.Bottom or Direction.Left => Absolute,
                    _ => throw new ArgumentOutOfRangeException(nameof(direction)),
                };
            }

            /// <inheritdoc/>
            public override string ToString() => $"({Relative * 100}% + {Absolute}px)";

            /// <summary>
            /// Add two lots of components together.
            /// </summary>
            public static Value Add(Value a, Value b)
            {
                return new Value(
                    a.Relative + b.Relative,
                    a.Absolute + b.Absolute
                );
            }

            /// <summary>
            /// Subtract a lot of components from another.
            /// </summary>
            public static Value Subtract(Value a, Value b)
            {
                return new Value(
                    a.Relative - b.Relative,
                    a.Absolute - b.Absolute
                );
            }

            /// <summary>
            /// Multiply a lot of components by a scalar.
            /// </summary>
            public static Value Multiply(double scalar, Value components)
            {
                return new Value(
                    components.Relative * scalar,
                    components.Absolute * scalar
                );
            }

            /// <inheritdoc cref="Multiply(double, Value)"/>
            public static Value Multiply(Value components, double scalar) => Multiply(scalar, components);

            /// <summary>
            /// Divide a lot of components by a scalar.
            /// </summary>
            public static Value Divide(Value components, double scalar)
            {
                return new Value(
                    components.Relative / scalar,
                    components.Absolute / scalar
                );
            }

            /// <inheritdoc cref="Add(Value, Value)"/>
            public static Value operator +(Value a, Value b) => Add(a, b);

            /// <inheritdoc cref="Subtract(Value, Value)"/>
            public static Value operator -(Value a, Value b) => Subtract(a, b);

            /// <inheritdoc cref="Multiply(double, Value)"/>
            public static Value operator *(double scalar, Value value) => Multiply(scalar, value);

            /// <inheritdoc cref="Multiply(Value, double)"/>
            public static Value operator *(Value value, double scalar) => Multiply(value, scalar);

            /// <inheritdoc cref="Divide(Value, double)"/>
            public static Value operator /(Value value, double scalar) => Divide(value, scalar);

            /// <summary>
            /// If really close to 0 or 1, return that instead.
            /// </summary>
            private static double RoundAnchorValue(double value)
            {
                const double epsilon = 0.0005;
                var lowAbsValue = Math.Abs(value);
                var highAbsValue = Math.Abs(value - 1d);
                return lowAbsValue < epsilon ? 0d : highAbsValue < epsilon ? 1d : value;
            }
        }
    }
}
