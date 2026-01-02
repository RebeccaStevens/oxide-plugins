using System.Collections.Generic;


namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    private class MainData : PluginData<MainData.Data>
    {
        public MainData() : base(Panic.IfNull(builder))
        {
        }

        public class Data
        {
            public Dictionary<ulong, PlayerData> Players { get; } = new();

            public class PlayerData
            {
                public double ScreenAspectRatio;
                public double RenderScale;
            }
        }
    }

    /// <summary>
    /// Get the Screen Aspect Ratio for the given player.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public double GetScreenAspectRatio(BasePlayer player)
    {
        var playerData = GetPlayerData(player, false);
        return playerData.ScreenAspectRatio;
    }

    /// <summary>
    /// Sets the Screen Aspect Ratio for the given player.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="value"></param>
    public void SetScreenAspectRatio(BasePlayer player, double value)
    {
        var playerData = GetPlayerData(player);
        playerData.ScreenAspectRatio = value;
    }

    /// <summary>
    /// Get the Render Scale for the given player.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public double GetRenderScale(BasePlayer player)
    {
        var playerData = GetPlayerData(player, false);
        return playerData.RenderScale;
    }

    /// <summary>
    /// Sets the Render Scale for the given player.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="value"></param>
    public void SetRenderScale(BasePlayer player, double value)
    {
        var playerData = GetPlayerData(player);
        playerData.RenderScale = value;
    }

    /// <summary>
    /// Get the player data for the given player.
    /// This data can then be mutated.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="store">If the player doesn't already have any data, store a new copy.</param>
    /// <returns></returns>
    private MainData.Data.PlayerData GetPlayerData(BasePlayer player, bool store = true)
    {
        Debug.AssertNotNull(mainData.Values);
        if (mainData.Values.Players.TryGetValue(player.userID, out var data))
            return data;

        Debug.AssertNotNull(config.Values);
        var newPlayerEntry = new MainData.Data.PlayerData()
        {
            RenderScale = config.Values.DefaultRenderScale,
            ScreenAspectRatio = config.Values.DefaultScreenAspectRatio,
        };

        if (store)
            mainData.Values.Players[player.userID] = newPlayerEntry;

        return newPlayerEntry;
    }
}
