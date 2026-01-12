using Oxide.Game.Rust.Cui;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    public partial class ButtonElement
    {
        /// <inheritdoc/>
        public class ButtonElementState : PanelElementState
        {
            /// <inheritdoc cref="PanelElementState.Element"/>
            public new ButtonElement Element => (ButtonElement)base.Element;

            /// <inheritdoc cref="ButtonElementState(ButtonElement, BasePlayer)"/>
            public ButtonElementState(ButtonElement element, BasePlayer player) : base(element, player)
            {
            }

            /// <inheritdoc/>
            protected override void AddCuiComponents(ElementCuiElements cuiElements)
            {
                Debug.AssertNotNull(cuiElements.Root);

                cuiElements.Root.AddComponent(new CuiButtonComponent
                {
                    Color = ColorToCuiColor(Element.BgColor),
                    DisabledColor = ColorToCuiColor(Element.BgColorDisabled),
                    HighlightedColor = ColorToCuiColor(Element.BgColorHighlighted),
                    PressedColor = ColorToCuiColor(Element.BgColorPressed),
                    SelectedColor = ColorToCuiColor(Element.BgColorSelected),
                });

                if (Element.HasBorder())
                    cuiElements.Root.AddComponents(Element.Border.ApplyState(this));
            }
        }
    }
}
