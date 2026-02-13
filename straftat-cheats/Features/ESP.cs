using System;
using UnityEngine;

namespace strafthot.Features
{
    public class ESP
    {
        public static void OnGUI()
        {
            // ===== 2D ESP =====
            if (Config.Instance.ESP)
            {
                foreach (var player in Cheat.Instance.Catch.Players)
                {
                    player.Draw(Cheat.Instance.Catch.MainCamera);
                }
            }

            // ===== 3D ESP =====
            if (Config.Instance.ESP3d)
            {
                foreach (var player in Cheat.Instance.Catch.Players)
                {
                    player.Draw1(Cheat.Instance.Catch.MainCamera);
                }
            }

            // ===== Tracers =====
            if (Config.Instance.tracer) // new config flag for tracers
            {
                DrawTracers();
            }

            // ===== Chams =====
            if (Config.Instance.chams) // new config flag for chams
            {
                DrawChams();
            }
        }

        private static void DrawTracers()
        {
            if (Cheat.Instance.Catch.MainCamera == null) return;
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

            foreach (var player in Cheat.Instance.Catch.Players)
            {
                if (player == null || !player.IsValid) continue;

                Utils.DrawTracer(
                    Cheat.Instance.Catch.MainCamera,
                    player.HeadTransform.position,
                    screenCenter,
                    player.tracerColor,
                    1f
                );
            }
        }

        private static void DrawChams()
        {
            foreach (var player in Cheat.Instance.Catch.Players)
            {
                if (player == null || !player.IsValid) continue;

                Utils.EnableChams(player.GameObject, player.chamsColor);
            }
        }
    }
}