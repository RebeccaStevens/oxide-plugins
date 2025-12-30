using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A simple panel element.
    /// </summary>
    public class PanelElement : Element
    {
        /// <summary>
        /// The background color of the element.
        /// </summary>
        public Color BgColor = Color.gray;

        /// <summary>
        /// Create a new panel element.
        /// </summary>
        /// <param name="parent">The parent of this element.</param>
        public PanelElement(Element parent) : base(parent)
        {
        }

        /// <summary>
        /// Create a new top level panel element.
        /// </summary>
        internal PanelElement(string parentId) : base(parentId)
        {
        }

        /// <inheritdoc/>
        protected override ElementState InitialState(BasePlayer player)
        {
            return new PanelElementState(this, player);
        }
    }
}