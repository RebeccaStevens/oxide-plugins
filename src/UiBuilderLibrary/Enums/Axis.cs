using System;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// An axis.
    /// </summary>
    public enum Axis
    {
        /// <summary>
        /// The horizontal axis.
        /// </summary>
        X,

        /// <summary>
        /// The vertical axis.
        /// </summary>
        Y,
    }

    /// <summary>
    /// Rotate the given axis.
    /// </summary>
    public static Axis Rotate(Axis axis) => axis switch
    {
        Axis.X => Axis.Y,
        Axis.Y => Axis.X,
        _ => throw new ArgumentOutOfRangeException(nameof(axis)),
    };

    /// <summary>
    /// Rotate the given axis by the given rotation.<br />
    /// If the rotation is not a multiple of 90 degrees, null will be returned.
    /// </summary>
    public static Axis? Rotate(Axis axis, double rotation)
    {
        var normalizedRotation = RotationContext.Normalize(rotation);

        if (RotationContext.RoughlyEqual(normalizedRotation, 0) ||
            RotationContext.RoughlyEqual(normalizedRotation, 180))
            return axis;

        if (RotationContext.RoughlyEqual(normalizedRotation, 90) ||
            RotationContext.RoughlyEqual(normalizedRotation, 270))
            return Rotate(axis);

        return null;
    }

    /// <summary>
    /// Rotate the given axes.
    /// </summary>
    public static (Axis, Axis) Rotate((Axis, Axis) axes) => (Rotate(axes.Item1), Rotate(axes.Item2));

    /// <summary>
    /// Rotate the given axes.
    /// </summary>
    public static (Axis, Axis)? Rotate((Axis, Axis) axes, double rotation)
    {
        var a1 = Rotate(axes.Item1, rotation);
        var a2 = Rotate(axes.Item2, rotation);
        return a1 != null && a2 != null ? ((Axis, Axis)?)(a1, a2) : null;
    }
}
