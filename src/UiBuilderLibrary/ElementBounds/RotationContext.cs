using System;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A rotation that is applied to an element.
    /// </summary>
    public class RotationContext
    {
        private const double Epsilon = 0.01;

        private double degrees;

        /// <summary>
        /// The parent rotation context.
        /// </summary>
        private readonly RotationContext? parent;

        /// <summary>
        /// The rotation in degrees.<br />
        /// Positive values rotate counter-clockwise, negative values clockwise.
        /// </summary>
        public double Degrees
        {
            // ReSharper disable once FunctionRecursiveOnAllPaths
            get => Normalize(degrees + (parent?.Degrees ?? 0));
            set => degrees = value;
        }

        /// <summary>
        /// When <see cref="Degrees"/> is not 0, this point on the element
        /// will be moved to the center of the element.
        /// Then the element will be rotated around this point.
        /// </summary>
        public PositionAnchor Pivot { get; set; }

        /// <inheritdoc cref="RotationContext"/>
        internal RotationContext(RotationContext? parent, double degrees = 0,
            PositionAnchor pivot = PositionAnchor.MiddleCenter)
        {
            this.parent = parent;
            Degrees = degrees;
            Pivot = pivot;
        }

        /// <summary>
        /// Get the normalized value of the given rotation amount.
        /// </summary>
        public static double Normalize(double rotation) => ((rotation % 360) + 360) % 360;

        /// <summary>
        /// Check if two rotations are roughly equal to one another.
        /// </summary>
        /// <param name="a">The first rotation.</param>
        /// <param name="b">The second rotation.</param>
        /// <param name="normalize">Whether to normalize the rotations before comparing them.</param>
        /// <param name="epsilon">The value in which the rotations are considered equal.</param>
        public static bool RoughlyEqual(RotationContext a, RotationContext b, bool normalize = true,
            double epsilon = Epsilon) =>
            normalize
                ? RoughlyEqual(Normalize(a.Degrees), Normalize(b.Degrees), normalize, epsilon)
                : RoughlyEqual(a.Degrees, b.Degrees, normalize, epsilon);

        /// <inheritdoc cref="RoughlyEqual(RotationContext, RotationContext, bool, double)"/>
        public static bool RoughlyEqual(double a, double b, bool normalize = true, double epsilon = Epsilon) =>
            normalize
                ? Math.Abs(Normalize(a) - Normalize(b)) < epsilon
                : Math.Abs(a - b) < epsilon;
    }
}
