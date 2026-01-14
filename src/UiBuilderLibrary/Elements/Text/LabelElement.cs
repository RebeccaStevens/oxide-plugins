using System;
using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// An label element.
    /// </summary>
    public partial class LabelElement : BoxModelElementState
    {
        /// <summary>
        /// The text of the label.
        /// </summary>
        public string Text;

        /// <inheritdoc cref="BoxModelElementState.Color"/>
        public Color TextColor
        {
            get => Color;
            set => Color = value;
        }

        /// <summary>
        /// The font face to use for the label.
        /// </summary>
        public Font Font;

        /// <summary>
        /// The context for the font size of this element.
        /// </summary>
        internal readonly SizeContext FontSizeContext;

        public TextAnchor TextAlignment;
        public VerticalWrapMode VerticalOverflow;

        /// <summary>
        /// The font size to use for the label.
        /// </summary>
        public Size FontSize
        {
            get => FontSizeContext.Get();
            set
            {
                if (value is not Size.PixelSize size)
                    throw new InvalidOperationException(
                        "Only pixel sizes are currently supported for font sizes.");
                FontSizeContext.Set(size);
            }
        }

        /// <summary>
        /// Create a new label element.
        /// </summary>
        /// <param name="parent">The parent of this element.</param>
        public LabelElement(Element parent) : base(parent, null)
        {
            Text = "[no label]";
            TextColor = ColorPallete.TextRegular;
            Font = Font.Regular;
            FontSizeContext = new SizeContext("FontSize", this, Size.Pixels(14));
            TextAlignment = TextAnchor.UpperLeft;
            VerticalOverflow = VerticalWrapMode.Overflow;
        }

        /// <inheritdoc/>
        protected override ElementState InitialState(BasePlayer player) => new LabelElementState(this, player);
    }
}
