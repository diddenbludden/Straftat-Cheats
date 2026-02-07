using strafthot.Features;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace strafthot
{
    public class Cheat : MonoBehaviour
    {
        public static Cheat Instance { get; private set; }

        // Make the cache public so other code can access it
        public Cache Cache { get; private set; }
        public Misc Misc { get; private set; }

        private MonoBehaviour _fpcObj;
        private Type _fpcType;

        private Vector2 _watermarkPos = new Vector2(10, 10);
        private List<MonoBehaviour> _weapons = new List<MonoBehaviour>();

        private bool _menuOpen = true;
        private Rect _windowRect = new Rect(100, 100, 400, 750);

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;

            // Initialize Cache and Misc
            Cache = new Cache(1f);
            Misc = new Misc();

            Config.Instance.AddDebugLog("[Cheat] Awake called.");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
                _menuOpen = !_menuOpen;

            if (_menuOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
            }

            Cache.Update();        // Works now
            WeaponMods.Update();
            Misc.Update();

            HookFirstPersonController();
            ApplyMovementSettings();
        }

        private void HookFirstPersonController()
        {
            if (_fpcObj != null) return;

            foreach (var mb in FindObjectsOfType<MonoBehaviour>())
            {
                if (mb.GetType().Name == "FirstPersonController")
                {
                    _fpcObj = mb;
                    _fpcType = mb.GetType();
                    Config.Instance.AddDebugLog($"[Hook] FirstPersonController hooked: {_fpcObj.name}");
                    break;
                }
            }
        }

        private void ApplyMovementSettings()
        {
            if (_fpcObj == null) return;

            try
            {
                _fpcType.GetField("movementFactor")?.SetValue(_fpcObj, Config.Instance.MoveSpeed1);
                _fpcType.GetField("jumpFactor")?.SetValue(_fpcObj, Config.Instance.JumpSpeed1);
                _fpcType.GetField("maxWallJumps")?.SetValue(_fpcObj, Config.Instance.WallJump1);

                Config.Instance.AddDebugLog(
                    $"[Movement] WS{Config.Instance.MoveSpeed1:F1}, JH{Config.Instance.JumpSpeed1:F1}, WJ{Config.Instance.WallJump1:F1}"
                );
            }
            catch (Exception ex)
            {
            }
        }

        private void Menu(int id)
        {
            Config.Instance.Draw();
            GUI.DragWindow(new Rect(0, 0, 10000, 700));
        }

        private void OnGUI()
        {
            if (_menuOpen)
                _windowRect = GUI.Window(0, _windowRect, Menu, "", GUIStyle.none);

            if (Config.Instance.ESP)
                ESP.OnGUI();
            if (Config.Instance.ESP3d)
                ESP.OnGUI();
            if (Config.Instance.DrawFOVCircle)
                DrawFOVCircle.OnGUI();
        }
    }
}
