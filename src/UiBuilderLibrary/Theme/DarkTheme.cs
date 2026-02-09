using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A dark theme.
    /// </summary>
    public class DarkTheme : Theme
    {
        /// <inheritdoc />
        public override ThemeColors Colors { get; } = new DarkThemeColors();

        /// <inheritdoc />
        public class DarkThemeColors : ThemeColors
        {
            /// <inheritdoc />
            public override Color TextRegular => textRegular;

            private readonly Color textRegular = new Color(0.914f, 0.875f, 0.843f);

            /// <inheritdoc />
            public override Color TextHighlighted => White;

            /// <inheritdoc />
            public override Color TextMuted => textMuted;

            private readonly Color textMuted = new Color(0.369f, 0.353f, 0.329f);

            /// <inheritdoc />
            public override Color ItemLevel1 => itemLevel1;

            private readonly Color itemLevel1 = new Color(0.251f, 0.251f, 0.239f);

            /// <inheritdoc />
            public override Color ItemLevel2 => itemLevel2;

            private readonly Color itemLevel2 = new Color(0.201f, 0.201f, 0.191f);

            /// <inheritdoc />
            public override Color ItemLevel3 => itemLevel3;

            private readonly Color itemLevel3 = new Color(0.188f, 0.188f, 0.181f);

            /// <inheritdoc />
            public override Color BackgroundLevel1 => backgroundLevel1;

            private readonly Color backgroundLevel1 = new Color(0.149f, 0.145f, 0.129f);

            /// <inheritdoc />
            public override Color BackgroundLevel2 => backgroundLevel2;

            private readonly Color backgroundLevel2 = new Color(0.086f, 0.086f, 0.082f);

            /// <inheritdoc />
            public override Color BackgroundLevel3 => backgroundLevel3;

            private readonly Color backgroundLevel3 = new Color(0.067f, 0.067f, 0.067f);
        }
    }
}
