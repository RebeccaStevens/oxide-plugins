using System.Linq;
using Oxide.Game.Rust.Cui;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <inheritdoc cref="ElementState"/>
    public class PanelElementState : ElementState
    {
        /// <inheritdoc cref="ElementState.Element"/>
        public new PanelElementBase Element => (PanelElementBase)base.Element;

        /// <inheritdoc cref="ElementState"/>
        public PanelElementState(PanelElementBase element, BasePlayer player) : base(element, player)
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

            if (Element.HasBorder())
                CuiBounds.AddPosition(Element.Border.GetInnerBounds(this));

            ContentBounds.MaximizePosition();
            ContentBounds.AddPosition(Element.Padding.GetInnerBounds(this));
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
        protected virtual ElementCuiElements CreateCuiElementsBase()
        {
            var root = new SafeCuiElement(GetCuiRootName(), GetParentCuiContentName());
            var content = Element.GetChildren().Any()
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
        protected virtual void AddCuiComponents(ElementCuiElements cuiElements)
        {
            Debug.AssertNotNull(cuiElements.Root);

            cuiElements.Root.AddComponent(new CuiImageComponent
            {
                Color = ColorToCuiColor(Element.Color),
                Material = string.IsNullOrEmpty(Element.Material) ? null : Element.Material,
                Sprite = string.IsNullOrEmpty(Element.Sprite) ? null : Element.Sprite,
            });

            if (Element.HasBorder())
                cuiElements.Root.AddComponents(Element.Border.ApplyState(this));
        }
    }
}
