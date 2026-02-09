using System;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    public abstract partial class Size
    {
        /// <summary>
        /// A size that is computed based the given function.<br/>
        /// It can be used to combine multiple sizes together.
        /// </summary>
        internal class ComputedSize : Size
        {
            /// <summary>
            /// The number of pixels.
            /// </summary>
            public readonly Func<ElementState, Bounds.Value> ComputeBounds;

            internal ComputedSize(Func<ElementState, Bounds.Value> computeBounds)
            {
                ComputeBounds = computeBounds;
            }

            /// <inheritdoc/>
            internal override Bounds.Value GetBoundsValue(ElementState state) => ComputeBounds(state);

            /// <inheritdoc/>
            public override Size Add(Size other) =>
                new ComputedSize(state => GetBoundsValue(state) + other.GetBoundsValue(state));

            /// <inheritdoc/>
            public override Size Subtract(Size other) =>
                new ComputedSize(state => GetBoundsValue(state) - other.GetBoundsValue(state));

            /// <inheritdoc/>
            public override Size Multiply(double multiplier) =>
                new ComputedSize(state => GetBoundsValue(state) * multiplier);

            /// <inheritdoc/>
            public override Size Divide(double divisor) => new ComputedSize(state => GetBoundsValue(state) / divisor);

            /// <inheritdoc/>
            public override string ToString() => "(computed)";
        }
    }
}
