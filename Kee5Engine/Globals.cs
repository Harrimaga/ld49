using Kee5Engine.Audio;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kee5Engine
{
    /// <summary>
    /// Static class for holding all things that need to be globally accessable
    /// </summary>
    public static class Globals
    {
        public static Vector2 windowSize;
        public static List<Button> activeButtons = new List<Button>();

        public static int unloaded;

        /// <summary>
        /// Update the active buttons and the AudioManager
        /// </summary>
        public static void Update()
        {
            foreach (Button button in activeButtons)
            {
                button.Update();
            }
            AudioManager.Update();
        }

        /// <summary>
        /// Draw the active buttons
        /// </summary>
        public static void Draw()
        {
            foreach (Button button in activeButtons)
            {
                button.Draw();
            }
        }
    }
}
