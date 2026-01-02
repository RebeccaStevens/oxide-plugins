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
                new PanelElement(Root) { Name = "titleBar", Weight = -1 }
            );

            Root.Name = "window";
            Root.Margin.SetSize(Size.Pixels(32));
            Root.Border.SetSize(Size.Pixels(4));
            Root.Layout = new FlexLayout(Root)
            {
                Direction = FlexLayout.FlexDirection.Vertical,
                AlignItems = FlexLayout.ItemAlignment.Stretch,
                JustifyContent = FlexLayout.JustifyAlignment.Start,
            };

            TitleBar.Height = Size.Pixels(32);
            TitleBar.BgColor = new Color(0f, 0f, 0f, 0.8f);
            TitleBar.Padding.SetSize(Size.Pixels(1), Size.Pixels(1), Size.Zero, Size.Zero); // Workaround for non pixel-perfect client.
            TitleBar.Layout = new FlexLayout(TitleBar)
            {
                Direction = FlexLayout.FlexDirection.Horizontal,
                AlignItems = FlexLayout.ItemAlignment.Stretch,
                JustifyContent = FlexLayout.JustifyAlignment.End,
            };

            // ReSharper disable once UnusedVariable
            var divider = new PanelElement(Root)
            {
                Name = "divider",
                Weight = -0.5,
                BgColor = Color.black,
                Height = Size.Pixels(1),
            };

            // TODO: Make this a button.
            // ReSharper disable once UnusedVariable
            var closeButton = new PanelElement(TitleBar)
            {
                Name = "closeButton",
                BgColor = Color.red,
                Width = Size.Pixels(72),
            };

            initializer(Data);
        }
    }
}
