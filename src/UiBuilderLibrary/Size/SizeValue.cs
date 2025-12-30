using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// The value of a size in the context of an element's state.
    /// </summary>
    internal class SizeStateValue
    {
        protected static readonly ConditionalWeakTable<SizeContext, ConditionalWeakTable<ElementState, SizeStateValue>>
            ValueByStateBySize = new();

        private readonly Func<Bounds.Value>? compute;
        private bool dirty;
        private Bounds.Value cachedValue;

        /// <summary>
        /// Create a new value that we don't currently know the value for.
        /// </summary>
        private SizeStateValue(Func<Bounds.Value> compute)
        {
            this.compute = compute;
            dirty = true;
        }

        /// <summary>
        /// Create a new value that has a fixed value.
        /// </summary>
        private SizeStateValue(Bounds.Value value)
        {
            compute = null;
            dirty = false;
            cachedValue = value;
        }

        /// <summary>
        /// Get the bounds' value of this value.
        /// </summary>
        public Bounds.Value GetBoundsValue()
        {
            if (!dirty)
                return cachedValue;

            Panic.IfNull(compute, "The cached value is dirty but there is no compute function.");

            dirty = false;
            return cachedValue = compute();
        }

        /// <summary>
        /// Mark all values for the given context as needing recomputation.
        /// </summary>
        /// <param name="context">The scope to make the changes for.</param>
        internal static void MarkAllDirty(SizeContext context)
        {
            if (!ValueByStateBySize.TryGetValue(context, out var valueByState))
                return;

            foreach (var (_, applied) in valueByState)
                if (applied.compute != null)
                    applied.dirty = true;
        }

        /// <summary> 
        /// Try to get the value for the given context with the given state.
        /// </summary>
        /// <param name="context">The context to get the value for.</param>
        /// <param name="state">The state to get the value for.</param>
        /// <param name="value">The value if it exists, otherwise null.</param>
        /// <returns>True if the value exists, otherwise false.</returns>
        internal static bool TryGetValueByState(SizeContext context, ElementState state,
            [MaybeNullWhen(false)] out SizeStateValue value)
        {
            var valueByState = ValueByStateBySize.GetOrCreateValue(context);
            return valueByState.TryGetValue(state, out value);
        }

        /// <summary>
        /// Get or create a value for the given bounds value.
        /// </summary>
        /// <param name="context">The context to create the value for.</param>
        /// <param name="state">The state used to create the value.</param>
        /// <param name="value">The bounds value to use for the value.</param>
        /// <returns>The value for the given bounds value.</returns>
        public static SizeStateValue GetOrCreateValue(SizeContext context, ElementState state,
            Bounds.Value value)
        {
            Debug.Assert(context.Element == state.Element,
                $"The given state is for a different element. State Element: {state.Element.Name}, Context Element: {context.Element.Name}");
            var valueByState = ValueByStateBySize.GetOrCreateValue(context);
            return valueByState.GetValue(state, (_) => new SizeStateValue(value));
        }

        /// <summary>
        /// Get or create a value for the given dynamic bounds value.
        /// </summary>
        /// <param name="context">The context to create the value for.</param>
        /// <param name="state">The state used to create the value.</param>
        /// <param name="compute">The function to compute the bounds value to use for the value.</param>
        /// <returns>The value for the given bounds value.</returns>
        public static SizeStateValue GetOrCreateValue(SizeContext context, ElementState state,
            Func<Bounds.Value> compute)
        {
            Debug.Assert(context.Element == state.Element,
                $"The given state is for a different element. State Element: {state.Element.Name}, Context Element: {context.Element.Name}");
            var valueByState = ValueByStateBySize.GetOrCreateValue(context);
            return valueByState.GetValue(state, (_) => new SizeStateValue(compute));
        }
    }
}