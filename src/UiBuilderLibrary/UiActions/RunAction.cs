using System;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    public abstract partial class UiAction
    {
        /// <summary>
        /// Run the given action.
        /// </summary>
        public static UiAction RunAction(Action action)
        {
            return new RunActionAction(action);
        }

        /// <summary>
        /// An action that runs the given action.
        /// </summary>
        internal class RunActionAction : UiAction
        {
            private readonly Action action;

            /// <inheritdoc cref="RunActionAction"/>
            public RunActionAction(Action action)
            {
                this.action = action;
            }

            /// <inheritdoc/>
            public override string GetCommand()
            {
                // TODO: Implement.
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override void Execute(BasePlayer player) => action();
        }
    }
}
