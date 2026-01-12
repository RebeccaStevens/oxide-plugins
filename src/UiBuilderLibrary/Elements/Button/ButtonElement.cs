using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A button element.
    /// </summary>
    public partial class ButtonElement : PanelElement
    {
        /// <summary>
        /// The color of the icon when it is highlighted.
        /// </summary>
        public Color BgColorHighlighted;

        /// <summary>
        /// The color of the icon when it is pressed.
        /// </summary>
        public Color BgColorPressed;

        /// <summary>
        /// The color of the icon when it is selected.
        /// </summary>
        public Color BgColorSelected;

        /// <summary>
        /// The color of the icon when it is disabled.
        /// </summary>
        public Color BgColorDisabled;

        /// <summary>
        /// The action to perform when the button is clicked.
        /// </summary>
        public UiAction? Action;

        /// <summary>
        /// Create a new button element.
        /// </summary>
        internal ButtonElement(Element element) : base(element)
        {
            BgColor = ColorPallete.ItemLevel1;
            Material = null;

            SetHighlightBgColorModifier(1.2f);
            SetPressedBgColorModifier(0.8f);
            SetSelectedBgColorModifier(1.05f);
            SetDisabledBgColorModifier(0.5f);
        }

        /// <inheritdoc/>
        protected override ElementState InitialState(BasePlayer player) => new ButtonElementState(this, player);

        /// <summary>
        /// Set the amount to modify the background color by when the button is highlighted.
        /// </summary>
        /// <param name="modifier">How much to modify the color by (&gt;1 is brighter, &lt;1 is darker).</param>
        public void SetHighlightBgColorModifier(float modifier)
        {
            BgColorHighlighted = new Color(modifier, modifier, modifier);
        }

        /// <summary>
        /// Set the amount to modify the background color by when the button is highlighted.
        /// </summary>
        /// <param name="r">How much to modify the red channel by (&gt;1 is brighter, &lt;1 is darker).</param>
        /// <param name="g">How much to modify the green channel by (&gt;1 is brighter, &lt;1 is darker).</param>
        /// <param name="b">How much to modify the blue channel by (&gt;1 is brighter, &lt;1 is darker).</param>
        /// <param name="a">How much to modify the alpha channel by (&gt;1 is brighter, &lt;1 is darker).</param>
        public void SetHighlightBgColorModifier(float r, float g, float b, float a = 1f)
        {
            BgColorHighlighted = new Color(r, g, b, a);
        }

        /// <summary>
        /// Set the amount to modify the background color by when the button is pressed.
        /// </summary>
        /// <param name="modifier">How much to modify the color by (&gt;1 is brighter, &lt;1 is darker).</param>
        public void SetPressedBgColorModifier(float modifier)
        {
            BgColorPressed = new Color(modifier, modifier, modifier);
        }

        /// <summary>
        /// Set the amount to modify the background color by when the button is pressed.
        /// </summary>
        /// <param name="r">How much to modify the red channel by (&gt;1 is brighter, &lt;1 is darker).</param>
        /// <param name="g">How much to modify the green channel by (&gt;1 is brighter, &lt;1 is darker).</param>
        /// <param name="b">How much to modify the blue channel by (&gt;1 is brighter, &lt;1 is darker).</param>
        /// <param name="a">How much to modify the alpha channel by (&gt;1 is brighter, &lt;1 is darker).</param>
        public void SetPressedBgColorModifier(float r, float g, float b, float a = 1f)
        {
            BgColorPressed = new Color(r, g, b, a);
        }

        /// <summary>
        /// Set the amount to modify the background color by when the button is selected.
        /// </summary>
        /// <param name="modifier">How much to modify the color by (&gt;1 is brighter, &lt;1 is darker).</param>
        public void SetSelectedBgColorModifier(float modifier)
        {
            BgColorSelected = new Color(modifier, modifier, modifier);
        }

        /// <summary>
        /// Set the amount to modify the background color by when the button is selected.
        /// </summary>
        /// <param name="r">How much to modify the red channel by (&gt;1 is brighter, &lt;1 is darker).</param>
        /// <param name="g">How much to modify the green channel by (&gt;1 is brighter, &lt;1 is darker).</param>
        /// <param name="b">How much to modify the blue channel by (&gt;1 is brighter, &lt;1 is darker).</param>
        /// <param name="a">How much to modify the alpha channel by (&gt;1 is brighter, &lt;1 is darker).</param>
        public void SetSelectedBgColorModifier(float r, float g, float b, float a = 1f)
        {
            BgColorSelected = new Color(r, g, b, a);
        }

        /// <summary>
        /// Set the amount to modify the background color by when the button is disabled.
        /// </summary>
        /// <param name="modifier">How much to modify the color by (&gt;1 is brighter, &lt;1 is darker).</param>
        public void SetDisabledBgColorModifier(float modifier)
        {
            BgColorDisabled = new Color(modifier, modifier, modifier);
        }

        /// <summary>
        /// Set the amount to modify the background color by when the button is disabled.
        /// </summary>
        /// <param name="r">How much to modify the red channel by (&gt;1 is brighter, &lt;1 is darker).</param>
        /// <param name="g">How much to modify the green channel by (&gt;1 is brighter, &lt;1 is darker).</param>
        /// <param name="b">How much to modify the blue channel by (&gt;1 is brighter, &lt;1 is darker).</param>
        /// <param name="a">How much to modify the alpha channel by (&gt;1 is brighter, &lt;1 is darker).</param>
        public void SetDisabledBgColorModifier(float r, float g, float b, float a = 1f)
        {
            BgColorDisabled = new Color(r, g, b, a);
        }
    }
}
