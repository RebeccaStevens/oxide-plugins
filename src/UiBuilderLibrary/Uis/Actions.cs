using System;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// An action that can be performed by a UI element.
    /// </summary>
    public abstract class UiAction
    {
        /// <summary>
        /// Close the given UI.
        /// </summary>
        public static UiAction CloseUi(Ui ui)
        {
            return new UiActionCloseUi(ui);
        }

        /// <summary>
        /// Execute the given command.
        /// </summary>
        public static UiAction Command(string command)
        {
            return new UiActionCommand(command);
        }

        /// <summary>
        /// Run the given action.
        /// </summary>
        public static UiAction Run(Action action)
        {
            return new UiActionRun(action);
        }

        /// <summary>
        /// Get the command to run.
        /// </summary>
        public abstract string GetCommand();
    }

    /// <summary>
    /// An action that closes the given UI.
    /// </summary>
    internal class UiActionCloseUi : UiAction
    {
        private readonly Ui ui;

        public UiActionCloseUi(Ui ui)
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
        public override string GetCommand()
        {
            return ui.GetCloseCommand(false);
        }
    }

    /// <summary>
    /// An action that runs the given command.
    /// </summary>
    internal class UiActionCommand : UiAction
    {
        private readonly string command;

        public UiActionCommand(string command)
        {
            this.command = command;
        }

        /// <inheritdoc/>
        public override string GetCommand()
        {
            return command;
        }
    }

    /// <summary>
    /// An action that runs the given action.
    /// </summary>
    internal class UiActionRun : UiAction
    {
        private readonly Action action;

        public UiActionRun(Action action)
        {
            this.action = action;
        }

        /// <inheritdoc/>
        public override string GetCommand()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Run the action.
        /// </summary>
        public void Run() => action();
    }
}
