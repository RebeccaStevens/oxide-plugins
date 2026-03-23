using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A button element.
    /// </summary>
    public partial class ButtonElement : BoxModelElement
    {
        /// <summary>
        /// Backing field for the <see cref="BgColorHighlighted"/> property.
        /// </summary>
        public Color? BgColorHighlightedBacking { get; protected set; }

        /// <summary>
        /// Backing field for the <see cref="BgColorPressed"/> property.
        /// </summary>
        public Color? BgColorPressedBacking { get; protected set; }

        /// <summary>
        /// Backing field for the <see cref="BgColorSelected"/> property.
        /// </summary>
        public Color? BgColorSelectedBacking { get; protected set; }

        /// <summary>
        /// Backing field for the <see cref="BgColorDisabled"/> property.
        /// </summary>
        public Color? BgColorDisabledBacking { get; protected set; }

        /// <summary>
        /// The backing field for the <see cref="Label"/> property.
        /// </summary>
        public LabelElement? LabelBacking { get; protected set; }

        /// <summary>
        /// The backing field for the <see cref="Icon"/> property.
        /// </summary>
        public ImageElement? IconBacking { get; protected set; }

        /// <summary>
        /// Create a new button element.
        /// </summary>
        /// <param name="parent">The parent of this element.</param>
        public ButtonElement(Element parent) : base(parent)
        {
            Name = "button";
            BgColor = Theme.Colors.Item.Level1;
            Padding.SetSize(Theme.Spacing.Medium, Size.Zero);
            Width = Theme.ItemSizing.Large;
            Height = Theme.ItemSizing.ExtraSmall;
        }

        /// <summary>
        /// The color of the button.
        /// </summary>
        public Color BgColor { get; set; }

        /// <summary>
        /// The color of the button when it is highlighted.
        /// </summary>
        public Color BgColorHighlighted
        {
            get => BgColorHighlightedBacking ??= ColorMultiply(BgColor, 1.2f);
            set => BgColorHighlightedBacking = value;
        }

        /// <summary>
        /// The color of the button when it is pressed.
        /// </summary>
        public Color BgColorPressed
        {
            get => BgColorPressedBacking ??= ColorMultiply(BgColor, 0.8f);
            set => BgColorPressedBacking = value;
        }

        /// <summary>
        /// The color of the button when it is selected.
        /// </summary>
        public Color BgColorSelected
        {
            get => BgColorSelectedBacking ??= BgColor;
            set => BgColorSelectedBacking = value;
        }

        /// <summary>
        /// The color of the button when it is disabled.
        /// </summary>
        public Color BgColorDisabled
        {
            get => BgColorDisabledBacking ??= ColorMultiply(BgColor, 0.4f);
            set => BgColorDisabledBacking = value;
        }

        /// <summary>
        /// The color multiplier to apply to the background colors.<br/>
        /// See <a href="https://docs.unity3d.com/560/Documentation/ScriptReference/UI.ColorBlock-colorMultiplier.html">Unity's documentation</a> for more information.
        /// </summary>
        public float BgColorMultiplier { get; set; } = 1f;

        /// <summary>
        /// The material to apply to the element.
        /// </summary>
        public string? BgMaterial { get; set; }

        /// <summary>
        /// The action to perform when the button is clicked.
        /// </summary>
        public UiAction? OnClick { get; set; }

        /// <summary>
        /// The label element to display on the button.
        /// </summary>
        public LabelElement Label => LabelBacking ??= new LabelElement(this)
        {
            Name = $"{Name}-label",
            Weight = -1,
            TextAlignment = TextAnchor.MiddleCenter,
            TextColor = Theme.Colors.Text.Regular,
            Font = Font.Regular,
            FontSize = Theme.FontSize.Large,
        };

        /// <summary>
        /// The icon element to display on the button.
        /// </summary>
        public ImageElement Icon => IconBacking ??= new ImageElement(this)
        {
            Name = $"{Name}-icon",
            Weight = -2,
            Size = Theme.FontSize.Large,
            Color = Theme.Colors.Icon,
        };

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

        /// <inheritdoc/>
        protected override ElementState InitialState(BasePlayer player) => new ButtonElementState(this, player);
    }
}
