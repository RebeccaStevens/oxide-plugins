using System.Collections.Generic;
using System.Linq;
using Oxide.Game.Rust.Cui;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <inheritdoc cref="ElementState"/>
    public class PanelElementState : ElementState
    {
        /// <inheritdoc cref="ElementState.Element"/>
        public new PanelElement Element => (PanelElement)base.Element;

        /// <summary>
        /// All the cui elements that have been created for this state.
        /// </summary>
        protected readonly HashSet<string> CuiElementNames = new();

        /// <inheritdoc cref="ElementState"/>
        public PanelElementState(PanelElement element, BasePlayer player) : base(element, player)
        {
        }

        /// <inheritdoc/>
        internal override string[] GetCuiElementNames() => CuiElementNames.ToArray();

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

            var components = new ElementCuiElements
            {
                Root = root,
                Content = content
            };
            foreach (var cuiElement in components.GetAll())
            {
                Debug.Assert(!string.IsNullOrEmpty(cuiElement.Name));
                CuiElementNames.Add(cuiElement.Name);
            }

            return components;
        }

        /// <summary>
        /// Add CUI components to the CUI elements for this state.
        /// </summary>
        /// <param name="cuiElements">The CUI elements to add components to.</param>
        protected virtual void AddCuiComponents(ElementCuiElements cuiElements)
        {
            Debug.AssertNotNull(cuiElements.Root);

            cuiElements.Root.AddComponent(new CuiImageComponent { Color = ColorToCuiColor(Element.BgColor) });
            if (Element.HasBorder())
                cuiElements.Root.AddComponents(Element.Border.ApplyState(this));
        }
    }
}
