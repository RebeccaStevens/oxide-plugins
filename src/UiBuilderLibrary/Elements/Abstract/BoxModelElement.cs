namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// An element that has a margin, border, and padding.
    /// </summary>
    public abstract class BoxModelElement : Element
    {
        /// <summary>
        /// The backing field for the <see cref="Border"/> property.
        /// </summary>
        protected OutlineElementComponent? BorderBacking;

        /// <summary>
        /// The context for the rotation of this element.
        /// </summary>
        internal readonly RotationContext ContentRotationContext;

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
            ContentRotationContext = new RotationContext(RotationContext);
            Padding = new DirectionalSizeValues("Padding", this, Size.Zero);
        }

        /// <summary>
        /// The rotation (in degrees) of this element's conent.
        /// </summary>
        public double ContentRotation
        {
            get => ContentRotationContext.Degrees;
            set => ContentRotationContext.Degrees = value;
        }

        /// <summary>
        /// The pivot of this element's content.<br />
        /// See <see cref="Element.Pivot"/> for more information.
        /// </summary>
        public PositionAnchor ContentPivot
        {
            get => ContentRotationContext.Pivot;
            set => ContentRotationContext.Pivot = value;
        }

        /// <summary>
        /// The padding applied to this element.
        /// </summary>
        public DirectionalSizeValues Padding { get; }

        /// <summary>
        /// The border applied to this element.
        /// </summary>
        public OutlineElementComponent Border =>
            BorderBacking ??= new OutlineElementComponent { Color = Theme.Colors.Border };

        /// <inheritdoc/>
        public override RotationContext GetChildRotationContext() => ContentRotationContext;

        /// <summary>
        /// Does this element have a border?
        /// </summary>
        public virtual bool HasBorder() => BorderBacking?.HasSize() ?? false;
    }
}
