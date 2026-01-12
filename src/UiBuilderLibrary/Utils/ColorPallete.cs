using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A color pallete.
    /// </summary>
    public static class ColorPallete
    {
        public static Color RustRed => new Color(0.667f, 0.278f, 0.208f);
        public static Color RustGreen => new Color(0.365f, 0.447f, 0.220f);

        public static Color TextRegular => new Color(0.914f, 0.875f, 0.843f);
        public static Color TextMuted => new Color(0.369f, 0.353f, 0.329f);

        public static Color Border => new Color(0f, 0f, 0f);

        public static Color ItemLevel1 => new Color(0.251f, 0.251f, 0.239f);

        public static Color BackgroundLevel1 => new Color(0.149f, 0.145f, 0.129f);
        public static Color BackgroundLevel2 => new Color(0.086f, 0.086f, 0.082f);
        public static Color BackgroundLevel3 => new Color(0.067f, 0.067f, 0.067f);

        public static Color OverlayDarkenLevel1 => new Color(0f, 0f, 0f, 0.5f);
        public static Color OverlayDarkenLevel2 => new Color(0f, 0f, 0f, 0.65f);
        public static Color OverlayDarkenLevel3 => new Color(0f, 0f, 0f, 0.8f);

        public static Color Red => new Color(0.804f, 0.251f, 0.165f);
        public static Color OnRed => new Color(1f, 1f, 1f);
    }
}
