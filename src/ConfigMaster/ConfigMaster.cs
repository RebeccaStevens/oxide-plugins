using System.Linq;
using UnityEngine;
using static Oxide.Plugins.UiBuilderLibrary;

namespace Oxide.Plugins;

[Info("Config Master", "BlueBeka", "0.0.1")]
[Description("Provides a UI for configuring plugins in the Config Master series.")]
public class ConfigMaster : RustPlugin
{
    private UiBuilderLibrary.Ui? MainUi;

    #region Commands

    #endregion

    #region Hooks

    private void Init()
    {
        cmd.AddChatCommand("o", this, (player, command, args) =>
        {
            Debug.AssertNotNull(MainUi);
            MainUi.Open(player);
        });
        cmd.AddChatCommand("close", this, (player, command, args) =>
        {
            Debug.AssertNotNull(MainUi);
            MainUi.Close(player);
        });
    }

    private void Unload()
    {
        Debug.AssertNotNull(MainUi);
        MainUi.CloseForAll();
    }

    private void Loaded()
    {
        MainUi = new Window(components =>
        {
            components.WindowLabel.Text = "Config Master";

            var tabs = new TabsElement(components.MainPanel)
            {
                Layout = new FlexLayout()
                {
                    // Direction = FlexLayout.FlexDirection.Vertical,
                    Direction = FlexLayout.FlexDirection.Horizontal,
                    AlignItems = FlexLayout.ItemAlignment.Stretch,
                    JustifyContent = FlexLayout.JustifyAlignment.Start,
                },
            };

            tabs.AddTab(tab =>
            {
                tab.Button.Padding.SetSize(Size.Pixels(0));
                tab.Button.ContentRotation = 90;
                tab.Button.Label.Text = "Tab 1";
                tab.Button.Label.Width = Size.ContainerPercentage(0.5);
                tab.Button.Label.Height = Size.ContainerPercentage(0.5);
                tab.View.BgColor = Color.red;
                tab.View.Padding.SetSize(tab.View.Theme.Spacing.Medium);
                tab.View.Layout = new FlexLayout()
                {
                    Direction = FlexLayout.FlexDirection.Vertical,
                    AlignItems = FlexLayout.ItemAlignment.Center,
                    JustifyContent = FlexLayout.JustifyAlignment.Start,
                    Gap = Size.Pixels(8),
                };
            });

            tabs.AddTab(tab =>
            {
                tab.Button.Padding.SetSize(Size.Pixels(0));
                tab.Button.ContentRotation = 90;
                tab.Button.Label.Text = "Tab 2";
                tab.Button.Label.Width = Size.ContainerPercentage(0.5);
                tab.Button.Label.Height = Size.ContainerPercentage(0.5);
                tab.Button.Label.TextOutline.SetSize(Size.Pixels(2));
                tab.View.BgColor = Color.green;
                tab.View.Padding.SetSize(tab.View.Theme.Spacing.Medium);
            });

            tabs.AddTab(tab =>
            {
                tab.Button.Padding.SetSize(Size.Pixels(0));
                tab.Button.ContentRotation = 90;
                tab.Button.Label.Text = "Tab 3";
                tab.Button.Label.Width = Size.ContainerPercentage(0.5);
                tab.Button.Label.Height = Size.ContainerPercentage(0.5);
                tab.View.BgColor = Color.blue;
                tab.View.Padding.SetSize(tab.View.Theme.Spacing.Medium);
            });
        });

        var player = BasePlayer.activePlayerList.ElementAt(0);
        if (player != null)
            MainUi.Open(player);
    }

    #endregion
}
