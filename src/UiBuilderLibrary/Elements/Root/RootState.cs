using Oxide.Game.Rust.Cui;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// The root element of a UI.
    /// </summary>
    public partial class RootElement
    {
        /// <inheritdoc/>
        public class RootState : PanelElementState
        {
            /// <inheritdoc cref="PanelElementState(PanelElementBase, BasePlayer)"/>
            public RootState(RootElement element, BasePlayer player) : base(element, player)
            {
            }

            /// <inheritdoc/>
            protected override void AddCuiComponents(ElementCuiElements cuiElements)
            {
                Debug.AssertNotNull(cuiElements.Root);
                base.AddCuiComponents(cuiElements);
                cuiElements.Root.AddComponent(new CuiNeedsCursorComponent());
                cuiElements.Root.AddComponent(new CuiNeedsKeyboardComponent());
            }
        }
    }
}
