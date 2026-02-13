using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Serializing;
using FishNet;
using UnityEngine.SocialPlatforms;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using static Mono.Security.X509.X520;
using DG.Tweening;

namespace strafthot.Features
{
    public class Aimbot
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        public void FlyMode(FirstPersonController playerShootInstance, bool type)
        {
            try
            {
                var flyModeField = typeof(FirstPersonController).GetField("flymode", BindingFlags.NonPublic | BindingFlags.Instance);
                if (flyModeField != null)
                {
                    flyModeField.SetValue(playerShootInstance, type);
                    Config.Instance.AddDebugLog(type ? "Fly mode Enabled." : "Fly mode Disabled.");
                }
                else
                {
                    Config.Instance.AddDebugLog("Error: flymode field not found.");
                }
            }
            catch (Exception ex)
            {
                Config.Instance.AddDebugLog("Error setting flymode field via reflection: " + ex.Message);
            }
        }

        public void PlayHitMarker(Weapon _weapon)
        {
            try
            {
                _weapon.marker = UnityEngine.Object.Instantiate<GameObject>(_weapon.hitMarker, Crosshair.Instance.transform.position, Quaternion.identity, PauseManager.Instance.transform);
                _weapon.marker.transform.DOPunchScale(new Vector3(2.5f, 2.5f, 2.5f), 0.3f, 8, 2f);
                _weapon.marker.GetComponent<UnityEngine.UI.Image>().color = Color.red;
                UnityEngine.Object.Destroy(_weapon.marker, 0.3f);
                _weapon.audio.PlayOneShot(_weapon.headHitClip);
            }
            catch (Exception ex)
            {
                Config.Instance.AddDebugLog("Error hitmarker" + ex.Message);
            }
        }

        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private readonly Cache _cache;
        private bool _isEnabled = false;
        private bool _isEnabled1 = false;
        private const float HEAD_OFFSET = 1.5f;
        private bool _isMouseHold = false;
        private bool _previousFlyMode = false;
        private bool _previousFreezeEnemy = false;

        // FOV circle properties // Default FOV radius in pixels
        private Color _fovColor = Config.Instance.FOVColor;

        public bool IsEnabled => _isEnabled;
        public bool IsEnabled1 => _isEnabled1;

        public Aimbot(Cache cache)
        {
            _cache = cache;
        }

        public void Update()
        {
            // Custom keybind for Aimbot

            if (Config.Instance.MagicBullet && _cache.LocalWeaponRight != null && _cache.LocalWeaponRight.fire1.IsPressed())
            {
                var enemyHealthController = GetClosestTarget();
                if (enemyHealthController != null && enemyHealthController.PlayerHealth.health > 0)
                {
                    PlayHitMarker(_cache.LocalWeaponRight);
                    enemyHealthController.PlayerHealth.ChangeKilledState(true);
                    enemyHealthController.PlayerHealth.RemoveHealth(10f);
                    enemyHealthController.PlayerHealth.SetKiller(_cache.LocalWeaponLeft.transform);
                }
            }

            if (Config.Instance.FlyMode != _previousFlyMode)
            {
                var playerController = _cache?.LocalController;
                if (playerController != null)
                {
                    FlyMode(playerController, Config.Instance.FlyMode);
                }
                else
                {
                    Config.Instance.AddDebugLog("Error: Player controller not found.");
                }
                _previousFlyMode = Config.Instance.FlyMode;
            }

            if (Config.Instance.FreezeEnemy != _previousFreezeEnemy)
            {
                var enemy = GetClosestTarget();
                if (enemy != null && enemy.PlayerHealth.health > 0)
                    enemy.PlayerHealth.controller.sync___set_value_canMove(!enemy.PlayerHealth.controller.sync___get_value_canMove(), _cache.LocalPlayer.PlayerHealth.IsHost);
                _previousFreezeEnemy = Config.Instance.FreezeEnemy;
            }

            if (Config.Instance.Teleport)
            {
                Config.Instance.AddDebugLog("Teleport On");
                {
                    PlayerCache _closest = GetClosestTarget();

                    if (_closest != null && _closest.HeadTransform != null)
                    {
                        _cache.LocalController.Teleport(
                            _closest.HeadTransform.position,
                            0f,
                            false,
                            _closest.HeadTransform,
                            1,
                            1,
                            false
                        );

                        _closest.PlayerHealth.sync___set_value_health(100f, true);
                        Config.Instance.AddDebugLog("Teleport used");
                        Config.Instance.Teleport = !Config.Instance.Teleport;
                    }
                }
            }

            if (!Config.Instance.Aimbot || !_cache.LocalPlayer.IsValid || !_cache.MainCamera)
            {
                return;
            }

            // Only aim when Right Mouse Button is held and aimbot is enabled
            if (Input.GetMouseButton(1)) // Right mouse button held
            {
                AimAtClosestPlayer();
            }
            else if (_isMouseHold)
            {
                MouseRelease();
                _isMouseHold = false;
            }
        }

