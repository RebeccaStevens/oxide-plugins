using System;
using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A UI window that includes a close button that can be displayed to users.
    /// </summary>
    public class Window : Ui
    {
        /// <summary>
        /// The top level elements of a window.
        /// </summary>
        public readonly record struct WindowData(PanelElement Main, PanelElement TitleBar);

        /// <summary>
        /// The data for this window that will be stared to initializers.
        /// </summary>
        protected readonly WindowData Data;

        /// <summary>
        /// The main panel of this window.
        /// </summary>
        protected PanelElement Main => Data.Main;

        /// <summary>
        /// The title bar of this window.
        /// </summary>
        protected PanelElement TitleBar => Data.TitleBar;

        /// <summary>
        /// Create a new UI window on the "Overlay" layer.
        /// </summary>
        /// <param name="initializer">Defines what makes up the window.</param>
        public Window(Action<WindowData> initializer) : this("Overlay", initializer)
        {
        }

        /// <summary>
        /// Create a new UI window.
        /// </summary>
        /// <param name="layer">The layer to put this UI on.</param>
        /// <param name="initializer">Defines what makes up the window.</param>
        public Window(string layer, Action<WindowData> initializer) : base(layer)
        {
            Data = new WindowData(
                new PanelElement(Root) { Name = "main" },
                new PanelElement(Root) { Name = "titleBar" }
            );

            Root.Name = "window";
            Root.BgColor = new Color(1f, 1f, 1f, 0.25f);
            Root.Margin.SetAll(Size.Pixels(32));
            Root.Border.Size.SetAll(Size.Pixels(1));

            // var rootLayout = new FlexLayout(Root)
            // {
            //     Direction = FlexLayout.FlexDirection.Vertical,
            //     // AlignItems = FlexLayout.ItemAlignment.Stretch,
            //     // JustifyContent = FlexLayout.JustifyAlignment.Start,
            // };
            // rootLayout.Gap.SetAll(Size.Pixels(4));
            // Root.Layout = rootLayout;

            // TitleBar.BgColor = new Color(.5f, 1f, 1f, 0.25f);
            TitleBar.BgColor = Color.red;
            TitleBar.Height = Size.Pixels(32);
            TitleBar.Border.Size.SetAll(Size.Zero, Size.Zero, Size.Pixels(1), Size.Zero);

            // var titleBarLayout = new FlexLayout(titleBar)
            // {
            //     Direction = FlexLayout.FlexDirection.Horizontal,
            //     // AlignItems = FlexLayout.ItemAlignment.Stretch,
            //     // JustifyContent = FlexLayout.JustifyAlignment.End,
            // };
            // TitleBar.Layout = titleBarLayout;

            // var closeButton = new PanelElement(TitleBar)
            // {
            //     Name = "closeButton",
            //     BgColor = Color.red,
            // };
            // // closeButton.Margin.SetAll(Size.Pixels(8));
            // closeButton.Width.SetAll(Size.Pixels(72));

            // Main.BgColor = Color.blue;
            // Main.Border.Size.SetAll(Size.Pixels(1));
            // Main.Padding.SetAll(Size.Pixels(8));
            // Main.X.SetAll(Size.Pixels(200));
            // Main.Y.SetAll(Size.Pixels(50));
            // Main.Width.SetAll(Size.Pixels(100));
            // Main.Height.SetAll(Size.Pixels(100));

            initializer(Data);
        }
    }
}