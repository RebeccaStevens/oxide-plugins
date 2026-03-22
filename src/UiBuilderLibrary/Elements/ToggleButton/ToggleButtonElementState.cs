using Oxide.Game.Rust.Cui;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    public partial class ToggleButtonElement
    {
        /// <inheritdoc/>
        public class ToggleButtonElementState : BoxModelElementState
        {
            /// <summary>
            /// Backing field for the <see cref="IsActive"/> property.
            /// </summary>
            protected bool IsActiveBacking;

            /// <summary>
            /// The command to run to activate the toggle button.
            /// </summary>
            protected readonly string ActivateCommand;

            /// <summary>
            /// The command to run to deactivate the toggle button.
            /// </summary>
            protected readonly string DeactivateCommand;

            /// <inheritdoc cref="BoxModelElementState.Element"/>
            public new ToggleButtonElement Element => (ToggleButtonElement)base.Element;

            /// <inheritdoc cref="ToggleButtonElementState(ToggleButtonElement, BasePlayer)"/>
            public ToggleButtonElementState(ToggleButtonElement element, BasePlayer player) : base(element, player)
            {
                ActivateCommand = $"{CommandActivate} {Id}";
                DeactivateCommand = $"{CommandDeactivate} {Id}";
            }

            /// <summary>
            /// Whether the toggle button is active.
            /// </summary>
            public bool IsActive
            {
                get => Element.IsActive != null
                    ? (IsActiveBacking = Element.IsActive(this, IsActiveBacking))
                    : IsActiveBacking;
                set
                {
                    if (IsActiveBacking == value)
                        return;
                    IsActiveBacking = value;
                    if (IsActiveBacking)
                        Element.OnActivate?.Execute(Player);
                    else
                        Element.OnDeactivate?.Execute(Player);
                    NeedsSync = true;
                    Sync();
                }
            }

            /// <inheritdoc/>
            protected override void AddCuiComponents(ElementCuiElements cuiElements)
            {
                Debug.AssertNotNull(cuiElements.Root);

                cuiElements.Root.AddComponent(new CuiButtonComponent
                {
                    NormalColor = ColorToCuiColor(IsActive ? Element.BgColorActive : Element.BgColorInactive),
                    DisabledColor =
                        ColorToCuiColor(IsActive ? Element.BgColorActiveDisabled : Element.BgColorInactiveDisabled),
                    HighlightedColor =
                        ColorToCuiColor(
                            IsActive ? Element.BgColorActiveHighlighted : Element.BgColorInactiveHighlighted),
                    PressedColor =
                        ColorToCuiColor(IsActive ? Element.BgColorActivePressed : Element.BgColorInactivePressed),
                    SelectedColor = ColorToCuiColor(
                        Element.IsActive == null
                            ? IsActive
                                ? Element.BgColorInactive
                                : Element.BgColorActive
                            : IsActive
                                ? Element.BgColorActive
                                : Element.BgColorInactive),
                    ColorMultiplier = Element.BgColorMultiplier,
                    Material = IsActive
                        ? (string.IsNullOrEmpty(Element.BgMaterialActive) ? null : Element.BgMaterialActive)
                        : (string.IsNullOrEmpty(Element.BgMaterial) ? null : Element.BgMaterial),
                    Command = IsActive ? DeactivateCommand : ActivateCommand,
                });

                if (Element.HasBorder())
                    cuiElements.Root.AddComponents(
                        IsActive
                            ? Element.BorderActive.ApplyState(this)
                            : Element.Border.ApplyState(this));
            }
        }
    }
}
