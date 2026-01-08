using System;
using System.Collections.Generic;
using System.Linq;
using Oxide.Core;
using Oxide.Game.Rust.Cui;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// The base class of all UIs that can be displayed to players.
    /// </summary>
    public abstract class Ui
    {
        /// <summary>
        /// All UIs that have been created.
        /// </summary>
        private static readonly List<Ui> AllUIs = new();

        /// <summary>
        /// The root element that all other elements of this UI are children of.
        /// </summary>
        protected readonly RootElement Root;

        /// <summary>
        /// Create a new UI.
        /// </summary>
        /// <param name="layer">The layer of this UI. <a href="https://docs.oxidemod.com/guides/developers/basic-cui#layers">More info.</a></param>
        protected Ui(string layer)
        {
            if (layer == string.Empty)
                throw new ArgumentException("must be non-empty", nameof(layer));

            Root = new RootElement(this, layer);
            AllUIs.Add(this);
        }

        /// <summary>
        /// Open this UI for the given player.
        /// </summary>
        /// <param name="player"></param>
        public virtual void Open(BasePlayer player)
        {
            var rootState = Root.Open(player);
            var resyncStates = rootState.GetStatesNeedingSync(true);

            var (createNewStates, destroyOldStates) =
                TransposeAndFlatten(resyncStates.Select(ElementStateToJson));

            var dataComponents = destroyOldStates.Concat(createNewStates).ToArray();
            if (dataComponents.Length == 0)
            {
                Interface.Oxide.LogDebug("Cannot open UI: nothing to display (or remove).");
                return;
            }

            var data = $"[{string.Join(",", dataComponents)}]";
            CuiHelper.AddUi(player, data);
            builder.Puts(data);
        }

        /// <summary>
        /// Close this UI for the given player.
        /// </summary>
        /// <param name="player"></param>
        public virtual void Close(BasePlayer player)
        {
            var state = Root.Close(player);

            if (state == null || !player.Connection.active)
                return;

            CuiHelper.DestroyUi(player, state.GetCuiRootName());
        }

        /// <summary>
        /// Close this UI for all players.
        /// </summary>
        public void CloseForAll()
        {
            foreach (var player in BasePlayer.allPlayerList)
                Close(player);
        }

        /// <summary>
        /// Close all UI for all the players.
        /// </summary>
        internal static void CloseEverythingForAll()
        {
            foreach (var player in BasePlayer.allPlayerList)
                CloseEverything(player);
        }

        /// <summary>
        /// Close all UI for the given player.
        /// </summary>
        internal static void CloseEverything(BasePlayer player)
        {
            foreach (var ui in AllUIs)
            {
                var state = ui.Root.Close(player);
                if (state != null && player.Connection.active)
                    CuiHelper.DestroyUi(player, state.GetCuiRootName());
            }
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
            var cuiComponents = state.Element.Layout.Prepare(state);
            var cuiElements = state.GetCuiElements();
            Debug.Assert(cuiElements.Root != null);
            Debug.Assert(cuiElements.Content != null);

            if (cuiComponents != null)
                foreach (var component in cuiComponents)
                    cuiElements.Content.AddComponent(component);

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
