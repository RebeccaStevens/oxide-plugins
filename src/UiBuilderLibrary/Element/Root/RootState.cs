using System.Linq;
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
            /// <inheritdoc cref="PanelElementState(PanelElement, BasePlayer)"/>
            public RootState(RootElement element, BasePlayer player) : base(element, player)
            {
            }

            /// <inheritdoc/>
            internal override ElementCuiElements CreateCuiElements()
            {
                var cuiElements = base.CreateCuiElements();
                Debug.AssertNotNull(cuiElements.Root);

                cuiElements.Root.AddComponent(new CuiNeedsCursorComponent());
                cuiElements.Root.AddComponent(new CuiNeedsKeyboardComponent());
                return cuiElements;
            }
        }
    }
}
