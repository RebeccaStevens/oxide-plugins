using System;
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

        protected readonly HashSet<string> cuiElementNames = new();

        /// <inheritdoc cref="ElementState"/>
        public PanelElementState(PanelElement element, BasePlayer player) : base(element, player)
        {
            Element = element;
        }

        /// <inheritdoc/>
        internal override string[] GetCuiElementNames()
        {
            return cuiElementNames.ToArray();
        }

        /// <inheritdoc/>
        internal override ElementCuiElements CreateCuiElements()
        {
            var components = new ElementCuiElements
            {
                Root = new SafeCuiElement(GetCuiRootName(), GetParentCuiContentName()),
            };

            var rootPosition = Bounds + Element.Margin.GetInnerBounds(this);
            var contentPosition = Element.Padding.GetInnerBounds(this);

            components.Root.SetPosition(rootPosition);
            components.Root.AddComponent(new CuiImageComponent { Color = ColorToCuiColor(Element.BgColor) });

            if (Element.HasBorder())
            {
                contentPosition += Element.Border.GetInnerBounds(this);
                components.Root.AddComponents(Element.Border.ApplyState(this));
            }

            components.Content = new SafeCuiElement(GetCuiContentName(), components.Root.Name);
            components.Content.SetPosition(contentPosition);

            foreach (var cuiElement in components.GetAll())
            {
                Debug.Assert(!string.IsNullOrEmpty(cuiElement.Name));
                cuiElementNames.Add(cuiElement.Name);
            }

            return components;
        }
    }
}
