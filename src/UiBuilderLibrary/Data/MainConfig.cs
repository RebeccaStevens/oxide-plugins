
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
                Bounds.GetScreenSizeAsAbsolute(Axis.X) / Bounds.GetScreenSizeAsAbsolute(Axis.Y);

            /// <summary>
            /// The render scale to use for players that have not set their own.
            /// </summary>
            public double DefaultRenderScale = 1d;
        }
    }
}
