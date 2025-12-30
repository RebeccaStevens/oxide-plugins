using System;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// The direction of a directional unit.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// At the top.
        /// </summary>
        Top,

        /// <summary>
        /// On the right.
        /// </summary>
        Right,

        /// <summary>
        /// At the bottom.
        /// </summary>
        Bottom,

        /// <summary>
        /// On the left.
        /// </summary>
        Left,
    }

    /// <summary>
    /// Get the axis of the given direction.
    /// </summary>
    public static Axis GetAxis(Direction direction)
    {
        return direction switch
        {
            Direction.Top or Direction.Bottom => Axis.Y,
            Direction.Left or Direction.Right => Axis.X,
            _ => throw new ArgumentOutOfRangeException(nameof(direction)),
        };
    }

    /// <summary>
    /// Reverse the given direction.
    /// </summary>
    public static Direction Reverse(Direction direction) => direction switch
    {
        Direction.Top => Direction.Bottom,
        Direction.Right => Direction.Left,
        Direction.Bottom => Direction.Top,
        Direction.Left => Direction.Right,
        _ => throw new ArgumentOutOfRangeException(nameof(direction)),
    };
}