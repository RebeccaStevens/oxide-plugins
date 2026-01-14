using System;
using System.Collections.Generic;
using System.IO;

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
    /// Parse the command line arguments into short flags, long flags and values.
    /// </summary>
    public static ParsedCommandLineArgs ParseCommandLineArgs(string[] args)
    {
        var shortFlags = new HashSet<char>();
        var longFlags = new Dictionary<string, string?>();
        var values = new List<string>();

        foreach (var arg in args)
        {
            if (arg.StartsWith("--"))
            {
                if (arg.Length > 2)
                {
                    var parts = arg[2..].Split('=');
                    switch (parts.Length)
                    {
                        case 1:
                            longFlags[parts[0]] = null;
                            continue;
                        case 2:
                            longFlags[parts[0]] = parts[1];
                            continue;
                    }
                }

                Debug.Fail("Invalid long flag: " + arg);
            }
            else if (arg.StartsWith("-"))
            {
                if (arg.Length == 1)
                {
                    Debug.Fail("Invalid short flag: " + arg);
                    continue;
                }

                foreach (var c in arg[1..])
                {
                    Debug.Assert(c is >= 'a' and <= 'z' or >= 'A' and <= 'Z');
                    shortFlags.Add(c);
                }
            }
            else
                values.Add(arg);
        }

        return new ParsedCommandLineArgs(shortFlags, longFlags, values);
    }

    public record struct ParsedCommandLineArgs(
        HashSet<char> ShortFlags,
        Dictionary<string, string?> LongFlags,
        List<string> Values);

    /// <summary>
    /// Parse a boolean flag from the command line arguments.
    /// </summary>
    /// <exception cref="InvalidDataException">Thrown if a flag value is invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the flags conflict.</exception>
    public static bool ParseBooleanMultiFlag(ParsedCommandLineArgs args, char shortFlag, string longFlag,
        char notShortFlag, string notLongFlag, bool defaultValue = false)
    {
        var value = defaultValue;
        var trueMatches = 0;
        var falseMatches = 0;

        if (args.ShortFlags.Contains(shortFlag))
        {
            value = true;
            trueMatches += 1;
        }

        if (args.ShortFlags.Contains(notShortFlag))
        {
            value = false;
            falseMatches += 1;
        }

        if (args.LongFlags.TryGetValue(longFlag, out var stringValue))
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                value = true;
                trueMatches += 1;
            }
            else if (bool.TryParse(stringValue, out value))
            {
                if (value)
                    trueMatches += 1;
                else
                    falseMatches += 1;
            }
            else
                throw new InvalidDataException($"Invalid value for flag {longFlag}: {stringValue}");
        }

        if (args.LongFlags.TryGetValue(notLongFlag, out var stringNotValue))
        {
            if (string.IsNullOrEmpty(stringNotValue))
            {
                value = false;
                falseMatches += 1;
            }
            else if (bool.TryParse(stringNotValue, out value))
            {
                value = !value;
                if (value)
                    trueMatches += 1;
                else
                    falseMatches += 1;
            }
            else
                throw new InvalidDataException($"Invalid value for flag {notLongFlag}: {stringNotValue}");
        }

        Debug.Assert(trueMatches <= 1);
        Debug.Assert(falseMatches <= 1);
        Debug.Assert(trueMatches == 0 || falseMatches == 0);

        if (trueMatches > 0 && falseMatches > 0)
            throw new InvalidOperationException("Conflicting boolean flags.");

        return value;
    }

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
