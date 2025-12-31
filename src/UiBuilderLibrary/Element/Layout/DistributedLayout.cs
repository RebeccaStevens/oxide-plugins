using System.Collections.Generic;
using Oxide.Game.Rust.Cui;
using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A 1D layout where the children are evenly distributed along the given axis.<br/>
    /// Children are evenly distributed along that axis.<br/>
    /// Their sizes on that axis are all equal and they fill the available space in the other axis.<br/>
    /// <br/>
    /// For more complex 1D layouts, use <see cref="FlexLayout"/>.
    /// </summary>
    public class DistributedLayout : StaticElementLayout
    {
        /// <summary>
        /// The direction of the flex layout.
        /// </summary>
        public Axis Axis;

        /// <summary>
        /// The spacing between elements.
        /// </summary>
        public Size.PixelSize Gap;

        /// <inheritdoc cref="DistributedLayout"/>
        /// <param name="axis">The axis to layout the children of this element.</param>
        public DistributedLayout(Axis axis)
        {
            Axis = axis;
            Gap = Size.Zero;
        }

        /// <inheritdoc/>
        public override List<ICuiComponent> Prepare(ElementState state)
        {
            var components = base.Prepare(state) ?? new List<ICuiComponent>();

            CuiLayoutGroupComponent layoutComponent = Axis == Axis.X
                ? new CuiHorizontalLayoutGroupComponent()
                : new CuiVerticalLayoutGroupComponent();

            layoutComponent.ChildAlignment = (TextAnchor)state.Element.Anchor;
            layoutComponent.Spacing = Gap.Value;
            layoutComponent.ChildControlWidth = true;
            layoutComponent.ChildControlHeight = true;

            components.Add(layoutComponent);

            return components;
        }

        /// <inheritdoc/>
        protected override void PositionChild(ElementState childState)
        {
            // Nothing to do - The children will be positioned by the client.
        }
    }
}