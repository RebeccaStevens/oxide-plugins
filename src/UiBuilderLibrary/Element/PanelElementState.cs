using System;
using System.Collections.Generic;
using System.Linq;
using Oxide.Game.Rust.Cui;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <inheritdoc cref="ElementState"/>
    public class PanelElementState : ElementState
    {
        /// <inheritdoc cref="ElementState.Element"/>
        public new readonly PanelElement Element;

        protected readonly HashSet<string> cuiElementNames = new();

        protected Bounds borderPositionTop;
        protected Bounds borderPositionRight;
        protected Bounds borderPositionBottom;
        protected Bounds borderPositionLeft;

        /// <inheritdoc cref="ElementState"/>
        public PanelElementState(PanelElement element, BasePlayer player) : base(element, player)
        {
            Element = element;

            borderPositionTop = new Bounds();
            borderPositionRight = new Bounds();
            borderPositionBottom = new Bounds();
            borderPositionLeft = new Bounds();
        }

        internal override string GetCuiRootName()
        {
            Debug.Assert(!string.IsNullOrEmpty(Element.Name));
            Panic.If(string.IsNullOrEmpty(Id));
            return $"{Element.Name}-root-{Id}";
        }

        protected string GetCuiBorderName(Direction side)
        {
            Debug.Assert(!string.IsNullOrEmpty(Element.Name));
            Panic.If(string.IsNullOrEmpty(Id));
            return $"{Element.Name}-border-{side.ToString().ToLower()}-{Id}";
        }

        internal override string GetCuiContentName()
        {
            Debug.Assert(!string.IsNullOrEmpty(Element.Name));
            Panic.If(string.IsNullOrEmpty(Id));
            return $"{Element.Name}-content-{Id}";
        }

        protected Bounds GetBorderPosition(Direction direction) => direction switch
        {
            Direction.Top => borderPositionTop,
            Direction.Right => borderPositionRight,
            Direction.Bottom => borderPositionBottom,
            Direction.Left => borderPositionLeft,
            _ => throw new ArgumentOutOfRangeException(nameof(direction)),
        };

        protected SafeCuiElement CreateCuiBorderElementComponent(SafeCuiElement rootCui, Bounds position,
            Element.ElementBorder border, Direction direction)
        {
            var borderPosition = GetBorderPosition(direction);
            borderPosition
                .Maximize()
                .Add(Reverse(direction), new Bounds.Value(1d, 0d) - position.GetValue(direction));

            var name = GetCuiBorderName(direction);

            SafeCuiElement cui = new(name, rootCui.Name);
            cui.SetPosition(borderPosition);
            cui.AddComponent(new CuiImageComponent() { Color = ColorToCuiColor(border.Color) });
            return cui;
        }

        /// <inheritdoc/>
        internal override string[] GetCuiElementNames()
        {
            return cuiElementNames.ToArray();
        }

        /// <inheritdoc/>
        internal override ElementCuiElements CreateCuiElements()
        {
            var components = new ElementCuiElements
            {
                Root = new SafeCuiElement(GetCuiRootName(), GetParentCuiContentName())
            };

            var marginInnerPosition = Element.Margin.GetInnerBounds(this);
            var borderInnerPosition = Element.Border.Size.GetInnerBounds(this);
            var paddingInnerPosition = Element.Padding.GetInnerBounds(this);

            components.Root.SetPosition(Bounds + marginInnerPosition);
            components.Root.AddComponent(new CuiImageComponent() { Color = ColorToCuiColor(Element.BgColor) });

            components.Others.Add(
                CreateCuiBorderElementComponent(components.Root, borderInnerPosition, Element.Border, Direction.Top));
            components.Others.Add(
                CreateCuiBorderElementComponent(components.Root, borderInnerPosition, Element.Border, Direction.Right));
            components.Others.Add(
                CreateCuiBorderElementComponent(components.Root, borderInnerPosition, Element.Border, Direction.Bottom));
            components.Others.Add(
                CreateCuiBorderElementComponent(components.Root, borderInnerPosition, Element.Border, Direction.Left));

            components.Content = new(GetCuiContentName(), components.Root.Name);
            components.Content.SetPosition(borderInnerPosition + paddingInnerPosition);

            foreach (var cuiElement in components.GetAll())
            {
                Debug.Assert(!string.IsNullOrEmpty(cuiElement.Name));
                cuiElementNames.Add(cuiElement.Name);
            }

            return components;
        }
    }
}