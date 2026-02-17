using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    public partial record Theme
    {
        /// <summary>
        /// A dark theme.
        /// </summary>
        public static Theme CreateDarkTheme() => new Theme
        {
            Colors = new ThemeColors
            {
                Branding = new ThemeColors.BrandingColors
                {
                    Primary = new Color(0.365f, 0.447f, 0.220f),
                    Secondary = Color.cyan, // TODO: use a nice color.
                },
                Item = new ThemeColors.ColorLevels
                {
                    Level1 = new Color(0.251f, 0.251f, 0.239f),
                    Level2 = new Color(0.201f, 0.201f, 0.191f),
                    Level3 = new Color(0.188f, 0.188f, 0.181f)
                },
                Background = new ThemeColors.ColorLevels
                {
                    Level1 = new Color(0.149f, 0.145f, 0.129f),
                    Level2 = new Color(0.086f, 0.086f, 0.082f),
                    Level3 = new Color(0.067f, 0.067f, 0.067f)
                },
                Text = new ThemeColors.TextColors
                {
                    Regular = new Color(0.914f, 0.875f, 0.843f),
                    Highlighted = Color.white,
                    Muted = new Color(0.369f, 0.353f, 0.329f)
                },
            }
        };
    }
}
