using System.Collections.Generic;
using Oxide.Game.Rust.Cui;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// Used to position child elements.
    /// </summary>
    public abstract class ElementLayout
    {
        /// <summary>
        /// Create a new layout.
        /// </summary>
        protected ElementLayout()
        {
        }

        /// <summary>
        /// Update the bounds of the given child element to position it in this layout.
        /// </summary>
        /// <param name="childState">The state of a child element of this layout's element.</param>
        protected abstract void PositionChild(ElementState childState);

        /// <summary>
        /// Prepare this layout for positioning the children of the given element.
        /// </summary>
        /// <param name="state">The state of the element that needs to be prepared.</param>
        protected abstract void Prepare(ElementState state);

        /// <summary>
        /// Get any cui components that should be added to the element's cui parent.
        /// </summary>
        /// <param name="state">The state of the element to get the components for.</param>
        /// <returns>The cui components that should be added to the element's cui parent.</returns>
        public virtual List<ICuiComponent>? GetComponentsForContainer(ElementState state) => null;

        /// <summary>
        /// Position the given element in its parent's layout.
        /// </summary>
        /// <param name="state">The state of the element to position.</param>
        public static void PositionElementInParent(ElementState state)
        {
            var parentLayout = GetParentLayoutOf(state);
            var parentState = state.GetParent();
            parentState?.Element.Layout.Prepare(parentState);
            parentLayout.PositionChild(state);
        }

        /// <summary>
        /// Get the parent layout of the given element.
        /// </summary>
        protected static ElementLayout GetParentLayoutOf(ElementState state)
        {
            return state.Element.GetParent()?.Layout ?? AbsoluteLayout.Use();
        }
    }

    /// <summary>
    /// A layout that always positions the child element in the same place.
    /// </summary>
    public abstract class StaticElementLayout : ElementLayout
    {
        /// <inheritdoc cref="StaticElementLayout"/>
        protected StaticElementLayout()
        {
        }

        /// <inheritdoc/>
        protected sealed override void Prepare(ElementState state)
        {
            // Static layouts don't need to prepare anything - that what makes them static.
        }
    }

    /// <summary>
    /// A layout where the position of the child element is dependent on something (like sibling elements, children elements, etc.).
    /// </summary>
    public abstract class DynamicElementLayout : ElementLayout
    {
        /// <summary>
        /// The states that have been prepared for this layout.
        /// </summary>
        private readonly HashSet<ElementState> preparedFor = new();

        /// <inheritdoc cref="DynamicElementLayout"/>
        protected DynamicElementLayout()
        {
        }

        /// <inheritdoc/>
        protected override void Prepare(ElementState state)
        {
            // If the layout hasn't already been computed for this state, compute it.
            if (preparedFor.Add(state))
                ComputeLayout(state);
        }


        /// <summary>
        /// Compute the layout of the element.
        /// </summary>
        protected abstract void ComputeLayout(ElementState state);
    }
}
