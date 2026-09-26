using HarmonyLib;

using Il2Cpp;

using MelonLoader;

using UnityEngine;

using static XcereHook.Utilities;
using static XcereHook.Variables;

namespace XcereHook
{
    /// <summary>
    /// XcereHook Class
    /// </summary>
    public class XcereH : MelonMod
    {
        #region Client UI Watermark Styling & Variables

        public GUIStyle? WatermarkStyle;
        public static GUIStyle Color_PlayerCanMelee = new GUIStyle();

        public Texture2D? BlankTextureA;
        public Texture2D? BlankTextureB;

        public bool AlreadyStyled = false;

        #endregion

        /// <summary>
        /// One-time function to perform style setup for XcereHook UI Watermark-ing.
        /// </summary>
        private void WatermarkStyleLoader()
        {
            if(WatermarkStyle != null || AlreadyStyled)
            {
                return;
            }

            Color_PlayerCanMelee = new GUIStyle(GUI.skin.label);
            Color_PlayerCanMelee.fontSize = 13;
            Color_PlayerCanMelee.alignment = TextAnchor.MiddleLeft;
            Color_PlayerCanMelee.normal.textColor = Color.red;

            BlankTextureA = new Texture2D(width: 1, height: 1);
            BlankTextureA.SetPixel(x: 0, y: 0, Color.black);
            BlankTextureA.Apply();

            BlankTextureB = new Texture2D(width: 1, height: 1);
            BlankTextureB.SetPixel(x: 0, y: 0, Color.white);
            BlankTextureB.Apply();

            WatermarkStyle = new GUIStyle(GUI.skin.label);
            WatermarkStyle.fontSize = 13;
            WatermarkStyle.alignment = TextAnchor.MiddleLeft;
            WatermarkStyle.normal.textColor = Color.white;

            AlreadyStyled = true;
        }

        /// <summary>
        /// Handle actions relating to calling of GUI elements.
        /// </summary>
        public override void OnGUI()
        {
            // To prevent holding input, have a backup Escape key to close Custom Chat UI window.
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Game.UCCUI_ShowChatUI = false;
            }

            if(UI.ShowOrHide_XHMenuBar) // Global toggle for all window visibility. Also used to show menu bar.
            {
                UI.UIWindow_XHMenuBar = GUI.Window(
                    id: 0,                                          // ** XHMenuBar ID 0 **
                    UI.UIWindow_XHMenuBar,
                    (GUI.WindowFunction)Draw_XHMenuBar,
                    text: "XcereHook",
                    GUI.skin.window
                );
            }

            if(UI.ShowOrHide_XHMenuBar && UI.ShowOrHide_XHMenuBar_Config) // Draw XHMenuBar_Config derivative UI.
            {
                // The XcereHook Configuration menu is a derivative of XHMenuBar.
                UI.UIWindow_XHMenuBar_Config = GUI.Window(
                    id: 7,                                          // ** XHMenuBar_Config ID 7 **
                    UI.UIWindow_XHMenuBar_Config,
                    (GUI.WindowFunction)Draw_XHMenuBar_Config,
                    text: "XcereHook Configuration",
                    GUI.skin.window
                );
            }

            if(UI.ShowOrHide_XHMenuBar && UI.ShowOrHide_PlayerMenu) // Draw the Player Menu when asked and only during XHMenuBar visibility.
            {
                UI.UIWindow_Player = GUI.Window(
                    id: 1,                                          // ** PlayerMenu ID 1 **
                    UI.UIWindow_Player,
                    (GUI.WindowFunction)Draw_PlayerMenu,
                    text: "Player Functions Menu",
                    GUI.skin.window
                );
            }

            if(UI.ShowOrHide_XHMenuBar && UI.ShowOrHide_WeaponMenu)
            {
                UI.UIWindow_Weapon = GUI.Window(
                    id: 2,                                          // ** WeaponMenu ID 2 **
                    UI.UIWindow_Weapon,
                    (GUI.WindowFunction)Draw_WeaponMenu,
                    text: "Weapon Functions Menu",
                    GUI.skin.window
                );
            }

            if(UI.ShowOrHide_XHMenuBar && UI.ShowOrHide_RenderMenu)
            {
                UI.UIWindow_Render = GUI.Window(
                    id: 3,                                          // ** RenderMenu ID 3 **
                    UI.UIWindow_Render,
                    (GUI.WindowFunction)Draw_RenderMenu,
                    text: "Render Functions Menu",
                    GUI.skin.window
                );
            }

            if(UI.ShowOrHide_XHMenuBar && UI.ShowOrHide_GameMenu)
            {
                UI.UIWindow_Game = GUI.Window(
                    id: 4,                                          // ** GameMenu ID 4 **
                    UI.UIWindow_Game,
                    (GUI.WindowFunction)Draw_GameMenu,
                    text: "Game Functions Menu",
                    GUI.skin.window
                );
            }

            if(UI.ShowOrHide_XHMenuBar && UI.ShowOrHide_PlayerListMenu)
            {
                UI.UIWindow_PlayerList = GUI.Window(
                    id: 5,                                          // ** PlayerListMenu ID 5 **
                    UI.UIWindow_PlayerList,
                    (GUI.WindowFunction)Draw_PlayerListMenu,
                    text: "Player List Menu",
                    GUI.skin.window
                );

                Game.ResetBombSitePosition_CalledFunction = true;   // If you open the Player List menu, auto-refresh Bomb Site A/B Vector3 variables.
            }

            if(UI.ShowOrHide_XHMenuBar && UI.ShowOrHide_WeaponSwitcherMenu)
            {
                UI.UIWindow_WeaponSwitcher = GUI.Window(
                    id: 6,                                          // ** WeaponSwitcherMenu ID 6 **
                    UI.UIWindow_WeaponSwitcher,
                    (GUI.WindowFunction)Draw_WeaponSwitcherMenu,
                    text: "Weapon Switcher Menu",
                    GUI.skin.window
                );
            }

            if(Render.ShowBombActionStatistics && Input.GetKey(KeyCodeBinds.ShowBombStatisticsKey))
            {
                GUI.Label(
                    new Rect(
                        (Screen.width - 300f) / 2f,
                        (Screen.height - 55f) / 2f - 55f, // 55px @ Center Screen
                        width: 300f,
                        height: 55f
                    ),
                    $"Bomb Defusal Percentage: {Game.SBAS_BombDefusePercentage}%"
                );
            }

            if(Game.UCCUI_ShowChatUI)
            {
                UI.CustomChatUI = GUI.Window(
                    id: 8,
                    UI.CustomChatUI,
                    (GUI.WindowFunction)Draw_CustomChatUI,
                    text: "Custom Chat UI",
                    GUI.skin.window
                );
            }

            if(!UI.AlreadyPerformed_IsMe)
            {
                UI.Cached_IsMe = IsMe();
                UI.AlreadyPerformed_IsMe = true;
            }

            if(UI.Cached_IsMe == null)
            {
                UI.Cached_IsMe = IsMe();
            }

            if(Render.Draw2DPlayerNameTagESP)
            {
                foreach(ESP? Draw in ESPD.Players)
                {
                    GUIContent Content = new GUIContent(Draw.Data);

                    Vector2 Size = GUI.skin.label.CalcSize(Content);

                    GUI.Label(
                        new Rect(
                            Draw.Position.x - Size.x / 2,
                            Draw.Position.y - Size.y,
                            Size.x,
                            Size.y
                        ),
                        Content
                    );
                }
            }

            //// TODO: This lags the game terribly. Commented-out until fix developed.
            //if(Render.Draw2DObjectNameTagESP)
            //{
            //    foreach(OESP? Draw in ESPD.Objects)
            //    {
            //        GUIContent Content = new GUIContent(Draw.Data);

            //        Vector2 Size = GUI.skin.label.CalcSize(Content);

            //        GUI.Label(
            //            new Rect(
            //                Draw.Position.x - Size.x / 2,
            //                Draw.Position.y - Size.y,
            //                Size.x,
            //                Size.y
            //            ),
            //            Content
            //        );
            //    }
            //}

            WatermarkStyleLoader();

            string HookText = string.Empty;

            try
            {
                //HookText = $"[ XcereHook Anti-Malformed-RPC Edition™ ] | XYZ: {UI.Cached_IsMe?.lastPosition} | Username: {UI.Cached_IsMe?.profileName}";
                HookText = $"XcereHook Public | XYZ: {UI.Cached_IsMe?.lastPosition} | Username: {UI.Cached_IsMe?.profileName}";
            }
            catch
            {
                //HookText = $"[ XcereHook Anti-Malformed-RPC Edition™ ]";
                HookText = $"XcereHook Public | Loading...";
            }

            Vector2 HookSize = new Vector2(0f, 0f);

            if(WatermarkStyle != null)
            {
                HookSize = WatermarkStyle.CalcSize(new GUIContent(HookText));
            }

            float Width = HookSize.x + 20;
            float Height = 28;

            Rect Pane = new Rect(
                x: 20,
                Screen.height - Height - 20,
                Width,
                Height
            );

            GUI.DrawTexture(
                new Rect(
                    Pane.x - 1,
                    Pane.y - 1,
                    Pane.width + 2,
                    Pane.height + 2
                ),
                BlankTextureB
            );

            GUI.DrawTexture(
                Pane,
                BlankTextureA
            );

            GUI.Label(
                Pane,
                HookText,
                WatermarkStyle
            );

            // Below handles the Player Can Melee Indicator text as well as Quick Direction Indication text.

            float Remaining = Mathf.Max(0f, Variables.Player.EQD_TimeUntilNextQD - Time.time);
            float Progress = 1f - (Remaining / Variables.Player.EQD_MoveCooldown);

            Progress = Mathf.Clamp01(Progress);

            string IndicatorText = "Quick Direction Ready";

            int Letters = Mathf.FloorToInt(Progress * IndicatorText.Length);

            Letters = Mathf.Clamp(Letters, 0, IndicatorText.Length);

            string Displayed = IndicatorText.Substring(0, Letters);

            Rect QDRectangle = new Rect(
                Pane.x,
                Pane.y - 25,
                Pane.width,
                height: 30
            );

            GUI.Label(
                QDRectangle,
                Displayed,
                Color_PlayerCanMelee
            );

            if(Variables.Player.EKB_CanPlayerMelee)
            {
                Color_PlayerCanMelee.normal.textColor = Color.green;
            }
            else
            {
                Color_PlayerCanMelee.normal.textColor = Color.red;
            }

            const float MeleeHeight = 35;

            Rect MeleeRectangle = new Rect(
                Pane.x,
                Pane.y - MeleeHeight - 5,
                Pane.width,
                MeleeHeight
            );

            GUI.DrawTexture(
                new Rect(
                    MeleeRectangle.x - 1,
                    MeleeRectangle.y - 1,
                    MeleeRectangle.width + 2,
                    MeleeRectangle.height + 2
                ),
                BlankTextureB
            );

            GUI.DrawTexture(
                MeleeRectangle,
                BlankTextureA
            );

            GUI.Label(
                MeleeRectangle,
                $"Player In Melee Range",
                Color_PlayerCanMelee
            );
        }

