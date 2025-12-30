namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    public abstract partial class Size
    {
        /// <summary>
        /// A percentage (in decimal form) of the parent element's size.
        /// </summary>
        public static Size ContainerPercentage(double value) => new ContainerRelativeSize(value);

        /// <summary>
        /// A value with a size that is relative to the size of the element's parent.
        /// </summary>
        private class ContainerRelativeSize : Size
        {
            private readonly double value;

            public ContainerRelativeSize(double value)
            {
                this.value = value;
            }

            /// <inheritdoc />
            internal override SizeStateValue ApplyState(ElementState state, SizeContext context) =>
                SizeStateValue.GetOrCreateValue(context, state, new Bounds.Value(value, 0));

            /// <inheritdoc/>
            public override string ToString() => $"Container({value * 100}%)";
        }
    }
}