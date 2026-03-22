using System;
using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A button element.
    /// </summary>
    public partial class ToggleButtonElement : BoxModelElement
    {
        /// <summary>
        /// Backing field for the <see cref="BgColorInactiveHighlighted"/> property.
        /// </summary>
        protected Color? BgColorInactiveHighlightedBacking;

        /// <summary>
        /// Backing field for the <see cref="BgColorActiveHighlighted"/> property.
        /// </summary>
        protected Color? BgColorActiveHighlightedBacking;

        /// <summary>
        /// Backing field for the <see cref="BgColorInactivePressed"/> property.
        /// </summary>
        protected Color? BgColorInactivePressedBacking;

        /// <summary>
        /// Backing field for the <see cref="BgColorActivePressed"/> property.
        /// </summary>
        protected Color? BgColorActivePressedBacking;

        /// <summary>
        /// Backing field for the <see cref="BgColorInactiveDisabled"/> property.
        /// </summary>
        protected Color? BgColorInactiveDisabledBacking;

        /// <summary>
        /// Backing field for the <see cref="BgColorActiveDisabled"/> property.
        /// </summary>
        protected Color? BgColorActiveDisabledBacking;

        /// <summary>
        /// Backing field for the <see cref="BorderActive"/> property.
        /// </summary>
        protected OutlineElementComponent? BorderActiveBacking;

        /// <summary>
        /// Backing field for the <see cref="Label"/> property.
        /// </summary>
        protected LabelElement? LabelBacking;

        /// <summary>
        /// Backing field for the <see cref="LabelActive"/> property.
        /// </summary>
        protected LabelElement? LabelActiveBacking;

        /// <summary>
        /// Backing field for the <see cref="Icon"/> property.
        /// </summary>
        protected ImageElement? IconBacking;

        /// <summary>
        /// Backing field for the <see cref="IconActive"/> property.
        /// </summary>
        protected ImageElement? IconActiveBacking;

        private const string ElementCommandScope = $"{CommandScope}.toggle-button";
        private const string CommandActivate = $"{ElementCommandScope}.activate";
        private const string CommandDeactivate = $"{ElementCommandScope}.deactivate";

        /// <summary>
        /// The color of the button when it is inactive.
        /// </summary>
        public Color BgColorInactive { get; set; }

        /// <summary>
        /// The color of the button when it is active.
        /// </summary>
        public Color BgColorActive { get; set; }

        /// <summary>
        /// The color multiplier to apply to the background colors.<br/>
        /// See <a href="https://docs.unity3d.com/560/Documentation/ScriptReference/UI.ColorBlock-colorMultiplier.html">Unity's documentation</a> for more information.
        /// </summary>
        public float BgColorMultiplier = 1f;

        /// <summary>
        /// The material to apply to the button when it is inactive.
        /// </summary>
        public string? BgMaterial { get; set; }

        /// <summary>
        /// The material to apply to the button when it is active.
        /// </summary>
        public string? BgMaterialActive { get; set; }

        /// <summary>
        /// The action to perform when the button switched to the active state.
        /// </summary>
        public UiAction? OnActivate;

        /// <summary>
        /// The action to perform when the button switched to the inactive state.
        /// </summary>
        public UiAction? OnDeactivate;

        /// <summary>
        /// Create a new toggle button element.
        /// </summary>
        public ToggleButtonElement(Element parent) : base(parent)
        {
            Name = "toggle-button";
            Padding.SetSize(Theme.Spacing.Medium, Size.Zero);
            Width = Theme.ItemSizing.Large;
            Height = Theme.ItemSizing.ExtraSmall;
            BgColorInactive = parent.Theme.Colors.Item.Level1;
            BgColorActive = parent.Theme.Colors.Branding.Primary;
        }

        /// <summary>
        /// The color of the button when it is active and highlighted.
        /// </summary>
        public Color BgColorInactiveHighlighted
        {
            get => BgColorInactiveHighlightedBacking ??= ColorMultiply(BgColorInactive, 1.2f);
            set => BgColorInactiveHighlightedBacking = value;
        }

        /// <summary>
        /// The color of the button when it is active and highlighted.
        /// </summary>
        public Color BgColorActiveHighlighted
        {
            get => BgColorActiveHighlightedBacking ??= ColorMultiply(BgColorActive, 1.2f);
            set => BgColorActiveHighlightedBacking = value;
        }

        /// <summary>
        /// The color of the button when it is pressed.
        /// </summary>
        public Color BgColorInactivePressed
        {
            get => BgColorInactivePressedBacking ??= ColorMultiply(BgColorInactive, 0.8f);
            set => BgColorInactivePressedBacking = value;
        }

        /// <summary>
        /// The color of the button when it is pressed.
        /// </summary>
        public Color BgColorActivePressed
        {
            get => BgColorActivePressedBacking ??= ColorMultiply(BgColorActive, 0.8f);
            set => BgColorActivePressedBacking = value;
        }

        /// <summary>
        /// The color of the button when it is disabled.
        /// </summary>
        public Color BgColorInactiveDisabled
        {
            get => BgColorInactiveDisabledBacking ??= ColorMultiply(BgColorInactive, 0.4f);
            set => BgColorInactiveDisabledBacking = value;
        }

        /// <summary>
        /// The color of the button when it is disabled.
        /// </summary>
        public Color BgColorActiveDisabled
        {
            get => BgColorActiveDisabledBacking ??= ColorMultiply(BgColorActive, 0.4f);
            set => BgColorActiveDisabledBacking = value;
        }

        /// <summary>
        /// The label element to display on the button.
        /// </summary>
        public LabelElement Label => LabelBacking ??= CreateLabel("inactive");

        /// <summary>
        /// The label element to display on the button when it is active.<br/>
        /// If this is not set, the same label as the inactive state will be used.
        /// </summary>
        public LabelElement LabelActive => LabelActiveBacking ??= CreateLabel("active");

        /// <summary>
        /// The icon element to display on the button.
        /// </summary>
        public ImageElement Icon => IconBacking ??= CreateIcon("inactive");

        /// <summary>
        /// The active icon element to display on the button.
        /// </summary>
        public ImageElement IconActive => IconActiveBacking ??= CreateIcon("active");

        /// <inheritdoc/>
        public override ElementLayout Layout
        {
            get => LayoutBacking ??= new FlexLayout()
            {
                Direction = FlexLayout.FlexDirection.Horizontal,
                AlignItems = FlexLayout.ItemAlignment.Center,
                JustifyContent = FlexLayout.JustifyAlignment.Center,
                Gap = Theme.Spacing.Medium,
            };
            set => LayoutBacking = value;
        }

        /// <summary>
        /// The border applied to this element when it is active.
        /// </summary>
        public OutlineElementComponent BorderActive => BorderActiveBacking ??= Border;

        /// <summary>
        /// Does this element have a border?
        /// </summary>
        public override bool HasBorder() =>
            // Only true if there is both an active and inactive border - a null active outline just means to use the same border for both states.
            base.HasBorder() && (BorderActiveBacking?.HasSize() ?? true);

        /// <inheritdoc/>
        protected override ElementState InitialState(BasePlayer player) => new ToggleButtonElementState(this, player);

        /// <inheritdoc cref="Element.GetState"/>
        public new ToggleButtonElementState GetState(BasePlayer player) =>
            (ToggleButtonElementState)base.GetState(player);

        /// <summary>
        /// Creates the label element to be used by this element.
        /// </summary>
        /// <returns>A new label element.</returns>
        protected LabelElement CreateLabel(string subname) => new LabelElement(this)
        {
            Name = $"{Name}-label-${subname}",
            Weight = -1,
            TextAlignment = TextAnchor.MiddleCenter,
            TextColor = Theme.Colors.Text.Regular,
            Font = Font.Regular,
            FontSize = Theme.FontSize.Large,
        };

        /// <summary>
        /// Creates the icon element to be used by this element.
        /// </summary>
        /// <returns>A new icon element.</returns>
        protected ImageElement CreateIcon(string subname) => new ImageElement(this)
        {
            Name = $"{Name}-icon-{subname}",
            Weight = -2,
            Size = Theme.FontSize.Large,
            Color = Theme.Colors.Icon,
        };

        /// <summary>
        /// Register the commands UIs have available.
        /// </summary>
        internal static void RegisterCommands(RustPlugin plugin, Oxide.Game.Rust.Libraries.Command cmd)
        {
            cmd.AddConsoleCommand(CommandActivate, plugin, HandleCommandToggle(true));
            cmd.AddConsoleCommand(CommandDeactivate, plugin, HandleCommandToggle(false));
        }

        /// <summary>
        /// Create the handler for the command to toggle the active state of a toggle button element to either true or false.
        /// </summary>
        internal static Func<ConsoleSystem.Arg, bool> HandleCommandToggle(bool markActive) =>
            (data) =>
            {
                var args = Utils.ParseCommandLineArgs(data.Args);

                switch (args.Values.Count)
                {
                    case 0:
                        data.ReplyWith($"No ids specified.\nUsage: {CommandActivate} <id>");
                        return false;
                    case > 2:
                        data.ReplyWith($"Too many ids specified.\nUsage: {CommandActivate} <id>");
                        return false;
                }

                var player = (BasePlayer)data.Connection.player;
                var id = args.Values[0];
                Debug.Assert(!string.IsNullOrEmpty(id));

                var state = ElementState.GetById(id);
                if (state == null)
                {
#if DEBUG
                    data.ReplyWith($"No element state with id '{id}' found.");
#else
                    data.ReplyWith($"Invalid element state id '{id}'.");
#endif
                    return false;
                }

                if (state is not ToggleButtonElementState elementState)
                {
#if DEBUG
                    data.ReplyWith($"Element with id '{id}' is not a toggle button element state.");
#else
                    data.ReplyWith($"Invalid element state id '{id}'.");
#endif
                    return false;
                }

                if (elementState.Player != player)
                {
#if DEBUG
                    data.ReplyWith($"Element with id '{id}' is not owned by the player.");
#else
                    data.ReplyWith($"Invalid element state id '{id}'.");
#endif
                    return false;
                }

                elementState.IsActive = markActive;
                return true;
            };
    }
}
