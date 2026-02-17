namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    public abstract partial class UiAction
    {
        /// <summary>
        /// Execute the given command.
        /// </summary>
        public static UiAction RunCommand(string command)
        {
            return new RunCommandAction(command);
        }

        /// <summary>
        /// An action that runs the given command.
        /// </summary>
        internal class RunCommandAction : UiAction
        {
            private readonly string command;

            /// <inheritdoc cref="RunCommandAction"/>
            public RunCommandAction(string command)
            {
                this.command = command;
            }

            /// <inheritdoc/>
            public override string GetCommand() => command;

            /// <inheritdoc/>
            public override void Execute(BasePlayer player) => ExecuteViaCommand(player);
        }
    }
}