        /// <summary>
        /// Draw Custom Chat UI
        /// </summary>
        /// <param name="PageID">Not used. Required for UnityEngine related API reference.</param>
        public void Draw_CustomChatUI(int PageID)
        {
            Cursor.lockState = CursorLockMode.None;

            GUI.contentColor = Color.white;

            GUILayout.Space(5);

            UI.CustomChatUI_ScrollPositionOnUI = GUILayout.BeginScrollView(UI.CustomChatUI_ScrollPositionOnUI, GUILayout.ExpandHeight(true));

            GUILayout.BeginVertical();

            GUILayout.Label("Chat Message:");

            GUILayout.BeginHorizontal();

            UI.CustomChatUI_Input = GUILayout.TextArea(
                UI.CustomChatUI_Input,
                GUILayout.Width(275f),
                GUILayout.ExpandWidth(false)
            );

            if(GUILayout.Button("Send Message"))
            {
                if(UI.Cached_IsMe != null)
                {
                    if(UI.CustomChatUI_BotChatSelected)
                    {
                        UI.Cached_IsMe.SendChatMessageToServer(
                            message: UI.CustomChatUI_Input,
                            botChat: UI.CustomChatUI_BotChatSelected,
                            teamChat: false,
                            announcement: true // Never works. Server-sided checks in place. Setting to true doesn't cause issues though.
                        );
                    }

                    if(UI.CustomChatUI_TeamChatSelected)
                    {
                        UI.Cached_IsMe.SendChatMessageToServer(
                            message: UI.CustomChatUI_Input,
                            botChat: false,
                            teamChat: UI.CustomChatUI_TeamChatSelected,
                            announcement: true // Never works. Server-sided checks in place. Setting to true doesn't cause issues though.
                        );
                    }
                }

                Game.UCCUI_ShowChatUI = false;
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(1);

            GUILayout.BeginHorizontal();

            UI.CustomChatUI_TeamChatSelected = GUILayout.Toggle(
                UI.CustomChatUI_TeamChatSelected,
                "Send As Team Chat"
            );

            UI.CustomChatUI_BotChatSelected = GUILayout.Toggle(
                UI.CustomChatUI_BotChatSelected,
                "Send As Bot Chat"
            );

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.EndScrollView();

            GUI.DragWindow(new Rect(
                x: 0f,
                y: 0f,
                width: 10000f,
                height: 25f
            ));
        }

        /// <summary>
        /// Draw XcereHook Menu Bar
        /// </summary>
        /// <param name="PageID">Not used. Required for UnityEngine related API reference.</param>
        public void Draw_XHMenuBar(int PageID)
        {
            GUILayout.BeginArea(new Rect(
                x: 10f,
                y: 25f,
                width: 180f,
                height: 155f
            ));

            GUI.contentColor = Color.white;

            for(int I = 0; I < UI.FunctionTabs.Length; I++)
            {
                if(GUILayout.Button(UI.FunctionTabs[I]))
                {
                    UI.FunctionTab ClickedTab = (UI.FunctionTab)I;

                    switch(ClickedTab)
                    {
                        default:
                        case UI.FunctionTab.Player:
                            UI.ShowOrHide_PlayerMenu = !UI.ShowOrHide_PlayerMenu;
                            break;

                        case UI.FunctionTab.Weapon:
                            UI.ShowOrHide_WeaponMenu = !UI.ShowOrHide_WeaponMenu;
                            break;

                        case UI.FunctionTab.Render:
                            UI.ShowOrHide_RenderMenu = !UI.ShowOrHide_RenderMenu;
                            break;

                        case UI.FunctionTab.Game:
                            UI.ShowOrHide_GameMenu = !UI.ShowOrHide_GameMenu;
                            break;

                        case UI.FunctionTab.PlayerList:
                            UI.ShowOrHide_PlayerListMenu = !UI.ShowOrHide_PlayerListMenu;
                            break;

                        case UI.FunctionTab.WeaponSwitcher:
                            UI.ShowOrHide_WeaponSwitcherMenu = !UI.ShowOrHide_WeaponSwitcherMenu;
                            break;
                    }
                }
            }

            GUILayout.EndArea();

            GUI.DragWindow(new Rect(
                x: 0f,
                y: 0f,
                width: 10000f,
                height: 25f
            ));
        }

        /// <summary>
        /// Draw XcereHook Configuration Menu Bar
        /// </summary>
        /// <param name="PageID">Not used. Required for UnityEngine related API reference.</param>
        public void Draw_XHMenuBar_Config(int PageID)
        {
            GUI.contentColor = Color.white;

            GUILayout.BeginVertical();

            UI.XHMenuBar_Config_ScrollPositionOnUI = GUILayout.BeginScrollView(UI.XHMenuBar_Config_ScrollPositionOnUI, GUILayout.ExpandHeight(true));

            //GUILayout.Label("Configuration Management:");

            //GUILayout.Space(1);

            //GUILayout.Label("The config file you loaded last will auto-load with XcereHook.");
            ////                                                                                       Attempt to anonymize Windows username text.
            //GUILayout.Label($"Configuration files are saved in: {Application.persistentDataPath.Replace(Environment.UserName, "%USERNAME%", StringComparison.OrdinalIgnoreCase)}");

            //// TODO: Create Config Name Input Field

            //if(GUILayout.Button("Load Config"))
            //{
            //    // TODO: Load Config Functionality
            //}

            //if(GUILayout.Button("Save Config"))
            //{
            //    // TODO: Save Config Functionality
            //}

            GUILayout.Space(5);

            GUILayout.Label("Hot-Key Binds:");

            GUILayout.Space(1);

            DrawKeyBind(
                ID: "InstantBombDefuseKey",
                Label: "Instant Bomb Defuse",
                ref KeyCodeBinds.InstantBombDefuseKey
            );

            DrawKeyBind(
                ID: "InstantBombPlantKey",
                Label: "Instant Bomb Plant",
                ref KeyCodeBinds.InstantBombPlantKey
            );

            DrawKeyBind(
                ID: "ShowBombStatisticsKey",
                Label: "Show Bomb Statistics",
                ref KeyCodeBinds.ShowBombStatisticsKey
            );

            DrawKeyBind(
                ID: "SpamRadioSFXKey",
                Label: "Spam Radio SFX",
                ref KeyCodeBinds.SpamRadioSFXKey
            );

            DrawKeyBind(
                ID: "UseKnifePrimaryBypassKey",
                Label: "Use Knife Primary Bypass",
                ref KeyCodeBinds.UseKnifePrimaryBypassKey
            );

            DrawKeyBind(
                ID: "ReBuyWeaponFromSwitcherKey",
                Label: "Re-Buy Weapon From Switcher",
                ref KeyCodeBinds.ReBuyWeaponFromSwitcherKey
            );

            DrawKeyBind(
                ID: "QuickDirectionToLeftAKey",
                Label: "Quick Direction To Left",
                ref KeyCodeBinds.QuickDirectionToLeftAKey
            );

            DrawKeyBind(
                ID: "QuickDirectionToRightAKey",
                Label: "Quick Direction To Right",
                ref KeyCodeBinds.QuickDirectionToRightAKey
            );

            DrawKeyBind(
                ID: "QuickDirectionToUpAKey",
                Label: "Quick Direction To Up",
                ref KeyCodeBinds.QuickDirectionToUpAKey
            );

            DrawKeyBind(
                ID: "QuickDirectionToDownAKey",
                Label: "Quick Direction To Down",
                ref KeyCodeBinds.QuickDirectionToDownAKey
            );

            DrawKeyBind(
                ID: "CustomChatUIKey",
                Label: "Custom Chat UI Key",
                ref KeyCodeBinds.CustomChatUIKey
            );

            GUILayout.Space(5);

            GUILayout.Label($"XcereHook Debugging: {UI.DebuggingMode.ToString().ToUpperInvariant()}");

            GUILayout.EndVertical();

            GUILayout.EndArea();

            GUI.DragWindow(new Rect(
                x: 0f,
                y: 0f,
                width: 10000f,
                height: 25f
            ));
        }

        /// <summary>
        /// Draw Player Menu
        /// </summary>
        /// <param name="PageID">Not used. Required for UnityEngine related API reference.</param>
        public void Draw_PlayerMenu(int PageID)
        {
            GUI.contentColor = Color.white;

            GUILayout.BeginVertical();

            UI.HelpDescriptionCurrentDisplayedText = string.Empty;

            GUILayout.Space(5);

            UI.PlayerMenu_ScrollPositionOnUI = GUILayout.BeginScrollView(UI.PlayerMenu_ScrollPositionOnUI, GUILayout.ExpandHeight(true));

            // BEGIN Drawing UI

            Variables.Player.EnableFlying = ToggleWithHelpOnHover(
                Variables.Player.EnableFlying,
                "Enable Flying",
                "Flying in multiplayer matches may result in a kick or match ban. Use sparingly as flying is detected to an extent."
            );

            GUILayout.Space(5);

            Variables.Player.EnableNoFallDamage = ToggleWithHelpOnHover(
                Variables.Player.EnableNoFallDamage,
                "No Fall Damage Penalty",
                "Bypass fall damage infliction via server by faking ungrounded time values."
            );

            GUILayout.Space(5);

            GUILayout.Label("slopeLimit determines how steep a slope you can climb. radius determines your ability to squeeze into GameObject's.");

            Variables.Player.CharacterControllerSqueeze = ToggleWithHelpOnHover(
                Variables.Player.CharacterControllerSqueeze,
                "Character Controller Squeeze",
                "Increases Character Controller slopeLimit and radius values."
            );

            GUILayout.Space(5);

            Variables.Player.EnableAutoHop = ToggleWithHelpOnHover(
                Variables.Player.EnableAutoHop,
                "Enable Auto-Hop",
                "Allows you to bunny-hop automatically when holding the jump key down."
            );

            GUILayout.Space(5);

            Variables.Player.EnableSpeedFromJumpMultiplierOverride = ToggleWithHelpOnHover(
                Variables.Player.EnableSpeedFromJumpMultiplierOverride,
                "Override Speed From Jump Multiplier",
                "Apply a custom jump multiplier value to increase player speed on jump. High values may kick or match ban."
            );

            GUILayout.Label($"Speed From Jump Multiplier Override Value: {Variables.Player.ESFJMO_MultiplierOverrideAmount.ToString() ?? "N/A"}");

            Variables.Player.ESFJMO_MultiplierOverrideAmount = GUILayout.HorizontalSlider(
                Variables.Player.ESFJMO_MultiplierOverrideAmount,
                1f,
                10f
            );

            GUILayout.Space(5);

            Variables.Player.NoMovementRestrictions = ToggleWithHelpOnHover(
                Variables.Player.NoMovementRestrictions,
                "No Movement Restrictions",
                "Allow movement of the player during restricted scenarios such as match start count-down."
            );

            GUILayout.Space(5);

            GUILayout.Label("Trigger-Bot will fire a ray-cast to check if hitting a Player. If it is, it will auto-shoot your weapon.");

            GUILayout.Space(1);

            GUILayout.Label("To prevent unwanted fire, multiple checks on Player health, Team checks, etc., are performed before fire.");

            Variables.Player.EnableTriggerBot = ToggleWithHelpOnHover(
                Variables.Player.EnableTriggerBot,
                "Enable Trigger-Bot",
                "Automatically shoot Player objects that are valid and ray-cast hit passed."
            );

            GUILayout.Label("If you want the trigger-bot to auto-shoot at friendly/team-mate players:");

            Variables.Player.ETB_ShootTeam = ToggleWithHelpOnHover(
                Variables.Player.ETB_ShootTeam,
                "Trigger-Bot Shoots At Team?",
                "If you want the trigger-bot to shoot at team-mates, enable this option."
            );

            GUILayout.Label("If you want to wall-bang objects tagged with such layer, enable this option:");

            Variables.Player.ETB_AllowWallBang = ToggleWithHelpOnHover(
                Variables.Player.ETB_AllowWallBang,
                "Trigger-Bot Can Wall-Bang",
                "Allow trigger-bot to use alternative function that checks for wall-bang conditions?"
            );

            Variables.Player.ETB_AllowExtendedWallBang = ToggleWithHelpOnHover(
                Variables.Player.ETB_AllowExtendedWallBang,
                "Trigger-Bot Can Wall-Bang (Extended)",
                "Include objects that aren't tagged as wall-bang capable but are known to wall-bang."
            );

            GUILayout.Space(5);

            Variables.Player.EnableKnifeBot = ToggleWithHelpOnHover(
                Variables.Player.EnableKnifeBot,
                "Auto-Knife Bot",
                "Automatically knife when player-to-enemy server melee check is passing."
            );

            Variables.Player.EKB_UseDotValueEstimation = ToggleWithHelpOnHover(
                Variables.Player.EKB_UseDotValueEstimation,
                "Knife Bot Requires Dot Estimation",
                "Auto-Knife Bot needs to be enabled. Use Dot value calculation alongside server check."
            );

            GUILayout.Label($"Auto-Knife Dot Value: {Variables.Player.EKB_DotValueOffset.ToString() ?? "N/A"}");

            Variables.Player.EKB_DotValueOffset = GUILayout.HorizontalSlider(
                Variables.Player.EKB_DotValueOffset,
                -1f,
                1f
            );

            GUILayout.Space(5);

            Variables.Player.EnableQuickDirection = ToggleWithHelpOnHover(
                Variables.Player.EnableQuickDirection,
                "Quick Direction",
                "Use the arrow keys or custom defined keys to make quick movement changes."
            );

            GUILayout.Label($"Quick Direction Movement Shift Value: {Variables.Player.EQD_MovementOffsetValue.ToString() ?? "N/A"}");

            Variables.Player.EQD_MovementOffsetValue = GUILayout.HorizontalSlider(
                Variables.Player.EQD_MovementOffsetValue,
                55f,
                125f
            );

            GUILayout.Label("Quick Direction To Safety may cause the player to go out-of-bounds.");

            Variables.Player.EQD_SafetyFromEnemyFire = ToggleWithHelpOnHover(
                Variables.Player.EQD_SafetyFromEnemyFire,
                "Quick Direction To Safety",
                "Upon enemy fire and hit of player object, shift out of the way automatically."
            );

            GUILayout.Space(5);

            GUILayout.Label("Radio SFX Spamming is delayed every seven seconds to prevent kick or match ban.");
            GUILayout.Label("This value is pre-defined and cannot be changed in XcereHook.");

            Variables.Player.EnableRadioSFXSpam = ToggleWithHelpOnHover(
                Variables.Player.EnableRadioSFXSpam,
                "Enable Radio SFX Spamming",
           "Spam Defusing, Planting, and other Radio SFX using a defined hot-key."
            );

            GUILayout.Space(5);

            GUILayout.Label("Spin-Bot will fake XY angles to the Server. Spectators and other players will see this.");

            Variables.Player.EnableSpinBot = ToggleWithHelpOnHover(
                Variables.MalformedRPC_RedirectionBool,
                "[Patched/Disabled/Private] Enable Character Spin-Bot",
                "Private function not released in public builds."
            //"Detected for Malformed RPC violation 1/0."
            //"Fake XY angles to the server allowing your character to spin wildly."
            );

            // END Drawing UI

            GUILayout.EndScrollView();

            GUIStyle HelpDescriptionBoxStyle = new GUIStyle(GUI.skin.box);
            HelpDescriptionBoxStyle.wordWrap = true;
            HelpDescriptionBoxStyle.fontSize = 12;
            HelpDescriptionBoxStyle.alignment = TextAnchor.MiddleCenter;
            HelpDescriptionBoxStyle.normal.textColor = Color.white;

            float HelpDescriptionBoxHeight = HelpDescriptionBoxStyle.CalcHeight(
                new GUIContent(UI.HelpDescriptionCurrentDisplayedText),
                UI.DEFAULT_UIWindow_Default.width - 30
            );

            GUILayout.Box(
                UI.HelpDescriptionCurrentDisplayedText,
                HelpDescriptionBoxStyle,
                GUILayout.ExpandWidth(true),
                GUILayout.Height(HelpDescriptionBoxHeight + 35)
            );

            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(
                x: 0f,
                y: 0f,
                width: 10000f,
                height: 25f
            ));
        }

