using EpicSauceHack99;
using FishNet.Managing;
using FishNet.Managing.Object;
using FishNet.Object;
using HeathenEngineering.SteamworksIntegration;
using strafthot.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using static strafthot.Config.GUIItemSettings;

namespace strafthot
{
    public class Config
    {
        public enum UITheme
        {
            Dark, Neon, Red, Purple, Blue, Green, Orange, Pink, Cyan, Yellow, Gray, Brown
        }

        public UITheme CurrentTheme = UITheme.Dark;
        private enum Page { Aiming, Visuals, Weapons, WeaponMods, Player, Misc, Logs, Settings }
        private Page _currentPage = Page.Aiming;

        public Color AccentColor = new Color(1f, 1f, 1f, 1f);
        public ThemeColor CurrentThemeColor = new ThemeColor();
        public class ThemeColor
        {
            public Color bgDark = new Color(0.08f, 0.08f, 0.09f, 0.95f);
            public Color bgMid = new Color(0.15f, 0.15f, 0.16f, 1f);
            public Color bgHover = new Color(0.22f, 0.22f, 0.25f, 1f);
        }

        // Visuals
        public Color FOVColor = Color.cyan;
        public Color ESPColor = Color.red;
        public Color tracerColor = Color.cyan;
        public Color chamsColor = Color.red;

        public static Config Instance = new Config();

        private Vector2 _debugScrollPosition = Vector2.zero;
        private List<string> _debugLogs = new List<string>();
        public IReadOnlyList<string> DebugLogs => _debugLogs;
        private const int MAX_LOGS = 50;
        private Texture2D _svTex;
        private Texture2D _hueTex;
        private Texture2D _colorWheelTex;
        private float _pickerRadius = 90f; // radius of the wheel
        private Vector2 _wheelCenter;

        private bool CustomizeMode = false;
        private Vector2 _scrollPosition1 = Vector2.zero;
        public bool Menu = false;
        // Player settings
        public float speedValue = 2.3f;
        public float sspeedValue = 3f;
        public float jumpHeightValue = 2f;
        public float Crouch = 3f;
        public float Gravity = 20f;
        public float MoveSpeed1 = 1f;
        public float JumpSpeed1 = 1f;
        public float WallJump1 = 1f;
        public bool enableSpeedHack = false;
        public bool enableIsSlide = false;

        // Cheats
        public bool ESP = true;
        public bool ESP3d = false;
        public bool tracer = false;
        public bool chams = false;
        public bool DrawFOVCircle { get; set; } = true;
        public float _fovRadius = 100f;

        public bool InfiniteAmmo = false;
        public bool RapidFire = false;
        public bool InstaKill = false;
        public bool Wallbang = false;
        public bool NoSpread = false;
        public bool Aimbot = false;
        public bool NoRecoil = false;
        public bool FlyMode = false;
        public bool GodMode = false;
        public bool Teleport = false;
        // Restore missing fields so code referencing them compiles
        public static float Strength = 1;
        public float WDamage = 1;
        public bool SpamKillfeed = false;
        public static bool SpamEffects = false;    // Restore
                                                   // Additional movement / cheat field

        public bool MagicBullet = false;
        public bool FreezeEnemy = false;
        public bool ExplosiveAmmo = false;
        public bool RepulsorToMars = false;
        public bool RemoveEnemyWeapons = false;
        public bool SpamSuppression = false;
        public bool SpoofedName = false;
        public string Name = "Mai";
        public string KillfeedMessage = "<size=10><color=#FF5733>s</color><color=#33FF57>o</color><color=#5733FF>d</color><color=#FF33A1>h</color><color=#33FFF5>o</color><color=#F533FF>o</color><color=#FFAA33>k</color></size>";
        private GUIStyle _windowStyle, _headerStyle, _buttonStyle, _toggleStyle, _logStyle;

