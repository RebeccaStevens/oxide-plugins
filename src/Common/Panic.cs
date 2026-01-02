using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Oxide.Plugins;

/// <summary>
/// Test that nothing is fatally wrong.<br />
/// <br />
/// Use <see cref="Debug"/> instead if any of the following are true:
/// <ul>
/// <li>An exception would immediately be thrown after a check fails regardless.</li>
/// <li>The logic immediately before the check ensures that the check will never fail.</li>
/// </ul>
/// </summary>
public sealed class Panic
{
    /// <summary>
    /// Panic if the given condition is true.
    /// </summary>
    /// <param name="condition">When to panic.</param>
    /// <param name="message">Default's to the expression passed as the condition.</param>
    public static void If([DoesNotReturnIf(true)] bool condition,
        [CallerArgumentExpression(nameof(condition))]
        string? message = null)
    {
        if (condition)
            Now(message);
    }

    /// <summary>
    /// Panic if the given value is null.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="message">Default's to the expression passed as the condition.</param>
    public static T IfNull<T>([NotNull] T? value, [CallerArgumentExpression(nameof(value))] string? message = null)
    {
        If(value == null, $"{message} is null.");
        return value;
    }

    /// <summary>
    /// Panic if the given value is not null.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="message">Default's to the expression passed as the condition.</param>
    public static void IfNotNull(object? value, [CallerArgumentExpression(nameof(value))] string? message = null)
    {
        If(value != null, $"{message} is not null.");
    }

    /// <summary>
    /// Panic now.
    /// Somthing is wrong and the plugin should not continue.
    /// </summary>
    [DoesNotReturn]
    public static void Now(string? message = null) => throw new PanicException(message);

    private sealed class PanicException : Exception
    {
        internal PanicException(string? message) : base(message)
        {
        }
    }
}
