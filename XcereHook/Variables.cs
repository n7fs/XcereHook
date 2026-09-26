using Il2Cpp;

using UnityEngine;

namespace XcereHook
{
    /// <summary>
    /// Variables Class for XcereHook
    /// </summary>
    internal class Variables
    {
        /// <summary>
        /// A global System.Random() variable to allow RNG access globally.
        /// </summary>
        public static System.Random Cached_RNG = new System.Random();

        // Disable functions regarding Malformed RPC detections:
        public static bool MalformedRPC_RedirectionBool = false;

        /// <summary>
        /// Default and custom set key-binds for menu and function actions.
        /// </summary>
        internal static class KeyCodeBinds
        {
            public static KeyCode ShowHide_XHMenuBar = KeyCode.Insert;            // Default key-bind to open entire cheat menu is Insert.
            public static KeyCode InstantBombDefuseKey = KeyCode.PageUp;          // Default key-bind to defuse bomb instantly is PageUp.
            public static KeyCode InstantBombPlantKey = KeyCode.PageDown;         // Default key-bind to plant the bomb instantly is PageDown.
            public static KeyCode ShowBombStatisticsKey = KeyCode.E;              // Default key-bind to show bomb statistics is the E key.
            public static KeyCode SpamRadioSFXKey = KeyCode.End;                  // Default key-bind to spam Radio SFX is the End key.
            public static KeyCode UseKnifePrimaryBypassKey = KeyCode.F9;          // Default key-bind to bypass knife drop patch is the F9 key.
            public static KeyCode ReBuyWeaponFromSwitcherKey = KeyCode.Delete;    // Default key-bind to auto-buy weapon from switcher menu is the Delete key.
            public static KeyCode QuickDirectionToLeftAKey = KeyCode.LeftArrow;   // Default key-bind to move left using Quick Direction is the Left Arrow key.
            public static KeyCode QuickDirectionToRightAKey = KeyCode.RightArrow; // Default key-bind to move left using Quick Direction is the Right Arrow key.
            public static KeyCode QuickDirectionToUpAKey = KeyCode.UpArrow;       // Default key-bind to move left using Quick Direction is the Up Arrow key.
            public static KeyCode QuickDirectionToDownAKey = KeyCode.DownArrow;   // Default key-bind to move left using Quick Direction is the Down Arrow key.
            public static KeyCode CustomChatUIKey = KeyCode.Alpha6;               // Default key-bind to use Custom Chat UI is the Alpha Six (6) key.
        }

        /// <summary>
        /// Variables and other details relating to User Interface functionality.
        /// </summary>
        internal static class UI
        {
            public static bool ShowOrHide_XHMenuBar = false;                     // XH Menu Bar (Hiding this will hide all menu UI's.)
            public static bool ShowOrHide_XHMenuBar_Config = false;              // XH Menu Bar Config (A derivative control of XHMenuBar).
            public static bool ShowOrHide_PlayerMenu = false;                    // Player Cheat Menu
            public static bool ShowOrHide_WeaponMenu = false;                    // Weapon Cheat Menu
            public static bool ShowOrHide_RenderMenu = false;                    // Render Cheat Menu
            public static bool ShowOrHide_GameMenu = false;                      // Game Cheat Menu
            public static bool ShowOrHide_PlayerListMenu = false;                // Player List
            public static bool ShowOrHide_WeaponSwitcherMenu = false;            // Weapon Switcher

            public static bool DebuggingMode = false;                            // Enable XcereHook Developer Debugging Mode?

            /// <summary>
            /// Provides an easier way to switch-case between FunctionTabs.
            /// </summary>
            public enum FunctionTab : int
            {
                Player = 0,
                Weapon = 1,
                Render = 2,
                Game = 3,
                PlayerList = 4,
                WeaponSwitcher = 5
            }

            /// <summary>
            /// List of buttons displayed on the XHMenuBar.
            /// </summary>
            public static string[] FunctionTabs = new string[]
            {
                "Player",
                "Weapon",
                "Render",
                "Game",
                "Player List",
                "Weapon Switcher"
            };

            public static int SelectedFunctionTab = 0;                           // What cheat menu are you trying to open?
            public static int SelectedPlayerFromPlayerListIndex = 0;             // Selected Player object as Index number from Player List Menu.

            public static string[] WeaponTypesAsStringArray = Enum.GetNames(typeof(Weapons.WeaponType));
            public static int SelectedIndexFromWeaponListArray = 0;

            public static bool AlreadyPerformed_IsMe = false; // Did IsMe() function already run? Don't re-run, will bottleneck performance.
            public static Il2Cpp.Player? Cached_IsMe = null;  // Pre-cached/pre-computed Player object from IsMe() function.

            public static string HelpDescriptionCurrentDisplayedText = string.Empty; // Help text that is displayed when hovering over cheat.

