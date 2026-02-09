// ReSharper disable UnusedMember.Global

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    public abstract partial class Size
    {
        /// <summary>
        /// A percentage (in decimal form) of the player's screen width.
        /// </summary>
        public static Size ScreenWidthPercentage(double value) => new ScreenWidthRelativeSize(value);

        /// <summary>
        /// A value with a size that is relative to the screen width.
        /// </summary>
        private class ScreenWidthRelativeSize : Size
        {
            private readonly double value;

            public ScreenWidthRelativeSize(double value)
            {
                this.value = value;
            }

            /// <inheritdoc/>
            internal override Bounds.Value GetBoundsValue(ElementState state) => Bounds.AbsoluteValue(
                ScreenPercentageToPixels(value, state.Player, Axis.X));

            /// <inheritdoc/>
            public override Size Add(Size other) =>
                other switch
                {
                    ScreenWidthRelativeSize otherSize => new ScreenWidthRelativeSize(value + otherSize.value),
                    _ => new ComputedSize(state => GetBoundsValue(state) + other.GetBoundsValue(state)),
                };

            /// <inheritdoc/>
            public override Size Subtract(Size other) =>
                other switch
                {
                    ScreenWidthRelativeSize otherSize => new ScreenWidthRelativeSize(value - otherSize.value),
                    _ => new ComputedSize(state => GetBoundsValue(state) - other.GetBoundsValue(state)),
                };

            /// <inheritdoc />
            public override Size Multiply(double multiplier) =>
                new ScreenWidthRelativeSize(value * multiplier);

            /// <inheritdoc />
            public override Size Divide(double divisor) =>
                new ScreenWidthRelativeSize(value / divisor);

            /// <inheritdoc/>
            public override string ToString() => $"ScreenWidth({value * 100}%)";
        }
    }
}
