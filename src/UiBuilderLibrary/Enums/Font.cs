using System;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A font that can be using to display text.
    /// </summary>
    public enum Font
    {
        /// <summary>
        /// The regular font.
        /// </summary>
        Regular,

        /// <summary>
        /// The bold font.
        /// </summary>
        Bold,

        /// <summary>
        /// The monospace font.
        /// </summary>
        Mono,

        /// <summary>
        /// The font that looks like a a permanent marker.
        /// </summary>
        Marker
    }

    /// <summary>
    /// Get the file name of the given font.
    /// </summary>
    public static string GetFontFile(Font font)
    {
        return font switch
        {
            Font.Regular => "robotocondensed-regular.ttf",
            Font.Bold => "robotocondensed-bold.ttf",
            Font.Mono => "droidsansmono.ttf",
            Font.Marker => "permanentmarker.ttf",
            _ => throw new ArgumentOutOfRangeException(nameof(font)),
        };
    }
}
