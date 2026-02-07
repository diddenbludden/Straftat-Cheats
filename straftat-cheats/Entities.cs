using FishNet.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace strafthot
{
    public class PlayerCache
    {
        private readonly int FONT_SIZE = 14;
        private readonly Vector3 OFFSET = new Vector3(0, 0.5f, 0);

        public GameObject GameObject { get; private set; }
        public Color espColor = Config.Instance.ESPColor;
        public Transform HeadTransform { get; private set; }
        public Collider Collider { get; private set; }
        public PlayerHealth PlayerHealth { get; private set; }
        public string PlayerName { get; private set; } = "Unknown";
        public bool IsValid { get; private set; } = false;

        public PlayerCache(GameObject gameObject)
        {
            this.GameObject = gameObject;
            PlayerHealth = gameObject.GetComponent<PlayerHealth>();
            Collider = gameObject.GetComponent<Collider>();
            HeadTransform = Utils.RecursiveFind(gameObject.transform, "Head_Col");
            PlayerName = PlayerHealth.playerValues.playerClient.PlayerName;
            IsValid = true;
        }

        public void Draw(Camera camera)
        {
            if (GameObject == null || GameObject.transform == null || camera == null || Collider == null || PlayerHealth == null)
                return;

            Vector3 screenPos = camera.WorldToScreenPoint(GameObject.transform.position - OFFSET);
            if (screenPos.z < 0)
                return;
            screenPos.y = Screen.height - screenPos.y;

            Utils.SetupExtentsBounds(Collider.bounds);
            Vector3 minScreen = camera.WorldToScreenPoint(Collider.bounds.min);
            Vector3 maxScreen = camera.WorldToScreenPoint(Collider.bounds.max);
            if (minScreen.z < 0 || maxScreen.z < 0)
                return;
            minScreen.y = Screen.height - minScreen.y;
            maxScreen.y = Screen.height - maxScreen.y;

            Rect boxRect = new Rect(
                Mathf.Min(minScreen.x, maxScreen.x),
                Mathf.Min(minScreen.y, maxScreen.y),
                Mathf.Abs(maxScreen.x - minScreen.x),
                Mathf.Abs(maxScreen.y - minScreen.y)
            );

            // Use the live ESP color from config
            Utils.Draw2DBox(boxRect, Config.Instance.ESPColor);


            int idx = 0;
            Utils.DrawText(
                new Vector2(screenPos.x, screenPos.y + FONT_SIZE * idx++),
                PlayerName,
                Color.white,
                FONT_SIZE,
                true
            );

            int hp = Mathf.Clamp((int)(PlayerHealth.health * 25), 0, 100);
            Utils.DrawText(
                new Vector2(screenPos.x, screenPos.y + FONT_SIZE * idx++),
                hp + " HP",
                Utils.DoubleColorLerp(hp / 100.0f, Color.green, Color.yellow, Color.red),
                FONT_SIZE,
                true
            );
        }
        public void Draw1(Camera camera)
        {
            if (GameObject == null || GameObject.transform == null || camera == null || Collider == null || PlayerHealth == null)
                return;

            Vector3 screenPos = camera.WorldToScreenPoint(GameObject.transform.position - OFFSET);
            if (screenPos.z < 0)
                return;
            screenPos.y = Screen.height - screenPos.y;

            Utils.SetupExtentsBounds(Collider.bounds);
            Vector3 minScreen = camera.WorldToScreenPoint(Collider.bounds.min);
            Vector3 maxScreen = camera.WorldToScreenPoint(Collider.bounds.max);
            if (minScreen.z < 0 || maxScreen.z < 0)
                return;
            minScreen.y = Screen.height - minScreen.y;
            maxScreen.y = Screen.height - maxScreen.y;

            Rect boxRect = new Rect(
                Mathf.Min(minScreen.x, maxScreen.x),
                Mathf.Min(minScreen.y, maxScreen.y),
                Mathf.Abs(maxScreen.x - minScreen.x),
                Mathf.Abs(maxScreen.y - minScreen.y)
            );

            // Use the live ESP color from config
            Utils.Draw3DBox(camera, Config.Instance.ESPColor);


            int idx = 0;
            Utils.DrawText(
                new Vector2(screenPos.x, screenPos.y + FONT_SIZE * idx++),
                PlayerName,
                Color.white,
                FONT_SIZE,
                true
            );

            int hp = Mathf.Clamp((int)(PlayerHealth.health * 25), 0, 100);
            Utils.DrawText(
                new Vector2(screenPos.x, screenPos.y + FONT_SIZE * idx++),
                hp + " HP",
                Utils.DoubleColorLerp(hp / 100.0f, Color.green, Color.yellow, Color.red),
                FONT_SIZE,
                true
            );
        }
    }
}
