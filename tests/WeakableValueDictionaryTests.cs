namespace UiBuilderLibrary.Collections;

public class WeakableValueDictionaryUnitTests
{
    // ReSharper disable InconsistentNaming NotAccessedField.Local FieldCanBeMadeReadOnly.Local
    private class TestData
    {
        public int Value;

        public TestData(int value)
        {
            Value = value;
        }
    }
    // ReSharper restore InconsistentNaming NotAccessedField.Local FieldCanBeMadeReadOnly.Local

    private static WeakableValueDictionary<string, TestData> CreateWeakableValueDictionary() =>
        new WeakableValueDictionary<string, TestData>
        {
            { "foo", CreateTestData(1) },
            { "bar", CreateTestData(2) },
            { "baz", CreateTestData(3) }
        };

    private static TestData CreateTestData(int value) => new(value);

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CanAddAndRemove()
    {
        var dictionary = CreateWeakableValueDictionary();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(dictionary, Does.ContainKey("foo"), "key is not in dictionary");
            Assert.That(dictionary, Does.ContainKey("bar"), "key is not in dictionary");
            Assert.That(dictionary, Does.ContainKey("baz"), "key is not in dictionary");
            Assert.That(dictionary, Does.Not.ContainKey("qux"), "key is in dictionary");
        }

        dictionary.Add("qux", CreateTestData(4));

        Assert.That(dictionary, Does.ContainKey("qux"), "key is not in dictionary");

        Assert.Throws(typeof(ArgumentException), () => { dictionary.Add("bar", CreateTestData(5)); });

        var keys = dictionary.Keys.ToArray();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(keys, Has.Length.EqualTo(4));
            Assert.That(keys, Does.Contain("qux"));
            Assert.That(keys, Does.Contain("bar"));
        }

        var values = dictionary.Values.ToArray();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(values, Has.Length.EqualTo(4));
            Assert.That(values.ElementAt(0).Value, Is.EqualTo(1));
            Assert.That(values.ElementAt(1).Value, Is.EqualTo(2));
            Assert.That(values.ElementAt(2).Value, Is.EqualTo(3));
            Assert.That(values.ElementAt(3).Value, Is.EqualTo(4));
        }

        var kvp = ToEnumerable(dictionary.GetEnumerator).ToArray();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(kvp, Has.Length.EqualTo(4));
            Assert.That(kvp.ElementAt(3).Key, Is.EqualTo("qux"));
            Assert.That(kvp.ElementAt(3).Value.Value, Is.EqualTo(4));
        }

        dictionary.Remove("qux");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(dictionary.Keys, Has.Count.EqualTo(3));
            Assert.That(dictionary.Values, Has.Count.EqualTo(3));
            Assert.That(ToEnumerable(dictionary.GetEnumerator).Count(), Is.EqualTo(3));
        }

        dictionary.Clear();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(dictionary.Keys, Has.Count.EqualTo(0));
            Assert.That(dictionary.Values, Has.Count.EqualTo(0));
            Assert.That(ToEnumerable(dictionary.GetEnumerator).Count(), Is.EqualTo(0));
        }
    }

    [Test]
    public async Task WeakReferenceWillBeRemoved()
    {
        // Prepopulated with keys: "foo", "bar" and "baz".
        var dictionary = CreateWeakableValueDictionary();

        // ReSharper disable InlineOutVariableDeclaration
        // TestData? fooValue, barValue, bazValue;
        // ReSharper restore InlineOutVariableDeclaration

        // Test initial state.

        Assert.Multiple(() =>
        {
            Assert.That(dictionary, Has.Count.EqualTo(3));
            Assert.That(dictionary.TryGetValue("foo", out var fooValue), Is.True);
            Assert.That(fooValue, Is.Not.Null);
            Assert.That(dictionary.TryGetValue("bar", out var barValue), Is.True);
            Assert.That(barValue, Is.Not.Null);
            Assert.That(dictionary.TryGetValue("baz", out var bazValue), Is.True);
            Assert.That(bazValue, Is.Not.Null);
        });

        dictionary.MarkWeak("bar"); // Allow to be garbage collected.

        // No changes should have happened yet.

        Assert.Multiple(() =>
        {
            Assert.That(dictionary, Has.Count.EqualTo(3));
            Assert.That(dictionary.TryGetValue("foo", out var fooValue), Is.True);
            Assert.That(fooValue, Is.Not.Null);
            Assert.That(dictionary.TryGetValue("bar", out var barValue), Is.True);
            Assert.That(barValue, Is.Not.Null);
            Assert.That(dictionary.TryGetValue("baz", out var bazValue), Is.True);
            Assert.That(bazValue, Is.Not.Null);
        });

        GC.Collect();
        await Task.Delay(100);

        // Changes should have happened now that garbage collection has run and references are out of scope.

        Assert.Multiple(() =>
        {
            Assert.That(dictionary.TryGetValue("foo", out var fooValu), Is.True);
            Assert.That(fooValu, Is.Not.Null);
            Assert.That(dictionary.TryGetValue("bar", out var barValu), Is.False, "bar value should no longer exist");
            Assert.That(barValu, Is.Null);
            Assert.That(dictionary.TryGetValue("baz", out var bazValue), Is.True);
            Assert.That(bazValue, Is.Not.Null);
            Assert.That(dictionary, Has.Count.EqualTo(2), "bar value should no longer contribute to the count");
        });
    }

    private static IEnumerable<T> ToEnumerable<T>(Func<IEnumerator<T>> getEnumerator)
    {
        var enumerator = getEnumerator();
        while (enumerator.MoveNext())
            yield return enumerator.Current;
    }
}
