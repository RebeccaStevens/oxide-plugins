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
        /// The background color of the element.
        /// </summary>
        public Color BgColor { get; set; }

        /// <summary>
        /// The value stored for the background color when the button is highlighted.
        /// </summary>
        private Color? bgColorHighlighted;

        /// <summary>
        /// The color of the button when it is highlighted.
        /// </summary>
        public Color BgColorHighlighted
        {
            get => bgColorHighlighted ??= Theme.Colors.ButtonHighlighted ?? ColorMultiply(BgColor, 1.2f);
            set => bgColorHighlighted = value;
        }

        /// <summary>
        /// The value stored for the background color when the button is pressed.
        /// </summary>
        private Color? bgColorPressed;

        /// <summary>
        /// The color of the button when it is pressed.
        /// </summary>
        public Color BgColorPressed
        {
            get => bgColorPressed ??= Theme.Colors.ButtonPressed ?? ColorMultiply(BgColor, 0.8f);
            set => bgColorPressed = value;
        }

        /// <summary>
        /// The value stored for the background color when the button is selected.
        /// </summary>
        private Color? bgColorSelected;

        /// <summary>
        /// The color of the button when it is selected.
        /// </summary>
        public Color BgColorSelected
        {
            get => bgColorSelected ??= Theme.Colors.ButtonSelected ?? BgColor;
            set => bgColorSelected = value;
        }

        /// <summary>
        /// The value stored for the background color when the button is disabled.
        /// </summary>
        private Color? bgColorDisabled;

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
        public float BgColorMultiplier = 1f;

        /// <summary>
        /// The material to apply to the element.
        /// </summary>
        public string? BgMaterial { get; set; }

        /// <summary>
        /// The action to perform when the button is clicked.
        /// </summary>
        public UiAction? OnClick;

        /// <summary>
        /// The stored label element.
        /// </summary>
        private LabelElement? label;

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
                TextColor = Theme.Colors.TextRegular,
                Font = Font.Regular,
                FontSize = Theme.FontSize.Large,
            };
            set => label = value;
        }

        /// <summary>
        /// The stored icon element.
        /// </summary>
        private ImageElement? icon;

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

        /// <summary>
        /// Create a new button element.
        /// </summary>
        public ButtonElement(Element element) : base(element)
        {
            Name = "button";
            BgColor = Theme.Colors.ButtonBase;
            Padding.SetSize(Theme.Spacing.Medium, Size.Zero);
            Width = Theme.ItemSizing.ExtraLarge;
            Height = Theme.ItemSizing.Small;
            Layout = new FlexLayout()
            {
                Direction = FlexLayout.FlexDirection.Horizontal,
                AlignItems = FlexLayout.ItemAlignment.Center,
                JustifyContent = FlexLayout.JustifyAlignment.Center,
                Gap = Theme.Spacing.Medium,
            };
        }

        /// <inheritdoc/>
        protected override ElementState InitialState(BasePlayer player) => new ButtonElementState(this, player);
    }
}
