using System.Collections.Generic;
using Oxide.Game.Rust.Cui;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A component that can be applied to an element.
    /// </summary>
    public interface IElementComponent
    {
        /// <summary>
        /// Get the cui components to attach to the given element state.
        /// </summary>
        public IEnumerable<ICuiComponent> ApplyState(ElementState state);
    }
}
