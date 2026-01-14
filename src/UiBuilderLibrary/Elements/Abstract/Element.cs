using System;
using System.Collections.Generic;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// The base class of all elements.
    /// </summary>
    public abstract class Element
    {
        /// <summary>
        /// The state of this element for each player.<br />
        /// The state is weakly held when the element is closed.
        /// </summary>
        private readonly WeakableValueDictionary<ulong, ElementState> stateByPlayer = new();

        /// <summary>
        /// A name for this element - useful for debugging.
        /// </summary>
        public string Name = "unnamed";

        // TODO: Support changing parents.
        // `Parent` and `parentLayer` should not both be set.
        private readonly string? parentLayer;
        private readonly Element? parent;

        /// <summary>
        /// The children of this element.
        /// </summary>
        private readonly List<Element> children;

        /// <summary>
        /// Used to determine the order of this element in regards to its siblings.<br/>
        /// Lower values are considered to come before higher values.
        /// </summary>
        public double Weight;

        private ElementLayout? layout;

        /// <summary>
        /// How to layout this element's children.
        /// </summary>
        public ElementLayout Layout
        {
            get => layout ??= AbsoluteLayout.Use();
            set => layout = value;
        }

        /// <summary>
        /// The anchor point of this element.
        /// Used by some layouts; ignored by others.
        /// </summary>
        public PositionAnchor Anchor = PositionAnchor.UpperLeft;

        /// <summary>
        /// The context for the x position of this element.
        /// </summary>
        internal readonly SizeContext XContext;

        /// <summary>
        /// The context for the y position of this element.
        /// </summary>
        internal readonly SizeContext YContext;

        /// <summary>
        /// The context for the width of this element.
        /// </summary>
        internal readonly SizeContext WidthContext;

        /// <summary>
        /// The context for the height of this element.
        /// </summary>
        internal readonly SizeContext HeightContext;

        /// <summary>
        /// The x position of this element.
        /// Used by some layouts; ignored by others.
        /// </summary>
        public Size X
        {
            get => XContext.Get();
            set => XContext.Set(value);
        }

        /// <summary>
        /// The y position of this element.
        /// Used by some layouts; ignored by others.
        /// </summary>
        public Size Y
        {
            get => YContext.Get();
            set => YContext.Set(value);
        }

        /// <summary>
        /// The width of this element.
        /// Used by some layouts; ignored by others.
        /// </summary>
        public Size Width
        {
            get => WidthContext.Get();
            set => WidthContext.Set(value);
        }

        /// <summary>
        /// The height of this element.
        /// Used by some layouts; ignored by others.
        /// </summary>
        public Size Height
        {
            get => HeightContext.Get();
            set => HeightContext.Set(value);
        }

        /// <summary>
        /// The size of this element in both dimensions - useful for square elements.
        /// </summary>
        public Size Size
        {
            set
            {
                Width = value;
                Height = value;
            }
        }

        /// <summary>
        /// The margin applied to this element.
        /// </summary>
        public readonly DirectionalSizeValues Margin;

        /// <summary>
        /// Create a new element.
        /// </summary>
        /// <param name="parent">The parent element of this element.</param>
        protected Element(Element parent) : this(parent, null)
        {
        }

        /// <summary>
        /// Create a new top level element.
        /// </summary>
        /// <param name="parentId">The id of the parent element of this element.</param>
        internal Element(string parentId) : this(null, parentId)
        {
        }

        /// <summary>
        /// Create a new element.
        /// </summary>
        protected Element(Element? parent, string? layer)
        {
            Debug.Assert(parent == null && layer != null || parent != null && layer == null,
                "Must provide either parent or layer, but not both.");
            this.parent = parent;
            parentLayer = layer;
            children = new List<Element>();
            XContext = new SizeContext("X", this, Size.Auto);
            YContext = new SizeContext("Y", this, Size.Auto);
            WidthContext = new SizeContext("Width", this, Size.Auto);
            HeightContext = new SizeContext("Height", this, Size.Auto);
            Margin = new DirectionalSizeValues("Margin", this, Size.Zero);

            parent?.children.Add(this);
        }

        /// <summary>
        /// Does this element have a layout assigned to it?<br/>
        /// <br/>
        /// Note: A layout will be automatically assigned to an element when the layout is attempted to be used.
        /// </summary>
        public bool HasLayout() => layout != null;

        /// <summary>
        /// Open this element for the given player.
        /// </summary>
        /// <param name="player">The player to open this element for.</param>
        /// <returns>The state this element is in for the given player.</returns>
        public virtual ElementState Open(BasePlayer player)
        {
            stateByPlayer.TryGetValueAndMarkStrong(player.userID, out var state);
            state = EnsureState(player, state);
            state.Open();
            return state;
        }

        /// <summary>
        /// Close this element for the given player.
        /// </summary>
        /// <param name="player">The player to close this element for.</param>
        /// <returns>The state this element is in for the given player if one exists, otherwise null.</returns>
        public virtual ElementState? Close(BasePlayer player)
        {
            if (stateByPlayer.TryGetValueAndMarkWeak(player.userID, out var state))
                state.Close();
            return state;
        }

        /// <summary>
        /// Get the position of this element.
        /// </summary>
        public (Size X, Size Y) GetPosition()
        {
            return (X, Y);
        }

        /// <summary>
        /// Get the position of this element for the given axis.
        /// </summary>
        /// <param name="axis">The axis to get the position for.</param>
        /// <exception cref="ArgumentOutOfRangeException">When the given axis is not X or Y.</exception>
        public Size GetPosition(Axis axis)
        {
            return axis switch
            {
                Axis.X => X,
                Axis.Y => Y,
                _ => throw new ArgumentOutOfRangeException(nameof(axis)),
            };
        }

        /// <summary>
        /// Get the size of this element.
        /// </summary>
        public (Size Width, Size Height) GetSize()
        {
            return (Width, Height);
        }

        /// <summary>
        /// Get the size of this element for the given axis.
        /// </summary>
        /// <param name="axis">The axis to get the size for.</param>
        public Size GetSize(Axis axis)
        {
            return axis switch
            {
                Axis.X => Width,
                Axis.Y => Height,
                _ => throw new ArgumentOutOfRangeException(nameof(axis)),
            };
        }

        /// <summary>
        /// Get the position context of this element.
        /// </summary>
        internal (SizeContext X, SizeContext Y) GetPositionContext()
        {
            return (XContext, YContext);
        }

        /// <summary>
        /// Get the position context of this element for the given axis.
        /// </summary>
        /// <param name="axis">The axis to get the position for.</param>
        /// <exception cref="ArgumentOutOfRangeException">When the given axis is not X or Y.</exception>
        internal SizeContext GetPositionContext(Axis axis)
        {
            return axis switch
            {
                Axis.X => XContext,
                Axis.Y => YContext,
                _ => throw new ArgumentOutOfRangeException(nameof(axis)),
            };
        }

        /// <summary>
        /// Get the size context of this element.
        /// </summary>
        internal (SizeContext Width, SizeContext Height) GetSizeContext()
        {
            return (WidthContext, HeightContext);
        }

        /// <summary>
        /// Get the size context of this element for the given axis.
        /// </summary>
        /// <param name="axis">The axis to get the size for.</param>
        internal SizeContext GetSizeContext(Axis axis)
        {
            return axis switch
            {
                Axis.X => WidthContext,
                Axis.Y => HeightContext,
                _ => throw new ArgumentOutOfRangeException(nameof(axis)),
            };
        }

        /// <summary>
        /// Get the top most element in the hierarchy.
        /// </summary>
        public RootElement GetRoot()
        {
            var root = this;
            while (root.parent != null)
                root = root.parent;
            Debug.Assert(root is RootElement);
            return (RootElement)root;
        }

        /// <summary>
        /// Get the parent of this element.
        /// </summary>
        /// <returns>The parent of this element or null if this element is a top level element.</returns>
        public Element? GetParent()
        {
            return parent;
        }

        /// <summary>
        /// Get the name of the parent CUI root element for the given player.
        /// </summary>
        /// <param name="player">The player to get the name for.</param>
        internal string GetParentCuiRootName(BasePlayer player)
        {
            var parentName = parent == null
                ? parentLayer
                : parent.GetState(player).GetCuiRootName();

            Debug.Assert(!string.IsNullOrEmpty(parentName));

            return parentName;
        }

        /// <summary>
        /// Get the name of the parent CUI content element for the given player.
        /// </summary>
        /// <param name="player">The player to get the name for.</param>
        internal string GetParentCuiContentName(BasePlayer player)
        {
            var parentName = parent == null
                ? parentLayer
                : parent.GetState(player).GetCuiContentName();

            Debug.Assert(!string.IsNullOrEmpty(parentName));

            return parentName;
        }

        /// <summary>
        /// Get the children of this element.
        /// </summary>
        public IEnumerable<Element> GetChildren()
        {
            children.Sort((a, b) => a.Weight.CompareTo(b.Weight));
            return children;
        }

        /// <summary>
        /// Get the state of this element for the given player.
        /// </summary>
        /// <param name="player">The player to get the state for.</param>
        /// <returns>The existing state of this element for the given player if there is one, otherwise a newly created state.</returns>
        public ElementState GetState(BasePlayer player)
        {
            stateByPlayer.TryGetValue(player.userID, out var state);
            return EnsureState(player, state);
        }

        /// <summary>
        /// Ensure the state exists for the given player.
        /// </summary>
        /// <param name="player">The player to get the state for.</param>
        /// <param name="state">If not null, the state to use. If null, a new state will be created.</param>
        /// <returns>The existing state of this element for the given player if there is one, otherwise a newly created state.</returns>
        private ElementState EnsureState(BasePlayer player, ElementState? state)
        {
            if (state != null)
                return state;

            state = InitialState(player);
            stateByPlayer.Add(player.userID, state);
            return state;
        }

        /// <summary>
        /// Create the initial state for the given player for this element.
        /// </summary>
        /// <param name="player">The player to create the state for.</param>
        /// <returns>A new state for this element for the given player.</returns>
        protected abstract ElementState InitialState(BasePlayer player);
    }
}
