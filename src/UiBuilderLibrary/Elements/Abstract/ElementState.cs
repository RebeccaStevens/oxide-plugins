using System.Collections.Generic;
using System.Linq;
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
        /// The id of this state.
        /// </summary>
        protected readonly string Id;

        /// <summary>
        /// The element this state is for.
        /// </summary>
        public readonly Element Element;

        /// <summary>
        /// The player this state is for.
        /// </summary>
        public readonly BasePlayer Player;

        /// <summary>
        /// The bounds that this element will place itself within.
        /// </summary>
        public readonly Bounds Bounds;

        /// <summary>
        /// The bounds of the content (children) of this element.
        /// </summary>
        protected readonly Bounds ContentBounds;

        /// <summary>
        /// The bounds of the root cui element taking into account things like the element's margin.
        /// </summary>
        protected readonly Bounds CuiBounds;

        /// <summary>
        /// Does this state need to be (re)sent to the player?
        /// </summary>
        protected bool NeedsSync { get; private set; }

        /// <summary>
        /// States whether the element is being displayed to the player.
        /// </summary>
        public bool IsOpen { get; private set; }

        /// <summary>
        /// Create the state for the element for the player.
        /// </summary>
        /// <param name="element">The element this state is for.</param>
        /// <param name="player">The player this state is for.</param>
        protected ElementState(Element element, BasePlayer player)
        {
            Element = element;
            Id = CuiHelper.GetGuid();
            Player = player;
            NeedsSync = false;
            IsOpen = false;
            Bounds = new Bounds();
            ContentBounds = new Bounds();
            CuiBounds = new Bounds();
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
        /// <param name="markAsNoLongerNeedingResync">If true, each state that is returned will have its NeedsSync property set to false.</param>
        internal IEnumerable<ElementState> GetStatesNeedingSync(bool markAsNoLongerNeedingResync)
        {
            var list = new List<ElementState>();

            var stack = new Stack<ElementState>();
            stack.Push(this);

            var needsRerender = NeedsSync;
            while (stack.TryPop(out var state))
            {
                // When an state needs to be rerendered, all its children do too.
                needsRerender = needsRerender || state.NeedsSync;
                if (needsRerender)
                {
                    list.Add(state);
                    if (markAsNoLongerNeedingResync)
                        state.NeedsSync = false;
                }

                foreach (var childState in state.Element.GetChildren().Select((child) => child.GetState(Player))
                             .Reverse())
                    stack.Push(childState);
            }

            return list;
        }

        /// <summary>
        /// Open this element (and its children).
        /// </summary>
        public void Open()
        {
            foreach (var child in Element.GetChildren())
                child.Open(Player);
            IsOpen = true;
            NeedsSync = true;
        }

        /// <summary>
        /// Close this element (and its children).
        /// </summary>
        public void Close()
        {
            IsOpen = false;
            NeedsSync = true;
            foreach (var child in Element.GetChildren())
                child.Close(Player);
        }

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
            ContentBounds.MaximizePosition();
        }

        /// <summary>
        /// Get the Cui Elements for the element in this state.
        /// </summary>
        /// <returns>The Cui Elements that make up this element.</returns>
        public ElementCuiElements GetCuiElements()
        {
            ComputeInternalBounds();
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

            public void Add(SafeCuiElement other)
            {
                others.Add(other);
            }

            public IEnumerable<SafeCuiElement> GetAll() =>
                new[] { root, content }.Concat(others).OfType<SafeCuiElement>().Distinct();
        }
    }
}
