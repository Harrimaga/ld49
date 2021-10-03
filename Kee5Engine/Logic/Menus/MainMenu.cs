using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Kee5Engine
{
    public class MainMenu
    {
        public Button start, selectLevel, fullScreen, quit;
        public bool isFullScreen;
        public Texture levelTime;

        public MainMenu()
        {
            start = new Button(Window.WindowSize.X / 2, Window.WindowSize.Y / 2, 240, 50, 1, "Pixel", "Start Game", Vector4.One, new Vector3(0, 0, 0), TextAlignment.CENTER, true, () => { StartGame(); });
            selectLevel = new Button(Window.WindowSize.X / 2, Window.WindowSize.Y / 2 + 60, 240, 50, 1, "Pixel", "Select Level", Vector4.One, new Vector3(0, 0, 0), TextAlignment.CENTER, true, () => { OpenLevelSelect(); });

            string fs = (isFullScreen ? "ON" : "OFF");
            fullScreen = new Button(Window.WindowSize.X / 2, Window.WindowSize.Y / 2 + 120, 240, 50, 1, "Pixel", $"Fullscreen {fs}", Vector4.One, new Vector3(0, 0, 0), TextAlignment.CENTER, true, () => { ToggleFullscreen(); });
            quit = new Button(Window.WindowSize.X / 2, Window.WindowSize.Y - 100, 240, 50, 1, "Pixel", "Quit Game", Vector4.One, new Vector3(0, 0, 0), TextAlignment.CENTER, true, () => { Game.gameWindow.Close(); });
            Globals.activeButtons.Add(start);
            Globals.activeButtons.Add(selectLevel);
            Globals.activeButtons.Add(fullScreen);
            Globals.activeButtons.Add(quit);

            Globals.ActivateButtons();
        }

        public void StartGame(int i = 0)
        {
            Globals.currentLevel = i;
            Globals.level = new Level();
            Globals.activeButtons.Clear();
            Globals.gameState = GameState.PLAYING;
        }

        public void ToggleFullscreen()
        {
            if (!isFullScreen)
            {
                Game.gameWindow.WindowState = OpenTK.Windowing.Common.WindowState.Fullscreen;
            }
            else
            {
                Game.gameWindow.WindowState = OpenTK.Windowing.Common.WindowState.Normal;
            }

            isFullScreen = !isFullScreen;
        }


        public void OpenLevelSelect(double time = 0)
        {
            Globals.activeButtons.Clear();

            if (time > 0)
            {
                TimeSpan ts = TimeSpan.FromSeconds(time);
                string timestring = ts.ToString(@"h\:mm\:ss\.FFF");

                Window.textRenderer.SetSize(128);
                levelTime = Texture.LoadFromBmp(Window.textRenderer.RenderString(timestring, Color.White), "levelTime", false);
            }

            for (int i = 0; i < Globals.levelsUnlocked; i++)
            {
                int level = i;
                Globals.activeButtons.Add(
                    new Button(Window.WindowSize.X / 2, Window.WindowSize.Y / 2 + 60 * i, 200, 50, 1, "Pixel", Balance.levels[level], Vector4.One, new Vector3(0, 0, 0), TextAlignment.CENTER, true, () => { StartGame(level); })
                    );
            }

            Globals.activeButtons.Add(
                new Button(Window.WindowSize.X / 2, Window.WindowSize.Y - 100, 200, 50, 1, "Pixel", "Main Menu", Vector4.One, new Vector3(0, 0, 0), TextAlignment.CENTER, true, () => {
                    Globals.activeButtons.Clear();
                    Globals.gameState = GameState.MENU;
                    Globals.mainMenu = new MainMenu();
                }) 
                );

            Globals.ActivateButtons();
        }
    }
}
