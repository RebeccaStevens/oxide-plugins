#define DEBUG

using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Core.Libraries.Covalence;
using Oxide.Game.Rust.Cui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Oxide.Plugins;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
[Info("UI Builder Library", "BlueBeka", "0.0.7")]
[Description("Allows for easily creating complex UIs.")]
public class UiBuilderLibrary : RustPlugin
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
{

#nullable disable
    private static UiBuilderLibrary self;

    // Initialized in `Init()`
    private MainConfig config;
    private MainData data;
#nullable restore

    /// <summary>
    /// Initialize the UI Builder
    /// </summary>
    public UiBuilderLibrary()
    {
        Debug.Assert(self == null);
        self = this;
    }

    #region Hooks
#pragma warning disable IDE0051 // Don't report hooks as unused.

    private void Init()
    {
        config = new MainConfig();
        data = new MainData();
    }

    private void Loaded()
    {
        config.Load();
        data.Load();
    }

    private void OnServerSave()
    {
        data.Save();
    }

    private void Unload()
    {
        data.Save();
        UI.CloseEverything();
    }

    private void OnUserDisconnected(IPlayer iPlayer)
    {
        var player = (BasePlayer)iPlayer.Object;
        UI.CloseEverything(player);
    }

#pragma warning restore IDE0051
    #endregion

    #region API Enums

    /// <summary>
    /// Where to anchor an element.
    /// </summary>
    public enum PositionAnchor
    {
        UpperLeft,
        UpperCenter,
        UpperRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        LowerLeft,
        LowerCenter,
        LowerRight
    }

    /// <summary>
    /// A font that can be using to display text.
    /// </summary>
    public enum Font
    {
        Regular,
        Bold,
        Mono,
        Marker
    }

    #endregion

    #region API Abstract Classes

    /// <summary>
    /// The base class of all UIs that can be displayed to users.
    /// </summary>
    public abstract class UI
    {
        private static readonly ICollection<UI> allUIs = new List<UI>();

        /// <summary>
        /// The root element that all other elements are children of.
        /// </summary>
        protected readonly RootElement root;

        /// <summary>
        /// Create a new UI.
        /// </summary>
        /// <param name="parentId">The parent of this UI. For top-level UIs, one of: "Overlay", "Hud.Menu", "Hud" or "Under"</param>
        public UI(string parentId)
        {
            root = new RootElement(parentId);
            allUIs.Add(this);
        }

        /// <summary>
        /// Open this UI for the given player.
        /// </summary>
        /// <param name="player"></param>
        public void Open(BasePlayer player)
        {
            var rootState = root.Open(player);
            var dirtyStates = rootState.GetDirtyLineage();
            var rootIds = GetRootElementIds(dirtyStates);
            var dataComponents = dirtyStates.Select(state => ElementStateToJson(state, rootIds));
            if (dataComponents.Count() == 0)
            {
                Interface.Oxide.LogDebug("Cannot open UI: nothing to display (or remove).");
                return;
            }

            var data = $"[{string.Join(",", dataComponents)}]";
            CuiHelper.AddUi(player, data);

            foreach (var state in dirtyStates)
                state.MarkClean();
        }

        /// <summary>
        /// Close this UI for the given player.
        /// </summary>
        /// <param name="player"></param>
        public void Close(BasePlayer player)
        {
            var state = root.Close(player);

            if (state == null || !player.Connection.active)
                return;

            CuiHelper.DestroyUi(player, state.cuiGuid);
        }

        /// <summary>
        /// Close the UI for all players.
        /// </summary>
        public void CloseAll()
        {
            foreach (var player in BasePlayer.allPlayerList)
                Close(player);
        }

        /// <summary>
        /// Close all UI for all the players.
        /// </summary>
        internal static void CloseEverything()
        {
            foreach (var player in BasePlayer.allPlayerList)
                CloseEverything(player);
        }

        /// <summary>
        /// Close all UI for the given player.
        /// </summary>
        internal static void CloseEverything(BasePlayer player)
        {
            foreach (var ui in allUIs)
            {
                var state = ui.root.Close(player);
                if (state != null && player.Connection.active)
                    CuiHelper.DestroyUi(player, state.cuiGuid);
            }
        }

        /// <summary>
        /// Get a JSON representation of the given element state.
        /// </summary>
        /// <param name="state">What to encode.</param>
        /// <param name="rootIds">List of the root element ids.</param>
        /// <returns></returns>
        private static string ElementStateToJson(Element.State state, IEnumerable<string> rootIds)
        {
            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            var layout = state.element.parent?.Layout ?? new AbsoluteLayout();
            var cuiElement = state.GetCuiElement(layout);

            if (state.IsOpen)
            {
                var json = JsonConvert.SerializeObject(cuiElement, Formatting.None, settings).Replace("\\n", "\n");
                if (rootIds.Contains(cuiElement.Name))
                    return $"{{\"destroyUi\":\"{cuiElement.Name}\",{json.Substring(1)}";
                return json;
            }

            return $"{{\"destroyUi\":\"{cuiElement.Name}\",\"name\":\"{cuiElement.Name}\"}}";
        }

        /// <summary>
        /// Get a collection of the root element ids from the given collection of elements.
        /// </summary>
        /// <param name="states"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetRootElementIds(IEnumerable<Element.State> states)
        {
            var (allGuids, parentGuids) = Transpose(states.Select((state) => (state.cuiGuid, state.GetParentCuiGuid())));
            return allGuids.Except(parentGuids);
        }
    }

    /// <summary>
    /// The base class of all elements.
    /// </summary>
    public abstract class Element
    {
        private readonly WeakableValueDictionary<ulong, State> stateByPlayer = new WeakableValueDictionary<ulong, State>();

        // `Parent` and `parentCuiGuid` should not both be set.
        private readonly string? parentCuiGuid;
        internal readonly Element? parent;

        private readonly List<Element> children;

        /// <summary>
        /// How to layout this element's children.
        /// </summary>
        public IElementLayout? Layout;

        /// <summary>
        /// The anchor point of this element.
        /// Used by some layouts; ignored by others.
        /// </summary>
        public PositionAnchor Anchor { get; set; } = PositionAnchor.UpperLeft;

        private double? _x;
        private double? _y;
        private double? _width;
        private double? _height;

        /// <summary>
        /// The x position of this element as percentage in decimal form between 0 and 1.
        /// Used by some layouts; ignored by others.
        /// </summary>
        public double? X
        {
            set
            {
                if (value != null)
                    EnsureValidPercentageDecimal(value.Value);
                _x = value;
            }
        }

        /// <summary>
        /// The x position of this element as percentage in decimal form between 0 and 1.
        /// Used by some layouts; ignored by others.
        /// </summary>
        public double? Y
        {
            set
            {
                if (value != null)
                    EnsureValidPercentageDecimal(value.Value);
                _y = value;
            }
        }

        /// <summary>
        /// The width of this element as percentage in decimal form between 0 and 1.
        /// Used by some layouts; ignored by others.
        /// </summary>
        public double? Width
        {
            set
            {
                if (value != null)
                    EnsureValidPercentageDecimal(value.Value);
                _width = value;
            }
        }

        /// <summary>
        /// The height of this element as percentage in decimal form between 0 and 1.
        /// Used by some layouts; ignored by others.
        /// </summary>
        public double? Height
        {
            set
            {
                if (value != null)
                    EnsureValidPercentageDecimal(value.Value);
                _height = value;
            }
        }

        public double GetX(double defaultValue = 0d) => _x == null ? defaultValue : _x.Value;
        public bool IsXSet() => _x != null;

        public double GetY(double defaultValue = 0d) => _y == null ? defaultValue : _y.Value;
        public bool IsYSet() => _y != null;

        public double GetWidth(double defaultValue = 1d) => _width == null ? defaultValue : _width.Value;
        public bool IsWidthSet() => _width != null;

        public double GetHeight(double defaultValue = 1d) => _height == null ? defaultValue : _height.Value;
        public bool IsHeightSet() => _height != null;


        public Element(Element parent)
        {
            this.parent = parent;
            parent.children.Add(this);
            parentCuiGuid = null;
            children = new List<Element>();
        }

        internal Element(string parentId)
        {
            parent = null;
            parentCuiGuid = parentId;
            children = new List<Element>();
        }

        /// <summary>
        /// Open this element for the given player.
        /// </summary>
        /// <param name="player"></param>
        /// <returns>The new state of the element</returns>
        public State Open(BasePlayer player)
        {
            stateByPlayer.TryGetValueAndMarkStrong(player.userID, out State? state);
            state = EnsureState(player, state);
            state.Open();
            return state;
        }

        /// <summary>
        /// Close this element for the given player.
        /// </summary>
        /// <param name="player"></param>
        /// <returns>The new state of the element</returns>
        public State? Close(BasePlayer player)
        {
            if (stateByPlayer.TryGetValueAndMarkWeak(player.userID, out var state))
                state.Close();
            return state;
        }

        public IEnumerable<Element> GetChildren()
        {
            return children;
        }

        /// <summary>
        /// Get the state of this element for the given player.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public State GetState(BasePlayer player)
        {
            stateByPlayer.TryGetValue(player.userID, out State? state);
            return EnsureState(player, state);
        }

        /// <summary>
        /// Ensure the state exists.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private State EnsureState(BasePlayer player, State? state)
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
        /// <param name="player"></param>
        /// <returns></returns>
        protected abstract State InitialState(BasePlayer player);

        /// <summary>
        /// Add a child to this element.
        /// </summary>
        /// <param name="element">The element to add as a child.</param>
        /// <returns>The added element</returns>
        /// <exception cref="ArgumentNullException"></exception>
        // protected Element AddChild(Element element)
        // {
        //     if (element == null)
        //         throw new ArgumentNullException("Cannot add null as a child.");

        //     children.Add(element);
        //     return element;
        // }

        /// <summary>
        /// An instance of state of the element for an indiviual player.
        /// </summary>
        public abstract class State
        {
            internal readonly string cuiGuid;

            /// <summary>
            /// The element this state is for.
            /// </summary>
            internal readonly Element element;

            /// <summary>
            /// The player this state is for.
            /// </summary>
            public readonly BasePlayer Player;

            /// <summary>
            /// True if updates have not been sent to the player.
            /// </summary>
            protected bool IsDirty { get; private set; }

            /// <summary>
            /// States whether the element is being displayed to the player.
            /// </summary>
            public bool IsOpen { get; set; }

            /// <summary>
            /// Create the state for the element for the player.
            /// </summary>
            /// <param name="element"></param>
            /// <param name="player"></param>
            public State(Element element, BasePlayer player)
            {
                this.element = element;
                cuiGuid = CuiHelper.GetGuid();
                Player = player;
                IsDirty = true;
                IsOpen = false;
            }

            /// <summary>
            /// Get the cui guid of this element's state's parent's state.
            /// </summary>
            /// <returns></returns>
            public string GetParentCuiGuid()
            {
                var parentGuid = element.parent == null
                    ? element.parentCuiGuid
                    : element.parent.GetState(Player).cuiGuid;

                Debug.Assert(parentGuid != null);

                return parentGuid!;
            }

            internal IEnumerable<State> GetDirtyLineage()
            {
                var list = new List<State>();

                var stack = new Stack<State>();
                stack.Push(this);

                var dirtyAncestry = IsDirty;
                while (stack.TryPop(out var state))
                {
                    // Once we are dirty, all children are concidered dirty too.
                    dirtyAncestry = dirtyAncestry || state.IsDirty;
                    if (dirtyAncestry)
                        list.Add(state);

                    foreach (var childState in state.element.children.Select((child) => child.GetState(Player)).Reverse())
                        stack.Push(childState);
                }

                return list;
            }

            /// <summary>
            /// Open this element (and its children).
            /// </summary>
            public void Open()
            {
                // if (IsDirty)
                //     Rerender();
                foreach (var child in element.children)
                    child.Open(Player);
                IsOpen = true;
            }

            /// <summary>
            /// Close this element (and its children).
            /// </summary>
            public void Close()
            {
                IsOpen = false;
                foreach (var child in element.children)
                    child.Close(Player);
            }

            /// <summary>
            /// Mark that something has changed in the state and that a rerender is required for this element.
            /// </summary>
            protected void MarkDirty()
            {
                IsDirty = true;
            }

            /// <summary>
            /// Mark that the dirty state has been handle and that it is now clean.
            /// </summary>
            internal void MarkClean()
            {
                IsDirty = false;
            }

            /// <summary>
            /// Get the Cui Element for this element in it's current state.
            /// </summary>
            public abstract CuiElement GetCuiElement(IElementLayout layout);
        }

#if DEBUG
        internal bool IsChild(Element child)
        {
            return children.Contains(child);
        }
#endif
    }

    #endregion

    #region API UIs

    /// <summary>
    /// A UI window that includes a close button that can be displayed to users.
    /// </summary>
    public class UIWindow : UI
    {
        PanelElement main;
        PanelElement titleBar;

        public UIWindow(Action<PanelElement> renderer) : base("Overlay")
        {
            root.BgColor = Color.yellow;

            root.Layout = new FlexLayout(root)
            {
                Direction = FlexLayout.FlexDirection.Vertical,
                AlignItems = FlexLayout.ItemAlignment.Stretch,
                Gap = 0.01,
            };

            titleBar = new PanelElement(root)
            {
                BgColor = Color.cyan,
                Height = 0.05,
            };

            main = new PanelElement(root)
            {
                BgColor = Color.blue,
            };
        }
    }

    #endregion

    #region API Elements

    public class RootElement : PanelElement
    {
        internal RootElement(string parentId) : base(parentId)
        {
        }

        /// <inheritdoc/>
        protected override State InitialState(BasePlayer player)
        {
            return new RootState(this, player);
        }

        /// <inheritdoc/>
        public class RootState : PanelState
        {
            private readonly RootElement element;

            /// <inheritdoc/>
            public RootState(RootElement element, BasePlayer player) : base(element, player)
            {
                this.element = element;
            }

            /// <inheritdoc/>
            public override CuiElement GetCuiElement(IElementLayout layout)
            {
                CuiElement cui = base.GetCuiElement(layout);

                cui.Components.Add(new CuiNeedsCursorComponent());
                cui.Components.Add(new CuiNeedsKeyboardComponent());

                return cui;
            }
        }
    }

    public class PanelElement : Element
    {
        /// <summary>
        /// The background color of the element.
        /// </summary>
        public Color BgColor = Color.black;

        public PanelElement(Element parent) : base(parent)
        {
        }

        internal PanelElement(string parentId) : base(parentId)
        {
        }

        /// <inheritdoc/>
        protected override State InitialState(BasePlayer player)
        {
            return new PanelState(this, player);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return base.ToString() + " - " + BgColor.ToString();
        }

        /// <inheritdoc/>
        public class PanelState : State
        {
            private readonly PanelElement element;

            /// <inheritdoc/>
            public PanelState(PanelElement element, BasePlayer player) : base(element, player)
            {
                this.element = element;
            }

            /// <inheritdoc/>
            public override CuiElement GetCuiElement(IElementLayout layout)
            {
                CuiElement cui = new()
                {
                    Name = cuiGuid,
                    Parent = GetParentCuiGuid(),
                };

                // Color
                cui.Components.Add(new CuiImageComponent()
                {
                    Color = ColorToCuiColor(element.BgColor)
                });

                // Bounds
                cui.Components.Add(layout.GetBounds(element));

                return cui;
            }
        }
    }

    #endregion

    #region API Layouts

    public interface IElementLayout
    {
        public CuiRectTransformComponent GetBounds(Element child);
    }

    public class AbsoluteLayout : IElementLayout
    {
        /// <inheritdoc/>
        public CuiRectTransformComponent GetBounds(Element child)
        {
            var (xFactor, yFactor) = PositionAnchorToComponents(child.Anchor);

            var x = child.GetX();
            var y = child.GetY();
            var width = child.GetWidth();
            var height = child.GetHeight();

            double anchorMinX = x - width * xFactor;
            double anchorMaxX = x + width * (1 - xFactor);
            double anchorMinY = y - height * yFactor;
            double anchorMaxY = y + height * (1 - yFactor);

            return new CuiRectTransformComponent()
            {
                AnchorMin = $"{anchorMinX} {1d - anchorMaxY}",
                AnchorMax = $"{anchorMaxX} {1d - anchorMinY}",
            };
        }
    }

    public class FlexLayout : IElementLayout
    {
        protected readonly Element element;

        private IDictionary<Element, CuiRectTransformComponent>? layoutData;

        public FlexDirection Direction = FlexDirection.Horizontal;

        public ItemAlignment AlignItems = ItemAlignment.Center;

        private double _gapSize = 0d;

        /// <summary>
        /// The spacing between element as percentage in decimal form between 0 and 1.
        /// </summary>
        public double Gap
        {
            get
            {
                return _gapSize;
            }
            set
            {
                EnsureValidPercentageDecimal(value);
                _gapSize = value;
            }
        }

        public FlexLayout(Element element)
        {
            this.element = element;
        }

        private void ComputeLayout()
        {
            IEnumerable<Element> children = element.GetChildren().ToArray();

            int numberOfChildren = 0;
            double sizeOfAllChildren = 0;
            int numberOfImplicitlySizedChildren = 0;

            foreach (var child in children)
            {
                numberOfChildren += 1;

                var size = GetSize(child).Major;
                if (size == null)
                    numberOfImplicitlySizedChildren += 1;
                else
                    sizeOfAllChildren += size.Value;
            }

            Dictionary<Element, CuiRectTransformComponent> updateLayoutData = new();
            if (numberOfChildren == 0)
            {
                layoutData = updateLayoutData;
                return;
            }

            int numberOfGaps = numberOfChildren - 1;
            double sizeOfAllGaps = Gap * numberOfGaps;
            double totalSize = sizeOfAllChildren + sizeOfAllGaps;
            double flexMajorOffset = 0;

            if (Direction == FlexDirection.HorizontalReversed || Direction == FlexDirection.VerticalReversed)
                children = children.Reverse();

            var implicitSizeOfChildren = numberOfImplicitlySizedChildren == 0 || totalSize >= 1
                ? 0
                : (1 - totalSize) / numberOfImplicitlySizedChildren;
            totalSize += implicitSizeOfChildren * numberOfImplicitlySizedChildren;

            foreach (var child in children)
            {
                var (xFactor, yFactor) = PositionAnchorToComponents(child.Anchor);
                var (majorSize, minorSize) = GetSize(child);

                double flexMajorSize = majorSize == null
                    ? implicitSizeOfChildren
                    : majorSize.Value / totalSize;
                double gapSize = Gap / totalSize;

                (double flexMinorSize, double flexMinorOffset) = AlignItems switch
                {
                    ItemAlignment.Normal => Direction switch
                    {
                        // TODO: Respect PositionAnchor
                        FlexDirection.Horizontal or FlexDirection.HorizontalReversed => (minorSize, child.GetY()),
                        FlexDirection.Vertical or FlexDirection.VerticalReversed => (minorSize, child.GetY()),
                        _ => throw new ArgumentOutOfRangeException(),
                    },
                    ItemAlignment.Start => (minorSize, 0.0),
                    ItemAlignment.Center => (minorSize, (1 - minorSize) * 0.5),
                    ItemAlignment.End => (minorSize, 1 - minorSize),
                    ItemAlignment.Stretch => (1.0, 0.0),
                    _ => throw new ArgumentOutOfRangeException(),
                };

                double minMajor = flexMajorOffset;
                double maxMajor = flexMajorOffset + flexMajorSize;
                double minMinor = flexMinorOffset;
                double maxMinor = flexMinorOffset + flexMinorSize;

                if (layoutData?.TryGetValue(child, out CuiRectTransformComponent? component) != true)
                    component = new CuiRectTransformComponent();

                if (Direction == FlexDirection.Horizontal || Direction == FlexDirection.HorizontalReversed)
                {
                    component.AnchorMin = $"{minMajor} {1d - maxMinor}";
                    component.AnchorMax = $"{maxMajor} {1d - minMinor}";
                }
                else
                {
                    component.AnchorMin = $"{minMinor} {1d - maxMajor}";
                    component.AnchorMax = $"{maxMinor} {1d - minMajor}";
                }

                flexMajorOffset += flexMajorSize + gapSize;
                updateLayoutData.Add(child, component);
            }

            layoutData = updateLayoutData;
        }

        private (double? Major, double Minor) GetSize(Element element)
        {
            return Direction switch
            {
                FlexDirection.Horizontal or FlexDirection.HorizontalReversed => (element.IsWidthSet() ? element.GetWidth() : null, element.GetHeight()),
                FlexDirection.Vertical or FlexDirection.VerticalReversed => (element.IsHeightSet() ? element.GetHeight() : null, element.GetWidth()),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        /// <inheritdoc/>
        public CuiRectTransformComponent GetBounds(Element child)
        {
#if DEBUG
            if (!element.IsChild(child))
                throw new Exception("given element is not a child of the layout's element");
#endif
            ComputeLayout();

            Debug.Assert(layoutData != null);
            layoutData!.TryGetValue(child, out var bounds);
            Debug.Assert(bounds != null);
            return bounds!;
        }

        public enum FlexDirection
        {
            Horizontal,
            Vertical,
            HorizontalReversed,
            VerticalReversed,
        }

        public enum ItemAlignment
        {
            Normal,
            Stretch,
            Start,
            Center,
            End,
        }
    }

    #endregion

    #region Abstract

    private abstract class PluginConfig<T>
    {
        private DynamicConfigFile file;

        public T? data { get; private set; }

        public PluginConfig(string? filename)
        {
            file = new DynamicConfigFile(GetFilename(filename));
        }

        public void Save()
        {
            if (data == null)
                Load();
            else
                file.WriteObject(data, false);
        }

        public void Load()
        {
            data = file.ReadObject<T>();
            if (data == null)
            {
                data = InitializeData();
                Save();
            }
        }

        /// <summary>
        /// Get the filename path for the config file.
        /// </summary>
        private static string GetFilename(string? shortFilename)
        {
            return string.IsNullOrEmpty(shortFilename)
                ? Path.Combine(self.Manager.ConfigPath, self.Name + ".json") // $"oxide/config/{self.Name}.json"
                : Path.Combine(self.Manager.ConfigPath, self.Name, shortFilename + ".json"); // $"oxide/config/{self.Name}/{file}.json"
        }

        protected abstract T InitializeData();
    }

    private abstract class PluginData<T>
    {
        private readonly string _filename;
        public T? data { get; protected set; }

        public PluginData(string? filename)
        {
            _filename = string.IsNullOrEmpty(filename) ? self.Name : $"{self.Name}/{filename}";
        }

        public void Save()
        {
            if (data == null)
                Load();
            else
                Interface.Oxide.DataFileSystem.WriteObject(_filename, data);
        }

        public void Load()
        {
            data = Interface.Oxide.DataFileSystem.ReadObject<T>(_filename);
            if (data == null)
            {
                data = InitializeData();
                Save();
            }
        }

        protected abstract T InitializeData();
    }

    #endregion

    #region Data

    private class MainConfig : PluginConfig<MainConfig.Data>
    {
        public MainConfig() : base(null) { }

        protected override Data InitializeData()
        {
            return new Data();
        }

        public class Data
        {
        }
    }

    private class MainData : PluginData<MainData.Data>
    {

        public MainData() : base(null) { }

        protected override Data InitializeData()
        {
            return new Data();
        }

        public class Data
        {
        }
    }

    #endregion

    #region Collections

    /// <summary>
    /// A dictionary in which the values can be marked as weak references.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class WeakableValueDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TValue : class
    {
        private Dictionary<TKey, WeakableReference<TValue>> _dict = new();
        private int version, cleanVersion;
        private int lastGarbageCollectionCount;
        private const int minRehashInterval = 100;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public TValue this[TKey key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException();
#if DEBUG
                if (!_dict[key]?.IsStrong != true)
                    throw new NotSupportedException("Do not access via index when weak.");
#endif
                if (!_dict.ContainsKey(key))
                    throw new KeyNotFoundException();

                if (TryGetValue(key, out var value))
                    return value;

                throw new KeyNotFoundException();
            }
            set
            {
                Add(key, value);
            }
        }

        /// <summary>
        /// Create a new dictionary of values that can be made weak.
        /// </summary>
        public WeakableValueDictionary() { }


        /// <inheritdoc/>
        public void Add(TKey key, TValue value)
        {
            Add(key, value, true);
        }

        /// <inheritdoc/>
        public void Add(TKey key, TValue value, bool strong)
        {
            if (key == null)
                throw new ArgumentNullException("Cannot use null as a key of a WeakableValueDictionary.");
            if (value == null)
                throw new ArgumentNullException("Cannot add null value to a WeakableValueDictionary.");

            if (_dict.TryGetValue(key, out var reference))
            {
                if (reference.TryGetTarget(out _))
                    throw new ArgumentException("An element with the same key already exists in this dictionary.");

                reference.SetTarget(value, strong);
                AutoCleanup();
                return;
            }

            _dict.Add(key, new WeakableReference<TValue>(value, strong));
            AutoCleanup();
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            if (_dict.TryGetValue(key, out var reference))
                return reference.TryGetTarget(out _);
            else
                AutoCleanup();

            return false;
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            if (_dict.TryGetValue(key, out var reference))
                return _dict.Remove(key) && reference.TryGetTarget(out _);
            else
                AutoCleanup();

            return false;
        }

        /// <inheritdoc/>
#pragma warning disable CS8767
        public bool TryGetValue(TKey key, /*[NotNullWhen(true)]*/ out TValue? value)
#pragma warning restore CS8767
        {
            if (_dict.TryGetValue(key, out var reference))
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
        public bool TryGetValueAndMarkStrong(TKey key, /*[NotNullWhen(true)]*/ out TValue? value)
        {
            if (_dict.TryGetValue(key, out var reference))
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
        public bool TryGetValueAndMarkWeak(TKey key, /*[NotNullWhen(true)]*/ out TValue? value)
        {
            if (_dict.TryGetValue(key, out var reference))
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
            if (_dict.TryGetValue(key, out var reference))
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
            _dict.Clear();
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
            var toCopy = _dict
                .Select<KeyValuePair<TKey, WeakableReference<TValue>>, KeyValuePair<TKey, TValue>?>((kvp) =>
                {
                    if (kvp.Value.TryGetTarget(out var value))
                        return new KeyValuePair<TKey, TValue>(kvp.Key, value);
                    return null;
                })
                .ToArray()
                .Where((kvp) => kvp == null)
                .ToArray();

            AutoCleanup(_dict.Count - toCopy.Length);
            toCopy.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (TryGetValue(item.Key, out var value))
                if (value == item.Value)
                    return Remove(item.Key);
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                int count = 0;
                int nullCount = 0;

                foreach (var kvp in _dict)
                {
                    if (kvp.Value.TryGetTarget(out TValue target))
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
                List<TKey> keys = new();
                int nullCount = 0;

                foreach (var kvp in _dict)
                {
                    if (kvp.Value.TryGetTarget(out TValue target))
                        keys.Add(kvp.Key);
                    else
                        nullCount++;
                }

                AutoCleanup(nullCount);
                return keys;
            }
        }

        /// <inheritdoc/>
        public ICollection<TValue> Values
        {
            get
            {
                List<TValue> values = new();
                int nullCount = 0;

                foreach (var kvp in _dict)
                {
                    if (kvp.Value.TryGetTarget(out TValue target))
                        values.Add(target);
                    else
                        nullCount++;
                }

                AutoCleanup(nullCount);
                return values;
            }
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            int nullCount = 0;

            foreach (var kvp in _dict)
            {
                if (kvp.Value.TryGetTarget(out TValue target))
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
            if (delta > minRehashInterval + _dict.Count)
            {
                // A cleanup will be fruitless unless a GC has happened.
                // WeakReferences can become dead only during the GC.
                int currentCollectionCount = GC.CollectionCount(0);
                if (lastGarbageCollectionCount != currentCollectionCount)
                {
                    Cleanup();
                    lastGarbageCollectionCount = currentCollectionCount;
                    cleanVersion = version;
                }
                else
                    cleanVersion += minRehashInterval; // Wait a little while longer
            }
        }

        /// <summary>
        /// Clean up the internal dictionary by removing all pairs whose value has been garbage collected.
        /// </summary>
        private void Cleanup()
        {
            List<TKey> deadKeys = new List<TKey>();

            foreach (var kvp in _dict)
                if (!kvp.Value.TryGetTarget(out _))
                    deadKeys.Add(kvp.Key);

            foreach (TKey key in deadKeys)
                _dict.Remove(key);
        }
    }

    #endregion Collections

    #region References

    /// <summary>
    /// A reference to an object of type `T` that can be toggled between being either a strong or a weak reference.
    /// 
    /// A weak reference can be garbage collected and thus may become unavaliable.
    /// </summary>
    public class WeakableReference<T> where T : class
    {
        private T? strong;
        private readonly WeakReference<T> weak;

        /// <summary>
        /// Create a new WeakableReference
        /// </summary>
        /// <param name="target">The target to keep a reference to</param>
        /// <param name="initiallyStrong">whether the reference should start out as a strong reference</param>
        public WeakableReference(T target, bool initiallyStrong = true)
        {
            if (initiallyStrong)
                strong = target;
            weak = new WeakReference<T>(target);
        }

        /// <summary>
        /// Is the reference currently a strong reference.
        /// </summary>
        public bool IsStrong { get { return strong != null; } }

        /// <summary>
        /// Set the target to a new value.
        /// </summary>
        /// <param name="target">the new target</param>
        /// <param name="isStrong">whether the new reference should be a strong reference</param>
        public void SetTarget(T target, bool isStrong = true)
        {
            weak.SetTarget(target);
            strong = isStrong ? target : null;
        }

        /// <summary>
        /// Try and access the target.
        /// </summary>
        /// <param name="target">the target</param>
        /// <returns>true if the target could be accessed, otherwise false</returns>
        public bool TryGetTarget(/*[NotNullWhen(true)]*/ out T target)
        {
            if (strong != null)
            {
                target = strong;
                return true;
            }
            return weak.TryGetTarget(out target);
        }

        /// <summary>
        /// Try and access the target and mark it as a strong reference.
        /// </summary>
        /// <param name="target">the target</param>
        /// <returns>true if the target still exists an could be made strong, otherwise false implying that the target has already been garbage collected.</returns>
        public bool TryGetTargetAndMarkStrong(/*[NotNullWhen(true)]*/ out T target)
        {
            var result = TryGetTarget(out target);
            if (result)
                strong = target;
            return result;
        }

        /// <summary>
        /// Mark the target as a weak reference.
        /// </summary>
        public void MarkWeak()
        {
            strong = null;
        }
    }

    #endregion References

    #region Util Functions

    /// <summary>
    /// Transpose an enumable of 2-tuples into a 2-tuple of enumables.
    /// </summary>
    public static (IEnumerable<T1>, IEnumerable<T2>) Transpose<T1, T2>(IEnumerable<(T1, T2)> source)
    {
        int rows = source.Count();
        var result = (new T1[rows], new T2[rows]);

        var index = 0;
        foreach (var item in source)
        {
            result.Item1[index] = item.Item1;
            result.Item2[index] = item.Item2;
            index += 1;
        }

        return result;
    }

    public static string ColorToCuiColor(Color color)
    {
        return $"{color.r} {color.g} {color.b} {color.a}";
    }

    private static string GetFontFile(Font font)
    {
        return font switch
        {
            Font.Regular => "robotocondensed-regular.ttf",
            Font.Bold => "robotocondensed-bold.ttf",
            Font.Mono => "droidsansmono.ttf",
            Font.Marker => "permanentmarker.ttf",
            _ => throw new ArgumentOutOfRangeException(nameof(font)),
        };
    }

    private static void EnsureValidPercentageDecimal(double value, double min = 0, double max = 1)
    {
        Debug.Assert(value >= min && value <= max, $"Value must be a percentage in decimal form between {min} and {max}.");
    }

    public static (double X, double Y) PositionAnchorToComponents(PositionAnchor anchor)
    {
        return anchor switch
        {
            PositionAnchor.UpperLeft => (0.0, 0.0),
            PositionAnchor.UpperCenter => (0.5, 0.0),
            PositionAnchor.UpperRight => (1.0, 0.0),
            PositionAnchor.MiddleLeft => (0.0, 0.5),
            PositionAnchor.MiddleCenter => (0.5, 0.5),
            PositionAnchor.MiddleRight => (1.0, 0.5),
            PositionAnchor.LowerLeft => (0.0, 1.0),
            PositionAnchor.LowerCenter => (0.5, 1.0),
            PositionAnchor.LowerRight => (1.0, 1.0),
            _ => throw new ArgumentOutOfRangeException(nameof(anchor)),
        };
    }

    #endregion
}
