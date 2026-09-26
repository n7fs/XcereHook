using Il2Cpp;

using MelonLoader;

using UnityEngine;

using static XcereHook.Variables;

namespace XcereHook
{
    /// <summary>
    /// A collection of helpful functions to assist in development effort.
    /// </summary>
    internal class Utilities
    {
        /// <summary>
        /// Display helpful information on a cheat function when hovering over it.
        /// </summary>
        /// <param name="Text">The helpful text displayed on hover.</param>
        /// <param name="RectangleToCheck">The rectangle to check. Usually the cheat function UI control boundary.</param>
        public static void HelpOnHover(string Text, Rect RectangleToCheck)
        {
            if(RectangleToCheck.Contains(Event.current.mousePosition))
            {
                UI.HelpDescriptionCurrentDisplayedText = Text;
            }
        }

        /// <summary>
        /// Wrapper function for HelpOnHover to avoid IL2CPP-related errors. Use this function instead of 'HelpOnHover' alone.
        /// </summary>
        /// <param name="Value">Value of the Toggle control.</param>
        /// <param name="Label">Label displayed on said Toggle control.</param>
        /// <param name="Description">Helpful information displayed when hover event occurs.</param>
        /// <returns>A bool, (Value), supplied whenever function is completed.</returns>
        public static bool ToggleWithHelpOnHover(bool Value, string Label, string Description)
        {
            Rect RectangleTarget = GUILayoutUtility.GetRect(
                new GUIContent(Label),
                GUI.skin.toggle
            );

            Value = GUI.Toggle(RectangleTarget, Value, Label);

            HelpOnHover(Description, RectangleTarget);

            return Value;
        }

        /// <summary>
        /// Enumerates through every Player object until a return match for your player is found.
        /// </summary>
        /// <returns>A Player object that represents You as a usable GameObject.</returns>
        public static Il2Cpp.Player? IsMe()
        {
            string YourProfileName = AccountManager.GetProfileName() ?? "N/A";

            Il2Cpp.Player? YourPlayer = null;

            foreach(Il2Cpp.Player PlayerObject in UnityEngine.Resources.FindObjectsOfTypeAll<Il2Cpp.Player>())
            {
                if(PlayerObject.profileName == YourProfileName)
                {
                    YourPlayer = PlayerObject;
                    break;
                }
            }

            if(YourPlayer == null)
            {
                return null;
            }

            return YourPlayer;
        }

        /// <summary>
        /// Enumerates through every Player object and returns an Array containing every Player object found.
        /// </summary>
        /// <returns>A Player array containing all found Player objects in the Scene.</returns>
        public static Il2Cpp.Player[] IsAll()
        {
            // Attempts to prevent invalid Player objects due to Client/Server GameObject spawning.
            // For more information, view 2D Player ESP code that explains the Client/Server invalid GameObject issue.
            return UnityEngine.Resources.FindObjectsOfTypeAll<Il2Cpp.Player>().Where(
                Player => !string.IsNullOrWhiteSpace(Player.profileName)
            ).ToArray();
        }

