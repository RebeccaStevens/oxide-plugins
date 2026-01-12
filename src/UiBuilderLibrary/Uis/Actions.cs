using System;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    public class UiAction
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
    }

    /// <summary>
    /// An action that can be performed on a UI.
    /// </summary>
    public interface IUiActionRootUi
    {
        /// <summary>
        /// Get the name of the root CUI element.
        /// </summary>
        string GetUiName(BasePlayer player);
    }

    /// <summary>
    /// An action that will be performed via a command.
    /// </summary>
    public interface IUiActionCommand
    {
        /// <summary>
        /// Get the command to run.
        /// </summary>
        string GetCommand();
    }

    internal class UiActionCloseUi : UiAction, IUiActionRootUi
    {
        private readonly Ui ui;

        public UiActionCloseUi(Ui ui)
        {
            this.ui = ui;
        }

        /// <summary>
        /// Get the name of the CUI element to close for the given player.
        /// </summary>
        public string GetUiName(BasePlayer player)
        {
            return ui.Root.GetState(player).GetCuiRootName();
        }
    }

    internal class UiActionCommand : UiAction, IUiActionCommand
    {
        private readonly string command;

        public UiActionCommand(string command)
        {
            this.command = command;
        }

        /// <inheritdoc/>
        public string GetCommand()
        {
            return command;
        }
    }

    internal class UiActionRun : UiAction, IUiActionCommand
    {
        private readonly Action action;

        public UiActionRun(Action action)
        {
            this.action = action;
        }

        /// <inheritdoc/>
        public string GetCommand()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Run the action.
        /// </summary>
        public void Run() => action();
    }
}
