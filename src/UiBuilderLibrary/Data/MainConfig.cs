
namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    private class MainConfig : PluginConfig<MainConfig.Data>
    {
        public MainConfig() : base(Panic.IfNull(builder))
        {
        }

        public class Data
        {
            /// <summary>
            /// The screen aspect ratio to use for players that have not set their own.
            /// </summary>
            public double DefaultScreenAspectRatio =
                Bounds.GetScreenSize(Axis.X).AsAbsolute / Bounds.GetScreenSize(Axis.Y).AsAbsolute;

            /// <summary>
            /// The render scale to use for players that have not set their own.
            /// </summary>
            public double DefaultRenderScale = 1d;
        }
    }
}
