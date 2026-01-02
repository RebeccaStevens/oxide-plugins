using System;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A size that is applied to an element.
    /// </summary>
    public class SizeContext
    {
        /// <summary>
        /// A label for this size.<br />
        /// Useful for debugging, but doesn't have any other practical purpose.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The element this is applied to.
        /// </summary>
        public readonly Element Element;

        /// <summary>
        /// The value of this.
        /// </summary>
        private Size size;

        internal SizeContext(string name, Element element, Size initialValue)
        {
            Name = name;
            Element = element;
            size = initialValue;
        }

        /// <summary>
        /// Get the value of this.
        /// </summary>
        public Size Get()
        {
            return size;
        }

        /// <summary>
        /// Is this implicitly sized?
        /// </summary>
        public bool IsImplisit()
        {
            return size == Size.Auto;
        }

        /// <summary>
        /// Set the value of this.
        /// </summary>
        /// <param name="value">The value to set this to.</param>
        public void Set(Size value)
        {
            SizeStateValue.MarkAllDirty(this);
            size = value;
        }

        /// <summary>
        /// Get bounds' value for the given element state.
        /// </summary>
        /// <param name="state">The state of the element to get the value for.</param>
        /// <param name="autoValue">The value to use if the size is a <see cref="Size.AutoSize"/>.</param>
        /// <returns>The bounds' value for the given element state.</returns>
        public virtual Bounds.Value GetBoundsValue(ElementState state, Bounds.Value autoValue = default)
        {
            Debug.Assert(Element == state.Element, "The given state is for a different element.");
            return GetStateValue(state, autoValue).GetBoundsValue();
        }

        /// <summary>
        /// Get bounds' value for the given element state.
        /// </summary>
        /// <param name="state">The state of the element to get the value for.</param>
        /// <param name="computeAutoValue">A function to compute the value to use if the size is a <see cref="Size.AutoSize"/>.</param>
        /// <returns>The bounds' value for the given element state.</returns>
        public virtual Bounds.Value GetBoundsValue(ElementState state,
            Func<ElementState, Bounds.Value> computeAutoValue)
        {
            return GetBoundsValue(state, computeAutoValue(state));
        }

        /// <summary>
        /// Get the value for the given element state.
        /// </summary>
        /// <param name="state">The state of the element to get the value for.</param>
        /// <param name="autoValue">The value if the size is a <see cref="Size.AutoSize"/>.</param>
        /// <returns>The unit value for the given element state.</returns>
        private SizeStateValue GetStateValue(ElementState state, Bounds.Value autoValue)
        {
            Panic.If(Element != state.Element);

            if (size == Size.Auto)
                return SizeStateValue.GetOrCreateValue(this, state, autoValue);

            return ApplyState(state);
        }

        /// <summary>
        /// Apply the given state to this current size.
        /// </summary>
        /// <param name="state">The state to apply.</param>
        /// <returns>The applied value.</returns>
        private SizeStateValue ApplyState(ElementState state)
        {
            Panic.If(Element != state.Element);

            return SizeStateValue.TryGetValueByState(this, state, out var value)
                ? value
                : size.ApplyState(state, this);
        }
    }
}
