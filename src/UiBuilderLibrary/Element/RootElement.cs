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
        public readonly Ui Ui;

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
        protected override ElementState InitialState(BasePlayer player)
        {
            return new RootState(this, player);
        }

        /// <inheritdoc/>
        public override ElementState Open(BasePlayer player)
        {
            var state = base.Open(player);
            ElementLayout.PositionElementInParent(state);
            return state;
        }
    }
}