        public UITheme _lastTheme;
        public Color _lastAccent;
        public Color _lastBgDark;
        public Color _lastBgMid;
        public Color _lastBgHover;
        private bool _showAccentSliders = false;
        private bool _showThemeSliders = false;
        private bool _showbgMSliders = false;
        private bool _showbgHSliders = false;
        private bool _showESPSliders = false;
        private bool _showFOVSliders = false;
        private bool _showtracerSliders = false;
        private bool _showchamsSliders = false;
        public static List<PlayerHealth> players = new List<PlayerHealth>();

        public static List<Suppression> suppressions = new List<Suppression>();

        public static List<Weapon> weapons = new List<Weapon>();

        public static Weapon localWeapon = null;

        public static Camera mainCamera;

        public static PlayerHealth localPlayer;

        public static PlayerHealth enemyPlayer;

        public static Transform enemyHead = null;

        public static FirstPersonController controller;

        private float lastUpdated = 0f;

        private float lastSpam = 0f;

        public static Shader shader = null;

        private FieldInfo CosmeticsManager_suitsChildren = typeof(CosmeticsManager).GetField("suitsChildren", BindingFlags.Instance | BindingFlags.NonPublic);

        private FieldInfo CosmeticsManager_hatsChildren = typeof(CosmeticsManager).GetField("hatsChildren", BindingFlags.Instance | BindingFlags.NonPublic);

        private MethodInfo steal = typeof(PlayerPickup).GetMethod("RpcLogic___SetObjectInHandServer_46969756", BindingFlags.Instance | BindingFlags.NonPublic);

        private FieldInfo gravity = typeof(FirstPersonController).GetField("gravity", BindingFlags.Instance | BindingFlags.NonPublic);

        private FieldInfo crouchGravity = typeof(FirstPersonController).GetField("crouchGravity", BindingFlags.Instance | BindingFlags.NonPublic);

        private FieldInfo jumpGravity = typeof(FirstPersonController).GetField("jumpGravity", BindingFlags.Instance | BindingFlags.NonPublic);

        private FieldInfo WeaponHandSpawnerproximityMine = typeof(WeaponHandSpawner).GetField("proximityMine", BindingFlags.Instance | BindingFlags.NonPublic);

        private FieldInfo WeaponHandSpawnerclaymore = typeof(WeaponHandSpawner).GetField("claymore", BindingFlags.Instance | BindingFlags.NonPublic);

        private FieldInfo WeaponHandSpawnerobjToSpawn = typeof(WeaponHandSpawner).GetField("objToSpawn", BindingFlags.Instance | BindingFlags.NonPublic);

        private FieldInfo PlayerPickupcam = typeof(PlayerPickup).GetField("cam", BindingFlags.Instance | BindingFlags.NonPublic);

        private MethodInfo PlayerPickupRightHandPickup = typeof(PlayerPickup).GetMethod("RightHandPickup", BindingFlags.Instance | BindingFlags.NonPublic);

        public static FieldInfo behaviorInHand = typeof(PlayerPickup).GetField("behaviourInHand", BindingFlags.Instance | BindingFlags.NonPublic);

        private void DrawSidebarButton(string label, Page page)
        {
            bool selected = _currentPage == page;
            GUIStyle style = new GUIStyle(_buttonStyle);
            if (selected)
                style.normal.background = MakeRoundedTexture(2, 2, AccentColor * new Color(1, 1, 1, 0.35f), 6, GUIItemSettings.EdgeType.Round);

            if (GUILayout.Button(label, style, GUILayout.Height(36)))
                _currentPage = page;
        }

        private Texture2D MakeRoundedTexture(int width, int height, Color color, int radius, EdgeType edge = EdgeType.Boxy)
        {
            if (edge == EdgeType.Boxy || radius <= 0)
            {
                Texture2D flat = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                flat.SetPixel(0, 0, color);
                flat.Apply();
                return flat;
            }

            width = Mathf.Max(width, radius * 2 + 2);
            height = Mathf.Max(height, radius * 2 + 2);

            Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;

            float r = radius;
            float w = width;
            float h = height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dx = Mathf.Max(Mathf.Abs(x - w * 0.5f) - (w * 0.5f - r), 0);
                    float dy = Mathf.Max(Mathf.Abs(y - h * 0.5f) - (h * 0.5f - r), 0);
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    float alpha = Mathf.Clamp01(1f - (dist - r + 1f));
                    Color c = color;
                    c.a *= alpha;

                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            return tex;
        }

