// ReSharper disable UnusedMember.Global

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    public abstract partial class Size
    {
        /// <summary>
        /// A percentage (in decimal form) of the player's screen height.
        /// </summary>
        public static Size ScreenHeightPercentage(double value) => new ScreenHeightRelativeSize(value);

        /// <summary>
        /// A value with a size that is relative to the screen height.
        /// </summary>
        private class ScreenHeightRelativeSize : Size
        {
            private readonly double value;

            public ScreenHeightRelativeSize(double value)
            {
                this.value = value;
            }

            /// <inheritdoc />
            internal override SizeStateValue ApplyState(ElementState state, SizeContext context) =>
                SizeStateValue.GetOrCreateValue(context, state, () =>
                    new Bounds.Value(0, ScreenPercentageToPixels(value, state.Player, Axis.Y))
                );

            /// <inheritdoc/>
            public override string ToString() => $"ScreenHeight({value * 100}%)";
        }
    }
}