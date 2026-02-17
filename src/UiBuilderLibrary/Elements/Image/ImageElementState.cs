using Oxide.Game.Rust.Cui;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <inheritdoc cref="BoxModelElementState"/>
    public class ImageElementState : BoxModelElementState
    {
        /// <inheritdoc cref="BoxModelElementState.Element"/>
        public new ImageElement Element => (ImageElement)base.Element;

        /// <inheritdoc cref="BoxModelElementState"/>
        public ImageElementState(ImageElement element, BasePlayer player) : base(element, player)
        {
        }

        internal ImageElementState(ImageElement element, BasePlayer player, string id) : base(element,
            player, id)
        {
        }

        /// <inheritdoc/>
        protected override void AddCuiComponents(ElementCuiElements cuiElements)
        {
            Debug.AssertNotNull(cuiElements.Root);

            cuiElements.Root.AddComponent(new CuiImageComponent
            {
                Color = ColorToCuiColor(Element.Color),
                Sprite = string.IsNullOrEmpty(Element.Sprite) ? null : Element.Sprite,
            });

            if (Element.HasBorder())
                cuiElements.Root.AddComponents(Element.Border.ApplyState(this));
        }
    }
}
