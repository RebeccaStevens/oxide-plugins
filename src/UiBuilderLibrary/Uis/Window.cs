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
        public readonly record struct WindowComponents(
            /// <summary>
            /// The main panel of the window.
            /// </summary>
            ///
            PanelElement MainPanel,
            /// <summary>
            /// The title bar of the window.
            /// </summary>
            PanelElement TitleBar,

            /// <summary>
            /// The label displayed in the title bar.
            /// </summary>
            LabelElement WindowLabel,

            /// <summary>
            /// The divider between the title bar and the main panel.
            /// </summary>
            PanelElement Divider,

            /// <summary>
            /// The close button of the window.
            /// </summary>
            ButtonElement CloseButton,

            /// <summary>
            /// The icon displayed on the close button.
            /// </summary>
            ImageElement CloseButtonIcon
        );

        /// <summary>
        /// The components that make up a window
        /// </summary>
        protected readonly WindowComponents Components;

        /// <summary>
        /// Create a new UI window on the "Overlay" layer.
        /// </summary>
        /// <param name="initializer">Defines what makes up the window.</param>
        public Window(Action<WindowComponents> initializer) : this("Overlay", initializer)
        {
        }

        /// <summary>
        /// Create a new UI window.
        /// </summary>
        /// <param name="layer">The layer to put this UI on.</param>
        /// <param name="initializer">Defines what makes up the window.</param>
        public Window(string layer, Action<WindowComponents> initializer) : base(layer)
        {
            var main = new PanelElement(Root) { Name = "main" };
            var titleBar = new PanelElement(Root) { Name = "titleBar", Weight = -1 };

            Root.Name = "window";
            Root.Margin.SetSize(Size.Pixels(24));
            Root.Border.SetSize(Size.Pixels(2));
            Root.Layout = new FlexLayout(Root)
            {
                Direction = FlexLayout.FlexDirection.Vertical,
                AlignItems = FlexLayout.ItemAlignment.Stretch,
                JustifyContent = FlexLayout.JustifyAlignment.Start,
            };

            titleBar.Height = Size.Pixels(30);
            titleBar.BgColor = ColorPallete.OverlayDarkenLevel1;
            titleBar.Layout = new FlexLayout(titleBar)
            {
                Direction = FlexLayout.FlexDirection.Horizontal,
                AlignItems = FlexLayout.ItemAlignment.Stretch,
                JustifyContent = FlexLayout.JustifyAlignment.Start,
            };

            var windowLabel = new LabelElement(titleBar)
            {
                Name = "window-title-bar-label",
                Text = "Window Title",
                Font = Font.Bold,
                FontSize = Size.Pixels(20),
                TextColor = ColorPallete.TextRegular,
                TextAlignment = TextAnchor.MiddleLeft,
                Margin =
                {
                    Top = Size.Pixels(0),
                    Right = Size.Pixels(8),
                    Bottom = Size.Pixels(0),
                    Left = Size.Pixels(8)
                }
            };

            var closeButton = new ButtonElement(titleBar)
            {
                Name = "window-close-button",
                BgColor = ColorPallete.Red,
                Width = Size.Pixels(64),
                Weight = 100,
                Action = UiAction.CloseUi(this),
            };
            closeButton.Layout = new FlexLayout(closeButton)
            {
                Direction = FlexLayout.FlexDirection.Horizontal,
                AlignItems = FlexLayout.ItemAlignment.Center,
                JustifyContent = FlexLayout.JustifyAlignment.Center,
            };

            var closeButtonIcon = new ImageElement(closeButton)
            {
                Name = "window-close-button-icon",
                Color = ColorPallete.OnRed,
                Sprite = "assets/icons/close.png",
                Width = Size.Pixels(14),
                Height = Size.Pixels(14),
            };

            var divider = new PanelElement(Root)
            {
                Name = "window-divider",
                Weight = -0.5,
                BgColor = ColorPallete.Border,
                Height = Size.Pixels(1),
            };

            Components = new WindowComponents(
                MainPanel: main,
                TitleBar: titleBar,
                WindowLabel: windowLabel,
                Divider: divider,
                CloseButton: closeButton,
                CloseButtonIcon: closeButtonIcon
            );
            initializer(Components);
        }
    }
}
