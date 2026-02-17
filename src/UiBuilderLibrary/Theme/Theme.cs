using UnityEngine;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A theme that can be used for UI elements.
    /// </summary>
    public partial record Theme
    {
        private static Theme? _default;

        /// <summary>
        /// The default theme that is used if elements don't specify their own theme to use.
        /// </summary>
        public static Theme Default => _default ??= CreateDarkTheme();

        /// <summary>
        /// The colors used by this theme.
        /// </summary>
        public required ThemeColors Colors { get; init; }

        /// <summary>
        /// The spacing values used by this theme.
        /// </summary>
        public ThemeSpacing Spacing { get; init; } = new ThemeSpacing();

        /// <summary>
        /// The font size values used by this theme.
        /// </summary>
        public ThemeFontSize FontSize { get; init; } = new ThemeFontSize();

        /// <summary>
        /// The sizing values for items used by this theme.
        /// </summary>
        public ThemeItemSizing ItemSizing { get; init; } = new ThemeItemSizing();

        /// <summary>
        /// The line thickness values used by this theme.
        /// </summary>
        public ThemeLineThickness LineThickness { get; init; } = new ThemeLineThickness();

        /// <summary>
        /// The colors a theme uses.
        /// </summary>
        public readonly record struct ThemeColors()
        {
            /// <summary>
            /// The colors used to give things color.
            /// </summary>
            public required BrandingColors Branding { get; init; }

            /// <summary>
            /// The color used for borders.
            /// </summary>
            public Color Border { get; init; } = Color.black;

            /// <summary>
            /// The colors used for the background of items.
            /// </summary>
            public required ColorLevels Item { get; init; }

            /// <summary>
            /// The colors used for the background of a view (such as a window).
            /// </summary>
            public required ColorLevels Background { get; init; }

            /// <summary>
            /// The colors used for an overlay to darken the underlying content.
            /// </summary>
            public ColorLevels OverlayDarken { get; init; } = new ColorLevels
            {
                Level1 = new Color(0f, 0f, 0f, 0.5f),
                Level2 = new Color(0f, 0f, 0f, 0.65f),
                Level3 = new Color(0f, 0f, 0f, 0.8f)
            };

            /// <summary>
            /// The colors used for an overlay to lighten the underlying content.
            /// </summary>
            public ColorLevels OverlayLighten { get; init; } = new ColorLevels
            {
                Level1 = new Color(1f, 1f, 1f, 0.5f),
                Level2 = new Color(1f, 1f, 1f, 0.65f),
                Level3 = new Color(1f, 1f, 1f, 0.8f)
            };

            /// <summary>
            /// The colors used for text.
            /// </summary>
            public required TextColors Text { get; init; }

            /// <summary>
            /// The colors used for Rust's branding.
            /// </summary>
            public RustColors Rust { get; } = new RustColors();

            /// <summary>
            /// Common colors used for blocks of content and backgrounds of elements.
            /// </summary>
            public SolidColors Solid { get; init; } = new SolidColors();

            /// <summary>
            /// The colors used text and other foreground elements when they are on top of solids of content of the given color.
            /// </summary>
            public OnSolidColors OnSolid { get; init; } = new OnSolidColors();

            /// <summary>
            /// The color used for icons.
            /// </summary>
            public Color Icon { get; init; } = Color.white;

            /// <inheritdoc cref="Branding"/>
            public readonly record struct BrandingColors
            {
                /// <summary>
                /// The primary color to use for things that need color.
                /// </summary>
                public required Color Primary { get; init; }

                /// <summary>
                /// The secondary color to use for things that need color.
                /// </summary>
                public required Color Secondary { get; init; }
            }

            /// <inheritdoc cref="Text"/>
            public readonly record struct TextColors
            {
                /// <summary>
                /// The color used for regular text.<br/>
                /// This text color should contrast well with item backgrounds (<see cref="Item"/>).
                /// </summary>
                public required Color Regular { get; init; }

                /// <summary>
                /// The color used for important text.<br/>
                /// This text color should contrast well with item backgrounds (<see cref="Item"/>).
                /// </summary>
                public required Color Highlighted { get; init; }

                /// <summary>
                /// The color used for muted text.<br/>
                /// This text color should contrast well with item backgrounds (<see cref="Item"/>).
                /// </summary>
                public required Color Muted { get; init; }
            }

            /// <inheritdoc cref="Rust"/>
            public readonly record struct RustColors()
            {
                /// <summary>
                /// The red color from Rust's branding.
                /// </summary>
                public Color Red { get; } = new Color(0.667f, 0.278f, 0.208f);

                /// <summary>
                /// The green color from Rust's branding.
                /// </summary>
                public Color Green { get; } = new Color(0.365f, 0.447f, 0.220f);

                /// <summary>
                /// The color to use over <see cref="Red"/>.
                /// </summary>
                public Color OnRed { get; } = new Color(0.914f, 0.855f, 0.847f);

                /// <summary>
                /// The color to use over <see cref="Green"/>.
                /// </summary>
                public Color OnGreen { get; } = new Color(0.890f, 0.914f, 0.847f);
            }

            /// <inheritdoc cref="Solid"/>
            public readonly record struct SolidColors()
            {
                /// <summary>
                /// The color to use as black.
                /// </summary>
                public Color Black { get; init; } = Color.black;

                /// <summary>
                /// The color to use as white.
                /// </summary>
                public Color White { get; init; } = Color.white;

                /// <summary>
                /// The color to use as transparent.
                /// </summary>
                public Color Transparent { get; init; } = Color.clear;

                /// <summary>
                /// The color used to indicate danger or errors.
                /// </summary>
                public Color Danger { get; init; } = new Color(0.667f, 0.278f, 0.208f);

                /// <summary>
                /// The color used to indicate warnings.
                /// </summary>
                public Color Warning { get; init; } = new Color(0.659f, 0.557f, 0.220f);

                /// <summary>
                /// The color used to indicate success.
                /// </summary>
                public Color Success { get; init; } = new Color(0.365f, 0.447f, 0.220f);

                /// <summary>
                /// The color used to indicate information.
                /// </summary>
                public Color Info { get; init; } = new Color(0.20f, 0.58f, 0.64f);
            }

            /// <inheritdoc cref="OnSolid"/>
            public readonly record struct OnSolidColors()
            {
                /// <summary>
                /// The color to use over <see cref="SolidColors.Black"/>.
                /// </summary>
                public Color Black { get; init; } = Color.white;

                /// <summary>
                /// The color to use over <see cref="SolidColors.White"/>.
                /// </summary>
                public Color White { get; init; } = Color.black;

                /// <summary>
                /// The color to use over <see cref="SolidColors.Danger"/>.
                /// </summary>
                public Color Danger { get; init; } = new Color(0.914f, 0.855f, 0.847f);

                /// <summary>
                /// The color to use over <see cref="SolidColors.Warning"/>.
                /// </summary>
                public Color Warning { get; init; } = new Color(0.988f, 0.988f, 0.812f);

                /// <summary>
                /// The color to use over <see cref="SolidColors.Success"/>.
                /// </summary>
                public Color Success { get; init; } = new Color(0.890f, 0.914f, 0.847f);

                /// <summary>
                /// The color to use over <see cref="SolidColors.Info"/>.
                /// </summary>
                public Color Info { get; init; } = new Color(0.894f, 0.937f, 0.945f);
            }

            /// <summary>
            /// Multiple shades of a color.
            /// </summary>
            public readonly record struct ColorLevels
            {
                /// <summary>
                /// Used as the base level color and should be used in most cases.<br/>
                /// </summary>
                public required Color Level1 { get; init; }

                /// <summary>
                /// Used for nesting items inside one another, with higher levels used for deeper nesting.<br/>
                /// </summary>
                public required Color Level2 { get; init; }

                /// <inheritdoc cref="Level2"/>
                public required Color Level3 { get; init; }
            }
        }

        /// <summary>
        /// The spacing values a theme uses.
        /// </summary>
        public readonly record struct ThemeSpacing()
        {
            /// <summary>
            /// A very small spacing value.<br/>
            /// The smallest preset spacing value.<br/>
            /// </summary>
            /// <remarks>
            /// For no spacing, use <see cref="Size.Zero"/>.
            /// </remarks>
            public Size Tiny { get; init; } = Size.Pixels(2);

            /// <summary>
            /// A small spacing value.<br/>
            /// Larger than <see cref="Tiny"/> but smaller than <see cref="Small"/>.
            /// </summary>
            public Size Small { get; init; } = Size.Pixels(4);

            /// <summary>
            /// A medium spacing value.<br/>
            /// This value is a good default for general spacing needs, such as padding and gaps between elements.<br/>
            /// Larger than <see cref="Small"/> but smaller than <see cref="Medium"/>.
            /// </summary>
            public Size Medium { get; init; } = Size.Pixels(8);

            /// <summary>
            /// A large spacing value.<br/>
            /// Larger than <see cref="Medium"/> but smaller than <see cref="ExtraLarge"/>.
            /// </summary>
            public Size Large { get; init; } = Size.Pixels(16);

            /// <summary>
            /// A very large spacing value.<br/>
            /// Larger than <see cref="Large"/> but smaller than <see cref="ExtraLarge"/>.
            /// </summary>
            public Size ExtraLarge { get; init; } = Size.Pixels(24);

            /// <summary>
            /// Larger than <see cref="ExtraLarge"/> but smaller than <see cref="Huge"/>.
            /// </summary>
            public Size Huge { get; init; } = Size.Pixels(32);

            /// <summary>
            /// Larger than <see cref="Huge"/> but smaller than <see cref="Gigantic"/>.
            /// </summary>
            public Size Massive { get; init; } = Size.Pixels(48);

            /// <summary>
            /// The largest preset spacing value.
            /// </summary>
            public Size Gigantic { get; init; } = Size.Pixels(64);
        }

        /// <summary>
        /// The font size values a theme uses.
        /// </summary>
        public readonly record struct ThemeFontSize()
        {
            /// <summary>
            /// A very small font size.<br/>
            /// The smallest preset font size.
            /// </summary>
            public Size Tiny { get; init; } = Size.Pixels(10);

            /// <summary>
            /// A small font size.<br/>
            /// Larger than <see cref="Tiny"/> but smaller than <see cref="Medium"/>.
            /// </summary>
            public Size Small { get; init; } = Size.Pixels(12);

            /// <summary>
            /// A medium font size.<br/>
            /// This value is a good default for general font sizes, such as text.<br/>
            /// Larger than <see cref="Small"/> but smaller than <see cref="Large"/>.
            /// </summary>
            public Size Medium { get; init; } = Size.Pixels(14);

            /// <summary>
            /// A large font size.<br/>
            /// Larger than <see cref="Medium"/> but smaller than <see cref="ExtraLarge"/>.
            /// </summary>
            public Size Large { get; init; } = Size.Pixels(16);

            /// <summary>
            /// A very large font size.<br/>
            /// Larger than <see cref="Large"/> but smaller than <see cref="ExtraLarge"/>.
            /// </summary>
            public Size ExtraLarge { get; init; } = Size.Pixels(18);

            /// <summary>
            /// The font size used for top-level headings.
            /// </summary>
            public Size Heading1 { get; init; } = Size.Pixels(24);

            /// <summary>
            /// The font size used for second-level headings.
            /// </summary>
            public Size Heading2 { get; init; } = Size.Pixels(20);

            /// <summary>
            /// The font size used for third-level headings.
            /// </summary>
            public Size Heading3 { get; init; } = Size.Pixels(18);

            /// <summary>
            /// The font size used for fourth-level headings.
            /// </summary>
            public Size Heading4 { get; init; } = Size.Pixels(16);

            /// <summary>
            /// The font size used for fifth-level headings.
            /// </summary>
            public Size Heading5 { get; init; } = Size.Pixels(15);

            /// <summary>
            /// The font size used for sixth-level headings.
            /// </summary>
            public Size Heading6 { get; init; } = Size.Pixels(14);
        }

        /// <summary>
        /// The sizing values for items used by this theme.
        /// </summary>
        public readonly record struct ThemeItemSizing()
        {
            /// <summary>
            /// A very tiny item size.<br/>
            /// The smallest preset item size.
            /// </summary>
            public Size Tiny { get; init; } = Size.Pixels(24);

            /// <summary>
            /// An extra small item size.<br/>
            /// Larger than <see cref="Tiny"/> but smaller than <see cref="Small"/>.
            /// </summary>
            public Size ExtraSmall { get; init; } = Size.Pixels(32);

            /// <summary>
            /// A small item size.<br/>
            /// Larger than <see cref="ExtraSmall"/> but smaller than <see cref="Medium"/>.
            /// </summary>
            public Size Small { get; init; } = Size.Pixels(48);

            /// <summary>
            /// A medium item size.<br/>
            /// Larger than <see cref="Small"/> but smaller than <see cref="Large"/>.
            /// </summary>
            public Size Medium { get; init; } = Size.Pixels(64);

            /// <summary>
            /// A large item size.<br/>
            /// Larger than <see cref="Medium"/> but smaller than <see cref="ExtraLarge"/>.
            /// </summary>
            public Size Large { get; init; } = Size.Pixels(96);

            /// <summary>
            /// A very large item size.<br/>
            /// Larger than <see cref="Large"/> but smaller than <see cref="ExtraLarge"/>.
            /// </summary>
            public Size ExtraLarge { get; init; } = Size.Pixels(128);

            /// <summary>
            /// A huge item size.<br/>
            /// Larger than <see cref="ExtraLarge"/> but smaller than <see cref="Massive"/>.
            /// </summary>
            public Size Huge { get; init; } = Size.Pixels(192);

            /// <summary>
            /// A massive item size.<br/>
            /// Larger than <see cref="Huge"/> but smaller than <see cref="Gigantic"/>.
            /// </summary>
            public Size Massive { get; init; } = Size.Pixels(256);

            /// <summary>
            /// A gigantic item size.<br/>
            /// Larger than <see cref="Massive"/> but smaller than <see cref="Gigantic"/>.
            /// </summary>
            public Size Gigantic { get; init; } = Size.Pixels(384);
        }

        /// <summary>
        /// The line thickness values used by this theme.
        /// </summary>
        public readonly record struct ThemeLineThickness()
        {
            /// <summary>
            /// A very thin line thickness.<br/>
            /// The smallest preset line thickness.
            /// </summary>
            public Size Thin { get; init; } = Size.Pixels(1);

            /// <summary>
            /// A medium line thickness.<br/>
            /// Larger than <see cref="Thin"/> but smaller than <see cref="Thick"/>.
            /// </summary>
            public Size Medium { get; init; } = Size.Pixels(2);

            /// <summary>
            /// A thick line thickness.<br/>.
            /// Larger than <see cref="Medium"/> but smaller than <see cref="ExtraThick"/>.
            /// </summary>
            public Size Thick { get; init; } = Size.Pixels(3);

            /// <summary>
            /// A very thick line thickness.<br/>
            /// The largest preset line thickness.
            /// </summary>
            public Size ExtraThick { get; init; } = Size.Pixels(4);
        }

        /// <summary>
        /// The available icons.
        /// </summary>
        public static class Icons
        {
            public static readonly string Ammunition = "assets/icons/ammunition.png";
            public static readonly string ArrowRight = "assets/icons/arrow_right.png";
            public static readonly string Authorize = "assets/icons/authorize.png";
            public static readonly string Bite = "assets/icons/bite.png";
            public static readonly string Bleeding = "assets/icons/bleeding.png";
            public static readonly string Blueprint = "assets/icons/blueprint.png";
            public static readonly string BlueprintUnderlay = "assets/icons/blueprint_underlay.png";
            public static readonly string Blunt = "assets/icons/blunt.png";
            public static readonly string BpLock = "assets/icons/bp-lock.png";
            public static readonly string Broadcast = "assets/icons/broadcast.png";
            public static readonly string BuildStairs = "assets/icons/build/stairs.png";
            public static readonly string BuildWallDoorwayDoor = "assets/icons/build/wall.doorway.door.png";
            public static readonly string BuildWallWindowBars = "assets/icons/build/wall.window.bars.png";
            public static readonly string Bullet = "assets/icons/bullet.png";
            public static readonly string CargoShipBody = "assets/icons/cargo_ship_body.png";
            public static readonly string Cart = "assets/icons/cart.png";
            public static readonly string ChangeCode = "assets/icons/change_code.png";
            public static readonly string Check = "assets/icons/check.png";
            public static readonly string ChinookMapBlades = "assets/icons/chinook_map_blades.png";
            public static readonly string ChinookMapBody = "assets/icons/chinook_map_body.png";
            public static readonly string CircleClosed = "assets/icons/circle_closed.png";
            public static readonly string CircleClosedToedge = "assets/icons/circle_closed_toedge.png";
            public static readonly string CircleGradient = "assets/icons/circle_gradient.png";
            public static readonly string CircleGradientWhite = "assets/icons/circle_gradient_white.png";
            public static readonly string CircleOpen = "assets/icons/circle_open.png";
            public static readonly string Clear = "assets/icons/clear.png";
            public static readonly string ClearList = "assets/icons/clear_list.png";
            public static readonly string Close = "assets/icons/close.png";
            public static readonly string CloseDoor = "assets/icons/close_door.png";
            public static readonly string Clothing = "assets/icons/clothing.png";
            public static readonly string Cold = "assets/icons/cold.png";
            public static readonly string CommunityServers = "assets/icons/community_servers.png";
            public static readonly string Connection = "assets/icons/connection.png";
            public static readonly string Construction = "assets/icons/construction.png";
            public static readonly string Cooking = "assets/icons/cooking.png";
            public static readonly string Crate = "assets/icons/crate.png";
            public static readonly string CupWater = "assets/icons/cup_water.png";
            public static readonly string CursorHand = "assets/icons/cursor-hand.png";
            public static readonly string Deauthorize = "assets/icons/deauthorize.png";
            public static readonly string Demolish = "assets/icons/demolish.png";
            public static readonly string DemolishCancel = "assets/icons/demolish_cancel.png";
            public static readonly string DemolishImmediate = "assets/icons/demolish_immediate.png";
            public static readonly string Discord1 = "assets/icons/discord 1.png";
            public static readonly string Discord = "assets/icons/discord.png";
            public static readonly string Download = "assets/icons/download.png";
            public static readonly string Drop = "assets/icons/drop.png";
            public static readonly string Drowning = "assets/icons/drowning.png";
            public static readonly string Eat = "assets/icons/eat.png";
            public static readonly string Electric = "assets/icons/electric.png";
            public static readonly string Embrella = "assets/icons/embrella.png";
            public static readonly string Enter = "assets/icons/enter.png";
            public static readonly string Examine = "assets/icons/examine.png";
            public static readonly string Exit = "assets/icons/exit.png";
            public static readonly string Explosion = "assets/icons/explosion.png";
            public static readonly string ExplosionSprite = "assets/icons/explosion_sprite.png";
            public static readonly string Extinguish = "assets/icons/extinguish.png";
            public static readonly string FacebookBox = "assets/icons/facebook-box.png";
            public static readonly string Facebook = "assets/icons/facebook.png";
            public static readonly string Facepunch = "assets/icons/facepunch.png";
            public static readonly string Fall = "assets/icons/fall.png";
            public static readonly string FavouriteServers = "assets/icons/favourite_servers.png";
            public static readonly string File = "assets/icons/file.png";
            public static readonly string FlagsAf = "assets/icons/flags/af.png";
            public static readonly string FlagsAr = "assets/icons/flags/ar.png";
            public static readonly string FlagsCa = "assets/icons/flags/ca.png";
            public static readonly string FlagsCs = "assets/icons/flags/cs.png";
            public static readonly string FlagsDa = "assets/icons/flags/da.png";
            public static readonly string FlagsDe = "assets/icons/flags/de.png";
            public static readonly string FlagsEl = "assets/icons/flags/el.png";
            public static readonly string FlagsEnPt = "assets/icons/flags/en-pt.png";
            public static readonly string FlagsEn = "assets/icons/flags/en.png";
            public static readonly string FlagsEsEs = "assets/icons/flags/es-es.png";
            public static readonly string FlagsFi = "assets/icons/flags/fi.png";
            public static readonly string FlagsFr = "assets/icons/flags/fr.png";
            public static readonly string FlagsHe = "assets/icons/flags/he.png";
            public static readonly string FlagsHu = "assets/icons/flags/hu.png";
            public static readonly string FlagsIt = "assets/icons/flags/it.png";
            public static readonly string FlagsJa = "assets/icons/flags/ja.png";
            public static readonly string FlagsKo = "assets/icons/flags/ko.png";
            public static readonly string FlagsNl = "assets/icons/flags/nl.png";
            public static readonly string FlagsNo = "assets/icons/flags/no.png";
            public static readonly string FlagsPl = "assets/icons/flags/pl.png";
            public static readonly string FlagsPtBr = "assets/icons/flags/pt-br.png";
            public static readonly string FlagsPtPt = "assets/icons/flags/pt-pt.png";
            public static readonly string FlagsRo = "assets/icons/flags/ro.png";
            public static readonly string FlagsRu = "assets/icons/flags/ru.png";
            public static readonly string FlagsSr = "assets/icons/flags/sr.png";
            public static readonly string FlagsSvSe = "assets/icons/flags/sv-se.png";
            public static readonly string FlagsTr = "assets/icons/flags/tr.png";
            public static readonly string FlagsUk = "assets/icons/flags/uk.png";
            public static readonly string FlagsVi = "assets/icons/flags/vi.png";
            public static readonly string FlagsZhCn = "assets/icons/flags/zh-cn.png";
            public static readonly string FlagsZhTw = "assets/icons/flags/zh-tw.png";
            public static readonly string Fog = "assets/icons/fog.png";
            public static readonly string Folder = "assets/icons/folder.png";
            public static readonly string FolderUp = "assets/icons/folder_up.png";
            public static readonly string ForkAndSpoon = "assets/icons/fork_and_spoon.png";
            public static readonly string Freezing = "assets/icons/freezing.png";
            public static readonly string FriendsServers = "assets/icons/friends_servers.png";
            public static readonly string Gear = "assets/icons/gear.png";
            public static readonly string Grenade = "assets/icons/grenade.png";
            public static readonly string Greyout = "assets/icons/greyout.png";
            public static readonly string GreyoutLarge = "assets/icons/greyout_large.png";
            public static readonly string Health = "assets/icons/health.png";
            public static readonly string HistoryServers = "assets/icons/history_servers.png";
            public static readonly string Home = "assets/icons/home.png";
            public static readonly string HorseRide = "assets/icons/horse_ride.png";
            public static readonly string Hot = "assets/icons/hot.png";
            public static readonly string Ignite = "assets/icons/ignite.png";
            public static readonly string Info = "assets/icons/info.png";
            public static readonly string Inventory = "assets/icons/inventory.png";
            public static readonly string Isbroken = "assets/icons/isbroken.png";
            public static readonly string Iscooking = "assets/icons/iscooking.png";
            public static readonly string Isloading = "assets/icons/isloading.png";
            public static readonly string Isonfire = "assets/icons/isonfire.png";
            public static readonly string Joystick = "assets/icons/joystick.png";
            public static readonly string Key = "assets/icons/key.png";
            public static readonly string KnockDoor = "assets/icons/knock_door.png";
            public static readonly string LanServers = "assets/icons/lan_servers.png";
            public static readonly string Level = "assets/icons/level.png";
            public static readonly string LevelMetal = "assets/icons/level_metal.png";
            public static readonly string LevelStone = "assets/icons/level_stone.png";
            public static readonly string LevelTop = "assets/icons/level_top.png";
            public static readonly string LevelWood = "assets/icons/level_wood.png";
            public static readonly string Lick = "assets/icons/lick.png";
            public static readonly string LightCampfire = "assets/icons/light_campfire.png";
            public static readonly string Lightbulb = "assets/icons/lightbulb.png";
            public static readonly string Loading = "assets/icons/loading.png";
            public static readonly string Lock = "assets/icons/lock.png";
            public static readonly string Loot = "assets/icons/loot.png";
            public static readonly string Maparrow = "assets/icons/maparrow.png";
            public static readonly string Market = "assets/icons/market.png";
            public static readonly string Maximum = "assets/icons/maximum.png";
            public static readonly string Meat = "assets/icons/meat.png";
            public static readonly string Medical = "assets/icons/medical.png";
            public static readonly string MenuDots = "assets/icons/menu_dots.png";
            public static readonly string ModdedServers = "assets/icons/modded_servers.png";
            public static readonly string Occupied = "assets/icons/occupied.png";
            public static readonly string Open = "assets/icons/open.png";
            public static readonly string OpenDoor = "assets/icons/open_door.png";
            public static readonly string Peace = "assets/icons/peace.png";
            public static readonly string Pickup = "assets/icons/pickup.png";
            public static readonly string Pills = "assets/icons/pills.png";
            public static readonly string PlayerAssist = "assets/icons/player_assist.png";
            public static readonly string PlayerCarry = "assets/icons/player_carry.png";
            public static readonly string PlayerLoot = "assets/icons/player_loot.png";
            public static readonly string Poison = "assets/icons/poison.png";
            public static readonly string Portion = "assets/icons/portion.png";
            public static readonly string Power = "assets/icons/power.png";
            public static readonly string Press = "assets/icons/press.png";
            public static readonly string Radiation = "assets/icons/radiation.png";
            public static readonly string Rain = "assets/icons/rain.png";
            public static readonly string Reddit = "assets/icons/reddit.png";
            public static readonly string Refresh = "assets/icons/refresh.png";
            public static readonly string Resource = "assets/icons/resource.png";
            public static readonly string Rotate = "assets/icons/rotate.png";
            public static readonly string Rust = "assets/icons/rust.png";
            public static readonly string Save = "assets/icons/save.png";
            public static readonly string Shadow = "assets/icons/shadow.png";
            public static readonly string Sign = "assets/icons/sign.png";
            public static readonly string Slash = "assets/icons/slash.png";
            public static readonly string Sleeping = "assets/icons/sleeping.png";
            public static readonly string Sleepingbag = "assets/icons/sleepingbag.png";
            public static readonly string Square = "assets/icons/square.png";
            public static readonly string SquareGradient = "assets/icons/square_gradient.png";
            public static readonly string Stab = "assets/icons/stab.png";
            public static readonly string Star = "assets/icons/star.png";
            public static readonly string Steam = "assets/icons/steam.png";
            public static readonly string SteamInventory = "assets/icons/steam_inventory.png";
            public static readonly string Stopwatch = "assets/icons/stopwatch.png";
            public static readonly string Store = "assets/icons/store.png";
            public static readonly string Study = "assets/icons/study.png";
            public static readonly string Subtract = "assets/icons/subtract.png";
            public static readonly string Target = "assets/icons/target.png";
            public static readonly string Tools = "assets/icons/tools.png";
            public static readonly string Translate = "assets/icons/translate.png";
            public static readonly string Traps = "assets/icons/traps.png";
            public static readonly string Triangle = "assets/icons/triangle.png";
            public static readonly string Tweeter = "assets/icons/tweeter.png";
            public static readonly string Twitter1 = "assets/icons/twitter 1.png";
            public static readonly string Twitter = "assets/icons/twitter.png";
            public static readonly string Unlock = "assets/icons/unlock.png";
            public static readonly string Upgrade = "assets/icons/upgrade.png";
            public static readonly string Voice = "assets/icons/voice.png";
            public static readonly string VoteDown = "assets/icons/vote_down.png";
            public static readonly string VoteUp = "assets/icons/vote_up.png";
            public static readonly string Warning = "assets/icons/warning.png";
            public static readonly string Warning2 = "assets/icons/warning_2.png";
            public static readonly string Weapon = "assets/icons/weapon.png";
            public static readonly string Web = "assets/icons/web.png";
            public static readonly string Wet = "assets/icons/wet.png";
            public static readonly string Workshop = "assets/icons/workshop.png";
            public static readonly string Xp = "assets/icons/xp.png";
        }
    }
}
