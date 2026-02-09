using System;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    public abstract partial class Size
    {
        /// <summary>
        /// A size that means the value should be automatically calculated based on the context it's used in.<br/>
        /// Usually it means full as much space as available.
        /// </summary>
        public static readonly Size Auto = new AutoSize();

        /// <summary>
        /// Automatically calculates the value based on the context it's used in.
        /// </summary>
        private class AutoSize : Size
        {
            /// <inheritdoc/>
            internal override Bounds.Value GetBoundsValue(ElementState state) =>
                throw new InvalidOperationException("Auto sizes do not have a bounds value.");

            /// <inheritdoc/>
            public override Size Add(Size other) =>
                throw new InvalidOperationException("Cannot add to an Auto size.");

            /// <inheritdoc/>
            public override Size Subtract(Size other) =>
                throw new InvalidOperationException("Cannot subtract from an Auto size.");

            /// <inheritdoc/>
            public override Size Multiply(double multiplier) =>
                throw new InvalidOperationException("Cannot multiply an Auto size.");

            /// <inheritdoc/>
            public override Size Divide(double divisor) =>
                throw new InvalidOperationException("Cannot divide an Auto size.");

            /// <inheritdoc/>
            public override string ToString() => "Auto";
        }
    }
}
