using System;
using System.Diagnostics.CodeAnalysis;

namespace Oxide.Plugins;

/// <summary>
/// A reference to an object of type `T` that can be toggled between being either a strong or a weak reference.
/// 
/// A weak reference can be garbage collected and thus may become unavaliable.
/// </summary>
public class WeakableReference<T> where T : class
{
    private T? strong;
    private readonly WeakReference<T> weak;

    /// <summary>
    /// Create a new WeakableReference
    /// </summary>
    /// <param name="target">The target to keep a reference to</param>
    /// <param name="initiallyStrong">whether the reference should start out as a strong reference</param>
    public WeakableReference(T target, bool initiallyStrong = true)
    {
        if (initiallyStrong)
            strong = target;
        weak = new WeakReference<T>(target);
    }

    /// <summary>
    /// Is the reference currently a strong reference.
    /// </summary>
    public bool IsStrong => strong != null;

    /// <summary>
    /// Set the target to a new value.
    /// </summary>
    /// <param name="target">the new target</param>
    /// <param name="isStrong">whether the new reference should be a strong reference</param>
    public void SetTarget(T target, bool isStrong = true)
    {
        weak.SetTarget(target);
        strong = isStrong ? target : null;
    }

    /// <summary>
    /// Try and access the target.
    /// </summary>
    /// <param name="target">the target</param>
    /// <returns>true if the target could be accessed, otherwise false</returns>
    public bool TryGetTarget([MaybeNullWhen(false)] out T target)
    {
        if (strong == null)
            return weak.TryGetTarget(out target);
        target = strong;
        return true;

    }

    /// <summary>
    /// Try and access the target and mark it as a strong reference.
    /// </summary>
    /// <param name="target">the target</param>
    /// <returns>true if the target still exists an could be made strong, otherwise false implying that the target has already been garbage collected.</returns>
    public bool TryGetTargetAndMarkStrong([MaybeNullWhen(false)] out T target)
    {
        var result = TryGetTarget(out target);
        if (result)
            strong = target;
        return result;
    }

    /// <summary>
    /// Mark the target as a weak reference.
    /// </summary>
    public void MarkWeak()
    {
        strong = null;
    }
}