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
            internal override SizeStateValue ApplyState(ElementState state, SizeContext context)
            {
                Panic.Now("Auto sizes should not be applied directly.");
                return null;
            }

            /// <inheritdoc/>
            public override string ToString() => "Auto";
        }
    }
}