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

            /// <inheritdoc/>
            internal override Bounds.Value GetBoundsValue(ElementState state) => Bounds.RelativeValue(value);

            /// <inheritdoc />
            public override Size Add(Size other) =>
                other switch
                {
                    ContainerRelativeSize otherSize => new ContainerRelativeSize(value + otherSize.value),
                    _ => new ComputedSize(state => GetBoundsValue(state) + other.GetBoundsValue(state)),
                };

            /// <inheritdoc />
            public override Size Subtract(Size other) =>
                other switch
                {
                    ContainerRelativeSize otherSize => new ContainerRelativeSize(value - otherSize.value),
                    _ => new ComputedSize(state => GetBoundsValue(state) - other.GetBoundsValue(state)),
                };

            /// <inheritdoc />
            public override Size Multiply(double multiplier) =>
                new ContainerRelativeSize(value * multiplier);

            /// <inheritdoc />
            public override Size Divide(double divisor) =>
                new ContainerRelativeSize(value / divisor);

            /// <inheritdoc/>
            public override string ToString() => $"Container({value * 100}%)";
        }
    }
}
