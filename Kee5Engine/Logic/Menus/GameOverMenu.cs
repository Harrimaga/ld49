using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kee5Engine
{
    public class GameOverMenu
    {
        public Button restart, backToMenu;

        public GameOverMenu()
        {
            restart = new Button(Window.WindowSize.X / 2, Window.WindowSize.Y / 2, 250, 50, 1, "Pixel", $"Restart Level {Globals.currentLevel + 1}", Vector4.One, new Vector3(0, 0, 0), TextAlignment.CENTER, true, () => { Restartlevel(); });
            backToMenu = new Button(Window.WindowSize.X / 2, Window.WindowSize.Y / 2 + 70, 250, 50, 1, "Pixel", "Main Menu", Vector4.One, new Vector3(0, 0, 0), TextAlignment.CENTER, true, () => { GoToMainMenu(); });
            Globals.activeButtons.Add(restart);
            Globals.activeButtons.Add(backToMenu);
        }

        public void Restartlevel()
        {
            Globals.level = new Level();
            Globals.activeButtons.Clear();
            Globals.gameState = GameState.PLAYING;
        }

        public void GoToMainMenu()
        {
            Globals.activeButtons.Clear();
            Globals.gameState = GameState.MENU;
            Globals.mainMenu = new MainMenu();
        }
    }
}
