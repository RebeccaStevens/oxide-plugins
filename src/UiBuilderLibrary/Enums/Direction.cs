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

    /// <summary>
    /// Rotate the given direction clockwise.
    /// </summary>
    public static Direction RotateClockwise(Direction direction) => direction switch
    {
        Direction.Top => Direction.Right,
        Direction.Right => Direction.Bottom,
        Direction.Bottom => Direction.Left,
        Direction.Left => Direction.Top,
        _ => throw new ArgumentOutOfRangeException(nameof(direction)),
    };

    /// <summary>
    /// Rotate the given direction counter-clockwise.
    /// </summary>
    public static Direction RotateCounterClockwise(Direction direction) => direction switch
    {
        Direction.Top => Direction.Left,
        Direction.Right => Direction.Top,
        Direction.Bottom => Direction.Right,
        Direction.Left => Direction.Bottom,
        _ => throw new ArgumentOutOfRangeException(nameof(direction)),
    };

    /// <summary>
    /// Rotate the given direction by the given rotation.<br />
    /// If the rotation is not a multiple of 90 degrees, null will be returned.
    /// </summary>
    public static Direction? Rotate(Direction from, double rotation)
    {
        var normalizedRotation = RotationContext.Normalize(rotation);
        var quarterTurns =
            (RotationContext.RoughlyEqual(normalizedRotation, 0)) ? 0 :
            (RotationContext.RoughlyEqual(normalizedRotation, 90)) ? 1 :
            (RotationContext.RoughlyEqual(normalizedRotation, 180)) ? 2 :
            (RotationContext.RoughlyEqual(normalizedRotation, 270)) ? 3 :
            -1;

        return quarterTurns switch
        {
            -1 => null,
            0 => from,
            1 => RotateClockwise(from),
            2 => Reverse(from),
            3 => RotateCounterClockwise(from),
            _ => throw new Panic.Exception(),
        };
    }
}
