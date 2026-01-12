using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// Convert a Color to a CUI color string.
    /// </summary>
    internal static string ColorToCuiColor(Color color)
    {
        return $"{color.r} {color.g} {color.b} {color.a}";
    }

    /// <summary>
    /// Multiply the color channels of the given color by the given amount.
    /// </summary>
    internal static Color ColorMultiply(Color color, float amount)
    {
        return new Color(color.r * amount, color.g * amount, color.b * amount, color.a);
    }
}
