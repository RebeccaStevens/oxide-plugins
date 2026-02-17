namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// An action that can be performed by a UI element.
    /// </summary>
    public abstract partial class UiAction
    {
        /// <summary>
        /// Get the command to run.
        /// </summary>
        public abstract string GetCommand();

        /// <summary>
        /// Run the action now via running its command.
        /// </summary>
        public void ExecuteViaCommand(BasePlayer player)
        {
            ConsoleSystem.Run(ConsoleSystem.Option.Client.FromConnection(player.Connection), GetCommand());
        }

        /// <summary>
        /// Run the action now.
        /// </summary>
        public abstract void Execute(BasePlayer player);
    }
}
