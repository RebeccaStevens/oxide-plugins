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
            Root.Margin.SetAll(Size.Pixels(32));
            Root.Border.Size.SetAll(Size.Pixels(1));
            Root.Layout = new FlexLayout(Root)
            {
                Direction = FlexLayout.FlexDirection.Vertical,
                AlignItems = FlexLayout.ItemAlignment.Stretch,
                JustifyContent = FlexLayout.JustifyAlignment.Start,
            };

            TitleBar.Height = Size.Pixels(32);
            TitleBar.Border.Size.SetAll(Size.Zero, Size.Zero, Size.Pixels(1), Size.Zero);
            TitleBar.Layout = new FlexLayout(TitleBar)
            {
                Direction = FlexLayout.FlexDirection.Horizontal,
                AlignItems = FlexLayout.ItemAlignment.Stretch,
                JustifyContent = FlexLayout.JustifyAlignment.End,
            };

            // TODO: Make this a button.
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
