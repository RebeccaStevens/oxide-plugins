using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A simple panel element.
    /// </summary>
    public class PanelElement : BoxModelElement
    {
        /// <summary>
        /// The background color of the element.
        /// </summary>
        public Color BgColor { get; set; }

        /// <summary>
        /// The material to apply to the element.
        /// </summary>
        public string? Material { get; set; }

        /// <summary>
        /// Create a new panel element.
        /// </summary>
        /// <param name="parent">The parent of this element.</param>
        public PanelElement(Element parent) : this(parent, null)
        {
        }

        /// <summary>
        /// Create a new top level panel element.
        /// </summary>
        protected PanelElement(string layer) : this(null, layer)
        {
        }

        /// <summary>
        /// Create a new panel element.
        /// </summary>
        protected PanelElement(Element? parent, string? layer) : base(parent, layer)
        {
            BgColor = Theme.Colors.Background.Level1;
            Material = "assets/content/ui/ui.background.tiletex.psd";
        }

        /// <inheritdoc/>
        protected override ElementState InitialState(BasePlayer player) => new PanelElementState(this, player);
    }
}