        /// <summary>
        /// Enumerates through all active Bomb/C4 objects and various Grenade-type objects.
        /// </summary>
        /// <returns>Returns a list of GameObject's relating to the C4/Bomb and Thrown Grenades.</returns>
        public static GameObject[] IsBombObjectOrThrownGrenade()
        {
            int GrenadeLayer = LayerMask.NameToLayer("Grenade");

            return UnityEngine.Resources.FindObjectsOfTypeAll<GameObject>().Where(
                Is => Is != null &&
                Is.activeInHierarchy &&
                Is.layer == GrenadeLayer &&
                (
                    Is.name.IndexOf("Bomb", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    Is.name.Equals("Smoke Grenade", StringComparison.OrdinalIgnoreCase) ||
                    Is.name.Equals("Flashbang", StringComparison.OrdinalIgnoreCase) ||
                    Is.name.Equals("Incendiary Grenade", StringComparison.OrdinalIgnoreCase) ||
                    Is.name.Equals("Frag Grenade", StringComparison.OrdinalIgnoreCase)
                )
            ).ToArray();
        }

        /// <summary>
        /// Verifies that the Target is Valid by checking health, team status, valid account name, etc.
        /// </summary>
        /// <param name="Target">Target Il2Cpp.Player Object Reference</param>
        /// <returns>Returns a value, (Bool), True or False, True if Valid Target, False if not.</returns>
        public static bool IsValidTarget(Il2Cpp.Player Target)
        {
            if(Target == null)
            {
                return false;
            }

            if(Target.health == 0)
            {
                return false;
            }

            if(string.IsNullOrWhiteSpace(Target.profileName) || string.IsNullOrWhiteSpace(Target.accountUsername))
            {
                return false;
            }

            if(PlayerUtils.PlayersOnSameTeam(Target, UI.Cached_IsMe) && Variables.Player.ETB_ShootTeam == false)
            {
                return false;
            }
            else if(PlayerUtils.PlayersOnSameTeam(Target, UI.Cached_IsMe) && Variables.Player.ETB_ShootTeam == true)
            {
                return true;
            }

            if(PlayerUtils.PlayerOnLocalTeamOrSpectating(Target) && Variables.Player.ETB_ShootTeam == false)
            {
                return false;
            }
            else if(PlayerUtils.PlayerOnLocalTeamOrSpectating(Target) && Variables.Player.ETB_ShootTeam == true)
            {
                return true;
            }

            return true;
        }

        /// <summary>
        /// Automatically handle trigger-bot updating and ray-cast detection.
        /// </summary>
        /// <param name="YourPlayer">Your Il2Cpp.Player Object</param>
        /// <param name="YourCamera">Your Il2Cpp.Player's Camera Object</param>
        /// <param name="HitMask">Hit Mask LayerMask</param>
        public static void UpdateTriggerBot(Il2Cpp.Player YourPlayer, Camera YourCamera, LayerMask HitMask)
        {
            Ray HitTest = new Ray(
                YourCamera.transform.position,
                YourCamera.transform.forward
            );

            if(!Physics.Raycast(HitTest, out RaycastHit Hit, float.MaxValue, HitMask, QueryTriggerInteraction.Ignore))
            {
                return;
            }

            Il2Cpp.Player? Target = Hit.collider.GetComponentInParent<Il2Cpp.Player>();

            if(Target == null)
            {
                return;
            }

            if(Target == YourPlayer)
            {
                return;
            }

            if(!IsValidTarget(Target))
            {
                return;
            }

            YourPlayer.localPlayer.TryShoot();
        }

        /// <summary>
        /// Allows wall-banging. Automatically handle trigger-bot updating and ray-cast detection.
        /// </summary>
        /// <param name="YourPlayer">Your Il2Cpp.Player Object</param>
        /// <param name="YourCamera">Your Il2Cpp.Player's Camera Object</param>
        /// <param name="HitMask">Hit Mask LayerMask</param>
        public static void UpdateTriggerBotWB(Il2Cpp.Player YourPlayer, Camera YourCamera, LayerMask HitMask)
        {
            Ray HitTest = new Ray(
                YourCamera.transform.position,
                YourCamera.transform.forward
            );

            RaycastHit[] Hits = Physics.RaycastAll(
                HitTest,
                float.MaxValue,
                HitMask,
                QueryTriggerInteraction.Ignore
            );

            Array.Sort(Hits, (A, B) => A.distance.CompareTo(B.distance));

            foreach(RaycastHit Hit in Hits)
            {
                if(LayerMask.LayerToName(Hit.collider.gameObject.layer).Contains("Wallbang", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string Name = Hit.collider.gameObject.name;

                bool MaterialCheck =
                    Name.Contains("P_SmallBuilding_", StringComparison.OrdinalIgnoreCase) ||
                    Name.Contains("_Fence_CNC", StringComparison.OrdinalIgnoreCase) ||
                    Name.Contains("P_SmallBuilding_", StringComparison.OrdinalIgnoreCase) ||
                    Name.Contains("Safety_Collider_", StringComparison.OrdinalIgnoreCase) ||
                    Name.Contains("Shelf_", StringComparison.OrdinalIgnoreCase) ||
                    Name.Contains("Desert_Structure_Concrete_", StringComparison.OrdinalIgnoreCase) ||
                    Name.Contains("Vase_metal_", StringComparison.OrdinalIgnoreCase) ||
                    Name.Contains("Pole_metal_", StringComparison.OrdinalIgnoreCase) ||
                    Name.Contains("Lampost_metal_", StringComparison.OrdinalIgnoreCase) ||
                    Name.Contains("P_SmallBuilding_", StringComparison.OrdinalIgnoreCase) ||
                    Name.Contains("L_Street_Concrete_", StringComparison.OrdinalIgnoreCase) ||
                    Name.Contains("Cons_Platform_metal_A_", StringComparison.OrdinalIgnoreCase) ||
                    Name.Contains("Wintertown_Structure_wood_", StringComparison.OrdinalIgnoreCase) ||
                    Name.Contains("SM_Escalators", StringComparison.OrdinalIgnoreCase) ||
                    Name.Contains("FakeCOlliderCUbe", StringComparison.OrdinalIgnoreCase);

                bool NoMaterialCheck =
                    Name.Contains("Bounds_A", StringComparison.OrdinalIgnoreCase) ||
                    Name.Contains("Bounds_B", StringComparison.OrdinalIgnoreCase) ||
                    Name.Contains("BuyZone-Team", StringComparison.OrdinalIgnoreCase) ||
                    Name.Contains("SM_Barrier", StringComparison.OrdinalIgnoreCase);

                // Allows wall-bang of objects not tagged as such. Downside is the increase of false-positive firing.
                if(Variables.Player.ETB_AllowExtendedWallBang && (
                    NoMaterialCheck || (
                    MaterialCheck && ObjectContainsMaterials(
                                Hit.collider.gameObject.GetComponent<Renderer>(),
                                Variables.Player.INTERNAL_KnownWallBangObjects_NotLayeredAsSuch
                            )
                        )
                    )
                )
                {
                    continue;
                }

                Il2Cpp.Player? Target = Hit.collider.GetComponentInParent<Il2Cpp.Player>();

                if(Target == null)
                {
                    break;
                }

                if(Target == YourPlayer)
                {
                    break;
                }

                if(!IsValidTarget(Target))
                {
                    break;
                }

                YourPlayer.localPlayer.TryShoot();

                break;
            }
        }

        /// <summary>
        /// Check if a GameObject's Renderer contains a list of material names as a string array.
        /// </summary>
        /// <param name="Render">Renderer Instance</param>
        /// <param name="Materials">A string array containing the list of material names.</param>
        /// <returns>A value, bool, True or False, True if object contains material names given; otherwise false.</returns>
        public static bool ObjectContainsMaterials(Renderer Render, string[] Materials)
        {
            Material[] MaterialArray = Render.materials;

            for(int I = 0; I < MaterialArray.Length; I++)
            {
                Material TargetMaterial = MaterialArray[I];

                if(TargetMaterial != null)
                {
                    for(int II = 0; II < Materials.Length; II++)
                    {
                        if(TargetMaterial.name.Contains(Materials[II], StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    continue;
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// Performs a ray-cast hit check that lists debugging information to assist in cheat feature development.
        /// </summary>
        /// <param name="RH">RaycastHit Instance</param>
        /// <param name="R">Ray Instance</param>
        public static void RaycastHitDebugging(RaycastHit RH, Ray R)
        {
            GameObject Object = RH.collider.gameObject;
            Transform Transform = RH.transform;
            Collider Collision = RH.collider;

            MelonLogger.Msg($"===========");
            MelonLogger.Msg("Raycast Hit");
            MelonLogger.Msg($"==========={Environment.NewLine}");

            MelonLogger.Msg("- Ray Target -");
            MelonLogger.Msg($"Point: {RH.point}");
            MelonLogger.Msg($"Normal: {RH.normal}");
            MelonLogger.Msg($"Distance: {RH.distance}");
            MelonLogger.Msg($"Triangle: {RH.triangleIndex}");
            MelonLogger.Msg($"TextureCoord: {RH.textureCoord}");
            MelonLogger.Msg($"TextureCoord2: {RH.textureCoord2}");
            MelonLogger.Msg($"Barycentric Coordinate: {RH.barycentricCoordinate}");
            MelonLogger.Msg($"Lightmap Coordinate: {RH.lightmapCoord}");
            MelonLogger.Msg($"Transform: {(Transform != null ? Transform.name : "NULL")}{Environment.NewLine}");

            MelonLogger.Msg("- GameObject Target -");
            MelonLogger.Msg($"Name: {Object.name}");
            MelonLogger.Msg($"Instance ID: {Object.GetInstanceID()}");
            MelonLogger.Msg($"Tag: {Object.tag}");
            MelonLogger.Msg($"Layer: {Object.layer}");
            MelonLogger.Msg($"Layer Name: {LayerMask.LayerToName(Object.layer)}");
            MelonLogger.Msg($"Active Self: {Object.activeSelf}");
            MelonLogger.Msg($"Active In Hierarchy: {Object.activeInHierarchy}");
            MelonLogger.Msg($"Is Static: {Object.isStatic}{Environment.NewLine}");

            MelonLogger.Msg("- Collider Target -");
            MelonLogger.Msg($"Name: {Collision.name}");
            MelonLogger.Msg($"Type: {Collision.GetType().FullName}{Environment.NewLine}");

            MelonLogger.Msg("- Renderer Target -");
            Renderer? Render = Object.GetComponent<Renderer>() ?? null;

            if(Render != null)
            {
                MelonLogger.Msg($"Type: {Render.GetType().FullName}");
                MelonLogger.Msg($"Material Count: {Render.materials.Length}");

                Material[] Materials = Render.materials;

                for(int I = 0; I < Materials.Length; I++)
                {
                    Material Material = Materials[I];

                    if(Material != null)
                    {
                        MelonLogger.Msg($" > Material {Materials[I].name}");
                    }

                    continue;
                }
            }

            MelonLogger.Msg($"{Environment.NewLine}");
        }

        /// <summary>
        /// Global wait check for key-bind configuration code.
        /// </summary>
        public static string? WaitingForKeyBind = null;

        /// <summary>
        /// Used to draw a UI Button that allows changing cheat feature key-bind variables.
        /// </summary>
        /// <param name="ID">Cheat Feature Bound</param>
        /// <param name="Label">Cheat Feature Simple Name</param>
        /// <param name="KeyBind">Target Key-Bind</param>
        public static void DrawKeyBind(string ID, string Label, ref KeyCode KeyBind)
        {
            bool IsWaiting = WaitingForKeyBind == ID;

            GUILayout.BeginHorizontal();

            GUILayout.Label(Label);

            string ButtonText = IsWaiting ? "Press Key..." : KeyBind.ToString();

            if(GUILayout.Button(ButtonText))
            {
                WaitingForKeyBind = ID;

                Event.current.Use();
            }

            GUILayout.EndHorizontal();

            if(IsWaiting && Event.current.type == EventType.KeyDown)
            {
                KeyBind = Event.current.keyCode;

                WaitingForKeyBind = null;

                Event.current.Use();
            }
        }
    }
}