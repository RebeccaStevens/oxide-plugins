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

            /// <inheritdoc />
            internal override SizeStateValue ApplyState(ElementState state, SizeContext context) =>
                SizeStateValue.GetOrCreateValue(context, state, () =>
                    Bounds.AbsoluteValue(ScreenPercentageToPixels(value, state.Player, Axis.X))
                );

            /// <inheritdoc/>
            public override string ToString() => $"ScreenWidth({value * 100}%)";
        }
    }
}
