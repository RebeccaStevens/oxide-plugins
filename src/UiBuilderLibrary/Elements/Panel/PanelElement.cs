using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A simple panel element.
    /// </summary>
    public class PanelElement : PanelElementBase
    {
        /// <summary>
        /// The background color of the element.
        /// </summary>
        public Color BgColor {
            get => Color;
            set => Color = value;
        }

        /// <inheritdoc cref="PanelElementBase.Material"/>
        public new string? Material
        {
            get => base.Material;
            set => base.Material = value;
        }

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
            BgColor = ColorPallete.BackgroundLevel1;
            Material = "assets/content/ui/ui.background.tiletex.psd";
        }
    }
}
