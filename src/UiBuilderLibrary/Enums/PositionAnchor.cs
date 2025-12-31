using System;
using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// Where to anchor an element.
    /// </summary>
    public enum PositionAnchor
    {
        /// <summary>
        /// The top left corner.
        /// </summary>
        UpperLeft = TextAnchor.UpperLeft,

        /// <summary>
        /// The center of the top side.
        /// </summary>
        UpperCenter = TextAnchor.UpperCenter,

        /// <summary>
        /// The top right corner.
        /// </summary>
        UpperRight = TextAnchor.UpperRight,

        /// <summary>
        /// The middle of the left side.
        /// </summary>
        MiddleLeft = TextAnchor.MiddleLeft,

        /// <summary>
        /// The center in both the horizontal and vertical directions.
        /// </summary>
        MiddleCenter = TextAnchor.MiddleCenter,

        /// <summary>
        /// The middle of the right side.
        /// </summary>
        MiddleRight = TextAnchor.MiddleRight,

        /// <summary>
        /// The bottom left corner.
        /// </summary>
        LowerLeft = TextAnchor.LowerLeft,

        /// <summary>
        /// The center of the bottom side.
        /// </summary>
        LowerCenter = TextAnchor.LowerCenter,

        /// <summary>
        /// The bottom right corner.
        /// </summary>
        LowerRight = TextAnchor.LowerRight
    }

    /// <summary>
    /// Convert a PositionAnchor to relative container position factors.
    /// </summary>
    public static (double FromTop, double FromRight, double FromBottom, double FromLeft) PositionAnchorToEdgeFactors(
        PositionAnchor anchor)
    {
        return anchor switch
        {
            PositionAnchor.UpperLeft => (0.0, 1.0, 1.0, 0.0),
            PositionAnchor.UpperCenter => (0, 0.5, 1.0, 0.5),
            PositionAnchor.UpperRight => (0.0, 0.0, 1.0, 1.0),
            PositionAnchor.MiddleLeft => (0.5, 1.0, 0.5, 0.0),
            PositionAnchor.MiddleCenter => (0.5, 0.5, 0.5, 0.5),
            PositionAnchor.MiddleRight => (0.5, 0.0, 0.5, 1.0),
            PositionAnchor.LowerLeft => (1.0, 1.0, 0.0, 0.0),
            PositionAnchor.LowerCenter => (1.0, 0.5, 0.0, 0.5),
            PositionAnchor.LowerRight => (1.0, 0.0, 0.0, 1.0),
            _ => throw new ArgumentOutOfRangeException(nameof(anchor)),
        };
    }
}