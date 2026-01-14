using Oxide.Game.Rust.Cui;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    public partial class ButtonElement
    {
        /// <inheritdoc/>
        public class ButtonElementState : BoxModelElementStateState
        {
            /// <inheritdoc cref="BoxModelElementStateState.Element"/>
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
                    Enabled = Element.Enabled,
                    NormalColor = ColorToCuiColor(Element.BgColor),
                    DisabledColor = ColorToCuiColor(Element.BgColorDisabled),
                    HighlightedColor = ColorToCuiColor(Element.BgColorHighlighted),
                    PressedColor = ColorToCuiColor(Element.BgColorPressed),
                    SelectedColor = ColorToCuiColor(Element.BgColorSelected),
                    ColorMultiplier = Element.BgColorMultiplier,
                    Material = string.IsNullOrEmpty(Element.BgMaterial) ? null : Element.BgMaterial,
                    Sprite = string.IsNullOrEmpty(Element.Sprite) ? null : Element.Sprite,
                    Close = (Element.Action as UiActionCloseUi)?.GetUiRootCuiElementName(Player),
                    Command = Element.Action?.GetCommand(),
                });

                if (Element.HasBorder())
                    cuiElements.Root.AddComponents(Element.Border.ApplyState(this));
            }
        }
    }
}
