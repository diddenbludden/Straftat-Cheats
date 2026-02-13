using EpicSauceHack99;
using FishNet.Connection;
using HarmonyLib;
using HeathenEngineering.SteamworksIntegration.API;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.UI.GridLayoutGroup;
using System.Linq;
using System.Net;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Object;
using FishNet.Object;
using HeathenEngineering.SteamworksIntegration;

namespace strafthot.Features
{
    public class Misc : MonoBehaviour
    {
        private readonly Cache _cache;

        private float _spamTimer = 0f;
        private float _killfeedTimer = 0f;
        private float _spoofNameTimer = 0f;

        private const float SpamInterval = 0f; // 1 second interval for spam actions


        public Misc()
        {
            _cache = Cheat.Instance.Catch;
        }
        private void GodMode(PlayerHealth localPlayer)
        {
            if (localPlayer != null)
                localPlayer.sync___set_value_health(100f, localPlayer.IsHost);
        }

        // -----------------------
        // Reflection Helpers
        // -----------------------

        // Harmony patch for the method that returns the player's name
        [HarmonyPatch(typeof(ClientInstance), "SetSyncValues")]
        public static bool SetplayerName(string PlayerName, bool isServer, NetworkConnection owner, ulong currentLobbyID, ulong steamId)
        {
            if (string.IsNullOrEmpty(Config.Instance.Name))
            {
                return true;
            }

            PlayerName = Config.Instance.Name;
            steamId = 76561198000000000uL;
            return true;
        }
        private void CallMethod(GameObject obj, string methodName)
        {
            if (obj == null) return;

            var method = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            method?.Invoke(obj, null);
        }

        private void SendKillfeed(PlayerCache enemy, string message)
        {
            if (enemy == null) return;

            var health = enemy.GameObject.GetComponent<PlayerHealth>();
            if (health == null) return;

            var method = typeof(PlayerHealth).GetMethod("SendKillfeed", BindingFlags.Instance | BindingFlags.NonPublic);
            method?.Invoke(health, new object[] { message });
        }

        private void SpamSuppression(PlayerCache enemy)
        {
            if (enemy == null) return;

            var health = enemy.GameObject.GetComponent<PlayerHealth>();
            if (health == null) return;

            var method = typeof(PlayerHealth).GetMethod("ApplySuppression", BindingFlags.Instance | BindingFlags.NonPublic);
            method?.Invoke(health, null);
        }

        public void Update()
        {
            var localPlayer = _cache.LocalPlayer;
            float delta = Time.deltaTime;

            _spamTimer += delta;
            _killfeedTimer += delta;
            _spoofNameTimer += delta;

            // --------------------
            // GOD MODE
            // --------------------
            if (Config.Instance.GodMode && localPlayer != null)
                GodMode(localPlayer.PlayerHealth);

            // --------------------
            // SPAM SUPPRESSION
            // --------------------
            if (Config.Instance.SpamSuppression && _spamTimer >= SpamInterval)
            {
                foreach (var enemy in _cache.Players)
                    SpamSuppression(enemy);
                _spamTimer = 0f;
            }

            // --------------------
            // SPOOFED NAME
            // --------------------

            // --------------------
            // SPAM

            // --------------------
            // SPAM KILLFEED
            // --------------------
            if (Config.Instance.SpamKillfeed && _killfeedTimer >= SpamInterval)
            {
                foreach (var enemy in _cache.Players)
                    SendKillfeed(enemy, Config.Instance.KillfeedMessage);
                _killfeedTimer = 0f;
            }
        }
    }
}