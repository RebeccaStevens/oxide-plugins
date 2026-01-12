using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A button element.
    /// </summary>
    public partial class ButtonElement : PanelElementBase
    {
        /// <summary>
        /// Whether the button is enabled or not.
        /// </summary>
        public bool Enabled = true;

        /// <summary>
        /// The background color of the element.
        /// </summary>
        public Color BgColor
        {
            get => Color;
            set => Color = value;
        }

        /// <summary>
        /// The value stored for the background color when the button is highlighted.
        /// </summary>
        private Color? bgColorHighlighted;

        /// <summary>
        /// The color of the icon when it is highlighted.
        /// </summary>
        public Color BgColorHighlighted {
            get => bgColorHighlighted ??= ColorMultiply(Color, 1.2f);
            set => bgColorHighlighted = value;
        }

        /// <summary>
        /// The value stored for the background color when the button is pressed.
        /// </summary>
        private Color? bgColorPressed;

        /// <summary>
        /// The color of the icon when it is pressed.
        /// </summary>
        public Color BgColorPressed {
            get => bgColorPressed ??= ColorMultiply(Color, 0.8f);
            set => bgColorPressed = value;
        }

        /// <summary>
        /// The value stored for the background color when the button is selected.
        /// </summary>
        private Color? bgColorSelected;

        /// <summary>
        /// The color of the icon when it is selected.
        /// </summary>
        public Color BgColorSelected {
            get => bgColorSelected ??= ColorPallete.RustGreen;
            set => bgColorSelected = value;
        }

        /// <summary>
        /// The value stored for the background color when the button is disabled.
        /// </summary>
        private Color? bgColorDisabled;

        /// <summary>
        /// The color of the icon when it is disabled.
        /// </summary>
        public Color BgColorDisabled {
            get => bgColorDisabled ??= ColorMultiply(Color, 0.4f);
            set => bgColorDisabled = value;
        }

        /// <summary>
        /// The color multiplier to apply to the background colors.<br/>
        /// See <a href="https://docs.unity3d.com/560/Documentation/ScriptReference/UI.ColorBlock-colorMultiplier.html">Unity's documentation</a> for more information.
        /// </summary>
        public float BgColorMultiplier = 1f;

        /// <inheritdoc cref="PanelElementBase.Material"/>
        public string? BgMaterial
        {
            get => Material;
            set => Material = value;
        }

        /// <summary>
        /// The action to perform when the button is clicked.
        /// </summary>
        public UiAction? Action;

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
                TextAlignment = TextAnchor.MiddleCenter ,
                TextColor = ColorPallete.TextRegular,
                Font = Font.Regular,
                FontSize = Size.Pixels(18),
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
                Size = Size.Pixels(16),
                Color = ColorPallete.TextRegular,
            };
            set => icon = value;
        }

        /// <summary>
        /// Create a new button element.
        /// </summary>
        public ButtonElement(Element element) : base(element)
        {
            BgColor = ColorPallete.ItemLevel1;
            BgMaterial = null;
            Padding.SetSize(Size.Pixels(8), Size.Zero);
            Width = Size.Pixels(128);
            Height = Size.Pixels(32);
            Layout = new FlexLayout(this)
            {
                Direction = FlexLayout.FlexDirection.Horizontal,
                AlignItems = FlexLayout.ItemAlignment.Center,
                JustifyContent = FlexLayout.JustifyAlignment.Center,
                Gap = Size.Pixels(8),
            };
        }

        /// <inheritdoc/>
        protected override ElementState InitialState(BasePlayer player) => new ButtonElementState(this, player);
    }
}
