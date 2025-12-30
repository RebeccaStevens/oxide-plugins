namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    public abstract partial class Size
    {
        /// <summary>
        /// A number of pixels*.<br />
        /// <br />
        /// *Pixels don't map 1 to 1 with screen pixels. See https://docs.oxidemod.com/guides/developers/basic-cui#ui-position for more information.
        /// </summary>
        public static Size Pixels(int value) => new PixelSize(value);

        /// <summary>
        /// A value representing a number of pixels.
        /// </summary>
        private class PixelSize : Size
        {
            private readonly int value;

            public PixelSize(int value)
            {
                this.value = value;
            }

            /// <inheritdoc/>
            internal override SizeStateValue ApplyState(ElementState state, SizeContext context) =>
                SizeStateValue.GetOrCreateValue(context, state, new Bounds.Value(0, value));

            /// <inheritdoc/>
            public override string ToString() => $"Pixels({value})";
        }
    }
}