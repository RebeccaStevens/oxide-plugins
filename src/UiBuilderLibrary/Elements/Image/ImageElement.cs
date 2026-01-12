using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// An image element.
    /// </summary>
    public partial class ImageElement : PanelElementBase
    {
        /// <inheritdoc cref="PanelElementBase.Color"/>
        public new Color Color
        {
            get => base.Color;
            set => base.Color = value;
        }

        /// <inheritdoc cref="PanelElementBase.Sprite"/>
        public new string? Sprite
        {
            get => base.Sprite;
            set => base.Sprite = value;
        }

        /// <summary>
        /// Create a new image element.
        /// </summary>
        /// <param name="parent">The parent of this element.</param>
        public ImageElement(Element parent) : base(parent, null)
        {
        }
    }
}
