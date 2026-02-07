using FishNet.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace strafthot
{
    public static class Loader
    {
        private static GameObject _cheatObject;

        public static void Init()
        {
            _cheatObject = new GameObject();
            _cheatObject.AddComponent<Cheat>();
            UnityEngine.Object.DontDestroyOnLoad(_cheatObject);
        }
    }
}