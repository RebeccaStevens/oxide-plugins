using System;
using System.Collections.Generic;
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
        private static readonly Dictionary<string, Ui> AllUIs = new();

        /// <summary>
        /// A unique id for this UI.
        /// </summary>
        internal readonly string Id;

        /// <summary>
        /// The root element that all other elements of this UI are children of.
        /// </summary>
        protected internal readonly RootElement Root;

        /// <summary>
        /// Create a new UI.
        /// </summary>
        /// <param name="layer">The layer of this UI. <a href="https://docs.oxidemod.com/guides/developers/basic-cui#layers">More info.</a></param>
        protected Ui(string layer)
        {
            if (layer == string.Empty)
                throw new ArgumentException("must be non-empty", nameof(layer));

            Root = new RootElement(this, layer);
            Id = CuiHelper.GetGuid();
            AllUIs.Add(Id, this);
        }

        /// <summary>
        /// Open this UI for the given player.
        /// </summary>
        /// <param name="player">The player to open the UI for.</param>
        /// <param name="sync">Whether to send the updated UI to the player.</param>
        public virtual void Open(BasePlayer player, bool sync = true)
        {
            var state = Root.Open(player);

            if (sync)
                state.Sync();
        }

        /// <summary>
        /// Close this UI for the given player.
        /// </summary>
        /// <param name="player">The player to close the UI for.</param>
        /// <param name="sync">Whether to send the command to the player to close the UI on their end.</param>
        public virtual void Close(BasePlayer player, bool sync = true)
        {
            var state = Root.Close(player);

            if (state == null || !player.Connection.active)
                return;

            if (sync)
                state.Sync();
        }

        /// <summary>
        /// Close this UI for all players.
        /// </summary>
        /// <param name="sync">Whether to send the command to the player to close the UI on their end.</param>
        public void CloseForAll(bool sync = true)
        {
            foreach (var player in BasePlayer.allPlayerList)
                Close(player, sync);
        }

        /// <summary>
        /// Close all UI for all the players.
        /// </summary>
        /// <param name="sync">Whether to send the command to the player to close the UI on their end.</param>
        internal static void CloseEverythingForAll(bool sync = true)
        {
            foreach (var player in BasePlayer.allPlayerList)
                CloseEverything(player, sync);
        }

        /// <summary>
        /// Close all UI for the given player.
        /// </summary>
        /// <param name="player">The player to close the UI for.</param>
        /// <param name="sync">Whether to send the command to the player to close the UI on their end.</param>
        internal static void CloseEverything(BasePlayer player, bool sync = true)
        {
            foreach (var kvp in AllUIs)
            {
                var state = kvp.Value.Root.Close(player);
                if (sync && state != null && player.Connection.active)
                    CuiHelper.DestroyUi(player, state.GetCuiRootName());
            }
        }

        /// <summary>
        /// Find a UI by its id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Ui? Find(string id) => AllUIs.TryGetValue(id, out var ui) ? ui : null;

        /// <summary>
        /// Get the name of the root CUI element for this UI.
        /// </summary>
        /// <param name="player">The player to get the name for.</param>
        public string GetRootCuiElementName(BasePlayer player) => Root.GetState(player).GetCuiRootName();

        /// <summary>
        /// Get the command that will close this UI.
        /// </summary>
        public string GetCloseCommand(bool sync = true)
        {
            return sync ? $"{CommandClose} {Id}" : $"{CommandClose} -S {Id}";
        }
    }
}
