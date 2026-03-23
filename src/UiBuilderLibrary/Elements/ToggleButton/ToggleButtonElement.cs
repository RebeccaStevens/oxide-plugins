using System;
using System.Diagnostics.CodeAnalysis;
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
        public Color? BgColorInactiveHighlightedBacking { get; protected set; }

        /// <summary>
        /// Backing field for the <see cref="BgColorActiveHighlighted"/> property.
        /// </summary>
        public Color? BgColorActiveHighlightedBacking { get; protected set; }

        /// <summary>
        /// Backing field for the <see cref="BgColorInactivePressed"/> property.
        /// </summary>
        public Color? BgColorInactivePressedBacking { get; protected set; }

        /// <summary>
        /// Backing field for the <see cref="BgColorActivePressed"/> property.
        /// </summary>
        public Color? BgColorActivePressedBacking { get; protected set; }

        /// <summary>
        /// Backing field for the <see cref="BgColorInactiveDisabled"/> property.
        /// </summary>
        public Color? BgColorInactiveDisabledBacking { get; protected set; }

        /// <summary>
        /// Backing field for the <see cref="BgColorActiveDisabled"/> property.
        /// </summary>
        public Color? BgColorActiveDisabledBacking { get; protected set; }

        /// <summary>
        /// Backing field for the <see cref="BorderActive"/> property.
        /// </summary>
        public OutlineElementComponent? BorderActiveBacking { get; protected set; }

        /// <summary>
        /// Backing field for the <see cref="Label"/> property.
        /// </summary>
        public LabelElement? LabelBacking { get; protected set; }

        /// <summary>
        /// Backing field for the <see cref="LabelActive"/> property.
        /// </summary>
        public LabelElement? LabelActiveBacking { get; protected set; }

        /// <summary>
        /// Backing field for the <see cref="Icon"/> property.
        /// </summary>
        public ImageElement? IconBacking { get; protected set; }

        /// <summary>
        /// Backing field for the <see cref="IconActive"/> property.
        /// </summary>
        public ImageElement? IconActiveBacking { get; protected set; }

        private const string ElementCommandScope = $"{CommandScope}.toggle-button";
        private const string CommandBaseActivate = $"{ElementCommandScope}.activate";
        private const string CommandBaseDeactivate = $"{ElementCommandScope}.deactivate";
        private const string CommandBaseClicked = $"{ElementCommandScope}.clicked";

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
        /// If set, the button's active state is controlled by this function rather than being freely toggleable.
        /// </summary>
        public Func<ToggleButtonElementState, bool, bool>? IsActive { get; set; }

        /// <summary>
        /// The action to perform when the button switched to the active state.
        /// </summary>
        public UiAction? OnActivate { get; set; }

        /// <summary>
        /// The action to perform when the button switched to the inactive state.
        /// </summary>
        public UiAction? OnDeactivate { get; set; }

        /// <summary>
        /// The action to perform when the button is clicked.
        /// </summary>
        public UiAction? OnClick { get; set; }

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
        public LabelElement Label
        {
            get
            {
                if (LabelBacking == null)
                {
                    LabelBacking = CreateLabel("base");
                    if (LabelActiveBacking != null)
                        LabelBacking.IsEnabled = (labelState, _) => !GetState(labelState.Player).IsActive;
                }

                return LabelBacking;
            }
        }

        /// <summary>
        /// The label element to display on the button when it is active.<br/>
        /// If this is not set, the same label as the inactive state will be used.
        /// </summary>
        public LabelElement LabelActive
        {
            get
            {
                if (LabelActiveBacking == null)
                {
                    LabelActiveBacking = CreateLabel("active");
                    LabelActiveBacking.IsEnabled = (labelState, _) => GetState(labelState.Player).IsActive;
                }

                return LabelActiveBacking;
            }
        }

        /// <summary>
        /// The icon element to display on the button.
        /// </summary>
        public ImageElement Icon
        {
            get
            {
                if (IconBacking == null)
                {
                    IconBacking = CreateIcon("base");
                    if (IconActiveBacking != null)
                        IconBacking.IsEnabled = (iconState, _) => !GetState(iconState.Player).IsActive;
                }

                return IconBacking;
            }
        }

        /// <summary>
        /// The active icon element to display on the button.
        /// </summary>
        public ImageElement IconActive
        {
            get
            {
                if (IconActiveBacking == null)
                {
                    IconActiveBacking = CreateIcon("active");
                    IconActiveBacking.IsEnabled = (iconState, _) => GetState(iconState.Player).IsActive;
                }

                return IconActiveBacking;
            }
        }

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
        public new ToggleButtonElementState GetState(BasePlayer player, bool markStrong = false) =>
            (ToggleButtonElementState)base.GetState(player, markStrong);

        /// <summary>
        /// Creates the label element to be used by this element.
        /// </summary>
        /// <returns>A new label element.</returns>
        protected LabelElement CreateLabel(string subname) => new LabelElement(this)
        {
            Name = $"{Name}-label-{subname}",
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
            cmd.AddConsoleCommand(CommandBaseActivate, plugin, HandleCommandToggle(true));
            cmd.AddConsoleCommand(CommandBaseDeactivate, plugin, HandleCommandToggle(false));
            cmd.AddConsoleCommand(CommandBaseClicked, plugin, HandleCommandClicked);
        }

        /// <summary>
        /// Create the handler for the command indicating that the toggle button's state should be changed.
        /// </summary>
        internal static Func<ConsoleSystem.Arg, bool> HandleCommandToggle(bool markActive) =>
            (data) =>
            {
                if (!HandleCommandCommon(data, out var id, out var state))
                    return false;

                if (state.Element.IsActive != null)
                {
#if DEBUG
                    data.ReplyWith($"Cannot use command \"{data.cmd.Name}\" on a toggle button that is controlled.");
#else
                    data.ReplyWith($"Internal error: Element state id: {id}");
#endif
                    return false;
                }

                state.IsActive = markActive;
                state.Element.OnClick?.Execute(state.Player);

                state.Sync();
                return true;
            };


        /// <summary>
        /// Create the handler for the command indidcating that the toggle button has been clicked while it is controlled.
        /// </summary>
        internal static bool HandleCommandClicked(ConsoleSystem.Arg data)
        {
            if (!HandleCommandCommon(data, out var id, out var state))
                return false;

            if (state.Element.IsActive == null)
            {
#if DEBUG
                data.ReplyWith($"Cannot use command \"{data.cmd.Name}\" on a toggle button that is not controlled.");
#else
                data.ReplyWith($"Internal error: Element state id: {id}");
#endif
                return false;
            }

            state.Element.OnClick?.Execute(state.Player);
            return true;
        }

        /// <summary>
        /// Common code for handling commands.
        /// </summary>
        private static bool HandleCommandCommon(ConsoleSystem.Arg data, [NotNullWhen(true)] out string? id,
            [NotNullWhen(true)] out ToggleButtonElementState? state)
        {
            var args = Utils.ParseCommandLineArgs(data.Args);
            id = null;
            state = null;

            switch (args.Values.Count)
            {
                case 0:
                    data.ReplyWith($"No ids specified.\nUsage: {data.cmd.Name} <id>");
                    return false;
                case > 2:
                    data.ReplyWith($"Too many parameters specified.\nUsage: {data.cmd.Name} <id>");
                    return false;
            }

            var player = (BasePlayer)data.Connection.player;
            id = args.Values[0];
            Debug.Assert(!string.IsNullOrEmpty(id));

            try
            {
                state = (ToggleButtonElementState?)ElementState.GetById(id);
            }
            catch (InvalidCastException)
            {
#if DEBUG
                data.ReplyWith($"Element with id '{id}' is not a toggle button element state.");
#else
                data.ReplyWith($"Invalid element state id '{id}'.");
#endif
                return false;
            }


            if (state == null)
            {
#if DEBUG
                data.ReplyWith($"No element state with id '{id}' found.");
#else
                data.ReplyWith($"Invalid element state id '{id}'.");
#endif
                return false;
            }

            if (state.Player != player)
            {
#if DEBUG
                data.ReplyWith($"Element with id '{id}' is not owned by the player.");
#else
                data.ReplyWith($"Invalid element state id '{id}'.");
#endif
                return false;
            }

            return true;
        }
    }
}
