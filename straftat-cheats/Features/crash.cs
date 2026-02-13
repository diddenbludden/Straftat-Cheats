using EpicSauceHack99;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Object;
using FishNet.Object;
using HeathenEngineering.SteamworksIntegration;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace strafthot.Features
{
    public class Crash : MonoBehaviour
    {
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

        private List<LeaderboardEntry> cachedLeaderboard = null;

        private GUIStyle style = new GUIStyle();

        private GUIContent content = new GUIContent();

        private Rect drawRect = default(Rect);

        public static bool Menu = true;

        public static bool LeaderboardMenu = false;

        public static Rect MenuRect = new Rect(100f, 100f, 300f, 700f);

        public static Rect WeaponsRect = new Rect(500f, 100f, 150f, 200f);

        public static Rect SpawnRect = new Rect(500f, 400f, 200f, 300f);

        public static Rect LeaderboardRect = new Rect(700f, 100f, 250f, 500f);

        public static Vector2 scrollPos = Vector2.zero;

        public static Vector2 scrollPos1 = Vector2.zero;

        public static Vector2 scrollPos2 = Vector2.zero;

        public static Vector2 scrollPos3 = Vector2.zero;

        public static FieldInfo behaviorInHand = typeof(PlayerPickup).GetField("behaviourInHand", BindingFlags.Instance | BindingFlags.NonPublic);
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
            if (!Config.Spam)
            {
                Config.Instance.AddDebugLog("Spam disabled (Config.Spam == false)");
                return;
            }

            if (!localPlayer)
            {
                Config.Instance.AddDebugLog("localPlayer is null");
                return;
            }

            if (!enemyPlayer)
            {
                Config.Instance.AddDebugLog("enemyPlayer is null");
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
                for (int i = 0; i < Config.Instance.Strength; i++)
                {
                    method.Invoke(weapon, new object[2]
                    {
                    enemyPlayer.transform.position + Vector3.up,
                    Quaternion.identity
                    });
                    Config.Instance.AddDebugLog("Lagged");
                }
            }

            if (method2 != null)
            {
                for (int j = 0; j < Config.Instance.Strength; j++)
                {
                    method2.Invoke(weapon, new object[2]
                    {
                    enemyPlayer.transform.position + Vector3.up,
                    Quaternion.identity
                    });
                    Config.Instance.AddDebugLog("Lagged");
                }
            }
        }
        public void Update()
        {
            if ((bool)localPlayer && (bool)localWeapon && (bool)localWeapon.transform)
            {
                Spam(localWeapon);

            }
            else
                Config.Instance.AddDebugLog("Failed");
        }
    }
}