        public void SimulateLeftMouseClick(GameObject targetObject)
        {
          //  PointerEventData pointer = new PointerEventData(EventSystem.current) { pointerId = -1, position = Input.mousePosition };
           // ExecuteEvents.Execute(targetObject, pointer, ExecuteEvents.pointerClickHandler);
            // Debug.Log("Simulated left mouse click on " + targetObject.name);
        }

        public static void MouseHold()
        {
           // mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }

        public static void MouseRelease()
        {
          //  mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        private void AimAtClosestPlayer()
        {
            PlayerCache closestPlayer = GetClosestTarget();
            if (closestPlayer == null || closestPlayer.PlayerHealth.health <= 0)
                return;

            // Check if target is within FOV circle
            Vector3 screenPos = _cache.MainCamera.WorldToScreenPoint(closestPlayer.HeadTransform.position);
            float distanceFromCenter = Vector2.Distance(screenPos, new Vector2(Screen.width / 2, Screen.height / 2));

            if (distanceFromCenter > Config.Instance._fovRadius)
                return;

            Vector3 targetPosition;
            if (closestPlayer.HeadTransform != null)
            {
                targetPosition = closestPlayer.HeadTransform.position;
            }
            else
            {
                targetPosition = closestPlayer.GameObject.transform.position + Vector3.up * HEAD_OFFSET;
            }

            Vector3 direction = (targetPosition - _cache.MainCamera.transform.position).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _cache.LocalController.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            float targetPitch = targetRotation.eulerAngles.x;
            if (targetPitch > 180) targetPitch -= 360;
            targetPitch = Mathf.Clamp(targetPitch, -89f, 89f);
            _cache.MainCamera.transform.rotation = Quaternion.Euler(
                targetPitch, targetRotation.eulerAngles.y, 0
            );
        }

        public PlayerCache GetClosestTarget()
        {
            float closestDistance = float.MaxValue;
            PlayerCache closestPlayer = null;

            foreach (PlayerCache player in _cache.Players)
            {
                if (!player.IsValid)
                    continue;

                // Check if player is within FOV circle
                Vector3 screenPos = _cache.MainCamera.WorldToScreenPoint(player.HeadTransform.position);
                float distanceFromCenter = Vector2.Distance(screenPos, new Vector2(Screen.width / 2, Screen.height / 2));

                // Skip players outside the FOV circle
                if (distanceFromCenter > Config.Instance._fovRadius)
                    continue;

                float distance = Vector3.Distance(_cache.LocalPlayer.GameObject.transform.position, player.GameObject.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = player;
                }
            }

            return closestPlayer;
        }

        // Draw FOV circle (call this from OnGUI in your main script)
        public void DrawFOVCircle()
        {
            if (!Config.Instance.DrawFOVCircle) // use DrawFOVCircle toggle
                return;

            Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
            int segments = 100; // Smoothness of circle
            float radius = Config.Instance._fovRadius;

            // Use live color from config
            Color color = Config.Instance.FOVColor;
            color.a = 0.3f;

            GL.PushMatrix();
            Material mat = new Material(Shader.Find("Hidden/Internal-Colored"));
            mat.SetPass(0);

            GL.LoadOrtho();
            GL.Begin(GL.LINE_STRIP);
            GL.Color(color);

            for (int i = 0; i <= segments; i++)
            {
                float angle = 2 * Mathf.PI * i / segments;
                float x = center.x / Screen.width + Mathf.Cos(angle) * radius / Screen.width;
                float y = center.y / Screen.height + Mathf.Sin(angle) * radius / Screen.height;
                GL.Vertex(new Vector3(x, y, 0));
            }

            GL.End();
            GL.PopMatrix();
        }

        // Method to adjust FOV radius
        public void SetFOVRadius(float radius)
        {
            Config.Instance._fovRadius = Mathf.Clamp(radius, 50f, 300f);
        }

        // Method to adjust FOV color
        public void SetFOVColor(Color color)
        {
            _fovColor = color;
        }
    }
}