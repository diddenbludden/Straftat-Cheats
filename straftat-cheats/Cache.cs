using FishNet.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using strafthot.Features;

namespace strafthot
{
    public class Cache
    {
        // Reflection fields for weapon access
        private readonly FieldInfo PlayerPickup_weaponInHand =
            typeof(PlayerPickup).GetField("weaponInHand", BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly FieldInfo PlayerPickup_weaponInLeftHand =
            typeof(PlayerPickup).GetField("weaponInLeftHand", BindingFlags.NonPublic | BindingFlags.Instance);

        private float _lastTimeUpdated = 0f;
        private readonly float _updateInterval;

        private List<PlayerCache> _players = new List<PlayerCache>();

        // Core references
        public Player Player { get; private set; }
        public PlayerHealth PlayerH { get; private set; }
        public PlayerCache LocalPlayer { get; private set; }
        public PlayerShoot PlayerShootManager { get; private set; }
        public CosmeticInstance CosmeticInstance { get; private set; }
        public Weapon LocalWeaponLeft { get; private set; }
        public Weapon LocalWeaponRight { get; private set; }
        public FirstPersonController LocalController { get; private set; }
        public Camera MainCamera { get; private set; }
        public List<PlayerCache> Players => _players;
        public Aimbot Aimbot { get; private set; }
        public Settings Settings { get; private set; }

        // Convenience: all valid enemies
        public IEnumerable<PlayerCache> EnemyPlayers => _players.Where(p => p.IsValid);

        public Cache(float interval)
        {
            _updateInterval = interval;
            Aimbot = new Aimbot(this);
        }

        private void UpdateCache()
        {
            LocalController = Settings.Instance.localPlayer;
            if (!LocalController) return;

            MainCamera = LocalController.playerCamera;
            LocalPlayer = new PlayerCache(LocalController.gameObject);

            if (LocalPlayer.IsValid)
            {
                PlayerPickup pickup = LocalController.GetComponent<PlayerPickup>();
                if (pickup != null)
                {
                    LocalWeaponLeft = (Weapon)PlayerPickup_weaponInLeftHand.GetValue(pickup);
                    LocalWeaponRight = (Weapon)PlayerPickup_weaponInHand.GetValue(pickup);
                }

                PlayerShootManager = LocalController.GetComponent<PlayerShoot>();
                Player = LocalController.GetComponent<Player>();
                CosmeticInstance = LocalController.GetComponent<CosmeticInstance>();
            }

            _players.Clear();
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (go == LocalPlayer?.GameObject) continue;

                PlayerCache player = new PlayerCache(go);
                if (player.IsValid) _players.Add(player);
            }
        }

        // ----------------------------
        // FEATURE METHODS
        // ----------------------------
        public PlayerHealth EnemyPlayer
        {
            get
            {
                return EnemyPlayers
                    .Select(p => p.GameObject.GetComponent<PlayerHealth>())
                    .FirstOrDefault(h => h != null && h.health > 0f);
            }
        }

        // Strafthot expects Suppressions list
        public List<Suppression> Suppressions
        {
            get
            {
                return UnityEngine.Object.FindObjectsOfType<Suppression>()
                    .Where(s => s != null && s.transform != null)
                    .ToList();
            }
        }

        private void SpamKillfeed()
        {
            if (!Config.Instance.SpamKillfeed) return;

            WeaponMods.SendKillfeedMessage(Config.Instance.KillfeedMessage);
        }

        private void SpamSuppression()
        {
            if (!Config.Instance.SpamSuppression) return;

            foreach (var enemy in EnemyPlayers)
            {
                PlayerHealth health = enemy.GameObject.GetComponent<PlayerHealth>();
                MethodInfo suppress = typeof(PlayerHealth)
                    .GetMethod("ApplySuppression", BindingFlags.Instance | BindingFlags.NonPublic);
                suppress?.Invoke(health, null);
            }
        }

        // ----------------------------
        // MAIN UPDATE LOOP
        // ----------------------------


        public void Update()
        {
            // Refresh cache periodically
            if (Time.time - _lastTimeUpdated > _updateInterval)
            {
                UpdateCache();
                _lastTimeUpdated = Time.time;
            }

            // Aimbot update
            Aimbot.Update();
            Config.Instance.HandleKeyToggles();
            Config.Instance.ProcessKeyBind();

            // Auto features

            SpamKillfeed();
            SpamSuppression();
        }
    }
}
