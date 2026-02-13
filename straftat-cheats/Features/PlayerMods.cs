using HeathenEngineering.SteamworksIntegration.API;
using System;
using System.Reflection;
using UnityEngine;

namespace strafthot.Features
{
    public class PlayerMods
    {
        private static FieldInfo walkSpeedField =
            typeof(FirstPersonController).GetField("walkSpeed", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo sprintSpeedField =
            typeof(FirstPersonController).GetField("sprintSpeed", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo jumpHeightField =
            typeof(FirstPersonController).GetField("jumpForce", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo crouchSpeedField =
            typeof(FirstPersonController).GetField("crouchSpeed", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo gravityField =
            typeof(FirstPersonController).GetField("gravity", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo canMoveField =
            typeof(FirstPersonController).GetField("canMove", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo canJumpField =
            typeof(FirstPersonController).GetField("canJump", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo canCrouchField =
            typeof(FirstPersonController).GetField("canCrouch", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo jumpFactorField =
            typeof(FirstPersonController).GetField("jumpFactor", BindingFlags.NonPublic | BindingFlags.Instance);


        private readonly Cache _cache;

        public static void ApplyPlayerSettings(MonoBehaviour fpcObj)
        {
            if (fpcObj == null)
            {
                Config.Instance.AddDebugLog("[PlayerMods] FirstPersonController not found");
                return;
            }

            Type type = fpcObj.GetType();

            try
            {
                type.GetField("movementFactor")?.SetValue(fpcObj, Config.Instance.MoveSpeed1);
                type.GetField("jumpFactor")?.SetValue(fpcObj, Config.Instance.JumpSpeed1);
                type.GetField("maxWallJumps")?.SetValue(fpcObj, Config.Instance.WallJump1);

                Config.Instance.AddDebugLog(
                    $"[Movement] WS{Config.Instance.MoveSpeed1}, " +
                    $"JH{Config.Instance.JumpSpeed1}, " +
                    $"WJ{Config.Instance.WallJump1}");
            }
            catch (Exception ex)
            {
                Config.Instance.AddDebugLog($"[PlayerMods] Error applying movement: {ex.Message}");
            }
        }

        private static void Speedhack(object controller)
        {
            if (controller != null && jumpHeightField != null)
            {
                jumpHeightField.SetValue(controller, Config.Instance.jumpHeightValue);
            }
            if (controller == null || !Config.Instance.enableSpeedHack)
            return;

            float speed = Config.Instance.speedValue;
            float sspeed = Config.Instance.sspeedValue;


            walkSpeedField?.SetValue(controller, speed * Config.Instance.speedValue);
            sprintSpeedField?.SetValue(controller, sspeed * Config.Instance.sspeedValue);
            jumpHeightField?.SetValue(controller, Config.Instance.jumpHeightValue);
            crouchSpeedField?.SetValue(controller, Config.Instance.Crouch);
            gravityField?.SetValue(controller, Config.Instance.Gravity);
            canMoveField?.SetValue(controller, true);
            canJumpField?.SetValue(controller, true);
            canCrouchField?.SetValue(controller, true);
            jumpFactorField?.SetValue(controller, Config.Instance.jumpHeightValue);
        }

        private static void AntiAimMenu()
        {
            var controller = Cheat.Instance.Catch.LocalController;

            if (Config.Instance.enableIsSlide && controller != null)
            {
                controller.isSliding = true;
            }
        }

        // Update Method
        public void Update()
        {
            PlayerCache player = Cheat.Instance.Catch.LocalPlayer;

            if (UnityEngine.Input.GetKeyDown(KeyCode.F5))
            {
                _cache.Settings.rocketJumpsHatAch.Unlock();
                _cache.Settings.windowsBrokenHatAch.Unlock();
                _cache.Settings.headshotHatAch.Unlock();
                _cache.Settings.ragdollsThrownAwayHatAch.Unlock();
                _cache.Settings.taserShotsHatAch.Unlock();
                _cache.Settings.noscopeHatAch.Unlock();
                _cache.Settings.potsBrokenHatAch.Unlock();
                _cache.Settings.fiveGamesHatAch.Unlock();
                _cache.Settings.killsHatAch.Unlock();
                _cache.Settings.propKillsHatAch.Unlock();

                StatsAndAchievements.Client.StoreStats();
                _cache.Settings.SteamAchievementsCheck();
            }

            AntiAimMenu();
            var controller = Cheat.Instance.Catch.LocalController;
            Speedhack(controller);
        }
    }
}


