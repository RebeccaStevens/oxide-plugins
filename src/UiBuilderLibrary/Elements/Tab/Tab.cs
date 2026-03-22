namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A tab in a <see cref="TabsElement"/>.
    /// </summary>
    public class Tab
    {
        /// <summary>
        /// The <see cref="TabsElement"/> that this tab belongs to.
        /// </summary>
        public TabsElement TabsElement { get; }

        /// <summary>
        /// The panel element that is displayed when this tab is active.
        /// </summary>
        public PanelElement View { get; }

        /// <summary>
        /// Create a new tab.
        /// </summary>
        /// <param name="parent">The tabs element that this tab belongs to.</param>
        /// <param name="index">The index of this tab in the tabs element.</param>
        internal Tab(TabsElement parent, int index)
        {
            TabsElement = parent;
            View = new TabViewElement(this, TabsElement.View)
            {
                Name = $"{TabsElement.Name}-view[{index}]",
                BgColor = TabsElement.Theme.Colors.Background.Level2,
            };
        }

        /// <summary>
        /// The button element that represents this tab. Clicking on it will make this tab active.
        /// </summary>
        public required ToggleButtonElement Button { get; init; }

        /// <summary>
        /// Whether this tab is the default tab for the <see cref="TabsElement"/>.
        /// </summary>
        public bool IsDefaultTab
        {
            get => TabsElement.DefaultTab == this;
            set
            {
                if (value)
                    TabsElement.DefaultTab = this;
                else if (TabsElement.DefaultTab == this)
                    TabsElement.DefaultTab = null;
                else
                    Debug.Fail("Unnecessarily setting of IsDefaultTab to false when it is already false.");
            }
        }

        /// <summary>
        /// Whether this tab is active for the given player.
        /// </summary>
        public bool IsActive(BasePlayer player)
        {
            return TabsElement.GetState(player).ActiveTab == this;
        }

        /// <summary>
        /// Set this tab as the active tab for the given player.
        /// </summary>
        /// <returns>True if the tab was not already active, false otherwise.</returns>
        public bool SetActive(BasePlayer player, bool sync = true)
        {
            var state = TabsElement.GetState(player);
            if (state.ActiveTab == this)
                return false;
            state.ActiveTab = this;
            if (sync)
                state.Sync();
            return true;
        }
    }
}
