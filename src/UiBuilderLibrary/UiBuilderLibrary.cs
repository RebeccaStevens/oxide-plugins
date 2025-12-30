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

#if DEBUG
        cmd.AddConsoleCommand("close-all", this, (data) =>
        {
            Ui.CloseEverythingForAll();
            data.ReplyWith("Closed all user UIs.");
            return true;
        });
#endif
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
