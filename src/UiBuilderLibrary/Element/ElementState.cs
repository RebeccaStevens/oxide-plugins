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
        /// The bounds of this element.
        /// </summary>
        public readonly Bounds Bounds;

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
            Bounds = new Bounds();
            NeedsSync = false;
            IsOpen = false;
        }

        /// <summary>
        /// Get the name of the root CUI element for this state.
        /// </summary>
        internal abstract string GetCuiRootName();

        /// <summary>
        /// Get the name of the content CUI element for this state (can be the same as the root).
        /// </summary>
        internal abstract string GetCuiContentName();

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
        /// Get the names of the Cui Elements for this element in this state.
        /// </summary>
        internal abstract string[] GetCuiElementNames();

        /// <summary>
        /// Create the Cui Elements for the element in this state.
        /// </summary>
        /// <returns>A list of Cui Elements that make up this element.</returns>
        internal abstract SafeCuiElement[] CreateCuiElements();
    }
}