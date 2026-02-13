using EpicSauceHack99;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Object;
using FishNet.Object;
using HarmonyLib;
using HeathenEngineering.SteamworksIntegration;
using Sirenix.OdinInspector;
using Steamworks;
using strafthot.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using UnityEngine;
using VLB;
using static strafthot.Config;
namespace strafthot
{
    public class Cheat : MonoBehaviour
    {
        public static Cheat Instance { get; private set; }
        public static List<PlayerHealth> players = new List<PlayerHealth>();

        public static List<Suppression> suppressions = new List<Suppression>();

        public static List<Weapon> weapons = new List<Weapon>();

        public static Weapon localWeapon = null;

        public static Camera mainCamera;

        public static PlayerHealth localPlayer;

        public static PlayerHealth enemyPlayer;

        public static Transform enemyHead = null;

        public static FirstPersonController controller;

        // Make the cache public so other code can access it
        public Cache Catch { get; private set; }
        public Misc Misc { get; private set; }
        public PlayerMods PlayerMods { get => _playermods; }

        private PlayerMods _playermods = new PlayerMods();
        private MonoBehaviour _fpcObj;
        private Type _fpcType;

        private Vector2 _watermarkPos = new Vector2(10, 10);
        private List<MonoBehaviour> _weapons = new List<MonoBehaviour>();

