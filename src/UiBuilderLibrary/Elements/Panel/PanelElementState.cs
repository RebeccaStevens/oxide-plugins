using Oxide.Game.Rust.Cui;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <inheritdoc cref="BoxModelElementState"/>
    public class PanelElementState : BoxModelElementState
    {
        /// <inheritdoc cref="BoxModelElementState.Element"/>
        public new PanelElement Element => (PanelElement)base.Element;

        /// <inheritdoc cref="BoxModelElementState"/>
        public PanelElementState(PanelElement element, BasePlayer player) : base(element, player)
        {
        }

        internal PanelElementState(PanelElement element, BasePlayer player, string id) : base(element,
            player, id)
        {
        }

        /// <inheritdoc/>
        protected override void AddCuiComponents(ElementCuiElements cuiElements)
        {
            Debug.AssertNotNull(cuiElements.Root);

            cuiElements.Root.AddComponent(new CuiImageComponent
            {
                Color = ColorToCuiColor(Element.BgColor),
                Material = string.IsNullOrEmpty(Element.Material) ? null : Element.Material,
            });

            if (Element.HasBorder())
                cuiElements.Root.AddComponents(Element.Border.ApplyState(this));
        }
    }
}
