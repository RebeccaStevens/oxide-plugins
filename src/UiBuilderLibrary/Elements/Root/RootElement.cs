namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// The root element of a UI.
    /// </summary>
    public partial class RootElement : PanelElement
    {
        /// <summary>
        /// The UI that this element is the root of.
        /// </summary>
        public Ui Ui { get; }

        /// <summary>
        /// Create a new root element.
        /// </summary>
        /// <param name="ui">The UI that this element is the root of.</param>
        /// <param name="layer">The layer to put this Ui on.</param>
        internal RootElement(Ui ui, string layer) : base(layer)
        {
            Ui = ui;
        }

        /// <inheritdoc/>
        protected override ElementState InitialState(BasePlayer player) => new RootState(this, player);

        /// <inheritdoc/>
        public override ElementState Open(BasePlayer player, bool force = false)
        {
            var state = base.Open(player, force);
            ElementLayout.PositionElementInParent(state);
            return state;
        }
    }
}
