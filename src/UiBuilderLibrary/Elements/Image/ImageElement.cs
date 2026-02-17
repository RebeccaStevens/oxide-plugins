using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// An image element.
    /// </summary>
    public partial class ImageElement : BoxModelElement
    {
        /// <summary>
        /// The background color of the image.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// The sprite of the image.
        /// </summary>
        public string? Sprite { get; set; }

        /// <summary>
        /// Create a new image element.
        /// </summary>
        /// <param name="parent">The parent of this element.</param>
        public ImageElement(Element parent) : base(parent, null)
        {
        }

        /// <inheritdoc/>
        protected override ElementState InitialState(BasePlayer player) => new ImageElementState(this, player);
    }
}
