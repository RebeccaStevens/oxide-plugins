namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    public abstract partial class UiAction
    {
        /// <summary>
        /// Close the given UI.
        /// </summary>
        public static UiAction CloseUi(Ui ui)
        {
            return new CloseUiAction(ui);
        }

        /// <summary>
        /// An action that closes the given UI.
        /// </summary>
        internal class CloseUiAction : UiAction
        {
            private readonly Ui ui;

            public CloseUiAction(Ui ui)
            {
                this.ui = ui;
            }

            /// <summary>
            /// Get the name of the CUI element to close for the given player.
            /// </summary>
            public string GetUiRootCuiElementName(BasePlayer player)
            {
                return ui.GetRootCuiElementName(player);
            }

            /// <inheritdoc/>
            public override string GetCommand() => GetCommand(true);

            /// <summary>
            /// Get the command to run to close the UI.
            /// </summary>
            /// <param name="sync">Whether to send the command to the player to close the UI on their end.</param>
            /// <returns>The command to run to close the UI.</returns>
            public string GetCommand(bool sync) => ui.GetCloseCommand(sync);

            /// <inheritdoc/>
            public override void Execute(BasePlayer player) => ui.Close(player);
        }
    }
}
