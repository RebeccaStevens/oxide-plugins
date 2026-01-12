namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    public abstract partial class Size
    {
        /// <inheritdoc cref="PixelSize"/>
        public static Size Pixels(int value) => new PixelSize(value);

        /// <summary>
        /// A number of pixels*.<br />
        /// <br />
        /// *Pixels don't map 1 to 1 with screen pixels. See https://docs.oxidemod.com/guides/developers/basic-cui#ui-position for more information.
        /// </summary>
        internal class PixelSize : Size
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
                SizeStateValue.GetOrCreateValue(context, state, Bounds.AbsoluteValue(Value));

            /// <inheritdoc/>
            public override string ToString() => $"Pixels({Value})";
        }
    }
}
