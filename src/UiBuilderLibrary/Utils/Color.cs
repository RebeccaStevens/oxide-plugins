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
}