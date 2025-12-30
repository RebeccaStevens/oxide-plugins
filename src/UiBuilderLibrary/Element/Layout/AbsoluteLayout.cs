namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// Use the x, y, width and height of the child element to position them.<br />
    /// The child elements are positioned relative to their respective anchor.<br />
    /// <br />
    /// Note: The x value has no effect when the anchor is centered, and
    ///       the y value has no effect when the anchor is middled.
    /// </summary>
    public class AbsoluteLayout : StaticElementLayout
    {
        /// <inheritdoc cref="AbsoluteLayout"/>
        public AbsoluteLayout()
        {
        }

        /// <inheritdoc/>
        protected override void PositionChild(ElementState childState)
        {
            var x = childState.Element.XContext.GetBoundsValue(childState);
            var y = childState.Element.YContext.GetBoundsValue(childState);
            var complementWidth = childState.Element.WidthContext.GetBoundsValue(childState, () =>
                childState.Element.Anchor switch
                {
                    PositionAnchor.UpperCenter or PositionAnchor.MiddleCenter or PositionAnchor.LowerCenter =>
                        new Bounds.Value(1, 0),
                    _ => new Bounds.Value(1, 0) - x
                }).TakeComplement();
            var complementHeight = childState.Element.HeightContext.GetBoundsValue(childState, () =>
                childState.Element.Anchor switch
                {
                    PositionAnchor.MiddleLeft or PositionAnchor.MiddleCenter or PositionAnchor.MiddleRight =>
                        new Bounds.Value(1, 0),
                    _ => new Bounds.Value(1, 0) - y
                }).TakeComplement();
            var (topFactor, rightFactor, bottomFactor, leftFactor) =
                PositionAnchorToEdgeFactors(childState.Element.Anchor);

            childState.Bounds.FromTop = (bottomFactor - topFactor) * y - topFactor * complementHeight;
            childState.Bounds.FromRight = (leftFactor - rightFactor) * x - rightFactor * complementWidth;
            childState.Bounds.FromBottom = (topFactor - bottomFactor) * y - bottomFactor * complementHeight;
            childState.Bounds.FromLeft = (rightFactor - leftFactor) * x - leftFactor * complementWidth;
        }
    }
}