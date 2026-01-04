using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A simple panel element.
    /// </summary>
    public class PanelElement : Element
    {
        /// <summary>
        /// The padding applied to this element.
        /// </summary>
        public readonly DirectionalSizeValues Padding;

        /// <summary>
        /// The actual outline element component.
        /// </summary>
        private OutlineElementComponent? outline;

        /// <summary>
        /// The border applied to this element.
        /// </summary>
        public OutlineElementComponent Border => outline ??= new OutlineElementComponent();

        /// <summary>
        /// The background color of the element.
        /// </summary>
        public Color BgColor = Color.gray;

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
            Padding = new DirectionalSizeValues("Padding", this, Size.Zero);
        }

        /// <summary>
        /// Does this element have a border?
        /// </summary>
        public bool HasBorder() => outline != null && outline.HasSize();

        /// <inheritdoc/>
        protected override ElementState InitialState(BasePlayer player)
        {
            return new PanelElementState(this, player);
        }
    }
}
