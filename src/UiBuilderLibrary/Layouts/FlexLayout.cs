using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Oxide.Core;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// Use Flexbox to position child elements.
    /// </summary>
    public class FlexLayout : DynamicElementLayout
    {
        /// <summary>
        /// The element this layout is for.
        /// </summary>
        protected readonly Element Element;

        /// <summary>
        /// The direction of the flex layout.
        /// </summary>
        public FlexDirection Direction = FlexDirection.Horizontal;

        /// <summary>
        /// The alignment of the items in the major axis in the flex layout.
        /// </summary>
        public JustifyAlignment JustifyContent = JustifyAlignment.Center;

        /// <summary>
        /// The alignment of the items in the minor axis in the flex layout.
        /// </summary>
        public ItemAlignment AlignItems = ItemAlignment.Stretch;

        /// <summary>
        /// The context for the spacing between elements.
        /// </summary>
        internal readonly SizeContext GapContext;

        /// <summary>
        /// The spacing between elements.
        /// </summary>
        public Size Gap
        {
            get => GapContext.Get();
            set => GapContext.Set(value);
        }

        /// <summary>
        /// The results of the computed layout.
        /// </summary>
        private readonly ConditionalWeakTable<ElementState, Bounds> layoutData;

        /// <summary>
        /// Create a new FlexLayout for the given element.
        /// </summary>
        /// <param name="element">The element this layout is for - if that element doesn't already have a layout, this layout will be assigned to it.</param>
        public FlexLayout(Element element) : base(element)
        {
            Element = element;
            GapContext = new SizeContext("Gap", element, Size.Zero);
            layoutData = new ConditionalWeakTable<ElementState, Bounds>();
        }

        /// <inheritdoc/>
        protected override void ComputeLayout(ElementState state)
        {
            var childrenStates = state.GetChildren().ToArray();
            if (childrenStates.Length == 0)
                return;

            if (Direction is FlexDirection.HorizontalReversed or FlexDirection.VerticalReversed)
                Array.Reverse(childrenStates);

            var majorAxis = ToAxis(Direction);
            var majorAxisChildrenExplicitSize = new Bounds.Value();
            var numberOfImplicitlySizedChildren = 0;

            IEnumerable<(ElementState state, (SizeContext Major, SizeContext Minor) SizeContext)> childrenData =
                childrenStates.Select(childState => (childState, GetSizeContext(childState))).ToArray();

            // Get sizing information for all children in regards of the major axis.
            foreach (var child in childrenData)
            {
                if (child.SizeContext.Major.IsImplisit())
                    numberOfImplicitlySizedChildren += 1;
                else
                    majorAxisChildrenExplicitSize += child.SizeContext.Major.GetBoundsValue(child.state);
            }

            var gapCount = childrenStates.Length - 1;
            var gapValue = JustifyContent switch
            {
                JustifyAlignment.Start or
                    JustifyAlignment.Center or
                    JustifyAlignment.End => GapContext.GetBoundsValue(state),
                JustifyAlignment.SpaceBetween =>
                    (Bounds.Value.Full - majorAxisChildrenExplicitSize) / gapCount,
                JustifyAlignment.SpaceAround =>
                    (Bounds.Value.Full - majorAxisChildrenExplicitSize) / (gapCount + 1),
                JustifyAlignment.SpaceEvenly =>
                    (Bounds.Value.Full - majorAxisChildrenExplicitSize) / (gapCount + 2),
                _ => throw new ArgumentOutOfRangeException($"Invalid justification: {JustifyContent}"),
            };
            var majorContentSize = majorAxisChildrenExplicitSize + gapCount * gapValue;

            Bounds.Value majorChildImplicitSize;
            if (numberOfImplicitlySizedChildren == 0 || JustifyContent is JustifyAlignment.SpaceAround or
                    JustifyAlignment.SpaceEvenly or
                    JustifyAlignment.SpaceBetween)
            {
                if (numberOfImplicitlySizedChildren > 0)
                    Interface.Oxide.LogDebug(
                        $"A child of the Element ({state.Element.Name}) has no explicit size in the major axis ({majorAxis}) and thus it will have no size.\n" +
                        "Either give it a size or set JustifyContent to Start, Center, or End."
                    );
                majorChildImplicitSize = default;
            }
            else
            {
                var implicitChildrenSize = Bounds.Value.Full - majorContentSize;
                majorContentSize += implicitChildrenSize;
                majorChildImplicitSize = implicitChildrenSize / numberOfImplicitlySizedChildren;
            }

            var childMajorOffset = JustifyContent switch
            {
                JustifyAlignment.Start => new Bounds.Value(),
                JustifyAlignment.Center => (Bounds.Value.Full - majorContentSize) / 2,
                JustifyAlignment.End => Bounds.Value.Full - majorContentSize,
                JustifyAlignment.SpaceBetween => new Bounds.Value(),
                JustifyAlignment.SpaceAround => gapValue / 2,
                JustifyAlignment.SpaceEvenly => gapValue,
                _ => throw new ArgumentOutOfRangeException($"Invalid justification: {JustifyContent}"),
            };

            foreach (var child in childrenData)
            {
                var childMajorSize = child.SizeContext.Major.GetBoundsValue(child.state, majorChildImplicitSize);
                var childMinorSize = AlignItems switch
                {
                    ItemAlignment.Start or
                        ItemAlignment.Center or
                        ItemAlignment.End => child.SizeContext.Minor.GetBoundsValue(child.state, Bounds.Value.Full),
                    ItemAlignment.Stretch => Bounds.Value.Full,
                    _ => throw new ArgumentOutOfRangeException($"Invalid alignment: {AlignItems}"),
                };
                var childMinorOffset = AlignItems switch
                {
                    ItemAlignment.Start => new Bounds.Value(),
                    ItemAlignment.Center => (Bounds.Value.Full - childMinorSize) / 2,
                    ItemAlignment.End => Bounds.Value.Full - childMinorSize,
                    ItemAlignment.Stretch => new Bounds.Value(),
                    _ => throw new ArgumentOutOfRangeException($"Invalid alignment: {AlignItems}"),
                };

                if (!layoutData.TryGetValue(child.state, out var bounds))
                    bounds = new Bounds();

                bounds.FromTop = childMinorOffset;
                bounds.FromRight = (childMajorOffset + childMajorSize).TakeComplement();
                bounds.FromBottom = (childMinorOffset + childMinorSize).TakeComplement();
                bounds.FromLeft = childMajorOffset;
                if (majorAxis == Axis.Y)
                {
                    // Swap the axis.
                    (bounds.FromTop, bounds.FromLeft) = (bounds.FromLeft, bounds.FromTop);
                    (bounds.FromBottom, bounds.FromRight) = (bounds.FromRight, bounds.FromBottom);
                }

                childMajorOffset += childMajorSize + gapValue;
                layoutData.Add(child.state, bounds);
            }
        }

        /// <summary>
        /// Get the major and minor axis bounds' values for the given element state.
        /// </summary>
        private (SizeContext Major, SizeContext Minor) GetSizeContext(ElementState state)
        {
            var (majorAxis, minorAxis) = ToAxes(Direction);
            return (state.Element.GetSizeContext(majorAxis),
                state.Element.GetSizeContext(minorAxis));
        }

        /// <inheritdoc/>
        protected override void PositionChild(ElementState childState)
        {
            Element.AssertIsChild(childState.Element);
            layoutData.TryGetValue(childState, out var bounds);
            Panic.IfNull(bounds, "No bounds have been computed for this element.");
            childState.Bounds.SetTo(bounds);
        }

        /// <summary>
        /// Convert a flex direction to an axis.
        /// </summary>
        public static Axis ToAxis(FlexDirection direction)
        {
            return direction switch
            {
                FlexDirection.Horizontal or FlexDirection.HorizontalReversed => Axis.X,
                FlexDirection.Vertical or FlexDirection.VerticalReversed => Axis.Y,
                _ => throw new ArgumentOutOfRangeException(nameof(direction)),
            };
        }

        /// <summary>
        /// Convert a flex direction to the major and minor axes.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static (Axis Major, Axis Minor) ToAxes(FlexDirection direction)
        {
            var majorAxis = ToAxis(direction);
            var minorAxis = majorAxis switch
            {
                Axis.X => Axis.Y,
                Axis.Y => Axis.X,
                _ => throw new ArgumentOutOfRangeException(nameof(majorAxis)),
            };
            return (majorAxis, minorAxis);
        }

        /// <summary>
        /// The direction of the flex layout.
        /// </summary>
        public enum FlexDirection
        {
            /// <summary>
            /// The flex items are laid out horizontally.
            /// </summary>
            Horizontal,

            /// <summary>
            /// The flex items are laid out vertically.
            /// </summary>
            Vertical,

            /// <summary>
            /// The flex items are laid out horizontally, but in reverse order.
            /// </summary>
            HorizontalReversed,

            /// <summary>
            /// The flex items are laid out vertically, but in reverse order.
            /// </summary>
            VerticalReversed,
        }

        /// <summary>
        /// The alignment of the items in the major axis in the flex layout.
        /// </summary>
        public enum JustifyAlignment
        {
            /// <summary>
            /// Items are packed toward the start of the flex-direction.
            /// </summary>
            Start,

            /// <summary>
            /// Items are centered in the container.
            /// </summary>
            Center,

            /// <summary>
            /// Items are packed toward the end of the flex-direction.
            /// </summary>
            End,

            /// <summary>
            /// Items are evenly distributed in the line; first item is on the start line, last item on the end line.
            /// </summary>
            SpaceBetween,

            /// <summary>
            /// Items are evenly distributed in the line with equal space around them.
            ///
            /// Note that visually the spaces aren’t equal, since all the items have equal space on both sides.
            /// The first item will have one unit of space against the container edge,
            /// but two units of space between the next item because that next item has its own spacing that applies.
            /// </summary>
            SpaceAround,

            /// <summary>
            /// Items are distributed so that the spacing between any two items (and the space to the edges) is equal.
            /// </summary>
            SpaceEvenly,
        }

        /// <summary>
        /// The alignment of the items in the minor axis in the flex layout.
        /// </summary>
        public enum ItemAlignment
        {
            /// <summary>
            /// Items packed to the start of the container.
            /// </summary>
            Start,

            /// <summary>
            /// Items centered in the container
            /// </summary>
            Center,

            /// <summary>
            /// Items packed to the end of the container.
            /// </summary>
            End,

            /// <summary>
            /// Items are stretched to fill the available space.
            /// </summary>
            Stretch,
        }
    }
}
