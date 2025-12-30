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
            internal override SafeCuiElement[] CreateCuiElements()
            {
                var elements = base.CreateCuiElements();
                var rootCui = elements.First();
                Debug.AssertNotNull(rootCui);

                rootCui.AddComponent(new CuiNeedsCursorComponent());
                rootCui.AddComponent(new CuiNeedsKeyboardComponent());
                return elements;
            }
        }
    }
}