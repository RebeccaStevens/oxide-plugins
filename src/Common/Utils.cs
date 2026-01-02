using System;
using System.Collections.Generic;

namespace Oxide.Plugins;

/// <summary>
/// A collection of utility functions.
/// </summary>
internal static class Utils
{
    /// <summary>
    /// A function that returns the given value.
    /// </summary>
    public static T Identity<T>(T value) => value;

    /// <summary>
    /// Get or create a new value in the dictionary.
    /// </summary>
    public static TValue DictionaryGetOrCreateNew<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey key)
        where TKey : notnull where TValue : new()
    {
        if (dict.TryGetValue(key, out TValue? val))
            return val;

        val = new TValue();
        dict.Add(key, val);
        return val;
    }

    /// <summary>
    /// Pipe a value through a series of functions.
    /// </summary>
    public static B Pipe<A, B>(A value, Func<A, B> func1) => func1(value);

    /// <inheritdoc cref="Pipe{A, B}(A, Func{A, B})"/>
    public static C Pipe<A, B, C>(A value, Func<A, B> func1, Func<B, C> func2) => func2(func1(value));

    /// <inheritdoc cref="Pipe{A, B}(A, Func{A, B})"/>
    public static D Pipe<A, B, C, D>(A value, Func<A, B> func1, Func<B, C> func2, Func<C, D> func3) =>
        func3(func2(func1(value)));

    /// <inheritdoc cref="Pipe{A, B}(A, Func{A, B})"/>
    public static E
        Pipe<A, B, C, D, E>(A value, Func<A, B> func1, Func<B, C> func2, Func<C, D> func3, Func<D, E> func4) =>
        func4(func3(func2(func1(value))));

    /// <inheritdoc cref="Pipe{A, B}(A, Func{A, B})"/>
    public static F Pipe<A, B, C, D, E, F>(A value, Func<A, B> func1, Func<B, C> func2, Func<C, D> func3,
        Func<D, E> func4, Func<E, F> func5) => func5(func4(func3(func2(func1(value)))));
}