        private bool _menuOpen = true;
        private Rect _windowRect = new Rect(100, 100, 900, 750);

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
        public bool _placeholder = false;
        public static Transform RecursiveFind(Transform parent, string name)
        {
            foreach (Transform item in parent)
            {
                if (item.name == name)
                {
                    return item;
                }

                Transform transform2 = RecursiveFind(item, name);
                if (transform2 != null)
                {
                    return transform2;
                }
            }

            return null;
        }
        public static ItemBehaviour GetItemInHand(PlayerHealth playerHealth)
        {
            ItemBehaviour result = null;
            PlayerPickup component = playerHealth.gameObject.GetComponent<PlayerPickup>();
            if (component != null)
            {
                result = behaviorInHand.GetValue(component) as ItemBehaviour;
            }

            return result;
        }
        private void Spam(Weapon weapon)
        {
            if (!Config.SpamEffects)
            {
                return;
            }

            if (!localPlayer)
            {
                Config.Instance.AddDebugLog("localPlayer is null");
                return;
            }

            if (!enemyPlayer)
            {
                return;
            }

            if (!enemyPlayer.transform)
            {
                Config.Instance.AddDebugLog("enemyPlayer.transform is null");
                return;
            }

            MethodInfo method = weapon.GetType().GetMethod("RpcWriter___Server_ServerFX_3848837105", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo method2 = weapon.GetType().GetMethod("RpcWriter___Observers_ObserversFX_3848837105", BindingFlags.Instance | BindingFlags.NonPublic);
            if (method != null)
            {
                for (int i = 0; i < Config.Strength; i++)
                {
                    method.Invoke(weapon, new object[2]
                    {
                    enemyPlayer.transform.position + Vector3.up,
                    Quaternion.identity
                    });
                    Config.Instance.AddDebugLog("Lagging Host");
                }
            }

            if (method2 != null)
            {
                for (int j = 0; j < Config.Strength; j++)
                {
                    method2.Invoke(weapon, new object[2]
                    {
                    enemyPlayer.transform.position + Vector3.up,
                    Quaternion.identity
                    });
                    Config.Instance.AddDebugLog("Lagging Host");
                }
            }
        }
        private void RemoveEnemyWeapons()
        {
            if (!Config.Instance.RemoveEnemyWeapons || !enemyPlayer)
            {
                return;
            }

            PlayerPickup component = localPlayer.gameObject.GetComponent<PlayerPickup>();
            if (component == null)
            {
                return;
            }

            Weapon[] array = UnityEngine.Object.FindObjectsOfType<Weapon>();
            foreach (Weapon weapon in array)
            {
                if ((bool)weapon && weapon.OwnerId == enemyPlayer.OwnerId)
                {
                    steal?.Invoke(component, new object[5]
                    {
                    weapon.gameObject,
                    localPlayer.transform.position,
                    mainCamera.transform.rotation,
                    localPlayer.gameObject,
                    true
                    });
                }
            }
        }
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;

            // Initialize Cache and Misc
            Catch = new Cache(1f);
            Misc = new Misc();
            Config.Instance.Register("Menu");
            Config.Instance.Register("Aimbot");
            Config.Instance.Register("Silent Aim");
            Config.Instance.Register("ESP");
            Config.Instance.Register("ESP3d");
            Config.Instance.Register("Tracer");
            Config.Instance.Register("Chams");
            Config.Instance.Register("DrawFOVCircle");
            Config.Instance.Register("FlyMode");
            Config.Instance.Register("Teleport");
            Config.Instance.Register("GodMode");
            Config.Instance.Register("Crash");
            Config.Instance.Register("Speed");
            Config.Instance.Register("Slide");
            Config.Instance.Register("FreezeEnemy");
            Config.Instance.Register("RepulsorToMars");
            Config.Instance.AddDebugLog("[Cheat] Awake called.");
        }
        private void GunHacks()
        {
            if ((bool)localPlayer && (bool)localWeapon && (bool)localWeapon.transform)
            {
                Spam(localWeapon);
            }
        }
        private void Cache()
        {
            if (!(Time.time - lastUpdated > 1f))
            {
                return;
            }

            players = UnityEngine.Object.FindObjectsOfType<PlayerHealth>().ToList();
            suppressions = (from x in UnityEngine.Object.FindObjectsOfType<Suppression>().ToList()
                            where x != null && !x.IsOwner && x.transform != null
                            select x).ToList();
            weapons = (from x in UnityEngine.Object.FindObjectsOfType<Weapon>().ToList()
                       where x != null && x.transform != null
                       select x).ToList();
            foreach (PlayerHealth player in players)
            {
                if (!player || !player.gameObject || !player.transform)
                {
                    continue;
                }

                if (player.Owner != null && player.Owner.IsLocalClient)
                {
                    localPlayer = player;
                    if (!(localPlayer != null))
                    {
                        continue;
                    }

                    FirstPersonController component = localPlayer.GetComponent<FirstPersonController>();
                    if (component != null)
                    {
                        mainCamera = component.playerCamera;
                    }

                    ItemBehaviour itemInHand = GetItemInHand(localPlayer);
                    if (itemInHand != null)
                    {
                        Weapon component2 = itemInHand.GetComponent<Weapon>();
                        if (component2 != null)
                        {
                            localWeapon = component2;
                        }
                    }
                }
                else
                {
                    enemyPlayer = player;
                    if (enemyPlayer != null)
                    {
                        enemyHead = RecursiveFind(enemyPlayer.transform, "Head_Col");
                    }
                }
            }
        }
        public void DrawWeapons()
        {
            Config.Instance.ModernHeader("WEAPON SELECTOR");
            WeaponHandSpawner obj = localWeapon as WeaponHandSpawner;
            Event current = Event.current;
            if ((bool)localPlayer && weapons.Count > 0)
            {
                try
                {
                    HashSet<string> hashSet = new HashSet<string>();
                    foreach (Weapon weapon in weapons)
                    {
                        if (weapon.IsOwner || weapon.OwnerId != -1 || hashSet.Contains(weapon.behaviour.weaponName))
                        {
                            continue;
                        }

                        Config.Instance.ModernButton(weapon.behaviour.weaponName, () =>
                        {
                            PlayerPickup component = localPlayer.gameObject.GetComponent<PlayerPickup>();
                            if (component == null)
                            {
                                return;
                            }

                            Camera camera = PlayerPickupcam.GetValue(component) as Camera;
                            if (camera == null)
                            {
                                return;
                            }

                            Vector3 position = camera.transform.position;
                            Quaternion rotation = camera.transform.rotation;
                            camera.transform.position = weapon.transform.position + Vector3.up;
                            camera.transform.LookAt(weapon.transform.position);
                            PlayerPickupRightHandPickup.Invoke(component, null);
                            camera.transform.position = position;
                            camera.transform.rotation = rotation;
                        });

                        hashSet.Add(weapon.behaviour.weaponName);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                SinglePrefabObjects singlePrefabObjects = NetworkManager.Instances.FirstOrDefault()?.SpawnablePrefabs as SinglePrefabObjects;
                if (singlePrefabObjects != null)
                {
                    foreach (NetworkObject prefab in singlePrefabObjects.Prefabs)
                    {
                        if ((bool)prefab && (bool)mainCamera && (bool)mainCamera.transform)
                            Config.Instance.ModernHeader("Spawn Menu");
                        {
                            Config.Instance.ModernButton(prefab.PrefabId + ": " + prefab.name, () =>
                            {
                                Ray ray = new Ray(mainCamera.transform.position + mainCamera.transform.forward * 1f, mainCamera.transform.forward);
                                if (Physics.Raycast(ray, out var hitInfo, 99999f))
                                {
                                    bool flag = (bool)WeaponHandSpawnerproximityMine.GetValue(obj);
                                    bool flag2 = (bool)WeaponHandSpawnerclaymore.GetValue(obj);
                                    WeaponHandSpawnerproximityMine.SetValue(obj, false);
                                    WeaponHandSpawnerclaymore.SetValue(obj, false);
                                    Config.Instance.AddDebugLog("Spawning " + prefab.name + " with id " + prefab.PrefabId + " at " + hitInfo.point.ToString());
                                    WeaponHandSpawnerobjToSpawn.SetValue(obj, prefab.gameObject);
                                    WeaponHandSpawnerproximityMine.SetValue(obj, flag);
                                    WeaponHandSpawnerclaymore.SetValue(obj, flag2);
                                }
                            });
                        }
                    }
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Insert) || Config.Instance.Pressed("Menu"))
                _menuOpen = !_menuOpen;

            if (_menuOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {

            }
            // Works now

            WeaponMods.Update();
            Catch.Update();
            PlayerMods.Update();
            Misc.Update();
            GunHacks();
            RemoveEnemyWeapons();
            Cache();
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
            Config.Instance.draw();
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
            if (Config.Instance.chams)
                ESP.OnGUI();
            if (Config.Instance.tracer)
                ESP.OnGUI();
            if (Config.Instance.DrawFOVCircle)
                DrawFOVCircle.OnGUI();

        }
    }
}
