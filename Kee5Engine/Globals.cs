using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kee5Engine
{
    public static class Globals
    {
        public static Vector2 windowSize;
        public static List<Button> activeButtons = new List<Button>();

        public static void Update()
        {
            foreach (Button button in activeButtons)
            {
                button.Update();
            }
        }

        public static void Draw()
        {
            foreach (Button button in activeButtons)
            {
                button.Draw();
            }
        }
    }
}
