using System;

namespace Oxide.Plugins;

[TestClass]
public class WeakableReferenceUnitTests
{
    private class TestData
    {
        public int Value;
        public TestData(int value)
        {
            Value = value;
        }
    }

    private static UiBuilderLibrary.WeakableReference<TestData> CreateWeakableReference(int value) => new(new TestData(value));

    [TestMethod]
    public void ReferencesCanBeMadeWeak()
    {
        var reference = CreateWeakableReference(1);
        reference.MarkWeak();
        Assert.IsFalse(reference.IsStrong, "Reference was not weak");
    }

    [TestMethod]
    public void StrongReferencesWontBeGCed()
    {
        var reference = CreateWeakableReference(2);
        Assert.IsTrue(reference.IsStrong, "Reference was not strong");

        GC.Collect();

        Assert.IsTrue(reference.TryGetTarget(out var target), "Could not get target");
        Assert.IsNotNull(target);
    }

    [TestMethod]
    public void WeakReferencesCanBeGCed()
    {
        var reference = CreateWeakableReference(3);
        reference.MarkWeak();
        Assert.IsFalse(reference.IsStrong, "Reference was not weak");

        GC.Collect();

        Assert.IsFalse(reference.TryGetTarget(out var target), "Target should no longer exist");
        Assert.IsNull(target);
    }
}