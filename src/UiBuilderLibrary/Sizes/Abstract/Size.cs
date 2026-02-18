// ReSharper disable UnusedMember.Global

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// Can be used to size elements.
    /// </summary>
    public abstract partial class Size
    {
        /// <summary>
        /// A value of zero.
        /// </summary>
        public static readonly Size Zero = Pixels(0);

        /// <summary>
        /// Apply the given state to this.
        /// </summary>
        /// <param name="state">The state to apply.</param>
        /// <param name="context">The context.</param>
        /// <returns>An applied value.</returns>
        internal SizeStateValue ApplyState(ElementState state, SizeContext context) =>
            SizeStateValue.GetOrCreateValue(context, state, GetBoundsValue(state));

        /// <summary>
        /// Get the bounds value for this size.
        /// </summary>
        /// <param name="state">The state of the element to get the value for.</param>
        /// <returns>The bounds value.</returns>
        internal abstract Bounds.Value GetBoundsValue(ElementState state);

        /// <summary>
        /// Add a size to this size.
        /// </summary>
        /// <param name="other">The size to add.</param>
        /// <returns>A new size.</returns>
        public abstract Size Add(Size other);

        /// <summary>
        /// Subtract a size from this size.
        /// </summary>
        /// <param name="other">The size to subtract.</param>
        /// <returns>A new size.</returns>
        public abstract Size Subtract(Size other);

        /// <summary>
        /// Multiply this size by a multiplier.
        /// </summary>
        /// <param name="multiplier">The amount to multiply by.</param>
        /// <returns>A new size.</returns>
        public abstract Size Multiply(double multiplier);

        /// <summary>
        /// Divide this size by a divisor.
        /// </summary>
        /// <param name="divisor">The amount to divide by.</param>
        /// <returns>A new size.</returns>
        public abstract Size Divide(double divisor);

        /// <summary>
        /// Add two sizes together.
        /// </summary>
        public static Size operator +(Size a, Size b) => a.Add(b);

        /// <summary>
        /// Subtract a size from another.
        /// </summary>
        public static Size operator -(Size a, Size b) => a.Subtract(b);

        /// <summary>
        /// Multiply a size by a multiplier.
        /// </summary>
        public static Size operator *(Size value, double multiplier) => value.Multiply(multiplier);

        /// <summary>
        /// Multiply a size by a multiplier.
        /// </summary>
        public static Size operator *(double multiplier, Size value) => value.Multiply(multiplier);

        /// <summary>
        /// Divide a size by a divisor.
        /// </summary>
        public static Size operator /(Size value, double divisor) => value.Divide(divisor);
    }
}
