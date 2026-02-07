using strafthot.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace strafthot
{
    public class Config
    {
        public enum UITheme
        {
            Dark,
            Neon,
            Red,
            Purple
        }

        public UITheme CurrentTheme = UITheme.Dark;

        // Accent color (editable in GUI)
        public Color AccentColor = new Color(0.25f, 0.75f, 1f, 1f);
        public ThemeColor CurrentThemeColor = new ThemeColor();
        public class ThemeColor
        {
            public Color bgDark = new Color(0.08f, 0.08f, 0.09f, 0.95f);
            public Color bgMid = new Color(0.15f, 0.15f, 0.16f, 1f);
            public Color bgHover = new Color(0.22f, 0.22f, 0.25f, 1f);
        }
        // Visual colors
        public Color FOVColor = Color.cyan;
        public Color ESPColor = Color.red;
        public static Config Instance = new Config();
        private Vector2 _debugScrollPosition = Vector2.zero;
        private List<string> _debugLogs = new List<string>();
        private const int MAX_LOGS = 50;
        private bool _showDebugWindow = true;
        private bool _showCombat = false;
        private bool _showVisuals = false;
        private bool _showWeaponMods = false;
        private bool _showMovement = false;
        private bool _showMisc = false;
        private bool _showDebug = false;
        public float MoveSpeed1 = 1f;
        public float JumpSpeed1 = 1f;
        public float WallJump1 = 1f;

        public bool ESP = true;
        public bool ESP3d = false;
        public bool DrawFOVCircle { get; set; } = true; // Default to true or false based on your preference
        public bool InfiniteAmmo = false;
        public bool RapidFire = false;
        public bool InstaKill = false;
        public bool Wallbang = false;
        public bool NoSpread = false;
        public bool Aimbot = false;
        public bool NoRecoil = false;
        public bool FlyMode = false;
        public bool GodMode = false;
        public int Strength = 10;
        public bool MagicBullet = false;
        public bool FreezeEnemy = false;
        public bool ExplosiveAmmo = false;
        public bool RepulsorToMars = false;
        public bool RemoveEnemyWeapons = false;
        public bool SpamProjectiles = false;
        public bool SpamSuppression = false;
        public bool SpamEffects = false;
        public bool SpamKillfeed = false;
        public bool SpoofedName = false;
        public string KillfeedMessage = "<size=100><color=#FF5733>s</color><color=#33FF57>o</color><color=#5733FF>d</color><color=#FF33A1>h</color><color=#33FFF5>o</color><color=#F533FF>o</color><color=#FFAA33>k</color></size>";

        private GUIStyle _windowStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _toggleStyle;
        private GUIStyle _logStyle;
        private Vector2 _scrollPosition1 = Vector2.zero;

        private UITheme _lastTheme;
        private Color _lastAccent;
        private bool _showAccentSliders = false;
        private bool _showThemeSliders = false;
        private bool _showESPSliders = false;
        private bool _showFOVSliders = false;
        private Texture2D MakeRoundedTexture(int width, int height, Color color, int radius)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
            tex.filterMode = FilterMode.Bilinear;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool inside = true;

                    // Top-left corner
                    if (x < radius && y < radius)
                        inside = (new Vector2(radius - x, radius - y).magnitude <= radius);
                    // Top-right corner
                    if (x >= width - radius && y < radius)
                        inside = (new Vector2(x - (width - radius), radius - y).magnitude <= radius);
                    // Bottom-left corner
                    if (x < radius && y >= height - radius)
                        inside = (new Vector2(radius - x, y - (height - radius)).magnitude <= radius);
                    // Bottom-right corner
                    if (x >= width - radius && y >= height - radius)
                        inside = (new Vector2(x - (width - radius), y - (height - radius)).magnitude <= radius);

                    tex.SetPixel(x, y, inside ? color : new Color(0, 0, 0, 0)); // transparent outside corners
                }
            }

            tex.Apply();
            return tex;
        }



        private void InitializeStyles()
        {
            if (_windowStyle != null && _lastTheme == CurrentTheme && _lastAccent == AccentColor)
                return;

            _lastTheme = CurrentTheme;
            _lastAccent = AccentColor;

            Color bgDark, bgMid, bgHover;

            switch (CurrentTheme)
            {
                default:
                case UITheme.Dark:
                    bgDark = new Color(0.08f, 0.08f, 0.09f, 0.95f);
                    bgMid = new Color(0.15f, 0.15f, 0.16f, 1f);
                    bgHover = new Color(0.22f, 0.22f, 0.25f, 1f);
                    break;

                case UITheme.Neon:
                    bgDark = new Color(0.02f, 0.02f, 0.05f, 0.95f);
                    bgMid = new Color(0.05f, 0.05f, 0.15f, 1f);
                    bgHover = new Color(0.15f, 0.15f, 0.35f, 1f);
                    break;

                case UITheme.Red:
                    bgDark = new Color(0.12f, 0.02f, 0.02f, 0.95f);
                    bgMid = new Color(0.2f, 0.05f, 0.05f, 1f);
                    bgHover = new Color(0.3f, 0.08f, 0.08f, 1f);
                    break;

                case UITheme.Purple:
                    bgDark = new Color(0.07f, 0.02f, 0.12f, 0.95f);
                    bgMid = new Color(0.15f, 0.05f, 0.25f, 1f);
                    bgHover = new Color(0.25f, 0.12f, 0.45f, 1f);
                    break;
            }

            Color accent = AccentColor;
            Color accentSoft = new Color(accent.r, accent.g, accent.b, 0.25f);

            _windowStyle = new GUIStyle(GUI.skin.window);
            _windowStyle.normal.background = MakeRoundedTexture(128, 128, bgDark, 18); // 16px corner radius
            _windowStyle.normal.textColor = Color.white;
            _windowStyle.fontSize = 15;
            _windowStyle.fontStyle = FontStyle.Bold;
            _windowStyle.padding = new RectOffset(12, 12, 12, 12);


            _headerStyle = new GUIStyle(GUI.skin.button);
            _headerStyle.normal.background = MakeTexture(2, 2, bgMid);
            _headerStyle.hover.background = MakeTexture(2, 2, bgHover);
            _headerStyle.active.background = MakeTexture(2, 2, bgHover + accentSoft);
            _headerStyle.normal.textColor = accent;
            _headerStyle.fontSize = 13;
            _headerStyle.fontStyle = FontStyle.Bold;
            _headerStyle.alignment = TextAnchor.MiddleLeft;
            _headerStyle.padding = new RectOffset(12, 10, 6, 6);
            _headerStyle.margin = new RectOffset(4, 4, 6, 2);

            _toggleStyle = new GUIStyle(GUI.skin.toggle);
            _toggleStyle.normal.textColor = Color.white;
            _toggleStyle.onNormal.textColor = accent;
            _toggleStyle.hover.textColor = accent;
            _toggleStyle.onHover.textColor = accent;
            _toggleStyle.fontSize = 12;
            _toggleStyle.padding = new RectOffset(22, 5, 4, 4);
            _toggleStyle.margin = new RectOffset(6, 6, 3, 3);

            _buttonStyle = new GUIStyle(GUI.skin.button);
            _buttonStyle.normal.background = MakeTexture(2, 2, bgMid);
            _buttonStyle.hover.background = MakeTexture(2, 2, bgHover);
            _buttonStyle.active.background = MakeTexture(2, 2, accentSoft);
            _buttonStyle.normal.textColor = Color.white;
            _buttonStyle.hover.textColor = accent;
            _buttonStyle.fontSize = 12;
            _buttonStyle.padding = new RectOffset(10, 10, 6, 6);

            _logStyle = new GUIStyle(GUI.skin.label);
            _logStyle.normal.textColor = new Color(0.85f, 0.85f, 0.9f, 1f);
            _logStyle.fontSize = 11;
            _logStyle.wordWrap = true;
            _logStyle.richText = true;
            _logStyle.padding = new RectOffset(4, 4, 2, 2);
        }



        private Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = color;

            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }



        // GUI Accent Color (rebuilds styles)
        // Accent color sliders (with collapsible)
        // Accent color sliders (with collapsible)
        private Color DrawAccentColorSliders(Color c)
        {
            _showAccentSliders = GUILayout.Toggle(
                _showAccentSliders,
                "Accent Color Sliders" + (_showAccentSliders ? " [-]" : " [+]"),
                _buttonStyle
            );

            if (_showAccentSliders)
            {
                GUILayout.Label($"R: {(int)(c.r * 255)}", _logStyle);
                c.r = GUILayout.HorizontalSlider(c.r, 0, 1);

                GUILayout.Label($"G: {(int)(c.g * 255)}", _logStyle);
                c.g = GUILayout.HorizontalSlider(c.g, 0, 1);

                GUILayout.Label($"B: {(int)(c.b * 255)}", _logStyle);
                c.b = GUILayout.HorizontalSlider(c.b, 0, 1);

                // Update GUI accent color and rebuild styles
                AccentColor = c;
                InitializeStyles();
            }

            return c;
        }
        private void DrawThemeColorSliders()
        {
            _showThemeSliders = GUILayout.Toggle(
                _showThemeSliders,
                "Theme Color Sliders" + (_showThemeSliders ? " [-]" : " [+]"),
                _buttonStyle
            );

            if (_showThemeSliders)
            {
                // Use the instance
                CurrentThemeColor.bgDark = DrawSingleColorSlider("Dark Background", CurrentThemeColor.bgDark);
                CurrentThemeColor.bgMid = DrawSingleColorSlider("Middle Background", CurrentThemeColor.bgMid);
                CurrentThemeColor.bgHover = DrawSingleColorSlider("Hover Background", CurrentThemeColor.bgHover);

                InitializeStyles();
            }
        }

        // Helper method to draw sliders for one color
        private Color DrawSingleColorSlider(string label, Color c)
        {
            GUILayout.Label(label, _logStyle);

            GUILayout.Label($"R: {(int)(c.r * 255)}", _logStyle);
            c.r = GUILayout.HorizontalSlider(c.r, 0, 1);

            GUILayout.Label($"G: {(int)(c.g * 255)}", _logStyle);
            c.g = GUILayout.HorizontalSlider(c.g, 0, 1);

            GUILayout.Label($"B: {(int)(c.b * 255)}", _logStyle);
            c.b = GUILayout.HorizontalSlider(c.b, 0, 1);

            GUILayout.Label($"A: {(int)(c.a * 255)}", _logStyle);
            c.a = GUILayout.HorizontalSlider(c.a, 0, 1);

            return c;
        }

        // ESP color sliders (with collapsible)
        private Color DrawESPColorSliders(Color c)
        {
            _showESPSliders = GUILayout.Toggle(
                _showESPSliders,
                "ESP Color Sliders" + (_showESPSliders ? " [-]" : " [+]"),
                _buttonStyle
            );

            if (_showESPSliders)
            {
                GUILayout.Label($"R: {(int)(c.r * 255)}", _logStyle);
                c.r = GUILayout.HorizontalSlider(c.r, 0, 1);

                GUILayout.Label($"G: {(int)(c.g * 255)}", _logStyle);
                c.g = GUILayout.HorizontalSlider(c.g, 0, 1);

                GUILayout.Label($"B: {(int)(c.b * 255)}", _logStyle);
                c.b = GUILayout.HorizontalSlider(c.b, 0, 1);
            }

            return c;
        }

        // FOV color sliders (with collapsible)
        private Color DrawFOVColorSliders(Color c)
        {
            _showFOVSliders = GUILayout.Toggle(
                _showFOVSliders,
                "FOV Color Sliders" + (_showFOVSliders ? " [-]" : " [+]"),
                _buttonStyle
            );

            if (_showFOVSliders)
            {
                GUILayout.Label($"R: {(int)(c.r * 255)}", _logStyle);
                c.r = GUILayout.HorizontalSlider(c.r, 0, 1);

                GUILayout.Label($"G: {(int)(c.g * 255)}", _logStyle);
                c.g = GUILayout.HorizontalSlider(c.g, 0, 1);

                GUILayout.Label($"B: {(int)(c.b * 255)}", _logStyle);
                c.b = GUILayout.HorizontalSlider(c.b, 0, 1);
            }

            return c;
        }






        public void Draw()
        {
            InitializeStyles();

            GUILayout.BeginVertical(_windowStyle);
            _scrollPosition1 = GUILayout.BeginScrollView(_scrollPosition1, GUILayout.Height(700));
            GUILayout.Label(
                "straftard - 80HE",
                new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 16,
                    fontStyle = FontStyle.BoldAndItalic,
                    normal = { textColor = AccentColor }
                }
            );

            GUILayout.Space(4);
            GUILayout.Label("Theme", _logStyle);

            CurrentTheme = (UITheme)GUILayout.SelectionGrid(
                (int)CurrentTheme,
                System.Enum.GetNames(typeof(UITheme)),
                4,
                _buttonStyle
            );
            GUILayout.Label("Text Color", _logStyle);
            AccentColor = DrawAccentColorSliders(AccentColor);

            if (GUILayout.Button("Aiming" + (_showCombat ? " [-]" : " [+]"), _headerStyle))
                _showCombat = !_showCombat;
            if (_showCombat)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                Aimbot = GUILayout.Toggle(Aimbot, "Aimbot [LEFT ALT]", _toggleStyle);
                MagicBullet = GUILayout.Toggle(MagicBullet, "Silent Aim", _toggleStyle);
                GUILayout.EndVertical();
            }
            if (GUILayout.Button("Visuals" + (_showVisuals ? " [-]" : " [+]"), _headerStyle))
                _showVisuals = !_showVisuals;
            if (_showVisuals)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                ESP = GUILayout.Toggle(ESP, "Square ESP", _toggleStyle);
                ESP3d = GUILayout.Toggle(ESP3d, "Box ESP", _toggleStyle);
                GUILayout.Label("ESP Color", _logStyle);
                ESPColor = DrawESPColorSliders(ESPColor);
                DrawFOVCircle = GUILayout.Toggle(DrawFOVCircle, "FOV", _toggleStyle);
                GUILayout.Label("FOV Color", _logStyle);
                FOVColor = DrawFOVColorSliders(FOVColor);
                GUILayout.Space(4);
                GUILayout.Label("Visual Spam", _logStyle);
                SpamEffects = GUILayout.Toggle(SpamEffects, "Spam Effects - Patched", _toggleStyle);
                SpamKillfeed = GUILayout.Toggle(SpamKillfeed, "Spam Killfeed - Patched", _toggleStyle);

                GUILayout.EndVertical();
            }
            if (GUILayout.Button("Weapon Mods" + (_showWeaponMods ? " [-]" : " [+]"), _headerStyle))
                _showWeaponMods = !_showWeaponMods;
            if (_showWeaponMods)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                InfiniteAmmo = GUILayout.Toggle(InfiniteAmmo, "Infinite Ammo", _toggleStyle);
                InstaKill = GUILayout.Toggle(InstaKill, "Insta Kill", _toggleStyle);
                RapidFire = GUILayout.Toggle(RapidFire, "Rapid Fire", _toggleStyle);
                NoSpread = GUILayout.Toggle(NoSpread, "No Spread", _toggleStyle);
                NoRecoil = GUILayout.Toggle(NoRecoil, "No Recoil", _toggleStyle);
                Wallbang = GUILayout.Toggle(Wallbang, "WallBang - Patched", _toggleStyle);
                GUILayout.EndVertical();
            }
            if (GUILayout.Button("Movement" + (_showMovement ? " [-]" : " [+]"), _headerStyle))
                _showMovement = !_showMovement;
            if (_showMovement)
            {
                GUILayout.Label($"Walk Speed: {MoveSpeed1:F1}", _logStyle);
                float newMove = GUILayout.HorizontalSlider(MoveSpeed1, 1f, 20f);

                GUILayout.Label($"Jump Height: {JumpSpeed1:F1}", _logStyle);
                float newJump = GUILayout.HorizontalSlider(JumpSpeed1, 1f, 20f);

                GUILayout.Label($"Wall Jump Limit: {WallJump1:F1}", _logStyle);
                float newFall = GUILayout.HorizontalSlider(WallJump1, 1f, 50f);
                if (newMove != MoveSpeed1 || newJump != JumpSpeed1 || newFall != WallJump1)
                {
                    MoveSpeed1 = newMove;
                    JumpSpeed1 = newJump;
                    WallJump1 = newFall;
                }

                GUILayout.Space(1);
                GUILayout.BeginVertical(GUI.skin.box);
                if (GUILayout.Button("Teleport to Enemy (L)", _buttonStyle))
                {
                }
                FlyMode = GUILayout.Toggle(FlyMode, "Flight", _toggleStyle);
                GUILayout.EndVertical();
            }
            if (GUILayout.Button("Miscellaneous" + (_showMisc ? " [-]" : " [+]"), _headerStyle))
                _showMisc = !_showMisc;
            if (_showMisc)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                GodMode = GUILayout.Toggle(GodMode, "God Mode", _toggleStyle);
                SpamProjectiles = GUILayout.Toggle(SpamProjectiles, "Lag Enemies", _toggleStyle);
                SpamSuppression = GUILayout.Toggle(SpamSuppression, "Damage Suppression", _toggleStyle);
                FreezeEnemy = GUILayout.Toggle(FreezeEnemy, "Freeze Enemy", _toggleStyle);
                RepulsorToMars = GUILayout.Toggle(RepulsorToMars, "Repulsor To Mars", _toggleStyle);
                RemoveEnemyWeapons = GUILayout.Toggle(RemoveEnemyWeapons, "Disarm Enemies", _toggleStyle);
                SpoofedName = GUILayout.Toggle(SpoofedName, "Spoof name", _toggleStyle);
                GUILayout.EndVertical();
            }
            if (GUILayout.Button("Logs" + (_showDebug ? " [-]" : " [+]"), _headerStyle))
                _showDebug = !_showDebug;
            if (_showDebug)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                _debugScrollPosition = GUILayout.BeginScrollView(_debugScrollPosition, GUILayout.Height(100));
                foreach (string log in _debugLogs)
                {
                    GUILayout.Label(log, _logStyle);
                }
                GUILayout.EndScrollView();

                if (GUILayout.Button("Clear Logs", _buttonStyle))
                {
                    _debugLogs.Clear();
                }
                GUILayout.EndVertical();
            }
            GUILayout.Label("github.com/diddenbludden/      https://discord.gg/SnYxPM7Qqr");
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        public void AddDebugLog(string message)
        {
            _debugLogs.Insert(0, $"[{Time.time:F1}] {message}");
            if (_debugLogs.Count > MAX_LOGS)
                _debugLogs.RemoveAt(_debugLogs.Count - 1);
        }
    }
}
