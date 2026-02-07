using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;

namespace strafthot.Features
{
    public class WeaponMods
    {
        private static FieldInfo _recoilField;
        private static Weapon _lastWeapon;
        private static float _lastSpam = 0f;
        private static bool _previousFreezeEnemy = false;

        // --------------------------
        // WALLBREAK INTEGRATION
        // --------------------------
        public static void FireWallbreak(Transform shooter, Weapon weapon, float maxDistance = 1000f)
        {
            if (shooter == null || weapon == null) return;
            if (!weapon.fire1.IsPressed()) return;

            Transform originPoint = weapon.muzzleFlashPoint != null ? weapon.muzzleFlashPoint : shooter.GetComponentInChildren<Camera>()?.transform;
            if (originPoint == null)
            {
                Config.Instance.AddDebugLog("[ExplosiveAmmo] Shooter has no camera or muzzle point!");
                return;
            }

            Vector3 origin = originPoint.position;
            Vector3 direction = originPoint.forward;

            // Raycast all hits
            RaycastHit[] hits = Physics.RaycastAll(origin, direction, maxDistance);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            Config.Instance.AddDebugLog($"[ExplosiveAmmo] Fired from {origin}. Hits: {hits.Length}");

            foreach (var hit in hits)
            {
                // Ignore self
                if (hit.transform.root.gameObject == shooter.root.gameObject) continue;

                Config.Instance.AddDebugLog($"[ExplosiveAmmo] Hit {hit.transform.name} at distance {hit.distance}");

                // --------------------------
                // EXPLOSIVE DAMAGE TO PLAYERS
                // --------------------------
                if (hit.transform.TryGetComponent<PlayerHealth>(out var enemyHealth))
                {
                    ApplyCombatEffects(enemyHealth, hit.point, direction, weapon);
                    if (Config.Instance.InstaKill)
                    {
                        MethodInfo killServer = typeof(PlayerHealth)
                            .GetMethod("KillServer", BindingFlags.Instance | BindingFlags.NonPublic);
                        killServer?.Invoke(enemyHealth, null);
                    }
                }

                // --------------------------
                // DESTROY WALLS & PROPS
                // --------------------------
                if (hit.transform.CompareTag("Wall") || hit.transform.CompareTag("Prop"))
                {
                    Config.Instance.AddDebugLog($"[ExplosiveAmmo] Destroying wall/prop: {hit.transform.name}");
                    MethodInfo destroyServer = hit.transform.GetType().GetMethod("DestroyServer", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (destroyServer != null)
                    {
                        destroyServer.Invoke(hit.transform, null);
                    }
                    else
                    {
                        Object.Destroy(hit.transform.gameObject);
                    }
                }

                // --------------------------
                // BREAK GLASS
                // --------------------------
                if (hit.transform.CompareTag("ShatterableGlass"))
                {
                    Config.Instance.AddDebugLog($"[ExplosiveAmmo] Breaking glass: {hit.transform.name}");
                    var shootComponent = hit.transform.GetComponent<PlayerShoot>();
                    var breakGlassMethod = shootComponent?.GetType().GetMethod("BreakGlassServer", BindingFlags.Instance | BindingFlags.NonPublic);
                    breakGlassMethod?.Invoke(shootComponent, new object[] { hit.point, direction, hit.transform.gameObject });
                }

                // --------------------------
                // SPAWN EXPLOSION VFX
                // --------------------------
                MethodInfo spawnVFX = typeof(Weapon).GetMethod("SpawnVFXServer", BindingFlags.Instance | BindingFlags.NonPublic);
                spawnVFX?.Invoke(weapon, new object[] { 0, hit.point, UnityEngine.Quaternion.LookRotation(hit.normal), hit.transform.tag, hit.transform });
                spawnVFX?.Invoke(weapon, new object[] { 1, hit.point, UnityEngine.Quaternion.LookRotation(hit.normal), hit.transform.tag, hit.transform });

                // --------------------------
                // SPAWN BULLET TRAIL
                // --------------------------
                MethodInfo spawnTrail = typeof(Weapon).GetMethod("SpawnBulletTrailServer", BindingFlags.Instance | BindingFlags.NonPublic);
                spawnTrail?.Invoke(weapon, new object[] { hit.point });

                // --------------------------
                // OPTIONAL: PHYSICAL EXPLOSION FORCE
                // --------------------------
                Collider[] nearby = Physics.OverlapSphere(hit.point, 4f);
                foreach (var col in nearby)
                {
                    if (col.TryGetComponent<Rigidbody>(out var rb))
                    {
                        rb.AddExplosionForce(2000f, hit.point, 4f, 1.5f, ForceMode.Impulse);
                    }
                }
            }

            if (hits.Length == 0) Config.Instance.AddDebugLog("[ExplosiveAmmo] Ray hit nothing!");
        }


        public static void SendKillfeedMessage(string message)
        {
            foreach (var player in Cheat.Instance.Cache.Players)
            {
                if (player?.GameObject == null)
                    continue;

                var health = player.GameObject.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    var method = typeof(PlayerHealth)
                        .GetMethod("SendKillfeed", BindingFlags.Instance | BindingFlags.NonPublic);

                    method?.Invoke(health, new object[] { message });
                }
            }
        }


