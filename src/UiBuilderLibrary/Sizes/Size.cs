using System;

// ReSharper disable UnusedMember.Global

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// Can be used to size elements.
    /// </summary>
    public abstract partial class Size
    {
        /// <summary>
        /// A value of zero.
        /// </summary>
        public static readonly PixelSize Zero = Pixels(0);

        /// <summary>
        /// Apply the given state to this.
        /// </summary>
        /// <param name="state">The state to apply.</param>
        /// <param name="context">The context.</param>
        /// <returns>An applied value.</returns>
        internal abstract SizeStateValue ApplyState(ElementState state, SizeContext context);
    }
}
