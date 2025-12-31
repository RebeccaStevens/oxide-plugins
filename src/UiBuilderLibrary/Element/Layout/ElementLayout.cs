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
        /// Does the layout needs recomputing.
        /// </summary>
        protected bool IsDirty { get; set; }

        /// <summary>
        /// Create a new layout.
        /// </summary>
        protected ElementLayout()
        {
        }

        /// <summary>
        /// Mark that this layout needs recomputing.
        /// </summary>
        public void MarkDirty() => IsDirty = true;

        /// <summary>
        /// Update the bounds of the given child element to position it in this layout.
        /// </summary>
        /// <param name="childState">The state of a child element of this layout's element.</param>
        protected abstract void PositionChild(ElementState childState);


        /// <summary>
        /// Prepare this layout for positioning the element in the its current state.
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
            Debug.Assert(!parentLayout.IsDirty,
                "Trying to position an element in a layout that is not prepared.");
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
            IsDirty = false;
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
        /// <summary>
        /// The thing that this layout is for.
        /// </summary>
        internal Element For;

        /// <inheritdoc cref="DynamicElementLayout"/>
        /// <param name="element">The element this layout is for.</param>
        protected DynamicElementLayout(Element element)
        {
            IsDirty = true;
            For = element;
            if (!element.HasLayout())
                element.Layout = this;
        }

        /// <summary>
        /// Compute the layout of the element.
        /// </summary>
        protected abstract void ComputeLayout(ElementState state);

        /// <inheritdoc/>
        public override List<ICuiComponent>? Prepare(ElementState state)
        {
            Debug.Assert((state.Element.Layout as DynamicElementLayout)?.For == state.Element,
                "The layout assigned to the element is not for the element.");

            // Compute this layout.
            if (IsDirty)
                ComputeLayout(state);
            IsDirty = false;

            return null;
        }
    }
}