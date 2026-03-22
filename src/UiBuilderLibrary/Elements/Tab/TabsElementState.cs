using System.Diagnostics.CodeAnalysis;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    public partial class TabsElement
    {
        /// <inheritdoc/>
        public class TabsElementState : BoxModelElementState
        {
            /// <inheritdoc cref="ElementState.Element"/>
            public new TabsElement Element => (TabsElement)base.Element;

            /// <summary>
            /// The backing field for the <see cref="ActiveTab"/> property.
            /// </summary>
            protected Tab? ActiveTabBacking;

            /// <inheritdoc cref="TabsElementState(TabsElement, BasePlayer)"/>
            public TabsElementState(TabsElement element, BasePlayer player) : base(element, player)
            {
            }

            /// <summary>
            /// The tab that is currently active.
            /// </summary>
            [MaybeNull]
            public Tab ActiveTab
            {
                get
                {
                    if (ActiveTabBacking != null)
                        return ActiveTabBacking;
                    if (Element.DefaultTab != null)
                        ActiveTab = Element.DefaultTab; // Use the setter to set the backing field.
                    return ActiveTabBacking;
                }
                set
                {
                    Debug.Assert(Element.Tabs.Contains(value), "Tab is not part of this TabsElement.");
                    if (value == ActiveTabBacking)
                        return;

                    ActiveTabBacking?.View.Close(Player);
                    ActiveTabBacking = value;
                    ActiveTabBacking.View.Open(Player);
                }
            }

            /// <inheritdoc/>
            protected override void AddCuiComponents(ElementCuiElements cuiElements)
            {
                Debug.AssertNotNull(cuiElements.Root);

                if (Element.HasBorder())
                    cuiElements.Root.AddComponents(Element.Border.ApplyState(this));
            }
        }
    }
}
