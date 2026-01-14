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

            components.MainPanel.Margin.SetSize(Size.Pixels(8));
            components.MainPanel.Layout = new FlexLayout()
            {
                Direction = FlexLayout.FlexDirection.Vertical,
                AlignItems = FlexLayout.ItemAlignment.Center,
                JustifyContent = FlexLayout.JustifyAlignment.Start,
                Gap = Size.Pixels(8),
            };

            var temp3 = new ButtonElement(components.MainPanel)
            {
                Name = "temp3",
                Label =
                {
                    Text = "Click Me",
                },
                Icon =
                {
                    Sprite = "assets/icons/close.png",
                },
            };
        });

        var player = BasePlayer.activePlayerList.ElementAt(0);
        if (player != null)
            MainUi.Open(player);

        // var gapFactor = 0.005;
        // var borderFactor = 0.002;
        // var titleBarHeight = 0.03;

        // MainUi = new Ui("Overlay", (mainPanelBorder) =>
        // {
        //     // Using the player's screen aspect ratio, calculate the x and y gap values.
        //     var screenAspectRatio = Ui.GetScreenAspectRatio(mainPanelBorder.Player);
        //     var screenGapX = gapFactor;
        //     var screenGapY = gapFactor * screenAspectRatio;
        //     var screenBorderX = borderFactor;
        //     var screenBorderY = borderFactor * screenAspectRatio;

        //     // Display
        //     mainPanelBorder.CursorEnabled = true;
        //     mainPanelBorder.Image.Color = "0 0 0 0.9";

        //     // Bounds.
        //     mainPanelBorder.Bounds.MinX = 0.1;
        //     mainPanelBorder.Bounds.MaxX = 1 - mainPanelBorder.Bounds.MinX;
        //     mainPanelBorder.Bounds.MinY = mainPanelBorder.Bounds.MinX * screenAspectRatio;
        //     mainPanelBorder.Bounds.MaxY = 1 - mainPanelBorder.Bounds.MinY;

        //     // Border size
        //     var mainBorderX = screenBorderX / mainPanelBorder.Bounds.GetWidth();
        //     var mainBorderY = screenBorderY / mainPanelBorder.Bounds.GetHeight();

        //     // Child gap values.
        //     var mainBorderGapX = screenGapX / mainPanelBorder.Bounds.GetWidth();
        //     var mainBorderGapY = screenGapY / mainPanelBorder.Bounds.GetHeight();

        //     var mainBorderTitleBarHeight = titleBarHeight / mainPanelBorder.Bounds.GetHeight();

        //     mainPanelBorder.AddPanel((mainTitleBar) =>
        //     {
        //         // Display
        //         mainTitleBar.Image.Color = "1 1 1 1";

        //         // Bounds.
        //         mainTitleBar.Bounds.MinX = mainBorderX;
        //         mainTitleBar.Bounds.MaxX = 1 - mainBorderX;
        //         mainTitleBar.Bounds.MinY = 1 - mainBorderY - mainBorderTitleBarHeight;
        //         mainTitleBar.Bounds.MaxY = 1 - mainBorderY;

        //         // Child gap values.
        //         var subPanelGapX = mainBorderGapX / mainTitleBar.Bounds.GetWidth();
        //         var subPanelGapY = mainBorderGapY / mainTitleBar.Bounds.GetHeight();

        //         mainTitleBar.AddButton((closeButton) =>
        //         {
        //             // Display
        //             closeButton.Button.Color = "1 0 0 1";

        //             closeButton.Text.Color = "1 1 1 1";
        //             closeButton.Text.Text = "Close";
        //             closeButton.Text.Align = UnityEngine.TextAnchor.MiddleCenter;
        //             closeButton.Text.FontSize = (int)Math.Ceiling(10 / (titleBarHeight / mainBorderTitleBarHeight));

        //             // Bounds.
        //             closeButton.Bounds.MinX = 0.925;
        //             closeButton.Bounds.MaxX = 1;
        //             closeButton.Bounds.MinY = 0;
        //             closeButton.Bounds.MaxY = 1;

        //             return false;
        //         });

        //         return false;
        //     });

        //     mainPanelBorder.AddPanel((mainPanel) =>
        //     {
        //         // Display
        //         mainPanel.Image.Color = "0 1 1 1";

        //         // Bounds.
        //         mainPanel.Bounds.MinX = mainBorderX;
        //         mainPanel.Bounds.MaxX = 1 - mainBorderX;
        //         mainPanel.Bounds.MinY = mainBorderY;
        //         mainPanel.Bounds.MaxY = 1 - mainBorderY - mainBorderTitleBarHeight - mainBorderY;

        //         // Child gap values.
        //         var subPanelGapX = mainBorderGapX / mainPanel.Bounds.GetWidth();
        //         var subPanelGapY = mainBorderGapY / mainPanel.Bounds.GetHeight();

        //         return false;
        //     });

        //     return false;
        // });
    }

    #endregion
}
