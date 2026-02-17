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
            ButtonElement CloseButton
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
            Root.Margin.SetSize(Root.Theme.Spacing.ExtraLarge);
            Root.Border.SetSize(Root.Theme.LineThickness.Medium);
            Root.Layout = new FlexLayout()
            {
                Direction = FlexLayout.FlexDirection.Vertical,
                AlignItems = FlexLayout.ItemAlignment.Stretch,
                JustifyContent = FlexLayout.JustifyAlignment.Start,
            };

            titleBar.Height = Root.Theme.Spacing.Huge - Root.Theme.LineThickness.Thin;
            titleBar.BgColor = Root.Theme.Colors.OverlayDarken.Level1;
            titleBar.Layout = new FlexLayout()
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
                FontSize = FontSizeForHeight(titleBar.Height),
                TextColor = Root.Theme.Colors.Text.Regular,
                TextAlignment = TextAnchor.MiddleLeft,
                Margin =
                {
                    Top = Size.Zero,
                    Right = titleBar.Theme.Spacing.Medium,
                    Bottom = Size.Zero,
                    Left = titleBar.Theme.Spacing.Medium
                }
            };

            var closeButton = new ButtonElement(titleBar)
            {
                Name = "window-close-button",
                BgColor = titleBar.Theme.Colors.Solid.Danger,
                BgColorSelected = titleBar.Theme.Colors.Solid.Danger,
                Width = titleBar.Theme.ItemSizing.Medium,
                Weight = 100,
                OnClick = UiAction.CloseUi(this),
                Icon =
                {
                    Sprite = Theme.Icons.Close,
                    Size = titleBar.Theme.FontSize.Large,
                }
            };

            var divider = new PanelElement(Root)
            {
                Name = "window-divider",
                Weight = -0.5,
                BgColor = Root.Theme.Colors.Border,
                Height = Root.Theme.LineThickness.Thin,
            };

            Components = new WindowComponents(
                MainPanel: main,
                TitleBar: titleBar,
                WindowLabel: windowLabel,
                Divider: divider,
                CloseButton: closeButton
            );
            initializer(Components);
        }
    }
}
