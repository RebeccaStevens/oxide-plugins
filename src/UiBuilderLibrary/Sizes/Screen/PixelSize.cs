namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    public abstract partial class Size
    {
        /// <inheritdoc cref="PixelSize"/>
        public static PixelSize Pixels(int value) => new PixelSize(value);

        /// <summary>
        /// A number of pixels*.<br />
        /// <br />
        /// *Pixels don't map 1 to 1 with screen pixels. See https://docs.oxidemod.com/guides/developers/basic-cui#ui-position for more information.
        /// </summary>
        public class PixelSize : Size
        {
            /// <summary>
            /// The number of pixels.
            /// </summary>
            public readonly int Value;

            internal PixelSize(int value)
            {
                Value = value;
            }

            /// <inheritdoc/>
            internal override SizeStateValue ApplyState(ElementState state, SizeContext context) =>
                SizeStateValue.GetOrCreateValue(context, state, new Bounds.Value(0, Value));

            /// <inheritdoc/>
            public override string ToString() => $"Pixels({Value})";
        }
    }
}
