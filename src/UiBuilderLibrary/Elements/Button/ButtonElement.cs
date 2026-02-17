using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A button element.
    /// </summary>
    public partial class ButtonElement : BoxModelElement
    {
        private Color? bgColorHighlighted;
        private Color? bgColorPressed;
        private Color? bgColorSelected;
        private Color? bgColorDisabled;
        private LabelElement? label;
        private ImageElement? icon;

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
            Layout = new FlexLayout()
            {
                Direction = FlexLayout.FlexDirection.Horizontal,
                AlignItems = FlexLayout.ItemAlignment.Center,
                JustifyContent = FlexLayout.JustifyAlignment.Center,
                Gap = Theme.Spacing.Medium,
            };
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
            get => bgColorHighlighted ??= ColorMultiply(BgColor, 1.2f);
            set => bgColorHighlighted = value;
        }

        /// <summary>
        /// The color of the button when it is pressed.
        /// </summary>
        public Color BgColorPressed
        {
            get => bgColorPressed ??= ColorMultiply(BgColor, 0.8f);
            set => bgColorPressed = value;
        }

        /// <summary>
        /// The color of the button when it is selected.
        /// </summary>
        public Color BgColorSelected
        {
            get => bgColorSelected ??= BgColor;
            set => bgColorSelected = value;
        }

        /// <summary>
        /// The color of the button when it is disabled.
        /// </summary>
        public Color BgColorDisabled
        {
            get => bgColorDisabled ??= ColorMultiply(BgColor, 0.4f);
            set => bgColorDisabled = value;
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
        public LabelElement Label
        {
            get => label ??= new LabelElement(this)
            {
                Name = $"{Name}-label",
                Weight = -1,
                TextAlignment = TextAnchor.MiddleCenter,
                TextColor = Theme.Colors.Text.Regular,
                Font = Font.Regular,
                FontSize = Theme.FontSize.Large,
            };
            set => label = value;
        }

        /// <summary>
        /// The icon element to display on the button.
        /// </summary>
        public ImageElement Icon
        {
            get => icon ??= new ImageElement(this)
            {
                Name = $"{Name}-icon",
                Weight = -2,
                Size = Theme.FontSize.Large,
                Color = Theme.Colors.Icon,
            };
            set => icon = value;
        }

        /// <inheritdoc/>
        protected override ElementState InitialState(BasePlayer player) => new ButtonElementState(this, player);
    }
}
