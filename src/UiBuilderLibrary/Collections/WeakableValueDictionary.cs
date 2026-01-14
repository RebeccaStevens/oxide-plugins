using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Facepunch;

namespace Oxide.Plugins;

/// <summary>
/// A dictionary in which the values can be marked as weak references.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class WeakableValueDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull where TValue : class
{
    private readonly Dictionary<TKey, WeakableReference<TValue>> dict = new();
    private int version, cleanVersion;
    private int lastGarbageCollectionCount;
    private const int MinRehashInterval = 100;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get
        {
            if (!dict.ContainsKey(key))
                throw new KeyNotFoundException();
#if DEBUG
            if (dict[key]?.IsStrong == false)
                throw new NotSupportedException("Do not access via [index] when weakly referenced.");
#endif
            return TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();
        }
        set => Add(key, value);
    }

    /// <summary>
    /// Create a new dictionary of values that can be made weak.
    /// </summary>
    public WeakableValueDictionary()
    {
    }


    /// <summary>
    /// Adds an element with the provided key and value to the dictionary, strongly referencing the value.
    /// </summary>
    /// <param name="key">The object to use as the key of the element to add.</param>
    /// <param name="value">The object to use as the value of the element to add.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the dictionary.</exception>
    public void Add(TKey key, TValue value)
    {
        Add(key, value, true);
    }

    /// <summary>
    /// Adds an element with the provided key and value to the dictionary.
    /// </summary>
    /// <param name="key">The object to use as the key of the element to add.</param>
    /// <param name="value">The object to use as the value of the element to add.</param>
    /// <param name="strong">If true, the value will be held as a strong reference. If false, it will be held as a weak reference.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the dictionary.</exception>
    public void Add(TKey key, TValue value, bool strong)
    {
        if (dict.TryGetValue(key, out var reference))
        {
            if (reference.TryGetTarget(out _))
                throw new ArgumentException("An element with the same key already exists in this dictionary.");

            reference.SetTarget(value, strong);
            AutoCleanup();
            return;
        }

        dict.Add(key, new WeakableReference<TValue>(value, strong));
        AutoCleanup();
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        if (dict.TryGetValue(key, out var reference))
            return reference.TryGetTarget(out _);
        else
            AutoCleanup();

        return false;
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        if (dict.TryGetValue(key, out var reference))
            return dict.Remove(key) && reference.TryGetTarget(out _);
        else
            AutoCleanup();

        return false;
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (dict.TryGetValue(key, out var reference))
        {
            if (!reference.TryGetTarget(out value))
                AutoCleanup();
        }
        else
        {
            AutoCleanup();
            value = null;
        }

        return value != null;
    }

    /// <summary>
    /// Gets the value associated with the specified key and mark the reference as strong.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns>true if the value was obtained, otherwise false</returns>
    public bool TryGetValueAndMarkStrong(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (dict.TryGetValue(key, out var reference))
        {
            if (!reference.TryGetTargetAndMarkStrong(out value))
                AutoCleanup();
        }
        else
        {
            AutoCleanup();
            value = null;
        }

        return value != null;
    }

    /// <summary>
    /// Gets the value associated with the specified key and mark the reference as weak.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns>true if the value was obtained, otherwise false</returns>
    public bool TryGetValueAndMarkWeak(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (dict.TryGetValue(key, out var reference))
        {
            if (reference.TryGetTarget(out value))
                reference.MarkWeak();
            else
                AutoCleanup();
        }
        else
        {
            AutoCleanup();
            value = null;
        }

        return value != null;
    }

    /// <summary>
    /// Mark the value for the given key as weak.
    /// </summary>
    /// <param name="key">The key</param>
    public void MarkWeak(TKey key)
    {
        if (dict.TryGetValue(key, out var reference))
        {
            reference.MarkWeak();
            return;
        }

        AutoCleanup();
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        dict.Clear();
        version = 0;
        cleanVersion = 0;
        lastGarbageCollectionCount = 0;
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if (TryGetValue(item.Key, out var value))
            return value == item.Value;
        return false;
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        var toCopy = dict
            .Select<KeyValuePair<TKey, WeakableReference<TValue>>, KeyValuePair<TKey, TValue>?>((kvp) =>
            {
                if (kvp.Value.TryGetTarget(out var value))
                    return new KeyValuePair<TKey, TValue>(kvp.Key, value);
                return null;
            })
            .ToArray()
            .Where((kvp) => kvp == null)
            .ToArray();

        AutoCleanup(dict.Count - toCopy.Length);
        toCopy.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (!TryGetValue(item.Key, out var value))
            return false;
        return value == item.Value && Remove(item.Key);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public int Count
    {
        get
        {
            var count = 0;
            var nullCount = 0;

            foreach (var kvp in dict)
            {
                if (kvp.Value.TryGetTarget(out var _))
                    count++;
                else
                    nullCount++;
            }

            AutoCleanup(nullCount);
            return count;
        }
    }

    /// <inheritdoc/>
    public ICollection<TKey> Keys
    {
        get
        {
            var keys = Pool.Get<List<TKey>>();
            var nullCount = 0;

            foreach (var kvp in dict)
            {
                if (kvp.Value.TryGetTarget(out var _))
                    keys.Add(kvp.Key);
                else
                    nullCount++;
            }

            AutoCleanup(nullCount);
            var result = keys.ToArray();
            Pool.FreeUnmanaged(ref keys);
            return result;
        }
    }

    /// <inheritdoc/>
    public ICollection<TValue> Values
    {
        get
        {
            var values = Pool.Get<List<TValue>>();
            var nullCount = 0;

            foreach (var kvp in dict)
            {
                if (kvp.Value.TryGetTarget(out var target))
                    values.Add(target);
                else
                    nullCount++;
            }

            AutoCleanup(nullCount);
            var result = values.ToArray();
            Pool.FreeUnmanaged(ref values);
            return result;
        }
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        var nullCount = 0;

        foreach (var kvp in dict)
        {
            if (kvp.Value.TryGetTarget(out var target))
                yield return new KeyValuePair<TKey, TValue>(kvp.Key, target);
            else
                nullCount++;
        }

        AutoCleanup(nullCount);
    }

    /// <summary>
    /// Perform a cleanup if it seems like a good idea based on how much stuff has happened.
    /// </summary>
    private void AutoCleanup(int incVersion = 1)
    {
        version += incVersion;

        // Cleanup the table every so often - less often for larger tables.
        long delta = version - cleanVersion;
        if (delta <= MinRehashInterval + dict.Count)
            return;

        // A cleanup will be fruitless unless a GC has happened.
        // WeakReferences can become dead only during the GC.
        var currentCollectionCount = GC.CollectionCount(0);
        if (lastGarbageCollectionCount != currentCollectionCount)
        {
            Cleanup();
            lastGarbageCollectionCount = currentCollectionCount;
            cleanVersion = version;
        }
        else
            cleanVersion += MinRehashInterval; // Wait a little while longer
    }

    /// <summary>
    /// Clean up the internal dictionary by removing all pairs whose value has been garbage collected.
    /// </summary>
    private void Cleanup()
    {
        var deadKeys = (from kvp in dict where !kvp.Value.TryGetTarget(out _) select kvp.Key);

        foreach (var key in deadKeys)
            dict.Remove(key);
    }
}
