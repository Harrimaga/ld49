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
            Game.gameWindow.CursorVisible = true;
            Globals.activeButtons.Clear();
            restart = new Button(Window.WindowSize.X / 2, Window.WindowSize.Y / 2, 250, 50, 1, "Pixel", $"Restart {Balance.levelNames[Globals.currentLevel]}", Vector4.One, new Vector3(0, 0, 0), TextAlignment.CENTER, true, () => { Restartlevel(); });
            backToMenu = new Button(Window.WindowSize.X / 2, Window.WindowSize.Y / 2 + 70, 250, 50, 1, "Pixel", "Main Menu", Vector4.One, new Vector3(0, 0, 0), TextAlignment.CENTER, true, () => { GoToMainMenu(); });
            Globals.activeButtons.Add(restart);
            Globals.activeButtons.Add(backToMenu);

            Globals.ActivateButtons();
        }

        public void Restartlevel()
        {
            Globals.activeButtons.Clear();
            Globals.level = new Level();
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