            public static readonly Rect DEFAULT_UIWindow_Default = new Rect(
                x: 350f,
                y: 350f,
                width: 375f,
                height: 420f
            );

            const float XHMB_Width = 200f;       // UIWindow_XHMenuBar | Width
            const float XHMB_Height = 180f;      // UIWindow_XHMenuBar | Height
            const float XHMB_Margin = 10f;       // UIWindow_XHMenuBar | Margin
            public static Rect UIWindow_XHMenuBar = new Rect(
                x: Screen.width - XHMB_Width - XHMB_Margin,
                y: XHMB_Margin,
                width: XHMB_Width,
                height: XHMB_Height
            );

            const float XHMB_ConfigMenu_Width = 300f;       // UIWindow_XHMenuBar_Config | Width
            const float XHMB_ConfigMenuHeight = 350f;       // UIWindow_XHMenuBar_Config | Height
            const float XHMB_ConfigMenuMargin = 10f;        // UIWindow_XHMenuBar_Config | Margin
            public static Rect UIWindow_XHMenuBar_Config = new Rect(
                x: Screen.width - XHMB_ConfigMenu_Width - XHMB_ConfigMenuMargin,
                y: XHMB_Margin + XHMB_Height + XHMB_ConfigMenuMargin, // Margin below XHMenuBar to keep them organized together by default.
                width: XHMB_ConfigMenu_Width,
                height: XHMB_ConfigMenuHeight
            );

            public static Rect UIWindow_Player = DEFAULT_UIWindow_Default;
            public static Rect UIWindow_Weapon = DEFAULT_UIWindow_Default;
            public static Rect UIWindow_Render = DEFAULT_UIWindow_Default;
            public static Rect UIWindow_Game = DEFAULT_UIWindow_Default;
            public static Rect UIWindow_PlayerList = new Rect(
                x: 350f,
                y: 350f,
                width: 790f,
                height: 570f
            );
            public static Rect UIWindow_WeaponSwitcher = new Rect(
                x: 350f,
                y: 350f,
                width: 195f,
                height: 310f
            );
            public static Rect CustomChatUI = new Rect(
                x: 100f,
                y: 100f,
                width: 425f,
                height: 300f
            );

            public static string CustomChatUI_Input = string.Empty;
            public static bool CustomChatUI_TeamChatSelected = false;
            public static bool CustomChatUI_BotChatSelected = false;

            public static Vector2 PlayerMenu_ScrollPositionOnUI = Vector2.zero;
            public static Vector2 WeaponMenu_ScrollPositionOnUI = Vector2.zero;
            public static Vector2 RenderMenu_ScrollPositionOnUI = Vector2.zero;
            public static Vector2 GameMenu_ScrollPositionOnUI = Vector2.zero;
            public static Vector2 PlayerListMenu_ScrollPositionOnUI = Vector2.zero;
            public static Vector2 WeaponSwitcherMenu_ScrollPositionOnUI = Vector2.zero;
            public static Vector2 XHMenuBar_Config_ScrollPositionOnUI = Vector2.zero;
            public static Vector2 CustomChatUI_ScrollPositionOnUI = Vector2.zero;
        }

        /// <summary>
        /// Variables and other details relating to Player functionality.
        /// </summary>
        internal static class Player
        {
            public static bool EnableFlying = false;
            public static bool EnableNoFallDamage = false;
            public static bool CharacterControllerSqueeze = false;
            public static bool EnableAutoHop = false;

            public static bool EnableSpeedFromJumpMultiplierOverride = false;
            public static float ESFJMO_MultiplierOverrideAmount = 4.25f; // Known safe value that doesn't cause a kick or match ban.

            public static bool NoMovementRestrictions = false;

            public static bool EnableTriggerBot = false;
            public static bool ETB_ShootTeam = false;                 // Do you want to shoot at your own team?
            public static bool ETB_AllowWallBang = false;             // Allow the trigger-bot to shoot past wall-bang layer tagged objects.
            public static bool ETB_AllowExtendedWallBang = false;     // Allow the trigger-bot to shoot past objects that can wall-bang but aren't tagged as such.

            public static string[] INTERNAL_KnownWallBangObjects_NotLayeredAsSuch = new string[] // Known objects that you can wall-bang, but aren't tagged as such.
            {
                "m_Buildings_02",
                "m_Buildings_MASTER",
                "DesertTown_EXT_Props",
                "DesertTown_EXT_Alpha_01",
                "DesertTown_EXT_Props2",
                "DesertTown_EXT_Walls",
                "DesertTown_INT_Floor",
                "BombsiteBounds",
                "Buy Zone",
                "Downtown_A_Doors_EXT_01",
                "Downtown_A_Trims_EXT_01",
                "WinterTown_Trims",
                "WinterTown_Tiling_Ground_",
                "m_Mall_Window",
                "m_Mall_atlas_NoVertex",
                "Default-Material"
            };

