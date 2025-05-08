using System;
using System.Collections.Generic;
using System.Linq;

namespace Oxide.Plugins;

[TestClass]
public class WeakableValueDictionaryUnitTests
{
    private class TestData
    {
        public int Value;
        public TestData(int value)
        {
            Value = value;
        }
    }

    private static UiBuilderLibrary.WeakableValueDictionary<string, TestData> CreateWeakableValueDictionary() => new UiBuilderLibrary.WeakableValueDictionary<string, TestData>
        {
            {"foo", CreateTestData(1)},
            {"bar", CreateTestData(2)},
            {"baz", CreateTestData(3)}
        };

    private static TestData CreateTestData(int value) => new(value);

    [TestMethod]
    public void CanAddAndRemove()
    {
        var dictionary = CreateWeakableValueDictionary();

        Assert.IsTrue(dictionary.ContainsKey("foo"), "key is not in dictionary");
        Assert.IsTrue(dictionary.ContainsKey("bar"), "key is not in dictionary");
        Assert.IsTrue(dictionary.ContainsKey("baz"), "key is not in dictionary");
        Assert.IsFalse(dictionary.ContainsKey("qux"), "key is in dictionary");

        dictionary.Add("qux", CreateTestData(4));

        Assert.IsTrue(dictionary.ContainsKey("qux"), "key is not in dictionary");

        Assert.ThrowsException<ArgumentException>(() =>
        {
            dictionary.Add("bar", CreateTestData(5));
        });

        var keys = dictionary.Keys.ToArray();
        Assert.AreEqual(4, keys.Length);
        Assert.IsTrue(keys.Contains("qux"));
        Assert.IsTrue(keys.Contains("bar"));

        var values = dictionary.Values.ToArray();
        Assert.AreEqual(4, values.Length);
        Assert.AreEqual(1, values.ElementAt(0)!.Value);
        Assert.AreEqual(2, values.ElementAt(1)!.Value);
        Assert.AreEqual(3, values.ElementAt(2)!.Value);
        Assert.AreEqual(4, values.ElementAt(3)!.Value);

        var kvp = ToEnumerable(dictionary.GetEnumerator).ToArray();
        Assert.AreEqual(4, kvp.Length);
        Assert.AreEqual("qux", kvp.ElementAt(3).Key);
        Assert.AreEqual(4, kvp.ElementAt(3).Value!.Value);

        dictionary.Remove("qux");

        Assert.AreEqual(3, dictionary.Keys.ToArray().Length);
        Assert.AreEqual(3, dictionary.Values.ToArray().Length);
        Assert.AreEqual(3, ToEnumerable(dictionary.GetEnumerator).ToArray().Length);

        dictionary.Clear();

        Assert.AreEqual(0, dictionary.Keys.ToArray().Length);
        Assert.AreEqual(0, dictionary.Values.ToArray().Length);
        Assert.AreEqual(0, ToEnumerable(dictionary.GetEnumerator).ToArray().Length);
    }

    [TestMethod]
    public void WeakReferenceWillBeRemoved()
    {
        // Prepopulated with keys: "foo", "bar" and "baz".
        var dictionary = CreateWeakableValueDictionary();

        TestData? fooValue, barValue, bazValue;

        // Test initial state.

        Assert.AreEqual(3, dictionary.Count);
        Assert.IsTrue(dictionary.TryGetValue("foo", out fooValue), "Could not get foo value");
        Assert.IsNotNull(fooValue);
        Assert.IsTrue(dictionary.TryGetValue("bar", out barValue), "Could not get bar value");
        Assert.IsNotNull(barValue);
        Assert.IsTrue(dictionary.TryGetValue("baz", out bazValue), "Could not get baz value");
        Assert.IsNotNull(bazValue);

#pragma warning disable IDE0059
        // Resetting to null seems to be required.
        fooValue = barValue = bazValue = null;
#pragma warning restore IDE0059

        dictionary.MarkWeak("bar"); // Allow to be garbage collected.

        // No changes should have happened yet.

        Assert.AreEqual(3, dictionary.Count);
        Assert.IsTrue(dictionary.TryGetValue("foo", out fooValue), "Could not get foo value");
        Assert.IsNotNull(fooValue);
        Assert.IsTrue(dictionary.TryGetValue("bar", out barValue), "Could not get bar value");
        Assert.IsNotNull(barValue);
        Assert.IsTrue(dictionary.TryGetValue("baz", out bazValue), "Could not get baz value");
        Assert.IsNotNull(bazValue);

#pragma warning disable IDE0059
        fooValue = barValue = bazValue = null;
#pragma warning restore IDE0059

        GC.Collect();

        // Changes should have happened now that garbage collection has run.

        Assert.IsTrue(dictionary.TryGetValue("foo", out fooValue), "Could not get foo value again");
        Assert.IsNotNull(fooValue);
        Assert.IsFalse(dictionary.TryGetValue("bar", out barValue), "bar value should no longer exist");
        Assert.IsNull(barValue);
        Assert.IsTrue(dictionary.TryGetValue("baz", out bazValue), "Could not get baz value again");
        Assert.IsNotNull(bazValue);
        Assert.AreEqual(2, dictionary.Count, "bar value should no longer contribute to the count");
    }

    private static IEnumerable<T> ToEnumerable<T>(Func<IEnumerator<T>> getEnumerator)
    {
        var enumerator = getEnumerator();
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
}