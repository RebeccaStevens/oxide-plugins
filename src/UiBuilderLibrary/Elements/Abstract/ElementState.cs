using System.Collections.Generic;
using System.Linq;
using Facepunch;
using Oxide.Core;
using Oxide.Game.Rust.Cui;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    // TODO: Pool ElementStates.

    /// <summary>
    /// An instance of state of the element for an indiviual player.
    /// </summary>
    public abstract class ElementState
    {
        /// <summary>
        /// All open states for all elements.
        /// </summary>
        private static readonly Dictionary<string, ElementState> AllOpenStates = new();

        /// <summary>
        /// The id of this state.
        /// </summary>
        protected readonly string Id;

        /// <summary>
        /// The element this state is for.
        /// </summary>
        public Element Element { get; }

        /// <summary>
        /// The player this state is for.
        /// </summary>
        public BasePlayer Player { get; }

        /// <summary>
        /// The backing field for the <see cref="IsEnabled"/> property.
        /// </summary>
        protected bool IsEnabledBacking;

        /// <summary>
        /// The bounds that this element will place itself within.
        /// </summary>
        public Bounds Bounds { get; }

        /// <summary>
        /// The bounds of the content (children) of this element.
        /// </summary>
        protected Bounds ContentBounds { get; }

        /// <summary>
        /// The bounds of the root cui element taking into account things like the element's margin.
        /// </summary>
        protected Bounds CuiBounds { get; }

        /// <summary>
        /// Does this state need to be (re)sent to the player?
        /// </summary>
        protected bool NeedsSync { get; set; }

        /// <summary>
        /// States whether the element is being displayed to the player.
        /// </summary>
        public bool IsOpen => AllOpenStates.ContainsKey(Id);

        /// <summary>
        /// Create the state for the element for the player.
        /// </summary>
        /// <param name="element">The element this state is for.</param>
        /// <param name="player">The player this state is for.</param>
        protected ElementState(Element element, BasePlayer player) : this(element, player, CuiHelper.GetGuid())
        {
        }

        internal ElementState(Element element, BasePlayer player, string id)
        {
            Element = element;
            Id = id;
            Player = player;
            IsEnabled = true;
            NeedsSync = false;
            Bounds = new Bounds();
            ContentBounds = new Bounds();
            CuiBounds = new Bounds();
        }

        /// <summary>
        /// Whether this state is enabled.<br/>
        /// A state that is not enabled will not be rendered nor will any of its children.
        /// </summary>
        public bool IsEnabled
        {
            get => Element.IsEnabled != null
                ? (IsEnabledBacking = Element.IsEnabled(this, IsEnabledBacking))
                : IsEnabledBacking;
            set
            {
                if (value == IsEnabledBacking)
                    return;
                IsEnabledBacking = value;
                NeedsSync = true;
                if (!IsEnabledBacking)
                    Close();
            }
        }

        /// <summary>
        /// Get the name of the root CUI element for this state.
        /// </summary>
        internal virtual string GetCuiRootName()
        {
            Debug.Assert(!string.IsNullOrEmpty(Element.Name));
            Panic.If(string.IsNullOrEmpty(Id));
            return $"{Element.Name}-root-{Id}";
        }

        /// <summary>
        /// Get the name of the content CUI element for this state (can be the same as the root).
        /// </summary>
        internal virtual string GetCuiContentName()
        {
            Debug.Assert(!string.IsNullOrEmpty(Element.Name));
            Panic.If(string.IsNullOrEmpty(Id));
            return $"{Element.Name}-content-{Id}";
        }

        /// <summary>
        /// Get the name of the parent CUI content element for this state.
        /// </summary>
        internal string GetParentCuiContentName() => Element.GetParentCuiContentName(Player);

        /// <summary>
        /// Get all the states that need to be synced/rerendered.<br />
        /// This looks at this state and all its children states.
        /// </summary>
        /// <param name="markAsNoLongerNeedingResync">If true, each state that is returned will have its <see cref="NeedsSync"/> property set to false.</param>
        private IEnumerable<ElementState> GetStatesNeedingSync(bool markAsNoLongerNeedingResync)
        {
            var resultList = Pool.Get<List<ElementState>>();

            var stack = new Stack<ElementState>();
            stack.Push(this);

            var needsRerender = NeedsSync;
            if (needsRerender && markAsNoLongerNeedingResync)
                NeedsSync = false;
            while (stack.TryPop(out var state))
            {
                // When a state needs to be rerendered, all its children do too.
                needsRerender = needsRerender || state.NeedsSync;

                // Add the state to the result if it needs to be rerendered.
                if (needsRerender)
                {
                    resultList.Add(state);
                    if (markAsNoLongerNeedingResync)
                        state.NeedsSync = false;
                }

                // No need to check the children of a state that isn't enabled.
                // The disabled state itself still needs to be checked in case it needs to be destroyed on the client.
                if (!state.IsEnabled)
                    continue;

                // Check all the child states of this state.
                // TODO: Can this be optimized - reduce the number of iterations needed to add all the child states in the correct order?
                foreach (var childState in state.Element.GetChildren().Select((child) => child.GetState(Player))
                             .Reverse())
                    stack.Push(childState);
            }

            var result = resultList.ToArray();
            Pool.FreeUnmanaged(ref resultList);
            return result;
        }

        /// <summary>
        /// Send all states in this state's hierarchy needing to be synced to the player.
        /// </summary>
        public void Sync()
        {
            var resyncStates = GetStatesNeedingSync(true);

            var (createNewStates, destroyOldStates) =
                TransposeAndFlatten(resyncStates.Select(ElementStateToJson));

            var dataComponents = destroyOldStates.Concat(createNewStates).ToArray();
            if (dataComponents.Length == 0)
            {
                Interface.Oxide.LogDebug("Cannot open UI: nothing to display (or remove).");
                return;
            }

            var data = $"[{string.Join(",", dataComponents)}]";
            CuiHelper.AddUi(Player, data);
            // builder.Puts(data);
        }

        /// <summary>
        /// Open this element (and its children).
        /// </summary>
        /// <param name="force">If true, the element will be marked as needing to be resynced even if it was already open.</param>
        public virtual void Open(bool force = false)
        {
            // Can't open a state that isn't enabled.
            if (!IsEnabled)
            {
                // Debug.Fail("Attempted to open a state that isn't enabled.");
                return;
            }

            if (!AllOpenStates.TryAdd(Id, this) && !force)
                return;

            NeedsSync = true;
            foreach (var child in Element.GetChildren())
                child.Open(Player, force);
        }

        /// <summary>
        /// Close this element (and its children).
        /// </summary>
        /// <param name="force">If true, the element will be marked as needing to be resynced even if it was already closed.</param>
        public virtual void Close(bool force = false)
        {
            if (!AllOpenStates.Remove(Id) && !force)
                return;

            NeedsSync = true;
            foreach (var child in Element.GetChildren())
                child.Close(Player, force);
        }

        /// <summary>
        /// Get the parent state of this state.
        /// </summary>
        /// <returns>The state of this element's parent for the same player, or null if this element has no parent.</returns>
        public ElementState? GetParent() => Element.GetParent()?.GetState(Player);

        /// <summary>
        /// Get the state of each child element of this state's element.
        /// </summary>
        public IEnumerable<ElementState> GetChildren()
        {
            return Element.GetChildren().Select((child) => child.GetState(Player));
        }

        /// <summary>
        /// Get the names of the Cui Elements for this element in this state.
        /// </summary>
        internal abstract string[] GetCuiElementNames();

        /// <summary>
        /// Compute (update) the cui and content bounds of this element based on the bounds, margin, padding ect. of the element.
        /// </summary>
        protected virtual void ComputeInternalBounds()
        {
            CuiBounds.SetTo(Bounds);
            CuiBounds.AddPosition(Element.Margin.GetInnerBounds(this));
            CuiBounds.SetPivot(Element.Pivot);
            CuiBounds.SetRotation(Element.Rotation);

            ContentBounds.MaximizePosition();
            ContentBounds.SetPivot(Element.Pivot);
            ContentBounds.SetRotation(Element.Rotation);
        }

        /// <summary>
        /// Get the Cui Elements for the element in this state.
        /// </summary>
        /// <returns>The Cui Elements that make up this element.</returns>
        public ElementCuiElements GetCuiElements()
        {
            ComputeInternalBounds();

            // Workaround for how the client renders nested elements.
            // For some reason this fixes things in both the x and y directions.
            CuiBounds.FromRight += Bounds.AbsoluteValue(1);
            ContentBounds.FromRight += Bounds.AbsoluteValue(-1);

            return CreateCuiElements();
        }

        /// <summary>
        /// Create the Cui Elements for the element in this state.<br/>
        /// <br/>
        /// Note: UpdateCuiBounds and UpdateContentBounds will be called before this method is called.
        /// </summary>
        /// <returns>The Cui Elements that make up this element.</returns>
        protected abstract ElementCuiElements CreateCuiElements();

        /// <summary>
        /// Get the element state for the given id.
        /// </summary>
        /// <param name="id">The id of the element state.</param>
        /// <returns>The element state for the given id when that state is open, otherwise null.</returns>
        public static ElementState? GetById(string id) => AllOpenStates.GetValueOrDefault(id);

        /// <summary>
        /// The Cui Elements for the element in this state.
        /// </summary>
        public struct ElementCuiElements
        {
            private SafeCuiElement? root;
            private SafeCuiElement? content;
            private readonly List<SafeCuiElement> others;

            public SafeCuiElement? Root
            {
                get => root;
                set => root = value;
            }

            public SafeCuiElement? Content
            {
                get => content ?? root;
                set => content = value;
            }

            public ElementCuiElements()
            {
                root = null;
                content = null;
                others = new List<SafeCuiElement>();
            }

            public bool HasContent => content != null;

            public void Add(SafeCuiElement other)
            {
                others.Add(other);
            }

            public IEnumerable<SafeCuiElement> GetAll() =>
                new[] { root, content }.Concat(others).OfType<SafeCuiElement>().Distinct();
        }

        /// <summary>
        /// Get a JSON representation of the given element state.
        /// </summary>
        /// <param name="state">What to encode.</param>
        /// <returns>A tuple of JSON representations of the new objects to create and the old objects to destroy.</returns>
        private static (IEnumerable<string> Update, IEnumerable<string> Destroy) ElementStateToJson(ElementState state)
        {
            if (!state.IsOpen)
                return (
                    Enumerable.Empty<string>(),
                    state.GetCuiElementNames().Select(name => $"{{\"destroyUi\":\"{name}\"}}")
                );

            ElementLayout.PositionElementInParent(state);
            var cuiComponents = state.Element.Layout.GetComponentsForContainer(state);
            var cuiElements = state.GetCuiElements();
            Debug.Assert(cuiElements.Root != null);
            Debug.Assert(cuiElements.Content != null);

            if (cuiComponents != null)
            {
                foreach (var component in cuiComponents)
                    cuiElements.Content.AddComponent(component);
                Pool.FreeUnmanaged(ref cuiComponents);
            }

            var allCuis = cuiElements.GetAll().ToArray();
            var updatedStates = allCuis.Select(cuiElement => cuiElement.Encode(state));
            var deleteStates = state
                .GetCuiElementNames()
                .Except(allCuis.Select(cuiElement => cuiElement.Name))
                .Select(name => $"{{\"destroyUi\":\"{name}\"}}");

            return (updatedStates, deleteStates);
        }
    }
}
