using System;
using UnityEngine;

namespace strafthot.Features
{
    public class PlayerMods
    {
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
                // Apply movement values from config
                type.GetField("movementFactor")?.SetValue(fpcObj, Config.Instance.MoveSpeed1);
                type.GetField("jumpFactor")?.SetValue(fpcObj, Config.Instance.JumpSpeed1);
                type.GetField("maxWallJumps")?.SetValue(fpcObj, Config.Instance.WallJump1);

                Config.Instance.AddDebugLog(
                    $"[Movement] WS{Config.Instance.MoveSpeed1}, JH{Config.Instance.JumpSpeed1}, WJ{Config.Instance.WallJump1}"
                );
            }
            catch (Exception ex)
            {
            }

        }
    }
}
