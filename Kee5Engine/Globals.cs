using Kee5Engine.Audio;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kee5Engine
{
    public enum GameState
    {
        PLAYING,
        PAUSED,
        LOST,
        MENU
    }

    /// <summary>
    /// Static class for holding all things that need to be globally accessable
    /// </summary>
    public static class Globals
    {
        public static Vector2 windowSize;
        public static List<Button> activeButtons = new List<Button>();
        public static float tileSize = 128;
        public static double deltaTime;
        public static Level level;
        public static MainMenu mainMenu;
        public static GameOverMenu gameOverMenu;
        public static GameState gameState;
        public static int currentLevel;
        public static int levelsUnlocked;

        public static int unloaded;

        /// <summary>
        /// Update the active buttons and the AudioManager
        /// </summary>
        public static void Update()
        {
            switch(gameState)
            {
                case GameState.PLAYING:
                    level.Update();
                    break;
                case GameState.MENU:
                    break;
                case GameState.LOST:
                    break;
            }
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
            switch (gameState)
            {
                case GameState.PLAYING:
                    level.Draw();
                    break;
                case GameState.MENU:
                    break;
                case GameState.LOST:
                    break;
            }

            foreach (Button button in activeButtons)
            {
                button.Draw();
            }
        }
    }
}
