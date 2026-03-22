using System;
using System.Collections.Generic;
using System.Linq;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A container element that displays its children in tabs, with one child
    /// per tab. Only one child is visible at a time, and the user can switch
    /// between them by clicking on the corresponding tab.
    /// </summary>
    public partial class TabsElement : BoxModelElement
    {
        /// <summary>
        /// The backing field for the <see cref="ButtonsPanel"/> property.
        /// </summary>
        protected PanelElement? ButtonsPanelBacking;

        /// <summary>>
        /// The backing field for the <see cref="View"/> property.
        /// </summary>
        protected PanelElement? ViewBacking;

        /// <summary>
        /// The backing field for the <see cref="DefaultTab"/> property.
        /// </summary>
        protected Tab? DefaultTabBacking;

        /// <summary>
        /// The tabs that are part of this element.
        /// </summary>
        protected List<Tab> Tabs { get; } = new();

        /// <inheritdoc cref="TabsElement"/>
        public TabsElement(Element parent) : base(parent)
        {
            Name = "tabs";
        }

        /// <summary>
        /// The panel element that contains the buttons for the tabs.
        /// </summary>
        public PanelElement ButtonsPanel
        {
            get => ButtonsPanelBacking ??= new PanelElement(this)
            {
                Name = $"{Name}-buttons",
                Weight = -1,
                Size = Theme.ItemSizing.ExtraSmall,
                BgColor = Theme.Colors.Item.Level1,
                Layout = new FlexLayout()
                {
                    Direction = FlexLayout.OppositeDirection((Layout as FlexLayout)?.Direction ??
                                                             FlexLayout.FlexDirection.Vertical),
                    JustifyContent = FlexLayout.JustifyAlignment.Start,
                }
            };
            init => ButtonsPanelBacking = value;
        }

        /// <summary>
        /// The panel element that acts as the parent of the active tab's view.
        /// </summary>
        public PanelElement View
        {
            get => ViewBacking ??= new PanelElement(this)
            {
                Name = $"{Name}-view",
                Weight = 0,
                BgColor = Theme.Colors.Background.Level2,
            };
            set => ViewBacking = value;
        }

        /// <inheritdoc/>
        public override ElementLayout Layout
        {
            get => LayoutBacking ??= new FlexLayout()
            {
                Direction = FlexLayout.FlexDirection.Vertical,
                AlignItems = FlexLayout.ItemAlignment.Stretch,
                JustifyContent = FlexLayout.JustifyAlignment.Start,
            };
            set => LayoutBacking = value;
        }

        /// <summary>
        /// The tab that is active by default. If not set, the first tab will be active by default.
        /// </summary>
        public Tab? DefaultTab
        {
            get => DefaultTabBacking ??= Tabs.FirstOrDefault();
            set => DefaultTabBacking = value;
        }

        /// <summary>
        /// Add a tab to this element.
        /// </summary>
        /// <param name="initializeTab">A function to initialize the tab.</param>
        public void AddTab(Action<Tab> initializeTab)
        {
            var id = Tabs.Count;
            var newTab = new Tab(this, id)
            {
                // ReSharper disable once UseObjectOrCollectionInitializer
                Button = new ToggleButtonElement(ButtonsPanel)
                {
                    Name = $"{Name}-buttons[{id}]",
                    Size = Theme.ItemSizing.ExtraLarge,
                },
            };
            newTab.Button.IsActive = (buttonState, _) => GetState(buttonState.Player).ActiveTab == newTab;
            newTab.Button.OnClick = new UiAction.RunActionAction((player) => newTab.SetActive(player));
            Tabs.Add(newTab);
            initializeTab(newTab);

            if (newTab.Button.LabelActiveBacking != null)
            {
                newTab.Button.LabelActiveBacking.IsEnabled = (labelState, _) =>
                    GetState(labelState.Player).ActiveTab == newTab;

                if (newTab.Button.LabelBacking != null)
                    newTab.Button.LabelBacking.IsEnabled = (labelState, _) =>
                        GetState(labelState.Player).ActiveTab != newTab;
            }

            if (newTab.Button.IconActiveBacking != null)
            {
                newTab.Button.IconActiveBacking.IsEnabled = (iconState, _) =>
                    GetState(iconState.Player).ActiveTab == newTab;

                if (newTab.Button.IconBacking != null)
                    newTab.Button.IconBacking.IsEnabled = (iconState, _) =>
                        GetState(iconState.Player).ActiveTab != newTab;
            }
        }

        /// <inheritdoc cref="Element.GetState"/>
        public new TabsElementState GetState(BasePlayer player, bool markStrong = false) =>
            (TabsElementState)base.GetState(player, markStrong);

        /// <inheritdoc/>
        protected override ElementState InitialState(BasePlayer player) => new TabsElementState(this, player);
    }
}
