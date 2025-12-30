using System;

// ReSharper disable UnusedMember.Global

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// Convert a pixel value to a percentage of the screen size.
    /// </summary>
    internal static double PixelsToScreenPercentage(int value, BasePlayer player, Axis axis)
    {
        Debug.AssertNotNull(builder);
        var size = Bounds.GetScreenSize(axis).AsAbsolute;
        return axis switch
        {
            Axis.X => value / size,
            Axis.Y => value / size * builder.GetScreenAspectRatio(player),
            _ => throw new ArgumentOutOfRangeException(nameof(axis)),
        };
    }

    /// <summary>
    /// Convert a percentage of the screen size to a pixel value.
    /// </summary>
    internal static int ScreenPercentageToPixels(double value, BasePlayer player, Axis axis)
    {
        Debug.AssertNotNull(builder);
        var size = Bounds.GetScreenSize(axis).AsAbsolute;
        return axis switch
        {
            Axis.X => (int)(value * size),
            Axis.Y => (int)(value * size / builder.GetScreenAspectRatio(player)),
            _ => throw new ArgumentOutOfRangeException(nameof(axis)),
        };
    }
}