using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kee5Engine
{
    public class MainMenu
    {
        public Button start, selectLevel;

        public MainMenu()
        {
            start = new Button(Window.WindowSize.X / 2, Window.WindowSize.Y / 2, 200, 50, 1, "Pixel", "Start Game", Vector4.One, new Vector3(0, 0, 0), TextAlignment.CENTER, true, () => { StartGame(); });
            selectLevel = new Button(Window.WindowSize.X / 2, Window.WindowSize.Y / 2 + 60, 200, 50, 1, "Pixel", "Select Level", Vector4.One, new Vector3(0, 0, 0), TextAlignment.CENTER, true, () => { OpenLevelSelect(); });
            Globals.activeButtons.Add(start);
            Globals.activeButtons.Add(selectLevel);
        }

        public void StartGame(int i = 0)
        {
            Globals.currentLevel = i;
            Globals.level = new Level();
            Globals.activeButtons.Clear();
            Globals.gameState = GameState.PLAYING;
        }


        public void OpenLevelSelect(double time = 0)
        {
            Globals.activeButtons.Clear();

            if (time > 0)
            {
                TimeSpan ts = TimeSpan.FromSeconds(time);
                string timestring = ts.ToString(@"h\:mm\:ss\.FFF");

                Globals.activeButtons.Add(
                    new Button(Window.WindowSize.X / 2, 200, 300, 150, 2, timestring, Vector3.One, TextAlignment.CENTER, true, () => { })
                    );
            }

            for (int i = 0; i < Globals.levelsUnlocked; i++)
            {
                int level = i;
                Globals.activeButtons.Add(
                    new Button(Window.WindowSize.X / 2, Window.WindowSize.Y / 2 + 60 * i, 200, 50, 1, "Pixel", Balance.levels[level], Vector4.One, new Vector3(0, 0, 0), TextAlignment.CENTER, true, () => { StartGame(level); })
                    );
            }
        }
    }
}
