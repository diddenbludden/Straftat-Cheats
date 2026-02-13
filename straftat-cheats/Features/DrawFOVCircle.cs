using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strafthot.Features
{
    public class DrawFOVCircle
    {
        public static void OnGUI()
        {
            // Check if both the aimbot is enabled AND the FOV circle toggle is on
            if (Config.Instance.Aimbot && Config.Instance.DrawFOVCircle &&
                Cheat.Instance.Catch != null && Cheat.Instance.Catch.Aimbot != null)
            {
                // Call the DrawFOVCircle method from the Aimbot instance
                Cheat.Instance.Catch.Aimbot.DrawFOVCircle();
            }
        }
    }
}