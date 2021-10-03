using Kee5Engine.Audio;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
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
        public static int selectedButton = 0;
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

                    if (Window.inputHandler.IsLeftStickAngleChanged(IO.ControllerAngle.DOWN) || Window.inputHandler.IsKeyPressed(Keys.S) || Window.inputHandler.IsKeyPressed(Keys.Down) || Window.inputHandler.IsButtonPressed(IO.ControllerKeys.DOWN))
                    {
                        activeButtons[selectedButton].SetBackground(new Vector4(1f, 1f, 1f, 1));
                        selectedButton = Math.Min(activeButtons.Count - 1, selectedButton + 1);
                        activeButtons[selectedButton].SetBackground(new Vector4(0.6f, 0.6f, 0.6f, 1));
                    }

                    if (Window.inputHandler.IsLeftStickAngleChanged(IO.ControllerAngle.UP) || Window.inputHandler.IsKeyPressed(Keys.W) || Window.inputHandler.IsKeyPressed(Keys.Up) || Window.inputHandler.IsButtonPressed(IO.ControllerKeys.UP))
                    {
                        activeButtons[selectedButton].SetBackground(new Vector4(1f, 1f, 1f, 1));
                        selectedButton = Math.Max(0, selectedButton - 1);
                        activeButtons[selectedButton].SetBackground(new Vector4(0.6f, 0.6f, 0.6f, 1));
                    }

                    if (Window.inputHandler.IsButtonPressed(IO.ControllerKeys.A) || Window.inputHandler.IsKeyPressed(Keys.Enter))
                    {
                        activeButtons[selectedButton].OnClick();
                    }

                    break;
                case GameState.LOST:

                    if (Window.inputHandler.IsLeftStickAngleChanged(IO.ControllerAngle.DOWN) || Window.inputHandler.IsKeyPressed(Keys.S) || Window.inputHandler.IsKeyPressed(Keys.Down) || Window.inputHandler.IsButtonPressed(IO.ControllerKeys.DOWN))
                    {
                        activeButtons[selectedButton].SetBackground(new Vector4(1f, 1f, 1f, 1));
                        selectedButton = Math.Min(activeButtons.Count - 1, selectedButton + 1);
                        activeButtons[selectedButton].SetBackground(new Vector4(0.6f, 0.6f, 0.6f, 1));
                    }

                    if (Window.inputHandler.IsLeftStickAngleChanged(IO.ControllerAngle.UP) || Window.inputHandler.IsKeyPressed(Keys.W) || Window.inputHandler.IsKeyPressed(Keys.Up) || Window.inputHandler.IsButtonPressed(IO.ControllerKeys.UP))
                    {
                        activeButtons[selectedButton].SetBackground(new Vector4(1f, 1f, 1f, 1));
                        selectedButton = Math.Max(0, selectedButton - 1);
                        activeButtons[selectedButton].SetBackground(new Vector4(0.6f, 0.6f, 0.6f, 1));
                    }

                    if (Window.inputHandler.IsButtonPressed(IO.ControllerKeys.A) || Window.inputHandler.IsKeyPressed(Keys.Enter))
                    {
                        activeButtons[selectedButton].OnClick();
                    }

                    break;
            }
            foreach (Button button in activeButtons)
            {
                button.Update();
            }
            AudioManager.Update();
        }

        public static void ActivateButtons()
        {
            foreach (Button button in activeButtons)
            {
                button.SetBackground(new Vector4(1f, 1f, 1f, 1));
            }

            selectedButton = 0;
            activeButtons[selectedButton].SetBackground(new Vector4(0.6f, 0.6f, 0.6f, 1));

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
                    if (mainMenu.levelTime != null)
                    {
                        Window.spriteRenderer.DrawSprite(mainMenu.levelTime, new Vector2(Window.WindowSize.X / 2, 200) + Window.camera.Position.Xy, new Vector2(mainMenu.levelTime.Size.X, mainMenu.levelTime.Size.Y), 2, 0, Vector4.One);
                    }
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
