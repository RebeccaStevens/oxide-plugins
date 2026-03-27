using System.Linq;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <inheritdoc cref="ElementState"/>
    public abstract class BoxModelElementState : ElementState
    {
        /// <inheritdoc cref="ElementState.Element"/>
        public new BoxModelElement Element => (BoxModelElement)base.Element;

        /// <inheritdoc cref="ElementState"/>
        protected BoxModelElementState(BoxModelElement element, BasePlayer player) : base(element, player)
        {
        }

        internal BoxModelElementState(BoxModelElement element, BasePlayer player, string id) : base(element,
            player, id)
        {
        }

        /// <inheritdoc/>
        internal override string[] GetCuiElementNames() => new[]
        {
            GetCuiRootName(),
            GetCuiContentName(),
        };

        /// <inheritdoc/>
        protected override void ComputeInternalBounds()
        {
            CuiBounds.SetTo(Bounds);
            CuiBounds.AddPosition(Element.Margin.GetInnerBounds(this));
            CuiBounds.SetPivot(Element.Pivot);
            CuiBounds.SetRotation(Element.Rotation);

            if (Element.HasBorder())
                CuiBounds.AddPosition(Element.Border.GetInnerBounds(this));

            ContentBounds.MaximizePosition();
            ContentBounds.AddPosition(Element.Padding.GetInnerBounds(this));
            ContentBounds.SetPivot(Element.ContentPivot);
            ContentBounds.SetRotation(Element.ContentRotation);
        }

        /// <inheritdoc/>
        protected sealed override ElementCuiElements CreateCuiElements()
        {
            var cuiElements = CreateCuiElementsBase();
            AddCuiComponents(cuiElements);
            return cuiElements;
        }

        /// <summary>
        /// Create the CUI elements for this state without adding any CUI components to them.
        /// </summary>
        protected virtual ElementCuiElements CreateCuiElementsBase(bool forceContent = false)
        {
            var root = new SafeCuiElement(GetCuiRootName(), GetParentCuiContentName());
            var content = forceContent || Element.GetChildren().Any()
                ? new SafeCuiElement(GetCuiContentName(), root.Name)
                : null; // Content isn't needed if there are no children.

            root.SetPosition(CuiBounds);
            content?.SetPosition(ContentBounds);

            return new ElementCuiElements
            {
                Root = root,
                Content = content
            };
        }

        /// <summary>
        /// Add CUI components to the CUI elements for this state.
        /// </summary>
        /// <param name="cuiElements">The CUI elements to add components to.</param>
        protected abstract void AddCuiComponents(ElementCuiElements cuiElements);
    }
}
