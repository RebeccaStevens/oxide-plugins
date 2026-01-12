using System;
using System.Collections.Generic;
using Oxide.Game.Rust.Cui;
using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A component that outlines an element.
    /// </summary>
    public class OutlineElementComponent : IElementComponent
    {
        // TODO: Support other sizes - will require converting over sizes to pixels.

        private Size.PixelSize? xSize;
        private Size.PixelSize? ySize;

        /// <summary>
        /// The size of the outline on the left and right edges.
        /// </summary>
        public Size X
        {
            get => xSize ??= (Size.PixelSize)Size.Zero;
            set {
                if (value is not Size.PixelSize size)
                    throw new InvalidOperationException("Only pixel sizes are currently supported for outline offsets.");
                xSize = size;
            }
        }

        /// <summary>
        /// The size of the outline on the top and bottom edges.
        /// </summary>
        public Size Y
        {
            get => ySize ??= (Size.PixelSize)Size.Zero;
            set
            {
                if (value is not Size.PixelSize size)
                    throw new InvalidOperationException(
                        "Only pixel sizes are currently supported for outline offsets.");
                ySize = size;
            }
        }

        /// <summary>
        /// The color of the outline.
        /// </summary>
        public Color Color = ColorPallete.Border;

        /// <summary>
        /// Set the size of the outline.
        /// </summary>
        /// <param name="size"></param>
        public void SetSize(Size size)
        {
            X = size;
            Y = size;
        }

        /// <summary>
        /// Set the size of the outline on the x and y axes.
        /// </summary>
        public void SetSize(Size x, Size y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Does this outline have a size greater than zero in either direction?
        /// </summary>
        public bool HasSize() => ((Size.PixelSize)X).Value > 0 || ((Size.PixelSize)Y).Value > 0;

        /// <inheritdoc cref="IElementComponent.ApplyState"/>
        public IEnumerable<ICuiComponent> ApplyState(ElementState state)
        {
            return new ICuiComponent[]
            {
                new CuiOutlineComponent
                {
                    Distance = $"{((Size.PixelSize)X).Value} {((Size.PixelSize)Y).Value}",
                    Color = ColorToCuiColor(Color),
                    UseGraphicAlpha = true,
                },
            };
        }

        /// <summary>
        /// Get the bounds of a rectangle that is inset by this.
        /// </summary>
        public Bounds GetInnerBounds(ElementState state)
        {
            var x = new Bounds.Value(0, ((Size.PixelSize)X).Value);
            var y = new Bounds.Value(0, ((Size.PixelSize)Y).Value);
            return new Bounds()
            {
                FromTop = y,
                FromRight = x,
                FromBottom = y,
                FromLeft = x,
            };
        }
    }
}