        /// <summary>
        /// Draw Weapon Menu
        /// </summary>
        /// <param name="PageID">Not used. Required for UnityEngine related API reference.</param>
        public void Draw_WeaponMenu(int PageID)
        {
            GUI.contentColor = Color.white;

            GUILayout.BeginVertical();

            UI.HelpDescriptionCurrentDisplayedText = string.Empty;

            GUILayout.Space(5);

            UI.WeaponMenu_ScrollPositionOnUI = GUILayout.BeginScrollView(UI.WeaponMenu_ScrollPositionOnUI, GUILayout.ExpandHeight(true));

            // BEGIN Drawing UI

            Variables.Weapon.AllowScopeOnAllWeaponTypes = ToggleWithHelpOnHover(
                Variables.Weapon.AllowScopeOnAllWeaponTypes,
                "Allow Scope On All Weapon Types",
                "Allows you to scope in-or-out on any weapon regardless of weapon type."
            );

            GUILayout.Space(5);

            Variables.Weapon.ModifyWeaponPenetration = ToggleWithHelpOnHover(
                Variables.Weapon.ModifyWeaponPenetration,
                "Modify Weapon Bullet Penetration",
                "Modifies bullet health life when firing through penetrable objects. Might be a placebo effect?"
            );

            GUILayout.Space(5);

            Variables.Weapon.DecreaseAimRecoil = ToggleWithHelpOnHover(
                Variables.Weapon.DecreaseAimRecoil,
                "Decrease Aim Recoil",
                "Decreases the flinch recoil effect applied to the player weapon/camera."
            );

            GUILayout.Space(5);

            Variables.Weapon.DecreaseCameraRecoil = ToggleWithHelpOnHover(
                Variables.Weapon.DecreaseCameraRecoil,
                "Decrease Camera Recoil",
                "Decreases the recoil effect applied to the player camera."
            );

            GUILayout.Space(5);

            Variables.Weapon.DecreaseWeaponSpread = ToggleWithHelpOnHover(
                Variables.Weapon.DecreaseWeaponSpread,
                "Decrease Weapon Spread",
                "Decreases the weapon spread pattern when firing a weapon."
            );

            GUILayout.Space(5);

            Variables.Weapon.DecreaseWeaponBobbingAndKicking = ToggleWithHelpOnHover(
                Variables.Weapon.DecreaseWeaponBobbingAndKicking,
                "Decrease Weapon Bobbing & Kicking",
                "Decreases the weapon bobbing and kicking effect applied to player weapon/camera."
            );

            GUILayout.Space(5);

            Variables.Weapon.WeaponCanShootGrenades = ToggleWithHelpOnHover(
                Variables.Weapon.WeaponCanShootGrenades,
                "Weapon Can Shoot Grenades",
               "Allows you to shoot (a) grenade(s) when firing the weapon. Can cause kick or match ban if abused."
            );

            GUILayout.BeginHorizontal();

            Variables.Weapon.WCSG_ShootFragGrenades = ToggleWithHelpOnHover(
                Variables.Weapon.WCSG_ShootFragGrenades,
                "Frag",
                "Shoot Frag Grenade"
            );

            Variables.Weapon.WCSG_ShootSmokeGrenades = ToggleWithHelpOnHover(
                Variables.Weapon.WCSG_ShootSmokeGrenades,
                "Smoke",
                "Shoot Smoke Grenade"
            );

            Variables.Weapon.WCSG_ShootFlashbangGrenades = ToggleWithHelpOnHover(
                Variables.Weapon.WCSG_ShootFlashbangGrenades,
                "Flashbang",
                "Shoot Flashbang Grenade"
            );

            Variables.Weapon.WCSG_ShootIncendiaryGrenades = ToggleWithHelpOnHover(
                Variables.Weapon.WCSG_ShootIncendiaryGrenades,
                "Incendiary",
                "Shoot Incendiary Grenade"
            );

            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            Variables.Weapon.KeepDroppedWeapon = ToggleWithHelpOnHover(
                Variables.Weapon.KeepDroppedWeapon,
                "Keep Dropped Weapon",
                "Keep your weapon when dropping it instead of losing it."
            );

            GUILayout.Space(5);

            Variables.Weapon.TreatKnifeAsPrimaryWeaponDropBypass = ToggleWithHelpOnHover(
                Variables.Weapon.TreatKnifeAsPrimaryWeaponDropBypass,
                "Treat Knife As Primary Weapon (Drop Knives Bypass)",
                "Allows you to drop knives by treating knife as a primary weapon."
            );

            // END Drawing UI

            GUILayout.EndScrollView();

            GUIStyle HelpDescriptionBoxStyle = new GUIStyle(GUI.skin.box);
            HelpDescriptionBoxStyle.wordWrap = true;
            HelpDescriptionBoxStyle.fontSize = 12;
            HelpDescriptionBoxStyle.alignment = TextAnchor.MiddleCenter;
            HelpDescriptionBoxStyle.normal.textColor = Color.white;

            float HelpDescriptionBoxHeight = HelpDescriptionBoxStyle.CalcHeight(
                new GUIContent(UI.HelpDescriptionCurrentDisplayedText),
                UI.DEFAULT_UIWindow_Default.width - 30
            );

            GUILayout.Box(
                UI.HelpDescriptionCurrentDisplayedText,
                HelpDescriptionBoxStyle,
                GUILayout.ExpandWidth(true),
                GUILayout.Height(HelpDescriptionBoxHeight + 35)
            );

            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(
                x: 0f,
                y: 0f,
                width: 10000f,
                height: 25f
            ));
        }

        /// <summary>
        /// Draw Render Menu
        /// </summary>
        /// <param name="PageID">Not used. Required for UnityEngine related API reference.</param>
        public void Draw_RenderMenu(int PageID)
        {
            GUI.contentColor = Color.white;

            GUILayout.BeginVertical();

            UI.HelpDescriptionCurrentDisplayedText = string.Empty;

            GUILayout.Space(5);

            UI.RenderMenu_ScrollPositionOnUI = GUILayout.BeginScrollView(UI.RenderMenu_ScrollPositionOnUI, GUILayout.ExpandHeight(true));

            // BEGIN Drawing UI

            Render.DoNotFadePlayerDotOnMapRadarESP = ToggleWithHelpOnHover(
                Render.DoNotFadePlayerDotOnMapRadarESP,
                "Do Not Fade Player Dot On Mini-Map (Radar ESP)",
                "Prevents mini-map UI from fading created player dot(s). Radar ESP bypass/workaround."
            );

            GUILayout.Space(5);

            Render.DisableFlashbangEffects = ToggleWithHelpOnHover(
                Render.DisableFlashbangEffects,
                "Disable Flashbang Effects",
                "Safely view flashbang grenade detonation without adverse effects appearing/occurring."
            );

            GUILayout.Space(5);

            Render.CrosshairAlwaysVisible = ToggleWithHelpOnHover(
                Render.CrosshairAlwaysVisible,
                "Crosshair Always Visible",
                "Keep the crosshair visible regardless of weapon type. Allow knives to have crosshair."
            );

            GUILayout.Space(5);

            Render.Draw2DPlayerNameTagESP = ToggleWithHelpOnHover(
                Render.Draw2DPlayerNameTagESP,
                "Draw 2D Player Name Tag ESP",
                "Draw text with information about each Player object in 2D UI space."
            );

            GUILayout.BeginHorizontal();

            Render.D2DPNTE_DrawPlayerName = ToggleWithHelpOnHover(
                Render.D2DPNTE_DrawPlayerName,
                "Name",
                "Draw Player Name in 2D Name Tag ESP"
            );

            Render.D2DPNTE_DrawPlayerHealth = ToggleWithHelpOnHover(
                Render.D2DPNTE_DrawPlayerHealth,
                "Health",
                "Draw Player Health in 2D Name Tag ESP"
            );

            Render.D2DPNTE_DrawPlayerHeldWeaponName = ToggleWithHelpOnHover(
                Render.D2DPNTE_DrawPlayerHeldWeaponName,
                "Weapon",
                "Draw Player Held Weapon in 2D Name Tag ESP"
            );

            Render.D2DPNTE_DrawPlayerTeamType = ToggleWithHelpOnHover(
                Render.D2DPNTE_DrawPlayerTeamType,
                "Team",
                "Draw Player Team Type in 2D Name Tag ESP"
            );

            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            //GUILayout.Label("2D Object ESP only shows Thrown Grenades and the Bomb/C4 object (thrown or planted).");

            //Render.Draw2DObjectNameTagESP = ToggleWithHelpOnHover(
            //    Variables.MalformedRPC_RedirectionBool, // Re-used. This didn't ban for Malformed RPC violations but caused performance issues.
            //    "[DISABLED] Draw 2D Object Name Tag ESP",
            //    "DISABLED FOR PERFORMANCE REASONS"
            ////"Draw text with information about each Thrown Grenade and Bomb/C4 object in 2D UI space."
            //);

            GUILayout.Space(5);

            Render.ShowBombActionStatistics = ToggleWithHelpOnHover(
                Render.ShowBombActionStatistics,
                "Show Bomb Statistics",
                "Show bomb-related statistics when performing bomb-related actions"
            );

            // END Drawing UI

            GUILayout.EndScrollView();

            GUIStyle HelpDescriptionBoxStyle = new GUIStyle(GUI.skin.box);
            HelpDescriptionBoxStyle.wordWrap = true;
            HelpDescriptionBoxStyle.fontSize = 12;
            HelpDescriptionBoxStyle.alignment = TextAnchor.MiddleCenter;
            HelpDescriptionBoxStyle.normal.textColor = Color.white;

            float HelpDescriptionBoxHeight = HelpDescriptionBoxStyle.CalcHeight(
                new GUIContent(UI.HelpDescriptionCurrentDisplayedText),
                UI.DEFAULT_UIWindow_Default.width - 30
            );

            GUILayout.Box(
                UI.HelpDescriptionCurrentDisplayedText,
                HelpDescriptionBoxStyle,
                GUILayout.ExpandWidth(true),
                GUILayout.Height(HelpDescriptionBoxHeight + 35)
            );

            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(
                x: 0f,
                y: 0f,
                width: 10000f,
                height: 25f
            ));
        }

        /// <summary>
        /// Draw Game Menu
        /// </summary>
        /// <param name="PageID">Not used. Required for UnityEngine related API reference.</param>
        public void Draw_GameMenu(int PageID)
        {
            GUI.contentColor = Color.white;

            GUILayout.BeginVertical();

            UI.HelpDescriptionCurrentDisplayedText = string.Empty;

            GUILayout.Space(5);

            UI.GameMenu_ScrollPositionOnUI = GUILayout.BeginScrollView(UI.GameMenu_ScrollPositionOnUI, GUILayout.ExpandHeight(true));

            // BEGIN Drawing UI

            Game.DisableACTkModules = ToggleWithHelpOnHover(
                Game.DisableACTkModules,
                "Disable Anti-Cheat Tool-Kit (ACTk) Modules",
                "Attempts to force-disable client-sided Anti-Cheat Tool-Kit (ACTk) modules."
            );

            GUILayout.Space(5);

            Game.BypassAntiHackFunctions = ToggleWithHelpOnHover(
                Game.BypassAntiHackFunctions,
                "Bypass Client-Sided Anti-Hack Functions",
                "Attempts to disable various client-sided anti-hack/anti-cheat implementations."
            );

            GUILayout.Space(5);

            Game.InstantBombDefuse = ToggleWithHelpOnHover(
                Variables.Game.InstantBombDefuse,
                "Instant Bomb Defuse On Key",
            "Instantly defuse the bomb from anywhere on the map when on CT side."
            );

            GUILayout.Space(5);

            Game.InstantBombPlant = ToggleWithHelpOnHover(
                Variables.Game.InstantBombPlant,
                "Instant Bomb Plant On Key",
            "Instantly plant the bomb when on site (or from anywhere)*."
            );

            GUILayout.Space(5);

            Game.AllowPlantingBombAnywhere = ToggleWithHelpOnHover(
                Variables.Game.AllowPlantingBombAnywhere,
                "Allow Planting Bomb Anywhere",
            "Ignore pre-defined bomb-zone; allow planting anywhere on the map."
            );

            GUILayout.Space(5);

            Game.UncapFPSAllowUnlimited = ToggleWithHelpOnHover(
                Game.UncapFPSAllowUnlimited,
                "Uncap FPS - Allow Past 240 FPS",
                "Allows unlimited FPS; removes capped 240 FPS maximum limit."
            );

            GUILayout.Space(5);

            Game.BuyMenuAlwaysAllowBuyingGeneric = ToggleWithHelpOnHover(
                Game.BuyMenuAlwaysAllowBuyingGeneric,
                "Buy Menu - Always Allow Buying Generic",
                "Allows you to force the buy generic weapon check to always return true."
            );

            GUILayout.Space(5);

            Game.BuyZoneRemoveZoneRestrictions = ToggleWithHelpOnHover(
                Game.BuyZoneRemoveZoneRestrictions,
                "Buy Zone - Remove Zone Restrictions",
                "Ignore pre-defined buy-menu zone; allow opening buy-menu anywhere."
            );

            GUILayout.Space(5);

            Game.FakeSyncVarValues = ToggleWithHelpOnHover(
                Variables.MalformedRPC_RedirectionBool,
                "[Patched/Disabled/Private] Send Fake SyncVar Values",
                "Private function not released in public builds."
            //"Detected for Malformed RPC violation 1/0."
            //"Send randomized SyncVar variables and values to the server."
            );

            GUILayout.Space(5);

            GUILayout.Label("Custom Chat UI allows you to change the way messages appear, providing more functionality.");

            GUILayout.Label("You can open the custom chat UI using the same key-bind you have to open the in-game chat.");

            Game.UseCustomChatUI = ToggleWithHelpOnHover(
                Game.UseCustomChatUI,
                "Use Custom Chat UI",
                "Custom chat UI that directly talks to message sending function call."
            );

            // END Drawing UI

            GUILayout.EndScrollView();

            GUIStyle HelpDescriptionBoxStyle = new GUIStyle(GUI.skin.box);
            HelpDescriptionBoxStyle.wordWrap = true;
            HelpDescriptionBoxStyle.fontSize = 12;
            HelpDescriptionBoxStyle.alignment = TextAnchor.MiddleCenter;
            HelpDescriptionBoxStyle.normal.textColor = Color.white;

            float HelpDescriptionBoxHeight = HelpDescriptionBoxStyle.CalcHeight(
                new GUIContent(UI.HelpDescriptionCurrentDisplayedText),
                UI.DEFAULT_UIWindow_Default.width - 30
            );

            GUILayout.Box(
                UI.HelpDescriptionCurrentDisplayedText,
                HelpDescriptionBoxStyle,
                GUILayout.ExpandWidth(true),
                GUILayout.Height(HelpDescriptionBoxHeight + 35)
            );

            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(
                x: 0f,
                y: 0f,
                width: 10000f,
                height: 25f
            ));
        }

        /// <summary>
        /// To keep from bottle-necking performance, attempt to store cached Player objects.
        /// </summary>
        public static Il2Cpp.Player[] _Players = Array.Empty<Il2Cpp.Player>();

        /// <summary>
        /// To keep from bottle-necking performance, cache Player object's 'profileName' variable as a string.
        /// </summary>
        public static string[] _PlayerNames = Array.Empty<string>();

        /// <summary>
        /// Safe way to repopulate list of Player objects without choking performance.
        /// </summary>
        private void RefreshPlayersInPlayerListSafely()
        {
            _Players = IsAll();
            _PlayerNames = new string[_Players.Length];

            for(int I = 0; I < _Players.Length; I++)
            {
                _PlayerNames[I] = _Players[I].profileName;
            }

            if(UI.SelectedPlayerFromPlayerListIndex >= _Players.Length)
            {
                UI.SelectedPlayerFromPlayerListIndex = 0;
            }
        }

        /// <summary>
        /// Draw Player List Menu
        /// </summary>
        /// <param name="PageID">Not used. Required for UnityEngine related API reference.</param>
        public void Draw_PlayerListMenu(int PageID)
        {
            GUI.contentColor = Color.white;

            GUILayout.BeginVertical();

            UI.HelpDescriptionCurrentDisplayedText = string.Empty;

            GUILayout.Space(5);

            UI.PlayerListMenu_ScrollPositionOnUI = GUILayout.BeginScrollView(UI.PlayerListMenu_ScrollPositionOnUI, GUILayout.ExpandHeight(true));

            // BEGIN Drawing UI

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(195));

            if(GUILayout.Button("Refresh Player List"))
            {
                try
                {
                    RefreshPlayersInPlayerListSafely();
                }
                catch
                {
                    // Failed to refresh Player object list. Just ignore and wait for manual refresh call.
                }
            }

            GUILayout.Space(5);

            GUILayout.Label("Player List:");

            GUILayout.Space(5);

            UI.SelectedPlayerFromPlayerListIndex = GUILayout.SelectionGrid(
                UI.SelectedPlayerFromPlayerListIndex,
                _PlayerNames,
                1
            );

            GUILayout.EndVertical();

            GUILayout.Box(
                GUIContent.none,
                GUILayout.Width(7f),
                GUILayout.ExpandHeight(true)
            );

            GUILayout.Space(10);

            GUILayout.BeginVertical();

            GUILayout.Label("- Player Information -");

            GUILayout.Space(5);

            if(_PlayerNames.Length > 0 && UI.SelectedPlayerFromPlayerListIndex >= 0 && UI.SelectedPlayerFromPlayerListIndex < _Players.Length && UI.SelectedPlayerFromPlayerListIndex < _Players.Length)
            {
                try
                {
                    Il2Cpp.Player SelectedPlayer = _Players[UI.SelectedPlayerFromPlayerListIndex];

                    if(GUILayout.Button("Teleport To Player [lastLivingPosition]"))
                    {
                        try
                        {
                            if(UI.Cached_IsMe != null && UI.SelectedPlayerFromPlayerListIndex >= 0 && UI.SelectedPlayerFromPlayerListIndex < _Players.Length)
                            {
                                UI.Cached_IsMe.localPlayer.transform.position = _Players[UI.SelectedPlayerFromPlayerListIndex].lastLivingPosition;
                            }
                        }
                        catch
                        {
                            // Ignore any errors relating to Teleport To Player. Allow other data to be populated.
                        }
                    }

                    // Only show options relative to your Player object. These options aren't available otherwise.
                    if(SelectedPlayer == UI.Cached_IsMe)
                    {
                        if(GUILayout.Button("Teleport Above Map (Requires No Fall Damage Enabled)"))
                        {
                            try
                            {
                                if(UI.Cached_IsMe != null)
                                {
                                    UI.Cached_IsMe.localPlayer.transform.position = new Vector3(
                                        x: UI.Cached_IsMe.localPlayer.transform.position.x,
                                        y: UI.Cached_IsMe.localPlayer.transform.position.y * 35f, // y*35f should be plenty to reach the top of the map.
                                        z: UI.Cached_IsMe.localPlayer.transform.position.z
                                    );
                                }
                            }
                            catch
                            {
                                // Ignore any errors relating to Teleport Above Map. Allow other data to be populated.
                            }
                        }

                        if(GUILayout.Button("Teleport Bomb Site A"))
                        {
                            if(UI.Cached_IsMe != null)
                            {
                                UI.Cached_IsMe.localPlayer.transform.position = new Vector3(
                                    Game.BombSiteA_Position.x,
                                    Game.BombSiteA_Position.y + 5f, // Prevent being forced into the ground causing a stuck condition.
                                    Game.BombSiteA_Position.z
                                );
                            }
                        }

                        if(GUILayout.Button("Teleport Bomb Site B"))
                        {
                            if(UI.Cached_IsMe != null)
                            {
                                UI.Cached_IsMe.localPlayer.transform.position = new Vector3(
                                    Game.BombSiteB_Position.x,
                                    Game.BombSiteB_Position.y + 5f, // Prevent being forced into the ground causing a stuck condition.
                                    Game.BombSiteB_Position.z
                                );
                            }
                        }
                    }

                    GUILayout.Label($"PlayersOnSameTeam (As You): {PlayerUtils.PlayersOnSameTeam(UI.Cached_IsMe, SelectedPlayer).ToString().ToUpperInvariant() ?? "N/A"}");
                    GUILayout.Label($"PlayerOnLocalTeamOrSpectating: {PlayerUtils.PlayerOnLocalTeamOrSpectating(SelectedPlayer).ToString().ToUpperInvariant() ?? "N/A"}");
                    GUILayout.Label($"accountUsername: {SelectedPlayer.accountUsername ?? "N/A"}");
                    GUILayout.Label($"profileName: {SelectedPlayer.profileName ?? "N/A"}");
                    GUILayout.Label($"playerID: {SelectedPlayer.playerID.ToString() ?? "N/A"}");
                    GUILayout.Label($"aiming: {SelectedPlayer.aiming.ToString().ToUpperInvariant() ?? "N/A"}");
                    GUILayout.Label($"assists: {SelectedPlayer.assists.ToString() ?? "N/A"}");
                    GUILayout.Label($"chambering: {SelectedPlayer.chambering.ToString().ToUpperInvariant() ?? "N/A"}");
                    GUILayout.Label($"Crouching: {SelectedPlayer.Crouching.ToString().ToUpperInvariant() ?? "N/A"}");
                    GUILayout.Label($"crouching: {SelectedPlayer.crouching.ToString().ToUpperInvariant() ?? "N/A"}");
                    GUILayout.Label($"currentAmmoInMagazine: {SelectedPlayer.currentAmmoInMagazine.ToString() ?? "N/A"}");
                    GUILayout.Label($"currentAmmoNotInMagazine: {SelectedPlayer.currentAmmoNotInMagazine.ToString() ?? "N/A"}");
                    GUILayout.Label($"deaths: {SelectedPlayer.deaths.ToString() ?? "N/A"}");
                    GUILayout.Label($"grounded: {SelectedPlayer.grounded.ToString().ToUpperInvariant() ?? "N/A"}");
                    GUILayout.Label($"HasArmor: {SelectedPlayer.HasArmor.ToString().ToUpperInvariant() ?? "N/A"}");
                    GUILayout.Label($"headshotted: {SelectedPlayer.headshotted.ToString().ToUpperInvariant() ?? "N/A"}");
                    GUILayout.Label($"health: {SelectedPlayer.health.ToString() ?? "N/A"}");
                    GUILayout.Label($"holdingGrenade: {SelectedPlayer.holdingGrenade.ToString().ToUpperInvariant() ?? "N/A"}");
                    GUILayout.Label($"inspecting: {SelectedPlayer.inspecting.ToString().ToUpperInvariant() ?? "N/A"}");
                    GUILayout.Label($"IsBot: {SelectedPlayer.IsBot.ToString().ToUpperInvariant() ?? "N/A"}");
                    GUILayout.Label($"isMine: {SelectedPlayer.isMine.ToString().ToUpperInvariant() ?? "N/A"}");
                    GUILayout.Label($"IsHost: {SelectedPlayer.IsHost.ToString().ToUpperInvariant() ?? "N/A"}");
                    GUILayout.Label($"IsOfflinePlayer: {SelectedPlayer.IsOfflinePlayer.ToString().ToUpperInvariant() ?? "N/A"}");
                    GUILayout.Label($"IsServerBot: {SelectedPlayer.IsServerBot.ToString().ToUpperInvariant() ?? "N/A"}");
                    GUILayout.Label($"money: {SelectedPlayer.money.ToString() ?? "N/A"}");
                    GUILayout.Label($"kills: {SelectedPlayer.kills.ToString() ?? "N/A"}");
                    GUILayout.Label($"lastLivingPosition: {SelectedPlayer.lastLivingPosition.ToString() ?? "N/A"}");
                    GUILayout.Label($"ping: {SelectedPlayer.ping.ToString() ?? "N/A"}");
                    GUILayout.Label($"weapon.name: {SelectedPlayer.weapon?.name.ToString() ?? "N/A"}");
                    GUILayout.Label($"reloading: {SelectedPlayer.reloading.ToString().ToUpperInvariant() ?? "N/A"}");
                    GUILayout.Label($"score: {SelectedPlayer.score.ToString() ?? "N/A"}");
                    GUILayout.Label($"shooting: {SelectedPlayer.shooting.ToString().ToUpperInvariant() ?? "N/A"}");
                    GUILayout.Label($"Spectating: {SelectedPlayer.Spectating.ToString().ToUpperInvariant() ?? "N/A"}");
                    GUILayout.Label($"throwing: {SelectedPlayer.throwing.ToString().ToUpperInvariant() ?? "N/A"}");
                }
                catch
                {
                    GUILayout.Label($"Select a Player to view information about them.");
                }
            }
            else
            {
                GUILayout.Label($"Select a Player to view information about them.");
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            // END Drawing UI

            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(
                x: 0f,
                y: 0f,
                width: 10000f,
                height: 25f
            ));
        }

        /// <summary>
        /// Draw Weapon Switcher Menu
        /// </summary>
        /// <param name="PageID">Not used. Required for UnityEngine related API reference.</param>
        public void Draw_WeaponSwitcherMenu(int PageID)
        {
            GUI.contentColor = Color.white;

            GUILayout.BeginVertical();

            UI.HelpDescriptionCurrentDisplayedText = string.Empty;

            GUILayout.Space(5);

            UI.WeaponSwitcherMenu_ScrollPositionOnUI = GUILayout.BeginScrollView(UI.WeaponSwitcherMenu_ScrollPositionOnUI, GUILayout.ExpandHeight(true));

            // BEGIN Drawing UI

            GUILayout.Label($"Selected: {UI.WeaponTypesAsStringArray[UI.SelectedIndexFromWeaponListArray]}");

            GUILayout.Space(5);

            for(int I = 0; I < UI.WeaponTypesAsStringArray.Length; I++)
            {
                // Attempting to purchase the Bomb weapon type doesn't do what you think it does.
                // Instead, it causes IL2CPP errors that flood the MelonLoader Console. Nothing helpful.
                if(UI.WeaponTypesAsStringArray[I].Contains("Bomb", StringComparison.OrdinalIgnoreCase) || UI.WeaponTypesAsStringArray[I].Contains("C4", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if(GUILayout.Button(UI.WeaponTypesAsStringArray[I]))
                {
                    UI.SelectedIndexFromWeaponListArray = I;

                    Variables.Weapon.DidWeaponBuyRequestOccur = true;
                }
            }

            // END Drawing UI

            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(
                x: 0f,
                y: 0f,
                width: 10000f,
                height: 25f
            ));
        }

        /// <summary>
        /// Handle Input events and other functionality. Use sparingly to avoid bottlenecking game performance.
        /// </summary>
        public override void OnUpdate()
        {
            if(Input.GetKeyDown(KeyCodeBinds.ShowHide_XHMenuBar))
            {
                UI.ShowOrHide_XHMenuBar = !UI.ShowOrHide_XHMenuBar;
                UI.ShowOrHide_XHMenuBar_Config = !UI.ShowOrHide_XHMenuBar_Config;
            }

            if(Input.GetKeyDown(KeyCodeBinds.CustomChatUIKey))
            {
                Game.UCCUI_ShowChatUI = !Game.UCCUI_ShowChatUI;
            }
        }

        /// <summary>
        /// Verify that XcereHook was successfully registered and loaded by MelonLoader via hooking of the OnInitializeMelon function.
        /// </summary>
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg(
                $"<{DateTime.Now:T}> XcereHook was successfully registered and loaded. Developed and maintained by n7fs (n0xcere)."
            );

            LoggerInstance.Msg(
                $"<{DateTime.Now:T}> [1/2] You're using the Public build of XcereHook with Private features removed."
            );

            LoggerInstance.Msg(
                $"<{DateTime.Now:T}> [2/2] You're using the Public build of XcereHook with Private features removed."
            );

            //LoggerInstance.Msg(
            //    $"<{DateTime.Now:T}> [1/3] Certain XcereHook features have been disabled due to banning for Malformed RPC Violations."
            //);
            //LoggerInstance.Msg(
            //    $"<{DateTime.Now:T}> [2/3] Certain XcereHook features have been disabled due to banning for Malformed RPC Violations."
            //);
            //LoggerInstance.Msg(
            //    $"<{DateTime.Now:T}> [3/3] Certain XcereHook features have been disabled due to banning for Malformed RPC Violations."
            //);
        }

        // NOTICE:
        // Below is the patches made to IL2CPP functions that help XcereHook function.

        // SCROLLING TOO FAR:
        // Below is the patches made to IL2CPP functions that help XcereHook function.

        // READ ME:
        // Below is the patches made to IL2CPP functions that help XcereHook function.

        // LAST NOTICE:
        // Below is the patches made to IL2CPP functions that help XcereHook function.

        /// <summary>
        /// BombUI IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(BombUI), "Update")]
        class BombUI_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Inner-Variables/Functions</param>
            static void Prefix(BombUI __instance)
            {
                if(Render.ShowBombActionStatistics)
                {
                    Game.SBAS_BombDefusePercentage = __instance._defusingPercentage;
                }
            }
        }

        /// <summary>
        /// FirstPersonLook (2D Player Name Tag ESP) IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(FirstPersonLook), "Update")]
        class FirstPersonLook_2DPlayerNameTagESP_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Inner-Variables/Functions</param>
            static void Prefix(FirstPersonLook __instance)
            {
                // TODO: This is causing major performance issues when calling Resource.FindObjectsOfTypeAll<>() function!
                // TODO: Need to do lots of optimization here. Cache GameObject's, draw-call less every frame, etc.

                //// TODO: This lags the game terribly. Commented-out until fix developed.
                ESPD.Players.Clear();
                //ESPD.Objects.Clear();

                string MyProfileName = AccountManager.GetProfileName() ?? "N/A";

                Il2Cpp.Player? MyPlayer = UI.Cached_IsMe;

                if(MyPlayer == null)
                {
                    return;
                }

                Camera? MyCamera = MyPlayer.GetComponentInChildren<Camera>(true);

                if(MyCamera == null)
                {
                    return;
                }

                //// TODO: This lags the game terribly. Commented-out until fix developed.
                //foreach(GameObject GO in IsBombObjectOrThrownGrenade())
                //{
                //    Vector3 ScreenPosition = MyCamera.WorldToScreenPoint(GO.transform.position);

                //    if(ScreenPosition.z <= 0)
                //    {
                //        continue;
                //    }

                //    ESPD.Objects.Add(new OESP()
                //    {
                //        Position = new Vector2(
                //            ScreenPosition.x,
                //            Screen.height - ScreenPosition.y
                //        ),
                //        Data = GO.name.Replace("(Clone)", string.Empty, StringComparison.OrdinalIgnoreCase)
                //    });
                //}

                foreach(Il2Cpp.Player? PlayerTarget in UnityEngine.Resources.FindObjectsOfTypeAll<Il2Cpp.Player>())
                {
                    if(PlayerTarget == null || PlayerTarget == MyPlayer)
                    {
                        continue;
                    }

                    Collider? ColliderObject = PlayerTarget.GetComponentInChildren<Collider>();

                    if(ColliderObject == null)
                    {
                        continue;
                    }

                    Bounds Boundary = ColliderObject.bounds;

                    Vector3 Head2World = new Vector3(
                        Boundary.center.x,
                        Boundary.max.y + 0.2f,
                        Boundary.center.z
                    );

                    Vector3 Head = MyCamera.WorldToScreenPoint(Head2World);

                    if(Head.z <= 0)
                    {
                        continue;
                    }

                    // Player is dead or has no health? Player has no valid profileName? Server/Client GameObject that needs to be ignored.
                    // Sometimes, an empty, blank GameObject will be placed somewhere in the World that isn't valid to shoot at. Ignore drawing it.
                    if(PlayerTarget.health == 0 && string.IsNullOrWhiteSpace(PlayerTarget.profileName))
                    {
                        continue;
                    }

                    // Player is dead? Has a valid profileName? Player has died, stop rendering Name Tag ESP on the body object.
                    // No point in rendering a dead player object. Waste of performance and bottlenecks rendering.
                    if(PlayerTarget.health == 0 && !string.IsNullOrWhiteSpace(PlayerTarget.profileName))
                    {
                        continue;
                    }

                    if(Il2Cpp.Player.IsServerMeleeDamageWithinDistance(MyPlayer, PlayerTarget) && Variables.Player.EnableKnifeBot)
                    {
                        switch(MyPlayer.weapon.type)
                        {
                            case Weapons.WeaponType.Beretta:
                            case Weapons.WeaponType.BrassKnuckles:
                            case Weapons.WeaponType.ButterflyKnife:
                            case Weapons.WeaponType.Karambit:
                            case Weapons.WeaponType.Knife:
                            case Weapons.WeaponType.KukriKnife:
                            case Weapons.WeaponType.TecmixKnife:
                                Vector3 Me = (MyPlayer.transform.position - PlayerTarget.transform.position).normalized;

                                float DotValue = Vector3.Dot(PlayerTarget.transform.forward, Me);

                                bool IsDotCalculationInSpecification = DotValue < Variables.Player.EKB_DotValueOffset;

                                if(IsDotCalculationInSpecification && Variables.Player.EKB_UseDotValueEstimation)
                                {
                                    Variables.Player.EKB_CanPlayerMelee = true;
                                    MyPlayer.localPlayer.TryShoot();
                                }
                                else if(!IsDotCalculationInSpecification)
                                {
                                    Variables.Player.EKB_CanPlayerMelee = true;
                                    MyPlayer.localPlayer.TryShoot();
                                }
                                else
                                {
                                    Variables.Player.EKB_CanPlayerMelee = false;
                                }
                                break;
                        }
                    }
                    else
                    {
                        Variables.Player.EKB_CanPlayerMelee = false;
                    }

                    string CraftedDataString = string.Empty;

                    if(Render.D2DPNTE_DrawPlayerName)
                    {
                        CraftedDataString += $"{PlayerTarget.profileName}{Environment.NewLine}";
                    }

                    if(Render.D2DPNTE_DrawPlayerHealth)
                    {
                        CraftedDataString += $"{PlayerTarget.health} HP{Environment.NewLine}";
                    }

                    if(Render.D2DPNTE_DrawPlayerHeldWeaponName)
                    {
                        CraftedDataString += $"{PlayerTarget.weapon.name ?? "Weapon Name Unavailable"}{Environment.NewLine}";
                    }

                    if(Render.D2DPNTE_DrawPlayerTeamType)
                    {
                        CraftedDataString += $"Team: {PlayerUtils.PlayersOnSameTeam(PlayerTarget, MyPlayer).ToString().ToUpperInvariant()}{Environment.NewLine}";
                    }

                    ESPD.Players.Add(new ESP()
                    {
                        Position = new Vector2(
                            Head.x,
                            Screen.height - Head.y
                        ),
                        Data = CraftedDataString
                    });
                }
            }
        }

        /// <summary>
        /// Crosshair IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(Crosshair), "LateUpdate")]
        class Crosshair_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Inner-Variables/Functions</param>
            static void Prefix(Crosshair __instance)
            {
                if(Render.CrosshairAlwaysVisible)
                {
                    __instance.visible = true;
                    __instance.Visible = true;
                }
            }
        }

        /// <summary>
        /// HUDManager IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(HUDManager), "Update")]
        class HUDManager_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Inner-Variables/Functions</param>
            static void Prefix(HUDManager __instance)
            {
                if(Render.DisableFlashbangEffects)
                {
                    __instance.ClearFlash();

                    __instance.flashbangOverlay.color = new Color(r: 0, g: 0, b: 0, a: 0);

                    __instance.timeToFlash = 0f;
                    __instance.timeFlashed = 0f;
                }
            }
        }

        /// <summary>
        /// Bomb IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(Bomb), "LateUpdate")]
        class Bomb_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static void Prefix(Bomb __instance)
            {
                if(Game.ResetBombSitePosition_CalledFunction)
                {
                    Game.BombSiteA_Position = __instance.bombSiteBounds[0].center;
                    Game.BombSiteB_Position = __instance.bombSiteBounds[1].center;

                    Game.ResetBombSitePosition_CalledFunction = false; // Reset back to prevent re-calling and bottle-necking performance.
                }
            }
        }

        /// <summary>
        /// Bomb.CanPlantBomb IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(Bomb), nameof(Bomb.CanPlantBomb))]
        class Bomb_CanPlantBomb_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__result">Expected/Intended Result (To Override)</param>
            static void Prefix(ref bool __result)
            {
                if(Game.AllowPlantingBombAnywhere)
                {
                    __result = true;
                }
            }

            /// <summary>
            /// Hook Function (After Call)
            /// </summary>
            /// <param name="__result">Expected/Intended Result (To Override)</param>
            static void Postfix(ref bool __result)
            {
                if(Game.AllowPlantingBombAnywhere)
                {
                    __result = true;
                }
            }
        }

        /// <summary>
        /// Bomb.CanPlantInSite IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(BombUI), nameof(BombUI.CanPlantInSite))]
        class BombUI_CanPlantInSite_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__result">Expected/Intended Result (To Override)</param>
            static void Prefix(ref bool __result)
            {
                if(Game.AllowPlantingBombAnywhere)
                {
                    __result = true;
                }
            }

            /// <summary>
            /// Hook Function (After Call)
            /// </summary>
            /// <param name="__result">Expected/Intended Result (To Override)</param>
            static void Postfix(ref bool __result)
            {
                if(Game.AllowPlantingBombAnywhere)
                {
                    __result = true;
                }
            }
        }

        /// <summary>
        /// BombUI.CanPlant IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(BombUI), nameof(BombUI.CanPlant))]
        class BombUI_CanPlant_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__result">Expected/Intended Result (To Override)</param>
            static void Prefix(ref bool __result)
            {
                if(Game.InstantBombPlant)
                {
                    __result = true;
                }
            }

            /// <summary>
            /// Hook Function (After Call)
            /// </summary>
            /// <param name="__result">Expected/Intended Result (To Override)</param>
            static void Postfix(ref bool __result)
            {
                if(Game.InstantBombPlant)
                {
                    __result = true;
                }
            }
        }

        /// <summary>
        /// BombUI.CanDefuse IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(BombUI), nameof(BombUI.CanDefuse))]
        class BombUI_CanDefuse_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__result">Expected/Intended Result (To Override)</param>
            static void Prefix(ref bool __result)
            {
                if(Game.InstantBombDefuse)
                {
                    __result = true;
                }
            }

            /// <summary>
            /// Hook Function (After Call)
            /// </summary>
            /// <param name="__result">Expected/Intended Result (To Override)</param>
            static void Postfix(ref bool __result)
            {
                if(Game.InstantBombDefuse)
                {
                    __result = true;
                }
            }
        }

        /// <summary>
        /// FPSLimitController.GetCurrentFPSLimit IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(FPSLimitController), nameof(FPSLimitController.GetCurrentFPSLimit))]
        class FPSLimitController_GetCurrentFPSLimit_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__result">Expected/Intended Result (To Override)</param>
            static void Prefix(ref int __result)
            {
                if(Game.UncapFPSAllowUnlimited)
                {
                    __result = int.MaxValue;
                }
            }

            /// <summary>
            /// Hook Function (After Call)
            /// </summary>
            /// <param name="__result">Expected/Intended Result (To Override)</param>
            static void Postfix(ref int __result)
            {
                if(Game.UncapFPSAllowUnlimited)
                {
                    __result = int.MaxValue;
                }
            }
        }

        /// <summary>
        /// FirstPersonMovement IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(FirstPersonMovement), "Update")]
        class FirstPersonMovement_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static void Prefix(FirstPersonMovement __instance)
            {
                if(Variables.Player.EnableNoFallDamage)
                {
                    // You must set both variables. Otherwise, the "exploit" fails.
                    // Took way too long to figure this out. An unnecessarily long time...
                    __instance.timeUngrounded = 0f; // No-Fall Exploit
                    __instance.player.timeUngrounded = 0f; // No-Fall Exploit
                }

                __instance.allowFly = Variables.Player.EnableFlying;

                if(Variables.Player.CharacterControllerSqueeze)
                {
                    __instance.characterController.radius = 0.0001f; // Allows you to fit through cracks between GameObject placement on the map.
                    __instance.characterController.slopeLimit = float.MaxValue; // Allows you to scale terrain and objects easily.
                }

                if(Variables.Player.EnableAutoHop)
                {
                    if(Input.GetKey(KeyCode.Space))
                    {
                        __instance.TryJump();

                        if(Variables.Player.EnableSpeedFromJumpMultiplierOverride)
                        {
                            __instance.slowDownOnceGrounded = false;
                            __instance.maxWalkingSpeed = Variables.Player.ESFJMO_MultiplierOverrideAmount;
                        }
                    }
                }

                if(Variables.Player.NoMovementRestrictions)
                {
                    __instance.movementDisabled = false;
                    __instance.RecoverShotMovementPenalty(); // Testing movement penalty bypass function.
                }
            }
        }

        /// <summary>
        /// FirstPersonSettings IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(FirstPersonSettings), "Update")]
        class FirstPersonSettings_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static void Prefix(FirstPersonSettings __instance)
            {
                if(Variables.Weapon.DecreaseAimRecoil)
                {
                    __instance.visualRecoilAdditiveMultiplier = 0f;
                    __instance.aimVisualRecoilPositionMultiplier = 0f;
                    __instance.visualRecoilMoveToSpeed = 0f;
                    __instance.visualRecoilMoveToTimeMultiplier = 0f;
                }
            }
        }

        /// <summary>
        /// FirstPersonLook IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(FirstPersonLook), "Update")]
        class FirstPersonLook_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static void Prefix(FirstPersonLook __instance)
            {
                if(Variables.Weapon.DecreaseAimRecoil)
                {
                    __instance.aimRecoil = new Vector3(0f, 0f, 0f);
                    __instance.targetAimRecoil = new Vector3(0f, 0f, 0f);
                }

                if(Variables.Weapon.DecreaseCameraRecoil)
                {
                    __instance.cameraRecoil = new Vector3(0f, 0f, 0f);
                    __instance.initialCameraRecoil = new Vector3(0f, 0f, 0f);
                    __instance.cameraXRecoil = 0f;
                    __instance.cameraZRecoil = 0f;
                }

                if(Variables.Weapon.DecreaseFlinchRecoil)
                {
                    __instance.additiveFlinch = new Vector3(0f, 0f, 0f);
                }
            }
        }

        /// <summary>
        /// WeaponBalanceController IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(WeaponBalanceController), "UpdateFinalValues")]
        class WeaponBalanceController_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static void Prefix(WeaponBalanceController.FinalWeaponValues __instance)
            {
                if(Variables.Weapon.DecreaseWeaponSpread)
                {
                    __instance.addedHipFireSpread = 0f;
                    __instance.addedMovingSpreadPerMeterPerSecond = 0f;
                    __instance.bulletSpreadDecreasePerSecond = float.MaxValue;
                }

                if(Variables.Weapon.DecreaseAimRecoil)
                {
                    __instance.aimRecoil = new Vector3(0f, 0f, 0f);
                }

                if(Variables.Weapon.DecreaseCameraRecoil)
                {
                    __instance.cameraRecoilDecreaseSpeed = float.MaxValue;
                    __instance.visualRecoilDecreaseRate = float.MaxValue;
                    __instance.visualRotationalRecoilDecreaseRate = float.MaxValue;
                    __instance.cameraRecoilIncreaseTime = 0f;
                    __instance.cameraVerticalRecoil = 0f;
                    __instance.cameraZRecoil = 0f;
                    __instance.movementSwayMultiplier = 0f;
                    __instance.visualRecoil = new Vector3(0f, 0f, 0f);
                    __instance.visualRotationalRecoil = new Vector3(0f, 0f, 0f);
                    __instance.visualRecoilAdditiveRandomness = new Vector3(0f, 0f, 0f);
                }

                if(Variables.Weapon.ModifyWeaponPenetration)
                {
                    __instance.rangeModifier = float.MaxValue;
                }
            }
        }

        /// <summary>
        /// Weapon IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(Il2Cpp.Weapon), "Update")]
        class Weapon_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static void Prefix(Il2Cpp.Weapon __instance)
            {
                if(Variables.Weapon.DecreaseWeaponSpread)
                {
                    __instance.addedHipFireSpread = 0f;
                    __instance.addedMovingSpreadPerMeterPerSecond = 0f;
                    __instance.bulletSpreadDecreasePerSecond = float.MaxValue;
                }

                if(Variables.Weapon.DecreaseAimRecoil)
                {
                    __instance.aimRecoil = new Vector3(0f, 0f, 0f);
                }

                if(Variables.Weapon.DecreaseCameraRecoil)
                {
                    __instance.cameraRecoilDecreaseSpeed = float.MaxValue;
                    __instance.visualRecoilDecreaseRate = float.MaxValue;
                    __instance.visualRotationalRecoilDecreaseRate = float.MaxValue;
                    __instance.cameraRecoilIncreaseTime = 0f;
                    __instance.cameraVerticalRecoil = 0f;
                    __instance.cameraZRecoil = 0f;
                    __instance.movementSwayMultiplier = 0f;
                    __instance.visualRecoil = new Vector3(0f, 0f, 0f);
                    __instance.visualRotationalRecoil = new Vector3(0f, 0f, 0f);
                    __instance.visualRecoilAdditiveRandomness = new Vector3(0f, 0f, 0f);
                }

                if(Variables.Weapon.ModifyWeaponPenetration)
                {
                    __instance.rangeModifier = float.MaxValue;
                }
            }
        }

        /// <summary>
        /// FirstPersonWeaponHandler IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(FirstPersonWeaponHandler), "Update")]
        class FirstPersonWeaponHandler_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static void Prefix(FirstPersonWeaponHandler __instance)
            {
                __instance.weapon.canScopeIn = Variables.Weapon.AllowScopeOnAllWeaponTypes;

                if(Variables.Weapon.ModifyWeaponPenetration)
                {
                    __instance.weapon.penetration = float.MaxValue;
                }

                // TODO: Breaks normal scope-allowed weapons like AWP if not enabled...
                if(Variables.Weapon.AllowScopeOnAllWeaponTypes)
                {
                    __instance.weapon.visibleCrosshair = Render.CrosshairAlwaysVisible;

                    if(__instance.weapon.category == Weapons.WeaponCategory.Rifle)
                    {
                        __instance.weapon.scopeType = Il2Cpp.Weapon.ScopeType.RifleScope;
                        __instance.weapon.aimingFOV = 20f;
                    }
                    else
                    {
                        __instance.weapon.scopeType = Il2Cpp.Weapon.ScopeType.Ironsights;
                        __instance.weapon.aimingFOV = 35f;
                    }
                }

                if(Variables.Weapon.DecreaseAimRecoil)
                {
                    __instance.weapon.aimRecoil = new Vector3(0f, 0f, 0f);
                }

                if(Variables.Weapon.DecreaseWeaponBobbingAndKicking)
                {
                    __instance.jumpLandingWeaponSwayVerticalKick = 0f;
                    __instance.jumpLandingWeaponSwayYPositionKick = 0f;
                    __instance.weapon_BobbingTime = 0f;
                }

                if(Variables.Weapon.DecreaseCameraRecoil)
                {
                    __instance.weapon.cameraRecoilIncreaseTime = 0f;
                    __instance.weapon.cameraVerticalRecoil = 0f;
                    __instance.weapon.cameraZRecoil = 0f;
                    __instance.weapon.cameraRecoilDecreaseSpeed = 9999f;
                    __instance.weapon.visualRecoil = new Vector3(0f, 0f, 0f);
                    __instance.weapon.visualRecoilAdditiveRandomness = new Vector3(0f, 0f, 0f);
                    __instance.weapon.visualRotationalRecoil = new Vector3(0f, 0f, 0f);
                    __instance.weapon.visualRecoilDecreaseRate = float.MaxValue;
                    __instance.weapon.visualRotationalRecoilDecreaseRate = float.MaxValue;
                }

                if(Variables.Weapon.DecreaseWeaponSpread)
                {
                    __instance.weapon.addedHipFireSpread = 0f;
                }

                if(Render.CrosshairAlwaysVisible)
                {
                    __instance.weapon.visibleCrosshair = true;
                }

                if(Variables.Weapon.DecreaseAimRecoil)
                {
                    if(__instance.weapon.recoilPattern != null)
                    {
                        for(int I = 0; I < __instance.weapon.recoilPattern.Length; I++)
                        {
                            Vector2 TargetRecoilPattern = __instance.weapon.recoilPattern[I];

                            __instance.weapon.recoilPattern[I] = new Vector2(0f, 0f);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Mini-Map IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(Minimap), "Update")]
        class Minimap_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static void Prefix(Minimap __instance)
            {
                if(Render.DoNotFadePlayerDotOnMapRadarESP)
                {
                    __instance.SpeedToHidePlayerDot = 0f;
                }
            }
        }

        /// <summary>
        /// Wall-Bang IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(Wallbang), "Update")]
        class Wallbang_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static void Prefix(Wallbang __instance)
            {
                if(Variables.Weapon.ModifyWeaponPenetration)
                {
                    __instance.strength = float.MaxValue;
                }
            }
        }

        /// <summary>
        /// BulletManager.CanPenetrateThinWall IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(BulletManager), nameof(BulletManager.CanPenetrateThinWall))]
        class BulletManager_CanPenetrateThinWall_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            static bool Prefix(ref bool __result)
            {
                if(Variables.Weapon.ModifyWeaponPenetration)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// BuyMenu.InBuyZone IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(BuyMenu), nameof(BuyMenu.InBuyZone))]
        class BuyMenu_InBuyZone_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            static bool Prefix(ref bool __result)
            {
                if(Game.BuyZoneRemoveZoneRestrictions)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// BuyMenu.CanBuyGeneric IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(BuyMenu), nameof(BuyMenu.CanBuyGeneric))]
        class BuyMenu_CanBuyGeneric_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            static bool Prefix(ref bool __result)
            {
                if(Game.BuyMenuAlwaysAllowBuyingGeneric)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// BuyMenu.CanBuyWeapon IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(BuyMenu), nameof(BuyMenu.CanBuyWeapon))]
        class BuyMenu_CanBuyWeapon_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            static bool Prefix(ref bool __result)
            {
                if(Game.BuyMenuAlwaysAllowBuyingGeneric)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// BuyMenu IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(BuyMenu), "Update")]
        class BuyMenu_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static void Prefix(BuyMenu __instance)
            {
                if(Variables.Weapon.DidWeaponBuyRequestOccur || Input.GetKeyDown(KeyCodeBinds.ReBuyWeaponFromSwitcherKey))
                {
                    Il2Cpp.Weapon TargetWeapon = new Il2Cpp.Weapon();
                    TargetWeapon.type = (Weapons.WeaponType)UI.SelectedIndexFromWeaponListArray;

                    try
                    {
                        __instance.BuyWeapon(TargetWeapon);
                    }
                    catch
                    {
                        // Error relating to purchasing of weapon has occurred. Bomb was selected? Invalid weapon? Ignore and continue.
                    }

                    Variables.Weapon.DidWeaponBuyRequestOccur = false; // Reset to allow purchasing a new weapon from switcher menu.
                }

                if(Variables.Weapon.TreatKnifeAsPrimaryWeaponDropBypass && UI.Cached_IsMe != null)
                {
                    if(Input.GetKeyDown(KeyCodeBinds.UseKnifePrimaryBypassKey))
                    {
                        /* The idea behind buying two different weapons in one call is to:
                         * - Purchase M40 that will replace all Primary Loadout spaced weapons.
                         * - This will cause the previous Primary (Knife, Weapon), to be dropped.
                         * - Purchase the Weapon or Knife you wanted to equip in the first place.
                         * - This will cause you to equip the Knife and drop M40.
                         * > When you go to drop again, you'll drop the Knife, buy M40, drop, repeat.
                         * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                         * This allows you to drop knives. Previously, you could just select a knife,
                         * spam drop key, and you'd drop the knife. Either a game patch or maybe a
                         * bad function in this cheat has caused this to not work properly. This
                         * bypasses any of those two issues and allows dropping knives on hot-key.
                        */

                        Il2Cpp.Weapon BeforeKnifeBypass = new Il2Cpp.Weapon();
                        BeforeKnifeBypass.type = Weapons.WeaponType.M40;

                        __instance.BuyWeapon(BeforeKnifeBypass);

                        UI.Cached_IsMe.localPlayer.EquipWeaponFromSavedLoadout(
                            BeforeKnifeBypass.type,
                            dropOldWeapon: true
                        );

                        // Perform Knife Bypass:

                        // HACK: This shit won't work in certain lobby types. It used to. I can't be bothered to care.

                        Il2Cpp.Weapon KnifeBypass = new Il2Cpp.Weapon();
                        KnifeBypass.type = (Weapons.WeaponType)UI.SelectedIndexFromWeaponListArray;

                        UI.Cached_IsMe.weapon.slotType = Loadout.SlotType.Primary;

                        __instance.BuyWeapon(KnifeBypass);

                        UI.Cached_IsMe.localPlayer.EquipWeaponFromSavedLoadout(
                            KnifeBypass.type,
                            dropOldWeapon: true
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Player IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(Il2Cpp.Player), "Update")]
        class Player_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static void Prefix(Il2Cpp.Player __instance)
            {
                #region XcereHook Raycast Debugging

                if(Input.GetMouseButtonDown(0) && UI.Cached_IsMe != null && UI.DebuggingMode)
                {
                    Ray RaycastTest = UI.Cached_IsMe.localPlayer.mainCamera.ScreenPointToRay(Input.mousePosition);

                    Vector3 End = RaycastTest.origin + RaycastTest.direction * float.MaxValue;

                    if(Physics.Raycast(RaycastTest, out RaycastHit Hit, float.MaxValue, Physics.DefaultRaycastLayers))
                    {
                        GameObject HitWhat = Hit.collider.gameObject;

                        RaycastHitDebugging(Hit, RaycastTest);
                    }
                }

                #endregion

                // Private functions NOT to be released to Public builds of XcereHook.
                // Private functions NOT to be released to Public builds of XcereHook.
                // Private functions NOT to be released to Public builds of XcereHook.
                // Private functions NOT to be released to Public builds of XcereHook.
                //if(Game.FakeSyncVarValues)
                //{
                //    if(Time.time < Variables.Player.MalformedSyncVar_Timeout)
                //    {
                //        return;
                //    }

                //    Variables.Player.MalformedSyncVar_Timeout = Time.time + 7.5f;

                //    __instance.???( PRIVATE PRIVATE PRIVATE
                //    );

                //    __instance.???( PRIVATE PRIVATE PRIVATE
                //    );
                //}

                if(Variables.Player.EnableTriggerBot && UI.Cached_IsMe != null)
                {
                    if(Variables.Player.ETB_AllowWallBang)
                    {
                        // Same functionality but with more code to check for wall-bang conditions.
                        // Functions are separated to make code cleaner and easier to modify.
                        UpdateTriggerBotWB(UI.Cached_IsMe, UI.Cached_IsMe.localPlayer.mainCamera, Physics.DefaultRaycastLayers);
                    }
                    else
                    {
                        // Same functionality but zero wall-bang check code.
                        // Functions are separated to make code cleaner and easier to modify.
                        UpdateTriggerBot(UI.Cached_IsMe, UI.Cached_IsMe.localPlayer.mainCamera, Physics.DefaultRaycastLayers);
                    }
                }

                // Private functions NOT to be released to Public builds of XcereHook.
                // Private functions NOT to be released to Public builds of XcereHook.
                // Private functions NOT to be released to Public builds of XcereHook.
                // Private functions NOT to be released to Public builds of XcereHook.
                //if(Variables.Player.EnableSpinBot)
                //{
                //    if(Time.time < Variables.Player.SpinbotMalformedRPC_Timeout)
                //    {
                //        return;
                //    }

                //    Variables.Player.SpinbotMalformedRPC_Timeout = Time.time + 5f;

                //    __instance.?(
                //    );

                //    __instance.?(
                //    );
                //}

                if(Variables.Player.EnableRadioSFXSpam)
                {
                    if(Input.GetKeyDown(KeyCodeBinds.SpamRadioSFXKey))
                    {
                        if(Time.time < Variables.Player.ERSFXS_NextAllowedRadioCall)
                        {
                            return;
                        }

                        Variables.Player.ERSFXS_NextAllowedRadioCall = Time.time + Variables.Player.ERSFXS_SpamDelayValue;

                        System.Random RNG = new System.Random();

                        UI.Cached_IsMe?.RpcSendRadioVoice(
                            RNG.Next(
                                Enum.GetValues(typeof(Il2Cpp.Player.RadioVoice)).Cast<int>().Min(),
                                Enum.GetValues(typeof(Il2Cpp.Player.RadioVoice)).Cast<int>().Max()
                            )
                        );
                    }
                }

                if(Variables.Weapon.KeepDroppedWeapon)
                {
                    if(__instance.currentAmmoInMagazine == 0 && __instance.currentAmmoNotInMagazine == 0)
                    {
                        BuyMenu BuyMenuAPI = new BuyMenu();

                        Il2Cpp.Weapon WeaponAPI = new Il2Cpp.Weapon();
                        WeaponAPI.type = (Weapons.WeaponType)Variables.UI.SelectedIndexFromWeaponListArray;

                        try
                        {
                            BuyMenuAPI.BuyWeapon(WeaponAPI);
                            BuyMenuAPI.BuyWeapon(WeaponAPI); // Backup to verify that we did, indeed, purchase said weapon.
                        }
                        catch
                        {
                            // Attempted to purchase the Bomb weapon type? Another unknown error? Ignore as it doesn't cause code execution issues.
                        }
                    }
                }

                if(Input.GetMouseButtonDown(0)) // Left Mouse Button (Fire)
                {
                    if(Variables.Weapon.WeaponCanShootGrenades)
                    {
                        if(Variables.Weapon.WCSG_ShootFragGrenades)
                        {
                            __instance.SpawnGrenade(Weapons.WeaponType.FragGrenade);
                        }

                        if(Variables.Weapon.WCSG_ShootFlashbangGrenades)
                        {
                            __instance.SpawnGrenade(Weapons.WeaponType.Flashbang);
                        }

                        if(Variables.Weapon.WCSG_ShootSmokeGrenades)
                        {
                            __instance.SpawnGrenade(Weapons.WeaponType.SmokeGrenade);
                        }

                        if(Variables.Weapon.WCSG_ShootIncendiaryGrenades)
                        {
                            __instance.SpawnGrenade(Weapons.WeaponType.IncendiaryGrenade);
                        }
                    }
                }

                if(Game.InstantBombDefuse)
                {
                    if(Input.GetKeyDown(KeyCodeBinds.InstantBombDefuseKey))
                    {
                        __instance.DefuseBomb();
                    }
                }

                if(Game.InstantBombPlant)
                {
                    if(Input.GetKeyDown(KeyCodeBinds.InstantBombPlantKey))
                    {
                        __instance.PlantBomb();
                    }
                }

                if(Variables.Player.EnableQuickDirection && UI.Cached_IsMe != null)
                {
                    if(Time.time > Variables.Player.EQD_TimeUntilNextQD)
                    {
                        bool HasMoved = false;

                        Transform MyCamera = UI.Cached_IsMe.localPlayer.mainCamera.transform;

                        Vector3 ForwardV3 = MyCamera.forward;
                        Vector3 RightV3 = MyCamera.right;

                        ForwardV3.y = 0f;
                        RightV3.y = 0f;

                        ForwardV3.Normalize();
                        RightV3.Normalize();

                        float CurrentTimeLastHurt = UI.Cached_IsMe.timeLastHurt;
                        float CurrentHealth = UI.Cached_IsMe.health;

                        if(Variables.Player.EQD_SafetyFromEnemyFire && (CurrentTimeLastHurt != Variables.Player.EQD_LastTimeLastHurt && CurrentHealth < Variables.Player.EQD_LastHealth))
                        {
                            if(CurrentHealth >= 60) // ~60 HP should still be enough to combat enemy player(s).
                            {
                                return;
                            }

                            if(CurrentHealth < 60 && CurrentHealth >= 45) // We should start getting a little bit worried.
                            {
                                UI.Cached_IsMe.localPlayer.transform.position += -RightV3 * Variables.Player.EQD_MovementOffsetValue * Time.deltaTime;
                                HasMoved = true;
                            }

                            if(CurrentHealth < 45 && CurrentHealth >= 15) // We should REALLY be getting worried.
                            {
                                UI.Cached_IsMe.localPlayer.transform.position += RightV3 * Variables.Player.EQD_MovementOffsetValue * Time.deltaTime;
                                HasMoved = true;
                            }

                            if(CurrentHealth < 15) // Bail out. It's no longer a winning battle.
                            {
                                UI.Cached_IsMe.localPlayer.transform.position += -RightV3 * Variables.Player.EQD_MovementOffsetValue * Time.deltaTime;
                                UI.Cached_IsMe.localPlayer.transform.position += -ForwardV3 * Variables.Player.EQD_MovementOffsetValue * Time.deltaTime;
                                HasMoved = true;
                            }
                        }

                        Variables.Player.EQD_LastTimeLastHurt = CurrentTimeLastHurt;
                        Variables.Player.EQD_LastHealth = CurrentHealth;

                        if(Input.GetKeyDown(KeyCodeBinds.QuickDirectionToLeftAKey)) // Move Left
                        {
                            UI.Cached_IsMe.localPlayer.transform.position += -RightV3 * Variables.Player.EQD_MovementOffsetValue * Time.deltaTime;
                            HasMoved = true;
                        }

                        if(Input.GetKeyDown(KeyCodeBinds.QuickDirectionToRightAKey)) // Move Right
                        {
                            UI.Cached_IsMe.localPlayer.transform.position += RightV3 * Variables.Player.EQD_MovementOffsetValue * Time.deltaTime;
                            HasMoved = true;
                        }

                        if(Input.GetKeyDown(KeyCodeBinds.QuickDirectionToUpAKey)) // Move Forward
                        {
                            UI.Cached_IsMe.localPlayer.transform.position += ForwardV3 * Variables.Player.EQD_MovementOffsetValue * Time.deltaTime;
                            HasMoved = true;
                        }

                        if(Input.GetKeyDown(KeyCodeBinds.QuickDirectionToDownAKey)) // Move Backwards
                        {
                            UI.Cached_IsMe.localPlayer.transform.position += -ForwardV3 * Variables.Player.EQD_MovementOffsetValue * Time.deltaTime;
                            HasMoved = true;
                        }

                        if(HasMoved)
                        {
                            Variables.Player.EQD_TimeUntilNextQD = Time.time + Variables.Player.EQD_MoveCooldown;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ACTkController IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(ACTkController), "Update")]
        class ACTkController_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static void Prefix(ACTkController __instance)
            {
                try
                {
                    __instance.enabled = Game.DisableACTkModules;
                    __instance.gameObject.SetActive(Game.DisableACTkModules);
                }
                catch
                {
                    // Ignore all errors relating to ACTk if we couldn't force-disable.
                }
            }
        }

        /// <summary>
        /// PlayerAntiHack IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(PlayerAntiHack), "Update")]
        class PlayerAntiHack_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static void Prefix(PlayerAntiHack __instance)
            {
                try
                {
                    __instance.enabled = Game.BypassAntiHackFunctions;

                    if(Game.BypassAntiHackFunctions)
                    {
                        __instance.maxSpeed = float.MaxValue;
                    }

                    __instance.gameObject.SetActive(Game.BypassAntiHackFunctions);
                }
                catch
                {
                    // Ignore all errors relating to PlayerAntiHack if we couldn't force-disable.
                }
            }
        }

        /// <summary>
        /// PlayerAntiHack.CheckForSpeedHack IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(PlayerAntiHack), nameof(PlayerAntiHack.CheckForSpeedHack))]
        class PlayerAntiHack_CheckForSpeedHack_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            static bool Prefix()
            {
                try
                {
                    return !Game.BypassAntiHackFunctions;
                }
                catch
                {
                    // Ignore all errors relating to PlayerAntiHack if we couldn't force-disable by returning original function.
                    return false;
                }
            }
        }

        /// <summary>
        /// AntiHack IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(AntiHack), "Update")]
        class AntiHack_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static void Prefix(AntiHack __instance)
            {
                try
                {
                    __instance.enabled = Game.BypassAntiHackFunctions;
                    __instance.gameObject.SetActive(Game.BypassAntiHackFunctions);

                }
                catch
                {
                    // Ignore all errors relating to AntiHack if we couldn't force-disable.
                }
            }
        }

        /// <summary>
        /// GlobalDetectionManager IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(GlobalDetectionManager), "Update")]
        class GlobalDetectionManager_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static void Prefix(GlobalDetectionManager __instance)
            {
                try
                {
                    if(Game.BypassAntiHackFunctions)
                    {
                        __instance.recoilHDetections = 0;
                        __instance.radarHDetections = 0;
                    }

                    __instance.enabled = Game.BypassAntiHackFunctions;
                    __instance.gameObject.SetActive(Game.BypassAntiHackFunctions);

                }
                catch
                {
                    // Ignore all errors relating to GlobalDetectionManager if we couldn't force-disable.
                }
            }
        }

        /// <summary>
        /// GlobalDetectionManager.OnInjectionDetected IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(AntiHack), nameof(GlobalDetectionManager.OnInjectionDetected))]
        class GlobalDetectionManager_OnInjectionDetected_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static bool Prefix(GlobalDetectionManager __instance)
            {
                try
                {
                    if(__instance == null)
                    {
                        // Ignore all errors relating to GlobalDetectionManager if we couldn't force-disable by returning original function.
                        return false;
                    }

                    return !Game.BypassAntiHackFunctions;
                }
                catch
                {
                    // Ignore all errors relating to GlobalDetectionManager if we couldn't force-disable by returning original function.
                    return false;
                }
            }
        }

        /// <summary>
        /// GlobalDetectionManager.OnObscuredDetected IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(AntiHack), nameof(GlobalDetectionManager.OnObscuredDetected))]
        class GlobalDetectionManager_OnObscuredDetected_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static bool Prefix(GlobalDetectionManager __instance)
            {
                try
                {
                    if(__instance == null)
                    {
                        // Ignore all errors relating to GlobalDetectionManager if we couldn't force-disable by returning original function.
                        return false;
                    }

                    return !Game.BypassAntiHackFunctions;
                }
                catch
                {
                    // Ignore all errors relating to GlobalDetectionManager if we couldn't force-disable by returning original function.
                    return false;
                }
            }
        }

        /// <summary>
        /// GlobalDetectionManager.OnRecailHDetected IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(AntiHack), nameof(GlobalDetectionManager.OnRecailHDetected))]
        class GlobalDetectionManager_OnRecailHDetected_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static bool Prefix(GlobalDetectionManager __instance)
            {
                try
                {
                    if(__instance == null)
                    {
                        // Ignore all errors relating to GlobalDetectionManager if we couldn't force-disable by returning original function.
                        return false;
                    }

                    return !Game.BypassAntiHackFunctions;
                }
                catch
                {
                    // Ignore all errors relating to GlobalDetectionManager if we couldn't force-disable by returning original function.
                    return false;
                }
            }
        }

        /// <summary>
        /// GlobalDetectionManager.OnSpeedDetected IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(AntiHack), nameof(GlobalDetectionManager.OnSpeedDetected))]
        class GlobalDetectionManager_OnSpeedDetected_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static bool Prefix(GlobalDetectionManager __instance)
            {
                try
                {
                    if(__instance == null)
                    {
                        // Ignore all errors relating to GlobalDetectionManager if we couldn't force-disable by returning original function.
                        return false;
                    }

                    return !Game.BypassAntiHackFunctions;
                }
                catch
                {
                    // Ignore all errors relating to GlobalDetectionManager if we couldn't force-disable by returning original function.
                    return false;
                }
            }
        }

        /// <summary>
        /// GlobalDetectionManager.OnTimeDetected IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(AntiHack), nameof(GlobalDetectionManager.OnTimeDetected))]
        class GlobalDetectionManager_OnTimeDetected_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static bool Prefix(GlobalDetectionManager __instance)
            {
                try
                {
                    if(__instance == null)
                    {
                        // Ignore all errors relating to GlobalDetectionManager if we couldn't force-disable by returning original function.
                        return false;
                    }

                    return !Game.BypassAntiHackFunctions;
                }
                catch
                {
                    // Ignore all errors relating to GlobalDetectionManager if we couldn't force-disable by returning original function.
                    return false;
                }
            }
        }

        /// <summary>
        /// GlobalDetectionManager.OnWallDetected IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(AntiHack), nameof(GlobalDetectionManager.OnWallDetected))]
        class GlobalDetectionManager_OnWallDetected_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static bool Prefix(GlobalDetectionManager __instance)
            {
                try
                {
                    if(__instance == null)
                    {
                        // Ignore all errors relating to GlobalDetectionManager if we couldn't force-disable by returning original function.
                        return false;
                    }

                    return !Game.BypassAntiHackFunctions;
                }
                catch
                {
                    // Ignore all errors relating to GlobalDetectionManager if we couldn't force-disable by returning original function.
                    return false;
                }
            }
        }

        /// <summary>
        /// PlayerAimbotDetector IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(PlayerAimbotDetector), "Update")]
        class PlayerAimbotDetector_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static void Prefix(PlayerAimbotDetector __instance)
            {
                try
                {
                    __instance.enabled = Game.BypassAntiHackFunctions;
                    __instance.gameObject.SetActive(Game.BypassAntiHackFunctions);
                }
                catch
                {
                    // Ignore all errors relating to PlayerAimbotDetector if we couldn't force-disable.
                }
            }
        }

        /// <summary>
        /// PlayerSpeedHackDetector IL2CPP Patch
        /// </summary>
        [HarmonyPatch(typeof(PlayerSpeedHackDetector), "Update")]
        class PlayerSpeedHackDetector_Patch
        {
            /// <summary>
            /// Hook Function (Before Call)
            /// </summary>
            /// <param name="__instance">Expected/Intended Result (To Override)</param>
            static void Prefix(PlayerSpeedHackDetector __instance)
            {
                try
                {
                    __instance.enabled = Game.BypassAntiHackFunctions;
                    __instance.gameObject.SetActive(Game.BypassAntiHackFunctions);
                }
                catch
                {
                    // Ignore all errors relating to PlayerSpeedHackDetector if we couldn't force-disable.
                }
            }
        }
    }
}