        private void HoverRound(Rect r, float radius = 6f)
        {
            if (!r.Contains(Event.current.mousePosition)) return;

            Color glow = AccentColor * new Color(1, 1, 1, 0.18f);
            GUI.DrawTexture(r, MakeRoundedTexture(2, 2, glow, (int)radius, GUIItemSettings.EdgeType.Round));
        }
        private bool StylesAreDead()
        {
            if (_windowStyle == null) return true;

            var bg = _windowStyle.normal.background;
            if (bg == null) return true;

            // Unity destroys texture but keeps reference
            return !bg;
        }

        public void InitializeStyles()
        {
            if (!StylesAreDead() &&
                _lastTheme == CurrentTheme &&
                _lastAccent == AccentColor &&
                _lastBgDark == CurrentThemeColor.bgDark &&
                _lastBgMid == CurrentThemeColor.bgMid &&
                _lastBgHover == CurrentThemeColor.bgHover)
            {
                return;
            }

            _lastTheme = CurrentTheme;
            _lastAccent = AccentColor;
            _lastBgDark = CurrentThemeColor.bgDark;
            _lastBgMid = CurrentThemeColor.bgMid;
            _lastBgHover = CurrentThemeColor.bgHover;

            BuildStyles();
        }
        private void BuildStyles()
        {
            Color bgDark = CurrentThemeColor.bgDark;
            Color bgMid = CurrentThemeColor.bgMid;
            Color bgHover = CurrentThemeColor.bgHover;
            Color accentSoft = new Color(AccentColor.r, AccentColor.g, AccentColor.b, 0.25f);

            _windowStyle = new GUIStyle(GUI.skin.window)
            {
                normal = { background = MakeRoundedTexture(128, 128, bgDark, 18), textColor = Color.white },
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(12, 12, 12, 12)
            };

            _headerStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { background = MakeRoundedTexture(2, 2, bgMid, 4), textColor = AccentColor },
                hover = { background = MakeRoundedTexture(2, 2, bgHover, 4) },
                active = { background = MakeRoundedTexture(2, 2, bgHover + accentSoft, 4) },
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(12, 10, 6, 6),
                margin = new RectOffset(4, 4, 6, 2)
            };

            _toggleStyle = new GUIStyle(GUI.skin.toggle)
            {
                normal = { textColor = Color.white },
                onNormal = { textColor = AccentColor },
                hover = { textColor = AccentColor },
                onHover = { textColor = AccentColor },
                fontSize = 12,
                padding = new RectOffset(22, 5, 4, 4),
                margin = new RectOffset(6, 6, 3, 3)
            };

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { background = MakeRoundedTexture(2, 2, bgMid, 4), textColor = Color.white },
                hover = { background = MakeRoundedTexture(2, 2, bgHover, 4), textColor = AccentColor },
                active = { background = MakeRoundedTexture(2, 2, accentSoft, 4) },
                fontSize = 12,
                padding = new RectOffset(10, 10, 6, 6)
            };

