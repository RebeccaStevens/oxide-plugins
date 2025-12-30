namespace UiBuilderLibrary.Collections;

public class WeakableReferenceUnitTests
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

    private static WeakableReference<TestData> CreateWeakableReference(int value) => new(new TestData(value));

    [Test]
    public void ReferencesCanBeMadeWeak()
    {
        var reference = CreateWeakableReference(1);
        reference.MarkWeak();
        Assert.That(reference.IsStrong, Is.False, "Reference was not weak");
    }

    [Test]
    public void StrongReferencesWontBeGCed()
    {
        var reference = CreateWeakableReference(2);
        Assert.That(reference.IsStrong, Is.True, "Reference was not strong");

        GC.Collect();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(reference.TryGetTarget(out var target), Is.True, "Could not get target");
            Assert.That(target, Is.Not.Null);
        }
    }

    [Test]
    public void WeakReferencesCanBeGCed()
    {
        var reference = CreateWeakableReference(3);
        reference.MarkWeak();
        Assert.That(reference.IsStrong, Is.False, "Reference was not weak");

        GC.Collect();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(reference.TryGetTarget(out var target), Is.False, "Target should no longer exist");
            Assert.That(target, Is.Null);
        }
    }
}