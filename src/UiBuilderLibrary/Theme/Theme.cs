using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A theme that can be used for UI elements.
    /// </summary>
    public abstract class Theme
    {
        /// <summary>
        /// The default theme that is used if elements don't specify their own theme to use.
        /// </summary>
        public static Theme Default = new DarkTheme();

        /// <summary>
        /// The colors used by this theme.
        /// </summary>
        public abstract ThemeColors Colors { get; }

        /// <summary>
        /// The spacing values used by this theme.
        /// </summary>
        public virtual ThemeSpacing Spacing { get; } = new ThemeSpacing();

        /// <summary>
        /// The font size values used by this theme.
        /// </summary>
        public virtual ThemeFontSize FontSize { get; } = new ThemeFontSize();

        /// <summary>
        /// The sizing values for items used by this theme.
        /// </summary>
        public virtual ThemeItemSizing ItemSizing { get; } = new ThemeItemSizing();

        /// <summary>
        /// The line thickness values used by this theme.
        /// </summary>
        public virtual ThemeLineThickness LineThickness { get; } = new ThemeLineThickness();

        /// <summary>
        /// The colors a theme uses.
        /// </summary>
        public abstract class ThemeColors
        {
            /// <summary>
            /// The color used for borders.
            /// </summary>
            public virtual Color Border { get; } = Color.black;

            /// <summary>
            /// The color used for the background of items.<br/>
            /// Level 1 is the base level and should be used in most cases.<br/>
            /// Level 2 and above are designed for nesting items inside one another.
            /// </summary>
            public abstract Color ItemLevel1 { get; }

            /// <inheritdoc cref="ItemLevel1"/>
            public abstract Color ItemLevel2 { get; }

            /// <inheritdoc cref="ItemLevel1"/>
            public abstract Color ItemLevel3 { get; }

            /// <summary>
            /// The color used for the background of a view (such as a window).<br/>
            /// Level 1 is the base level and should be used in most cases.<br/>
            /// Level 2 and above are designed for nesting views inside one another.
            /// </summary>
            public abstract Color BackgroundLevel1 { get; }

            /// <inheritdoc cref="BackgroundLevel1"/>
            public abstract Color BackgroundLevel2 { get; }

            /// <inheritdoc cref="BackgroundLevel1"/>
            public abstract Color BackgroundLevel3 { get; }

            /// <summary>
            /// A transparent color that can be used on an overlay to darken the underlying content.<br/>
            /// Level 1 is the base level and should be used in most cases.<br/>
            /// Level 2 and above allow for extra darkening when needed.
            /// </summary>
            public virtual Color OverlayDarkenLevel1 { get; } = new Color(0f, 0f, 0f, 0.5f);

            /// <inheritdoc cref="OverlayDarkenLevel1"/>
            public virtual Color OverlayDarkenLevel2 { get; } = new Color(0f, 0f, 0f, 0.65f);

            /// <inheritdoc cref="OverlayDarkenLevel1"/>
            public virtual Color OverlayDarkenLevel3 { get; } = new Color(0f, 0f, 0f, 0.8f);

            /// <summary>
            /// A transparent color that can be used on an overlay to lighten the underlying content.<br/>
            /// Level 1 is the base level and should be used in most cases.<br/>
            /// Level 2 and above allow for extra lightening when needed.
            /// </summary>
            public virtual Color OverlayLightenLevel1 { get; } = new Color(1f, 1f, 1f, 0.5f);

            /// <inheritdoc cref="OverlayLightenLevel1"/>
            public virtual Color OverlayLightenLevel2 { get; } = new Color(1f, 1f, 1f, 0.65f);

            /// <inheritdoc cref="OverlayLightenLevel1"/>
            public virtual Color OverlayLightenLevel3 { get; } = new Color(1f, 1f, 1f, 0.8f);

            /// <summary>
            /// The color used for regular text.<br/>
            /// This text color should contrast well with item backgrounds (such as <see cref="ItemLevel1"/>).
            /// </summary>
            public abstract Color TextRegular { get; }

            /// <summary>
            /// The color used for important text.<br/>
            /// This text color should contrast well with item backgrounds (such as <see cref="ItemLevel1"/>).
            /// </summary>
            public abstract Color TextHighlighted { get; }

            /// <summary>
            /// The color used for muted text.<br/>
            /// This text color should contrast well with item backgrounds (such as <see cref="ItemLevel1"/>).
            /// </summary>
            public abstract Color TextMuted { get; }

            /// <summary>
            /// The color used when black is needed - for most themes, this will just be black.
            /// </summary>
            public virtual Color Black => Color.black;

            /// <summary>
            /// The color used for text when placed on top of black backgrounds.
            /// </summary>
            public virtual Color TextOnBlack => White;

            /// <summary>
            /// The color used when white is needed - for most themes, this will just be white.
            /// </summary>
            public virtual Color White => Color.white;

            /// <summary>
            /// The color used for text when placed on top of white backgrounds.
            /// </summary>
            public virtual Color TextOnWhite => Black;

            /// <summary>
            /// The red color used by Rust's UI.
            /// </summary>
            public virtual Color RustRed => new Color(0.667f, 0.278f, 0.208f);

            /// <summary>
            /// The color used for text when placed on top of <see cref="RustRed"/> backgrounds.
            /// </summary>
            public virtual Color TextOnRustRed => new Color(0.914f, 0.855f, 0.847f);

            /// <summary>
            /// The green color used by Rust's UI.
            /// </summary>
            public virtual Color RustGreen => new Color(0.365f, 0.447f, 0.220f);

            /// <summary>
            /// The color used for text when placed on top of <see cref="RustGreen"/> backgrounds.
            /// </summary>
            public virtual Color TextOnRustGreen => new Color(0.890f, 0.914f, 0.847f);

            /// <summary>
            /// The (red) color used for danger/error states.
            /// </summary>
            public virtual Color Danger => RustRed;

            /// <summary>
            /// The color used for text when placed on top of <see cref="Danger"/> backgrounds.
            /// </summary>
            public virtual Color TextOnDanger => TextOnRustRed;

            /// <summary>
            /// The (orange) color used for warning states.
            /// </summary>
            public virtual Color Warning => new Color(0.659f, 0.557f, 0.220f);

            /// <summary>
            /// The color used for text when placed on top of <see cref="Warning"/> backgrounds.
            /// </summary>
            public virtual Color TextOnWarning => new Color(0.988f, 0.988f, 0.812f);

            /// <summary>
            /// The (green) color used for success states.
            /// </summary>
            public virtual Color Success => RustGreen;

            /// <summary>
            /// The color used for text when placed on top of <see cref="Success"/> backgrounds.
            /// </summary>
            public virtual Color TextOnSuccess => TextOnRustGreen;

            /// <summary>
            /// The (blue) color used for informational states.
            /// </summary>
            public virtual Color Info => new Color(0.20f, 0.58f, 0.64f);

            /// <summary>
            /// The color used for text when placed on top of <see cref="Info"/> backgrounds.
            /// </summary>
            public virtual Color TextOnInfo => new Color(0.894f, 0.937f, 0.945f);

            /// <summary>
            /// The color used for button backgrounds.
            /// </summary>
            public virtual Color ButtonBase => ItemLevel1;

            /// <summary>
            /// The color used for button backgrounds when the button is highlighted.<br />
            /// If null, a lightened version of button's base color will be used.
            /// </summary>
            public virtual Color? ButtonHighlighted => null;

            /// <summary>
            /// The color used for button backgrounds when the button is pressed.<br />
            /// If null, a darkened version of button's base color will be used.
            /// </summary>
            public virtual Color? ButtonPressed => null;

            /// <summary>
            /// The color used for button backgrounds when the button is selected.<br />
            /// If null, the button's base color will be used.
            /// </summary>
            public virtual Color? ButtonSelected => null;

            /// <summary>
            /// The color used for icons.
            /// </summary>
            public virtual Color Icon => White;
        }

        /// <summary>
        /// The spacing values a theme uses.
        /// </summary>
        public class ThemeSpacing
        {
            /// <summary>
            /// A very small spacing value.<br/>
            /// The smallest preset spacing value.<br/>
            /// </summary>
            /// <remarks>
            /// For no spacing, use <see cref="Size.Zero"/>.
            /// </remarks>
            public virtual Size Tiny => Size.Pixels(2);

            /// <summary>
            /// A small spacing value.<br/>
            /// Larger than <see cref="Tiny"/> but smaller than <see cref="Small"/>.
            /// </summary>
            public virtual Size Small => Size.Pixels(4);

            /// <summary>
            /// A medium spacing value.<br/>
            /// This value is a good default for general spacing needs, such as padding and gaps between elements.<br/>
            /// Larger than <see cref="Small"/> but smaller than <see cref="Medium"/>.
            /// </summary>
            public virtual Size Medium => Size.Pixels(8);

            /// <summary>
            /// A large spacing value.<br/>
            /// Larger than <see cref="Medium"/> but smaller than <see cref="ExtraLarge"/>.
            /// </summary>
            public virtual Size Large => Size.Pixels(16);

            /// <summary>
            /// A very large spacing value.<br/>
            /// Larger than <see cref="Large"/> but smaller than <see cref="ExtraLarge"/>.
            /// </summary>
            public virtual Size ExtraLarge => Size.Pixels(24);

            /// <summary>
            /// Larger than <see cref="ExtraLarge"/> but smaller than <see cref="Huge"/>.
            /// </summary>
            public virtual Size Huge => Size.Pixels(32);

            /// <summary>
            /// Larger than <see cref="Huge"/> but smaller than <see cref="Gigantic"/>.
            /// </summary>
            public virtual Size Massive => Size.Pixels(48);

            /// <summary>
            /// The largest preset spacing value.
            /// </summary>
            public virtual Size Gigantic => Size.Pixels(64);
        }

        /// <summary>
        /// The font size values a theme uses.
        /// </summary>
        public class ThemeFontSize
        {
            /// <summary>
            /// A very small font size.<br/>
            /// The smallest preset font size.
            /// </summary>
            public virtual Size Tiny => Size.Pixels(10);

            /// <summary>
            /// A small font size.<br/>
            /// Larger than <see cref="Tiny"/> but smaller than <see cref="Medium"/>.
            /// </summary>
            public virtual Size Small => Size.Pixels(12);

            /// <summary>
            /// A medium font size.<br/>
            /// This value is a good default for general font sizes, such as text.<br/>
            /// Larger than <see cref="Small"/> but smaller than <see cref="Large"/>.
            /// </summary>
            public virtual Size Medium => Size.Pixels(14);

            /// <summary>
            /// A large font size.<br/>
            /// Larger than <see cref="Medium"/> but smaller than <see cref="ExtraLarge"/>.
            /// </summary>
            public virtual Size Large => Size.Pixels(16);

            /// <summary>
            /// A very large font size.<br/>
            /// Larger than <see cref="Large"/> but smaller than <see cref="ExtraLarge"/>.
            /// </summary>
            public virtual Size ExtraLarge => Size.Pixels(18);

            /// <summary>
            /// The font size used for top-level headings.
            /// </summary>
            public virtual Size Heading1 => Size.Pixels(24);

            /// <summary>
            /// The font size used for second-level headings.
            /// </summary>
            public virtual Size Heading2 => Size.Pixels(20);

            /// <summary>
            /// The font size used for third-level headings.
            /// </summary>
            public virtual Size Heading3 => Size.Pixels(18);

            /// <summary>
            /// The font size used for fourth-level headings.
            /// </summary>
            public virtual Size Heading4 => Size.Pixels(16);

            /// <summary>
            /// The font size used for fifth-level headings.
            /// </summary>
            public virtual Size Heading5 => Size.Pixels(15);

            /// <summary>
            /// The font size used for sixth-level headings.
            /// </summary>
            public virtual Size Heading6 => Size.Pixels(14);
        }

        /// <summary>
        /// The sizing values for items used by this theme.
        /// </summary>
        public class ThemeItemSizing
        {
            /// <summary>
            /// A very tiny item size.<br/>
            /// The smallest preset item size.
            /// </summary>
            public virtual Size Tiny => Size.Pixels(24);

            /// <summary>
            /// A small item size.<br/>
            /// Larger than <see cref="Tiny"/> but smaller than <see cref="Medium"/>.
            /// </summary>
            public virtual Size Small => Size.Pixels(32);

            /// <summary>
            /// A medium item size.<br/>
            /// Larger than <see cref="Small"/> but smaller than <see cref="Large"/>.
            /// </summary>
            public virtual Size Medium => Size.Pixels(48);

            /// <summary>
            /// A large item size.<br/>
            /// Larger than <see cref="Medium"/> but smaller than <see cref="ExtraLarge"/>.
            /// </summary>
            public virtual Size Large => Size.Pixels(64);

            /// <summary>
            /// A very large item size.<br/>
            /// Larger than <see cref="Large"/> but smaller than <see cref="ExtraLarge"/>.
            /// </summary>
            public virtual Size ExtraLarge => Size.Pixels(96);

            /// <summary>
            /// A huge item size.<br/>
            /// Larger than <see cref="ExtraLarge"/> but smaller than <see cref="Massive"/>.
            /// </summary>
            public virtual Size Huge => Size.Pixels(128);

            /// <summary>
            /// A massive item size.<br/>
            /// Larger than <see cref="Huge"/> but smaller than <see cref="Gigantic"/>.
            /// </summary>
            public virtual Size Massive => Size.Pixels(192);

            /// <summary>
            /// A gigantic item size.<br/>
            /// Larger than <see cref="Massive"/> but smaller than <see cref="Gigantic"/>.
            /// </summary>
            public virtual Size Gigantic => Size.Pixels(256);
        }

        /// <summary>
        /// The line thickness values used by this theme.
        /// </summary>
        public class ThemeLineThickness
        {
            /// <summary>
            /// A very thin line thickness.<br/>
            /// The smallest preset line thickness.
            /// </summary>
            public virtual Size Thin => Size.Pixels(1);

            /// <summary>
            /// A medium line thickness.<br/>
            /// Larger than <see cref="Thin"/> but smaller than <see cref="Thick"/>.
            /// </summary>
            public virtual Size Medium => Size.Pixels(2);

            /// <summary>
            /// A thick line thickness.<br/>.
            /// Larger than <see cref="Medium"/> but smaller than <see cref="ExtraThick"/>.
            /// </summary>
            public virtual Size Thick => Size.Pixels(3);

            /// <summary>
            /// A very thick line thickness.<br/>
            /// The largest preset line thickness.
            /// </summary>
            public virtual Size ExtraThick => Size.Pixels(4);
        }
    }
}
