namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A panel element that is used as the content of a <see cref="Tab"/>.
    /// </summary>
    internal class TabViewElement : PanelElement
    {
        /// <summary>
        /// The tab that this view belongs to.
        /// </summary>
        public Tab Tab { get; }

        internal TabViewElement(Tab tab, Element parent) : base(parent)
        {
            Tab = tab;
            IsEnabled = (elementState, _) => Tab.IsActive(elementState.Player);
        }

        /// <inheritdoc/>
        public override ElementState Open(BasePlayer player, bool force = false)
        {
            Tab.Button.GetState(player).IsActive = true;
            return base.Open(player, force);
        }

        /// <inheritdoc/>
        public override ElementState? Close(BasePlayer player, bool force = false)
        {
            Tab.Button.GetState(player).IsActive = false;
            return base.Close(player, force);
        }
    }
}
