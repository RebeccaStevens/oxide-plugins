namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// Get the font size to use to fit text within an element of the given height.
    /// </summary>
    internal static Size FontSizeForHeight(Size size) => size * (2d / 3d);
}
