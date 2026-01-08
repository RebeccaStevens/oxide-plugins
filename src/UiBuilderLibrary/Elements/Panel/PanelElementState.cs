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
        /// All the cui elements that have been created for this state.
        /// </summary>
        protected readonly HashSet<string> CuiElementNames = new();

        /// <inheritdoc cref="ElementState"/>
        public PanelElementState(PanelElement element, BasePlayer player) : base(element, player)
        {
            Element = element;
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

            // Workaround for how the client renders nested elements.
            CuiBounds.FromRight += new Bounds.Value(0, 1);
            ContentBounds.FromRight += new Bounds.Value(0, -1);
        }

        /// <inheritdoc/>
        protected override ElementCuiElements CreateCuiElements()
        {
            var root = new SafeCuiElement(GetCuiRootName(), GetParentCuiContentName());
            var content = Element.GetChildren().Any()
                ? new SafeCuiElement(GetCuiContentName(), root.Name)
                : null; // Content isn't needed if there are no children.

            root.SetPosition(CuiBounds);
            root.AddComponent(new CuiImageComponent { Color = ColorToCuiColor(Element.BgColor) });
            if (Element.HasBorder())
                root.AddComponents(Element.Border.ApplyState(this));

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
    }
}
