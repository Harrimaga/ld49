using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Kee5Engine
{
    public class Game
    {
        public static Window gameWindow;

        /// <summary>
        /// Initiate new Game
        /// </summary>
        public Game()
        {
            gameWindow = new Window(1920, 1080, "game");
            gameWindow.Run();
        }
    }
}