        private static void ApplyCombatEffects(PlayerHealth enemy, Vector3 hitPoint, Vector3 direction, Weapon weapon)
        {
            if (enemy == null) return;

            // -------- REPULSOR --------
            if (Config.Instance.RepulsorToMars)
            {
                if (enemy.TryGetComponent<Rigidbody>(out var rb))
                    rb.AddForce(direction * 2000f, ForceMode.Impulse);
            }

            // -------- FREEZE --------
            if (Config.Instance.FreezeEnemy != _previousFreezeEnemy)
            {
                // Get Aimbot from the existing Cache instance
                var aimbot = Cheat.Instance.Cache.Aimbot;
                var closestEnemy = aimbot.GetClosestTarget();

                if (closestEnemy != null && closestEnemy.PlayerHealth.health > 0)
                {
                    closestEnemy.PlayerHealth.controller.sync___set_value_canMove(
                        !closestEnemy.PlayerHealth.controller.sync___get_value_canMove(),
                        Cheat.Instance.Cache.LocalPlayer.PlayerHealth.IsHost
                    );
                }

                _previousFreezeEnemy = Config.Instance.FreezeEnemy;
            }

            // -------- DISARM --------
            if (Config.Instance.RemoveEnemyWeapons)
            {
                Weapon w = enemy.GetComponentInChildren<Weapon>();
                if (w != null) Object.Destroy(w.gameObject);
            }

            // -------- SUPPRESSION --------
            if (Config.Instance.SpamProjectiles)
            {
                MethodInfo suppress = typeof(PlayerHealth)
                    .GetMethod("ApplySuppression", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                suppress?.Invoke(enemy, null);
            }
        }

        private static void InfiniteAmmo(Weapon weapon)
        {
            if (!weapon) return;
            weapon.ammoCharge = 999;
            weapon.currentAmmo = 999;
            weapon.chargedBullets = 999f;
        }

        private static void RapidFire(Weapon weapon)
        {
            if (!weapon) return;
            weapon.onePressShoot = false;
            weapon.timeBetweenBullets = 0;
            weapon.timeBetweenFire = 0;
            weapon.strength = Vector3.zero;
            weapon.isReloading = false;
            weapon.reloadWeapon = false;

            FieldInfo fireTimer = weapon.GetType().GetField("fireTimer", BindingFlags.Instance | BindingFlags.NonPublic);
            fireTimer?.SetValue(weapon, 0);
        }

        private static void InstaKill(Weapon weapon)
        {
            if (!weapon) return;
            weapon.damage = 25001;
        }

        private static void NoSpread(Weapon weapon)
        {
            if (!weapon) return;
            weapon.minSpread = 0;
            weapon.maxSpread = 0;
        }

        private static void NoRecoil(Weapon weapon)
        {
            if (weapon == null) return;

            // Cache private 'recoil' field if needed
            if (_recoilField == null)
            {
                _recoilField = typeof(Weapon).GetField(
                    "recoil", BindingFlags.Instance | BindingFlags.NonPublic);
                if (_recoilField == null)
                {
                    Config.Instance.AddDebugLog("Recoil field NOT FOUND on Weapon type.");
                    return;
                }
            }

            // Only update once per weapon instance
            if (weapon != _lastWeapon)
            {
                _lastWeapon = weapon;
                Config.Instance.AddDebugLog("NoRecoil APPLY -> " + weapon.name.Replace("(Clone)", ""));
            }

            // Set the recoil field to zero
            _recoilField.SetValue(weapon, Vector3.zero);

            // Patch the camera tweens to prevent rotation
            if (weapon.TryGetComponent(out Camera cam))
            {
                // Cancel any active recoil tweens
                cam.transform.DOKill();
                // Immediately reset rotation to ignore residual recoil
                cam.transform.localEulerAngles = cam.transform.localEulerAngles;
            }
        }

        private static void SpamProjectiles(Weapon weapon)
        {
            if (!Config.Instance.SpamProjectiles || Cheat.Instance.Cache.EnemyPlayer == null ||
                !Cheat.Instance.Cache.EnemyPlayer.transform || !(Time.time - _lastSpam > 0.1f))
                return;

            MethodInfo method = weapon.GetType().GetMethod("ServerFire", BindingFlags.Instance | BindingFlags.NonPublic);
            if (method != null)
            {
                for (int i = 0; i < 360; i += 10)
                {
                    Vector3 dir = Quaternion.Euler(0f, i, 0f) * Vector3.forward;
                    Vector3 pos = Cheat.Instance.Cache.EnemyPlayer.transform.position + dir * 2f;
                    method?.Invoke(weapon, new object[] { pos, Vector3.up, weapon.TimeManager.Tick });
                }
                _lastSpam = Time.time;
            }
        }

        private static void SpamEffects(Weapon weapon)
        {
            if (!Config.Instance.SpamEffects || Cheat.Instance.Cache.LocalPlayer == null ||
                Cheat.Instance.Cache.EnemyPlayer == null || !Cheat.Instance.Cache.EnemyPlayer.transform)
                return;

            MethodInfo method = weapon.GetType().GetMethod("RpcWriter___Server_ServerFX_3848837105", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo method2 = weapon.GetType().GetMethod("RpcWriter___Observers_ObserversFX_3848837105", BindingFlags.Instance | BindingFlags.NonPublic);

            if (method != null)
            {
                for (int i = 0; i < Config.Instance.Strength; i++)
                {
                    method.Invoke(weapon, new object[] { Cheat.Instance.Cache.EnemyPlayer.transform.position + Vector3.up, Quaternion.identity });
                }
            }

            if (method2 != null)
            {
                for (int i = 0; i < Config.Instance.Strength; i++)
                {
                    method2.Invoke(weapon, new object[] { Cheat.Instance.Cache.EnemyPlayer.transform.position + Vector3.up, Quaternion.identity });
                }
            }
        }

        private static void RepulseToMars(Weapon weapon)
        {
            if (!Config.Instance.RepulsorToMars || Cheat.Instance.Cache.EnemyPlayer == null) return;

            weapon.GetType().GetMethod("BumpPlayerServer", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(weapon, new object[] { Vector3.up, 10, Cheat.Instance.Cache.EnemyPlayer });
        }

        private static void SpamSuppression(Weapon weapon)
        {
            if (!Config.Instance.SpamProjectiles || Cheat.Instance.Cache.LocalPlayer == null ||
                Cheat.Instance.Cache.EnemyPlayer == null || !Cheat.Instance.Cache.EnemyPlayer.transform)
                return;

            MethodInfo method = weapon.GetType().GetMethod("SupressionServer", BindingFlags.Instance | BindingFlags.NonPublic);
            if (method != null)
            {
                foreach (var suppression in Cheat.Instance.Cache.Suppressions)
                {
                    method.Invoke(weapon, new object[] { suppression.transform });
                }
            }
        }

        private static void GodMode(Weapon weapon)
        {
            if (!Config.Instance.GodMode || Cheat.Instance.Cache.LocalPlayer == null ||
                Cheat.Instance.Cache.PlayerH.health >= 9999f) return;

            weapon.GetType().GetMethod("GiveDamage", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(weapon, new object[] { float.MinValue, Cheat.Instance.Cache.LocalPlayer, "ballsack" });
        }

        public static void Update()
        {
            Weapon leftWeapon = Cheat.Instance.Cache.LocalWeaponLeft;
            Weapon rightWeapon = Cheat.Instance.Cache.LocalWeaponRight;

            // -----------------
            // WEAPON MODS
            // -----------------
            if (Config.Instance.InfiniteAmmo)
            {
                InfiniteAmmo(leftWeapon);
                InfiniteAmmo(rightWeapon);
            }

            if (Config.Instance.RapidFire)
            {
                RapidFire(leftWeapon);
                RapidFire(rightWeapon);
            }

            if (Config.Instance.InstaKill)
            {
                InstaKill(leftWeapon);
                InstaKill(rightWeapon);
            }

            if (Config.Instance.NoSpread)
            {
                NoSpread(leftWeapon);
                NoSpread(rightWeapon);
            }

            if (Config.Instance.NoRecoil)
            {
                NoRecoil(leftWeapon);
                NoRecoil(rightWeapon);
            }

            // -----------------
            // WALLBREAK / EXPLOSIVE AMMO
            // -----------------
            if (Config.Instance.Wallbang && rightWeapon != null && Cheat.Instance.Cache.LocalController != null)
            {
                FireWallbreak(Cheat.Instance.Cache.LocalController.transform, rightWeapon);
            }

            // -----------------
            // COMBAT EFFECTS
            // -----------------
            if (rightWeapon != null)
            {
                SpamProjectiles(rightWeapon);
                SpamEffects(rightWeapon);
                RepulseToMars(rightWeapon);
                SpamSuppression(rightWeapon);
                GodMode(rightWeapon);
            }
        }
    }
}