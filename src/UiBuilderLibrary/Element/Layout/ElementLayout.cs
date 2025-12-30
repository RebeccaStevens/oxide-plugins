namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// Used to position child elements.
    /// </summary>
    public abstract class ElementLayout
    {
        /// <summary>
        /// Update the bounds of the given child element to position it in this layout.
        /// </summary>
        /// <param name="childState">The state of a child element of this layout's element.</param>
        protected abstract void PositionChild(ElementState childState);


        /// <summary>
        /// Apply this layout to the given element.
        /// </summary>
        public virtual void Apply(ElementState state)
        {
            var parentLayout = GetParentLayout(state);
            Panic.IfNull(parentLayout);
            parentLayout.PositionChild(state);
        }

        /// <summary>
        /// Get the parent layout of the given element.
        /// </summary>
        protected ElementLayout? GetParentLayout(ElementState state)
        {
            return state.Element.GetParent()?.Layout ?? (state.Element as RootElement)?.Ui.Layout;
        }
    }

    /// <summary>
    /// A layout that always positions the child element in the same place.
    /// </summary>
    public abstract class StaticElementLayout : ElementLayout
    {
    }

    /// <summary>
    /// A layout where the position of the child element is dependent on something (like sibling elements, children elements, etc.).
    /// </summary>
    public abstract class DynamicElementLayout : ElementLayout
    {
        /// <summary>
        /// Does the layout needs recomputing.
        /// </summary>
        public bool IsDirty { get; private set; }

        /// <summary>
        /// Create a new layout.
        /// </summary>
        protected DynamicElementLayout()
        {
            IsDirty = true;
        }

        /// <summary>
        /// Compute the layout of the element.
        /// </summary>
        protected abstract void ComputeLayout(ElementState state);

        /// <summary>
        /// Mark that this layout needs recomputing.
        /// </summary>
        internal void MarkDirty() => IsDirty = true;

        /// <summary>
        /// Apply this layout to the given element.
        /// </summary>
        public override void Apply(ElementState state)
        {
            // Compute this layout.
            if (IsDirty)
            {
                ComputeLayout(state);
                IsDirty = false;
            }

            // The parent layout should already be applied and should not have been marked dirty since.
            Debug.Assert(
                (GetParentLayout(state) as DynamicElementLayout)?.IsDirty != true,
                "Layout is dirty when positioning child.");

            base.Apply(state);
        }
    }
}