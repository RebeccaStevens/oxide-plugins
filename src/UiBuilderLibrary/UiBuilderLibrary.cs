using System;
using System.IO;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Plugins;

/// <summary>
/// A library for easily creating complex UIs.
/// </summary>
[Info("UI Builder Library", "BlueBeka", "0.1.0")]
[Description("Allows for easily creating complex UIs.")]
public partial class UiBuilderLibrary : RustPlugin
{
    /// <summary>
    /// A reference to the instance of this library.
    /// </summary>
    private static UiBuilderLibrary? builder;

    private readonly MainConfig config;
    private readonly MainData mainData;

    /// <summary>
    /// The scope (prefix) of all console commands of this library.
    /// </summary>
    private const string CommandScope = "uibuilderlibrary";

    /// <summary>
    /// The command to close a UI.
    /// </summary>
    private const string CommandClose = $"{CommandScope}.close";

    /// <summary>
    /// Initialize the UI Builder
    /// </summary>
    public UiBuilderLibrary()
    {
        Panic.IfNotNull(builder, "Only one instance of UiBuilderLibrary can exist at a time.");
        builder = this;

        config = new MainConfig();
        mainData = new MainData();
    }

    // ReSharper disable UnusedMember.Local

    private void Init()
    {
        config.Init();

        cmd.AddConsoleCommand(CommandClose, this, (data) =>
        {
            var args = Utils.ParseCommandLineArgs(data.Args);

            if (args.Values.Count == 0)
            {
                data.ReplyWith($"No ids specified.\nUsage: {CommandClose} [flags] <id> [<id> ...]");
                return false;
            }

            bool sync;
            try
            {
                sync = Utils.ParseBooleanMultiFlag(args, 's', "sync", 'S', "no-sync", true);
            }
            catch (Exception e) when (e is InvalidDataException or InvalidOperationException)
            {
                data.ReplyWith(e.Message);
                return false;
            }

            var player = (BasePlayer)data.Connection.player;
            var uisClosed = 0;
            foreach (var id in args.Values)
            {
                Debug.Assert(!string.IsNullOrEmpty(id));

                var ui = Ui.Find(id);
                if (ui == null)
                {
                    data.ReplyWith($"UI with id '{id}' not found.");
                    continue;
                }

                ui.Close(player, sync);
                uisClosed += 1;
            }

            return uisClosed > 0;
        });
    }

    private void Loaded()
    {
        config.Load();
        mainData.Load();
    }

    private void OnServerSave()
    {
        mainData.Save();
    }

    private void Unload()
    {
        mainData.Save();
        Ui.CloseEverythingForAll();
        builder = null;
    }

    private void OnUserDisconnected(IPlayer iPlayer)
    {
        var player = (BasePlayer)iPlayer.Object;
        Ui.CloseEverything(player);
    }

    // ReSharper enable UnusedParameter.Local
}
