namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// An element that has a margin, border, and padding.
    /// </summary>
    public abstract class BoxModelElement : Element
    {
        /// <summary>
        /// The stored outline element component.
        /// </summary>
        private OutlineElementComponent? outline;

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
        /// The padding applied to this element.
        /// </summary>
        public DirectionalSizeValues Padding { get; }

        /// <summary>
        /// The border applied to this element.
        /// </summary>
        public OutlineElementComponent Border =>
            outline ??= new OutlineElementComponent { Color = Theme.Colors.Border };

        /// <summary>
        /// Does this element have a border?
        /// </summary>
        public virtual bool HasBorder() => outline?.HasSize() ?? false;
    }
}
