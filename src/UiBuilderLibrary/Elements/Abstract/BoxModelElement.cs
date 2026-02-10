using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// An element that has a margin, border, and padding.
    /// </summary>
    public abstract class BoxModelElement : Element
    {
        /// <summary>
        /// The padding applied to this element.
        /// </summary>
        public readonly DirectionalSizeValues Padding;

        /// <summary>
        /// The stored outline element component.
        /// </summary>
        private OutlineElementComponent? outline;

        /// <summary>
        /// The border applied to this element.
        /// </summary>
        public OutlineElementComponent Border =>
            outline ??= new OutlineElementComponent { Color = Theme.Colors.Border };

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
        protected BoxModelElement(Element parent) : this(parent, null)
        {
        }

        /// <summary>
        /// Create a new top level panel element.
        /// </summary>
        protected BoxModelElement(string layer) : this(null, layer)
        {
        }

        /// <summary>
        /// Create a new panel element.
        /// </summary>
        protected BoxModelElement(Element? parent, string? layer) : base(parent, layer)
        {
            Padding = new DirectionalSizeValues("Padding", this, Size.Zero);
        }

        /// <summary>
        /// Does this element have a border?
        /// </summary>
        public bool HasBorder() => outline?.HasSize() ?? false;

        /// <inheritdoc/>
        protected override ElementState InitialState(BasePlayer player) => new BoxModelElementState(this, player);
    }
}
