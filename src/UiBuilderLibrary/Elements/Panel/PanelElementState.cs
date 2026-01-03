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
        public new readonly PanelElement Element;

        /// <summary>
        /// The bounds of the outer edge of this element.
        /// </summary>
        protected readonly Bounds OuterBounds;

        /// <summary>
        /// The bounds of the content within this element.
        /// </summary>
        protected readonly Bounds InnerBounds;

        /// <summary>
        /// All the cui elements that have been created for this state.
        /// </summary>
        protected readonly HashSet<string> CuiElementNames = new();

        /// <inheritdoc cref="ElementState"/>
        public PanelElementState(PanelElement element, BasePlayer player) : base(element, player)
        {
            Element = element;
            OuterBounds = new Bounds();
            InnerBounds = new Bounds();
        }

        /// <inheritdoc/>
        internal override string[] GetCuiElementNames() => CuiElementNames.ToArray();

        /// <summary>
        /// Update the bounds of this element.
        /// </summary>
        protected void UpdateBounds()
        {
            OuterBounds.SetTo(Bounds);
            OuterBounds.AddPosition(Element.Margin.GetInnerBounds(this));
            if (Element.HasBorder())
                OuterBounds.AddPosition(Element.Border.GetInnerBounds(this));

            InnerBounds.SetTo(OuterBounds);
            InnerBounds.AddPosition(Element.Padding.GetInnerBounds(this));
        }

        /// <inheritdoc/>
        internal override ElementCuiElements CreateCuiElements()
        {
            UpdateBounds();
            var components = new ElementCuiElements
            {
                Root = new SafeCuiElement(GetCuiRootName(), GetParentCuiContentName()),
            };

            components.Root.SetPosition(OuterBounds);
            components.Root.AddComponent(new CuiImageComponent { Color = ColorToCuiColor(Element.BgColor) });
            if (Element.HasBorder())
                components.Root.AddComponents(Element.Border.ApplyState(this));

            components.Content = new SafeCuiElement(GetCuiContentName(), components.Root.Parent);
            components.Content.SetPosition(InnerBounds);

            foreach (var cuiElement in components.GetAll())
            {
                Debug.Assert(!string.IsNullOrEmpty(cuiElement.Name));
                CuiElementNames.Add(cuiElement.Name);
            }

            return components;
        }
    }
}
