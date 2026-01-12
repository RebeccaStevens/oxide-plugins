using System;
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
        /// Prepare this layout for positioning its children and any adjustments needed to the container element.
        /// </summary>
        /// <param name="state">The state of the element to position.</param>
        /// <returns>The cui components that should be added to the element's cui parent.</returns>
        public abstract List<ICuiComponent>? Prepare(ElementState state);

        /// <summary>
        /// Position the given element in its parent's layout.
        /// </summary>
        /// <param name="state">The state of the element to position.</param>
        public static void PositionElementInParent(ElementState state)
        {
            var parentLayout = GetParentLayoutOf(state);
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
        public override List<ICuiComponent>? Prepare(ElementState state)
        {
            return null;
        }
    }

    /// <summary>
    /// A layout where the position of the child element is dependent on something (like sibling elements, children elements, etc.).
    /// </summary>
    public abstract class DynamicElementLayout : ElementLayout
    {
        /// <inheritdoc cref="DynamicElementLayout"/>
        protected DynamicElementLayout()
        {
        }

        /// <summary>
        /// Compute the layout of the element.
        /// </summary>
        protected abstract void ComputeLayout(ElementState state);

        /// <inheritdoc/>
        public override List<ICuiComponent>? Prepare(ElementState state)
        {
            ComputeLayout(state);
            return null;
        }
    }
}