            public static bool EnableKnifeBot = false;
            public static bool EKB_UseDotValueEstimation = false;
            public static float EKB_DotValueOffset = -0.5f;           // (-1f, 1f) are the minimum and maximum range values. Default is -0.5f.
            public static bool EKB_CanPlayerMelee = false;

            public static bool EnableQuickDirection = false;
            public static bool EQD_SafetyFromEnemyFire = false;
            public static float EQD_MovementOffsetValue = 55f;        // Default (and minimum) value is 55f.
            public const float EQD_MoveCooldown = 1.15f;              // Abusing QuickDirection will cause a kick or match ban. This sets a usage cooldown.
            public static float EQD_TimeUntilNextQD = 0f;             // Time until next QuickDirection allowance.
            public static float EQD_LastTimeLastHurt = 0f;            // The last 'TimeLastHurt' value that was set.
            public static float EQD_LastHealth = 0f;                  // The last 'Health' value that was set.

            public static bool EnableRadioSFXSpam = false;
            public static float ERSFXS_SpamDelayValue = 10f; // 10 Seconds (Malformed RPC Violation Bypass?)
            public static float ERSFXS_NextAllowedRadioCall = 0f; // Used to determine if enough time has passed to spam Radio SFX again.

            public static bool EnableSpinBot = false;
            public static float SpinbotMalformedRPC_Timeout = 6.55f; // 6.55f seconds before re-spin
            public static float MalformedSyncVar_Timeout = 9f; // 9 seconds before sync var fake
        }

        /// <summary>
        /// Variables and other details relating to Weapon functionality.
        /// </summary>
        internal static class Weapon
        {
            public static bool AllowScopeOnAllWeaponTypes = false;
            public static bool ModifyWeaponPenetration = false;
            public static bool DecreaseAimRecoil = false;
            public static bool DecreaseFlinchRecoil = false;
            public static bool DecreaseCameraRecoil = false;
            public static bool DecreaseWeaponSpread = false;
            public static bool DecreaseWeaponBobbingAndKicking = false;

            public static bool WeaponCanShootGrenades = false;
            public static bool WCSG_ShootFragGrenades = false;
            public static bool WCSG_ShootFlashbangGrenades = false;
            public static bool WCSG_ShootSmokeGrenades = false;
            public static bool WCSG_ShootIncendiaryGrenades = false;

            public static bool KeepDroppedWeapon = false;
            public static bool TreatKnifeAsPrimaryWeaponDropBypass = false;

            public static bool DidWeaponBuyRequestOccur = false;
        }

        /// <summary>
        /// Variables and other details relating to Render functionality.
        /// </summary>
        internal static class Render
        {
            public static bool DoNotFadePlayerDotOnMapRadarESP = false;
            public static bool DisableFlashbangEffects = false;
            public static bool CrosshairAlwaysVisible = false;

            public static bool Draw2DPlayerNameTagESP = false;
            public static bool D2DPNTE_DrawPlayerName = false;
            public static bool D2DPNTE_DrawPlayerHealth = false;
            public static bool D2DPNTE_DrawPlayerHeldWeaponName = false;
            public static bool D2DPNTE_DrawPlayerTeamType = false;

            public static bool Draw2DObjectNameTagESP = false;

            public static bool ShowBombActionStatistics = false;
        }

        /// <summary>
        /// Variables and other details relating to Game functionality.
        /// </summary>
        internal static class Game
        {
            public static bool DisableACTkModules = true;
            public static bool BypassAntiHackFunctions = true;
            public static bool InstantBombDefuse = false;
            public static bool InstantBombPlant = false;
            public static bool UncapFPSAllowUnlimited = true;
            public static bool AllowPlantingBombAnywhere = false;
            public static bool BuyMenuAlwaysAllowBuyingGeneric = false;
            public static bool BuyZoneRemoveZoneRestrictions = false;

            public static float SBAS_BombDefusePercentage = 0f;

            public static bool ResetBombSitePosition_CalledFunction = true; // Set to true to initialize on cheat startup.
            public static Vector3 BombSiteA_Position = Vector3.zero;
            public static Vector3 BombSiteB_Position = Vector3.zero;

            public static bool FakeSyncVarValues = false;

            public static bool UseCustomChatUI = false;
            public static bool UCCUI_ShowChatUI = false;
        }

        /// <summary>
        /// ESP Player Class
        /// </summary>
        public class ESP
        {
            public Vector2 Position;
            public string? Data;
        }

        // TODO: This lags the game terribly. Commented-out until fix developed.

        ///// <summary>
        ///// ESP Object Class
        ///// </summary>
        //public class OESP
        //{
        //    public Vector2 Position;
        //    public string? Data;
        //}

        /// <summary>
        /// ESPD(ata) List Class
        /// </summary>
        public static class ESPD
        {
            public static List<ESP> Players = new();
            //public static List<OESP> Objects = new();
        }
    }
}