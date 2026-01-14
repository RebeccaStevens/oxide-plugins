using Oxide.Game.Rust.Cui;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    public partial class LabelElement
    {
        /// <inheritdoc/>
        public class LabelElementState : BoxModelElementStateState
        {
            /// <inheritdoc cref="BoxModelElementStateState.Element"/>
            public new LabelElement Element => (LabelElement)base.Element;

            /// <inheritdoc cref="LabelElementState(LabelElement, BasePlayer)"/>
            public LabelElementState(LabelElement element, BasePlayer player) : base(element, player)
            {
            }

            /// <summary>
            /// Compute the font size to use for this element.
            /// </summary>
            protected int ComputeFontSize()
            {
                var sizeValue = Element.FontSizeContext.GetBoundsValue(this, CuiBounds.Height);
                Debug.Assert(sizeValue.Absolute > 0);
                Debug.Assert(sizeValue.Relative == 0);
                return (int)sizeValue.Absolute;
            }

            /// <inheritdoc/>
            protected override void AddCuiComponents(ElementCuiElements cuiElements)
            {
                Debug.AssertNotNull(cuiElements.Root);

                cuiElements.Root.AddComponent(new CuiTextComponent
                {
                    Text = Element.Text,
                    Color = ColorToCuiColor(Element.TextColor),
                    Font = GetFontFile(Element.Font),
                    FontSize = ComputeFontSize(),
                    Align = Element.TextAlignment,
                    VerticalOverflow = Element.VerticalOverflow,
                });

                if (Element.HasBorder())
                    cuiElements.Root.AddComponents(Element.Border.ApplyState(this));
            }
        }
    }
}