            _logStyle = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = new Color(0.85f, 0.85f, 0.9f, 1f) },
                fontSize = 11,
                wordWrap = true,
                richText = true,
                padding = new RectOffset(4, 4, 2, 2)
            };
        }
        private void ModernKeyBind(string label, string keyName)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, _logStyle, GUILayout.Width(160));

            KeyCode current = KeyBinds.ContainsKey(keyName) && KeyBinds[keyName].Count > 0
                ? KeyBinds[keyName][0]
                : KeyCode.None;

            if (GUILayout.Button(current == KeyCode.None ? "NONE" : current.ToString(),
                                  _buttonStyle, GUILayout.Width(80)))
            {
                StartListeningForKey(keyName);
            }

            GUILayout.EndHorizontal();
        }

        public void draw()
        {
            InitializeStyles();

            GUILayout.BeginArea(new Rect(40, 40, 700, 600), _windowStyle);
            GUILayout.Label("straftard - 80HE", new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 18,
                fontStyle = FontStyle.BoldAndItalic,
                normal = { textColor = AccentColor }
            });

            GUILayout.Space(6);
            GUILayout.BeginHorizontal();

            // Sidebar
            GUILayout.BeginVertical(GUILayout.Width(170));
            DrawSidebarButton("Aiming", Page.Aiming);
            DrawSidebarButton("Visuals", Page.Visuals);
            DrawSidebarButton("Weapons", Page.Weapons);
            DrawSidebarButton("Weapon Mods", Page.WeaponMods);
            DrawSidebarButton("Player", Page.Player);
            DrawSidebarButton("Misc", Page.Misc);
            DrawSidebarButton("Logs", Page.Logs);
            DrawSidebarButton("Settings", Page.Settings);

            GUILayout.EndVertical();

            // Content
            GUILayout.BeginVertical(GUI.skin.box);
            _scrollPosition1 = GUILayout.BeginScrollView(_scrollPosition1);

            switch (_currentPage)
            {
                case Page.Aiming: DrawAiming(); break;
                case Page.Visuals: DrawVisuals(); break;
                case Page.Weapons: Cheat.Instance.DrawWeapons(); break;
                case Page.WeaponMods: DrawWeaponMods(); break;
                case Page.Player: DrawMovement(); break;
                case Page.Misc: DrawMisc(); break;
                case Page.Logs: DrawLogs(); break;
                case Page.Settings: DrawSettings(); break;
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        // ===== MODERN UI CONTROLS =====

        public bool ModernToggle(string label, bool value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, _logStyle, GUILayout.Width(160));

            GUIStyle style = new GUIStyle(_buttonStyle);
            style.normal.textColor = value ? AccentColor : Color.gray;
            style.normal.background = MakeRoundedTexture(2, 2,
                value ? AccentColor * new Color(1, 1, 1, 0.25f) : CurrentThemeColor.bgMid,
                6, GUIItemSettings.EdgeType.Round);

            if (GUILayout.Button(value ? "ON" : "OFF", style, GUILayout.Width(60), GUILayout.Height(24)))
                value = !value;

            GUILayout.EndHorizontal();
            return value;
        }
        private GUIStyle _invisibleSlider;
        private GUIStyle _invisibleSliderThumb;


        private float ModernSlider(string label, float value, float min, float max)
        {
            // Lazy init for invisible slider
            if (_invisibleSlider == null || _invisibleSliderThumb == null)
            {
                Texture2D clear = new Texture2D(1, 1);
                clear.SetPixel(0, 0, new Color(0, 0, 0, 0));
                clear.Apply();

                _invisibleSlider = new GUIStyle(GUI.skin.horizontalSlider)
                {
                    fixedHeight = 20
                };
                _invisibleSlider.normal.background = clear;
                _invisibleSlider.active.background = clear;
                _invisibleSlider.hover.background = clear;
                _invisibleSlider.focused.background = clear;

                _invisibleSliderThumb = new GUIStyle(GUI.skin.horizontalSliderThumb)
                {
                    fixedWidth = 0,   // grab area
                    fixedHeight = 20
                };
                _invisibleSliderThumb.normal.background = clear;
                _invisibleSliderThumb.active.background = clear;
                _invisibleSliderThumb.hover.background = clear;
                _invisibleSliderThumb.focused.background = clear;
            }

            // Label
            GUILayout.Label($"{label}: {value:F1}", _logStyle);

            // Get rect
            Rect r = GUILayoutUtility.GetRect(260, 20);

            // INPUT FIRST
            value = GUI.HorizontalSlider(r, value, min, max, _invisibleSlider, _invisibleSliderThumb);

            // DRAW AFTER (visual overlay)
            GUI.Box(r, "", GUI.skin.box);
            float pct = Mathf.InverseLerp(min, max, value);
            Rect fill = new Rect(r.x, r.y, r.width * pct, r.height);
            GUI.DrawTexture(fill, MakeRoundedTexture(1, 1, AccentColor, 0, GUIItemSettings.EdgeType.Boxy));

            return value;
        }
        public void ModernHeader(string text)
        {
            GUILayout.Space(8);
            GUILayout.Label("  " + text, _headerStyle);
            GUILayout.Space(4);
        }

        public void ModernButton(string label, Action onClick)
        {
            if (GUILayout.Button(label, _buttonStyle, GUILayout.Height(28)))
                onClick?.Invoke();
        }

        private void DrawAiming()
        {
            ModernHeader("AIMBOT");

            Aimbot = ModernToggle("Aimbot", Aimbot);
            MagicBullet = ModernToggle("Silent Aim", MagicBullet);
        }

        private void DrawVisuals()
        {
            ModernHeader("ESP");

            ESP = ModernToggle("Square ESP", ESP);
            ESP3d = ModernToggle("Box ESP", ESP3d);
            tracer = ModernToggle("Tracers", tracer);
            chams = ModernToggle("Chams", chams);
            DrawFOVCircle = ModernToggle("FOV Circle", DrawFOVCircle);
            GUILayout.Label("FOV Size - Default 100", _logStyle);
            _fovRadius = ModernSlider("FOV SIZE", _fovRadius, 10f, 500f);

            ModernHeader("COLORS");

            GUILayout.Label("ESP Color", _logStyle);
            ESPColor = DrawESPColorSliders(ESPColor);

            GUILayout.Label("Tracer Color", _logStyle);
            tracerColor = DrawTracerColorSliders(tracerColor);

            GUILayout.Label("Chams Color", _logStyle);
            chamsColor = DrawChamsColorSliders(chamsColor);
            GUILayout.Label("FOV Color", _logStyle);
            FOVColor = DrawFOVColorSliders(FOVColor);
        }
        bool showWeapons = false;
        private void DrawWeaponMods()
        {
            
        ModernHeader("WEAPON MODS");

            InfiniteAmmo = ModernToggle("Infinite Ammo", InfiniteAmmo);
            WDamage = ModernSlider("Damage", WDamage, 0f, 999f);
            InstaKill = ModernToggle("Insta Kill", InstaKill);
            RapidFire = ModernToggle("Rapid Fire", RapidFire);
            NoSpread = ModernToggle("No Spread", NoSpread);
            NoRecoil = ModernToggle("No Recoil", NoRecoil);
            Wallbang = ModernToggle("Wallbang", Wallbang);
        }
        
        private void DrawMovement()
        {
            ModernHeader("Player");
            GodMode = ModernToggle("God Mode", GodMode);
            SpoofedName = ModernToggle("Spoof Name", SpoofedName);
            enableIsSlide = ModernToggle("Spoof Slide", enableIsSlide);
            SpamSuppression = ModernToggle("Damage Suppression", SpamSuppression);
            Teleport = ModernToggle("TP to Enemy", Teleport);
            ModernHeader("Movement");
            FlyMode = ModernToggle("Flight Mode", FlyMode);
            enableSpeedHack = ModernToggle("Set Movement", enableSpeedHack);
            speedValue = ModernSlider("Walk Speed", speedValue, 1f, 100f);
            sspeedValue = ModernSlider("Sprint Speed", sspeedValue, 1f, 100f);
            Crouch = ModernSlider("Crouch Speed", Crouch, 1f, 50f);
            jumpHeightValue = ModernSlider("Jump Height", jumpHeightValue, 1f, 50f);
            Gravity = ModernSlider("Gravity", Gravity, 0f, 50f);
            MoveSpeed1 = ModernSlider("Gun Speed", MoveSpeed1, 1f, 100f);
            JumpSpeed1 = ModernSlider("Gun Jump Height", JumpSpeed1, 1f, 100f);
        }

        private void DrawMisc()
        {
            ModernHeader("MISC");
            SpamEffects = ModernToggle("Crasher", SpamEffects);
            Strength = ModernSlider("Lag Amount", Strength, 1, 500);
            FreezeEnemy = ModernToggle("Freeze Enemy", FreezeEnemy);
            RepulsorToMars = ModernToggle("Repulsor To Mars", RepulsorToMars);
            RemoveEnemyWeapons = ModernToggle("Disarm Enemies", RemoveEnemyWeapons);
        }

        private void DrawLogs()
        {
            ModernHeader("DEBUG LOGS");

            _debugScrollPosition = GUILayout.BeginScrollView(_debugScrollPosition, GUILayout.Height(300));
            foreach (string log in _debugLogs)
                GUILayout.Label(log, _logStyle);
            GUILayout.EndScrollView();

            ModernButton("Clear Logs", () => _debugLogs.Clear());
        }
        private T EnumPopup<T>(string label, T value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, _logStyle, GUILayout.Width(160));
            string[] options = Enum.GetNames(typeof(T));
            int index = Array.IndexOf(options, value.ToString());
            index = GUILayout.SelectionGrid(index, options, 1);
            value = (T)Enum.Parse(typeof(T), options[index]);
            GUILayout.EndHorizontal();
            return value;
        }
        private void DrawSettings()
        {

            ModernHeader("KEYBINDS");
            ModernKeyBind("Menu", "Menu");


            ModernKeyBind("Aimbot", "Aimbot");
            ModernKeyBind("Silent Aim", "Silent Aim");
            ModernKeyBind("ESP", "ESP");
            ModernKeyBind("ESP3d", "ESP3d");
            ModernKeyBind("Tracer", "Tracer");
            ModernKeyBind("Chams", "Chams");
            ModernKeyBind("DrawFOVCircle", "DrawFOVCircle");

            ModernHeader("MOVEMENT");
            ModernKeyBind("Speed", "Speed");
            ModernKeyBind("Slide", "Slide");
            ModernKeyBind("FlyMode", "FlyMode");
            ModernKeyBind("Teleport", "Teleport");

            ModernHeader("MISC");
            ModernKeyBind("GodMode", "GodMode");
            ModernKeyBind("SpamProjectiles", "SpamProjectiles");
            ModernKeyBind("SpamSuppression", "SpamSuppression");
            ModernKeyBind("FreezeEnemy", "FreezeEnemy");
            ModernKeyBind("RepulsorToMars", "RepulsorToMars");
            ModernKeyBind("Crash", "Crash");
            ModernHeader("CUSTOMIZATION");
            AccentColor = DrawAccentColorSliders(AccentColor);
            CurrentThemeColor.bgDark = DrawBgDarkColorSliders(CurrentThemeColor.bgDark);
            CurrentThemeColor.bgMid = DrawBgMidColorSliders(CurrentThemeColor.bgMid);
            CurrentThemeColor.bgHover = DrawBgHoverColorSliders(CurrentThemeColor.bgHover);

        }
            private Color DrawAccentColorSliders(Color c) => ModernColorPicker("Accent Color", ref _showAccentSliders, c);

        private Color DrawBgDarkColorSliders(Color c) => ModernColorPicker("Background Dark", ref _showThemeSliders, c);

        private Color DrawBgMidColorSliders(Color c) => ModernColorPicker("Background Mid", ref _showbgMSliders, c);

        private Color DrawBgHoverColorSliders(Color c) => ModernColorPicker("Background Hover", ref _showbgHSliders, c);


        public void AddDebugLog(string message)
        {
            _debugLogs.Insert(0, $"[{Time.time:F1}] {message}");
            if (_debugLogs.Count > MAX_LOGS) _debugLogs.RemoveAt(_debugLogs.Count - 1);
        }

        // Color slider helpers
        private Color DrawESPColorSliders(Color c) => ModernColorPicker("ESP Color", ref _showESPSliders, c);
        private Color DrawTracerColorSliders(Color c) => ModernColorPicker("Tracer Color", ref _showtracerSliders, c);
        private Color DrawChamsColorSliders(Color c) => ModernColorPicker("Chams Color", ref _showchamsSliders, c);
        private Color DrawFOVColorSliders(Color c) => ModernColorPicker("FOV Color", ref _showFOVSliders, c);
        private Color ModernColorPicker(string label, ref bool show, Color current)
        {
            Rect header = GUILayoutUtility.GetRect(260, 26);
            if (GUI.Button(header, $"{label} {(show ? "▾" : "▸")}", _buttonStyle))
                show = !show;

            if (!show) return current;

            GUILayout.Space(6);

            // Wheel size
            float wheelSize = 180f; // square so no squashing
            GenerateColorWheel((int)wheelSize);

            // Center position in GUI coordinates
            Rect wheelRect = GUILayoutUtility.GetRect(wheelSize, wheelSize, GUILayout.ExpandWidth(false));
            _wheelCenter = new Vector2(wheelRect.x + wheelSize / 2f, wheelRect.y + wheelSize / 2f);

            // Draw the wheel
            GUI.DrawTexture(wheelRect, _colorWheelTex);

            // Handle mouse input
            Vector2 mousePos = Event.current.mousePosition;
            Vector2 delta = mousePos - _wheelCenter;
            float dist = delta.magnitude / (wheelSize / 2f);

            if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) && dist <= 1f)
            {
                float angle = Mathf.Atan2(delta.y, delta.x);
                float hue = (angle / (2f * Mathf.PI) + 0.5f) % 1f;
                float sat = dist;

                // Keep the original value (brightness)
                Color.RGBToHSV(current, out _, out _, out float val);
                current = Color.HSVToRGB(hue, sat, val);

                Event.current.Use(); // consume event
            }

            // Value (brightness) slider
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Brightness", _logStyle, GUILayout.Width(60));
            Color.RGBToHSV(current, out float h, out float s, out float v);
            v = GUILayout.HorizontalSlider(v, 0f, 1f, GUILayout.Width(wheelSize));
            current = Color.HSVToRGB(h, s, v);
            GUILayout.EndHorizontal();

            // Preview
            Rect preview = GUILayoutUtility.GetRect(60, 22);
            GUI.DrawTexture(preview, MakeRoundedTexture(2, 2, current, 0, GUIItemSettings.EdgeType.Boxy));

            return current;
        }

        private void GenerateColorWheel(int size)
{
    if (_colorWheelTex != null && _colorWheelTex.width == size) return;

    _colorWheelTex = new Texture2D(size, size, TextureFormat.RGBA32, false);
    _colorWheelTex.wrapMode = TextureWrapMode.Clamp;

    Vector2 center = new Vector2(size / 2f, size / 2f);
    float radius = size / 2f;

    for (int y = 0; y < size; y++)
    {
        for (int x = 0; x < size; x++)
        {
            // Flip y so the wheel is upside-down properly
            Vector2 dir = new Vector2(x, size - 1 - y) - center;
            float dist = dir.magnitude / radius;
            if (dist > 1f)
            {
                _colorWheelTex.SetPixel(x, y, Color.clear);
                continue;
            }

            float angle = Mathf.Atan2(dir.y, dir.x);
            float hue = (angle / (2f * Mathf.PI) + 0.5f) % 1f;
            float sat = dist;

            Color c = Color.HSVToRGB(hue, sat, 1f);
            _colorWheelTex.SetPixel(x, y, c);
        }
    }

    _colorWheelTex.Apply();
}

        // GUI customization
        public class GUIItemSettings { public Color color = Color.black; public float opacity = 0.25f; public float size = 1f; public enum EdgeType { Boxy, Round } public EdgeType edge = EdgeType.Boxy; }
        public Dictionary<string, GUIItemSettings> GUISettings = new Dictionary<string, GUIItemSettings>()
        {
            { "Aiming", new GUIItemSettings() }, { "Visuals", new GUIItemSettings() },
            { "Weapon Mods", new GUIItemSettings() }, { "Movement", new GUIItemSettings() },
            { "Miscellaneous", new GUIItemSettings() }, { "Logs", new GUIItemSettings() }
        };
        private bool _captureNextFrame = false;
        private string _waitingForKey = null;

        // MULTI-KEY BINDS
        public Dictionary<string, List<KeyCode>> KeyBinds =
            new Dictionary<string, List<KeyCode>>();

        // Call once at startup

        public void Register(string id)
        {
            id = id.Trim();

            if (!KeyBinds.ContainsKey(id))
                KeyBinds[id] = new List<KeyCode>();
        }

        // Returns true if ANY key bound to this action is pressed
        // Track previous frame state
        private HashSet<KeyCode> _heldKeys = new HashSet<KeyCode>();
        private Dictionary<KeyCode, bool> _previousKeyState = new Dictionary<KeyCode, bool>();

        public bool Pressed(string action)
        {
            if (!KeyBinds.ContainsKey(action)) return false;

            foreach (var k in KeyBinds[action])
            {
                if (k != KeyCode.None && Input.GetKeyDown(k))
                    return true;
            }

            return false;
        }
        // Capture a new keybind
        public void ProcessKeyBind()
        {
            if (_waitingForKey == null) return;

            // Wait one frame after clicking button
            if (_captureNextFrame)
            {
                _captureNextFrame = false;
                return;
            }

            foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(k))
                {
                    KeyBinds[_waitingForKey].Clear(); // remove old key
                    KeyBinds[_waitingForKey].Add(k);  // set new key

                    Debug.Log($"Bound {_waitingForKey} -> {k}");
                    _waitingForKey = null;
                    break;
                }
            }
        }

        // Start listening for a new key
        public void StartListeningForKey(string label)
        {
            AddDebugLog("Started: " + label);

            foreach (var k in KeyBinds.Keys)
            //    AddDebugLog("Have: " + k);

            if (!KeyBinds.ContainsKey(label))
            {
                AddDebugLog("NOT FOUND");
                return;
            }

            _waitingForKey = label;
            _captureNextFrame = true;
            AddDebugLog("LISTENING");

            GUI.FocusControl(null);
        }

        // Remove a key
        public void RemoveBind(string label, KeyCode key)
        {
            if (KeyBinds.ContainsKey(label))
                KeyBinds[label].Remove(key);
        }

        private void Update()
        {
            HandleKeyToggles(); // ← check toggles FIRST
            ProcessKeyBind();  // ← then consume keybind input
        }

        // Your action logic
        public void HandleKeyToggles()
        {
            if (Pressed("Menu")) Menu = !Menu;
            if (Pressed("Aimbot")) Aimbot = !Aimbot;
            if (Pressed("Silent Aim")) MagicBullet = !MagicBullet;
            if (Pressed("ESP")) ESP = !ESP;
            if (Pressed("ESP3d")) ESP3d = !ESP3d;
            if (Pressed("Tracer")) tracer = !tracer;
            if (Pressed("Chams")) chams = !chams;
            if (Pressed("DrawFOVCircle")) DrawFOVCircle = !DrawFOVCircle;

            if (Pressed("FlyMode")) FlyMode = !FlyMode;
            if (Pressed("Teleport")) Teleport = !Teleport;

            if (Pressed("GodMode")) GodMode = !GodMode;
            if (Pressed("Crasher")) SpamEffects = !SpamEffects;
            if (Pressed("FreezeEnemy")) FreezeEnemy = !FreezeEnemy;
            if (Pressed("RepulsorToMars")) RepulsorToMars = !RepulsorToMars;
            if (Pressed("Speed")) enableSpeedHack = !enableSpeedHack;
            if (Pressed("Slide")) enableIsSlide = !enableIsSlide;
            if (Pressed("Crash")) SpamEffects = !SpamEffects;
        }
    }
}