using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Oxide.Plugins;

/// <summary>
/// Test that things are as expected.
///
/// These functions are only enabled in debug builds.
/// </summary>
public sealed class Debug
{
    /// <summary>
    /// Assert that the given condition must be true.
    /// </summary>
    /// <param name="condition">What must be true.</param>
    /// <param name="message">Default's to the expression passed as the condition.</param>
    [Conditional("DEBUG")]
    public static void Assert([DoesNotReturnIf(false)] bool condition,
        [CallerArgumentExpression(nameof(condition))]
        string? message = null)
    {
        if (!condition)
            Fail(message);
    }

    /// <summary>
    /// Assert that the given value is not null.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="message">Default's to the expression passed as the condition.</param>
    [Conditional("DEBUG")]
    public static void AssertNotNull([NotNull] object? value,
        [CallerArgumentExpression(nameof(value))] string? message = null)
    {
        Assert(value != null, $"{message} is not null.");
    }

    /// <summary>
    /// Assert that the given value is null.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="message">Default's to the expression passed as the condition.</param>
    [Conditional("DEBUG")]
    public static void AssertNull(object? value, [CallerArgumentExpression(nameof(value))] string? message = null)
    {
        Assert(value == null, $"{message} is null.");
    }

    /// <summary>
    /// Somthing is wrong and the plugin should not continue.
    /// </summary>
    [DoesNotReturn]
    [Conditional("DEBUG")]
    public static void Fail(string? message = null) => throw new AssertionException(message);

    private sealed class AssertionException : Exception
    {
        internal AssertionException(string? message) : base(message)
        {
        }
    }
}
