using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// The base class for all panel elements.
    /// </summary>
    public abstract class PanelElementBase : Element
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
        /// The color of the element.
        /// </summary>
        protected internal Color Color { get; protected set; }

        /// <summary>
        /// The material to apply to the element.
        /// </summary>
        protected internal string? Material { get; protected set; }

        /// <summary>
        /// The sprite to apply to the element.
        /// </summary>
        protected internal string? Sprite { get; protected set; }

        /// <summary>
        /// Create a new panel element.
        /// </summary>
        /// <param name="parent">The parent of this element.</param>
        protected PanelElementBase(Element parent) : this(parent, null)
        {
        }

        /// <summary>
        /// Create a new top level panel element.
        /// </summary>
        protected PanelElementBase(string layer) : this(null, layer)
        {
        }

        /// <summary>
        /// Create a new panel element.
        /// </summary>
        protected PanelElementBase(Element? parent, string? layer) : base(parent, layer)
        {
            Padding = new DirectionalSizeValues("Padding", this, Size.Zero);
        }

        /// <summary>
        /// Does this element have a border?
        /// </summary>
        public bool HasBorder() => outline?.HasSize() ?? false;

        /// <inheritdoc/>
        protected override ElementState InitialState(BasePlayer player) => new PanelElementState(this, player);
    }
}
