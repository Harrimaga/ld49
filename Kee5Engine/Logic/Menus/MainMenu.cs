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
        public List<Button> levels, activeLevelButtons;
        public List<Sprite> records;
        public Sprite deathCount;
        public int currentLevelList;

        private int levelsPerList = 5, buttonWidth = 260;

        public MainMenu()
        {
            Game.gameWindow.CursorVisible = true;
            Window.camera.Position = new Vector3(0, 0, 10);
            Globals.activeButtons.Clear();
            start = new Button(Window.WindowSize.X / 2, Window.WindowSize.Y / 2, buttonWidth, 50, 1, "Pixel", "Start Game", Vector4.One, new Vector3(0, 0, 0), TextAlignment.CENTER, true, () => { StartGame(); });
            selectLevel = new Button(Window.WindowSize.X / 2, Window.WindowSize.Y / 2 + 60, buttonWidth, 50, 1, "Pixel", "Select Level", Vector4.One, new Vector3(0, 0, 0), TextAlignment.CENTER, true, () => { OpenLevelSelect(); });

            fullScreen = new Button(Window.WindowSize.X / 2, Window.WindowSize.Y / 2 + 120, buttonWidth, 50, 1, "Pixel", "Toggle Fullscreen", Vector4.One, new Vector3(0, 0, 0), TextAlignment.CENTER, true, () => { ToggleFullscreen(); });
            quit = new Button(Window.WindowSize.X / 2, Window.WindowSize.Y - 100, buttonWidth, 50, 1, "Pixel", "Quit Game", Vector4.One, new Vector3(0, 0, 0), TextAlignment.CENTER, true, () => { Game.gameWindow.Close(); });

            levels = new List<Button>();
            activeLevelButtons = new List<Button>();
            records = new List<Sprite>();
            currentLevelList = 0;
            Globals.activeButtons.Add(start);
            Globals.activeButtons.Add(selectLevel);
            Globals.activeButtons.Add(fullScreen);
            Globals.activeButtons.Add(quit);

            Texture deathCountText = Texture.LoadFromBmp(Window.textRenderer.RenderString($"Total Deaths: {Globals.deathCount}", Color.White), "DeathCountText", false);

            deathCount = new Sprite(deathCountText, deathCountText.Size.X, deathCountText.Size.Y, Window.WindowSize.X / 2, Window.WindowSize.Y - 200, 7, 0, Vector4.One);

            Globals.ActivateButtons();
        }

        public void StartGame(int i = 0)
        {
            Game.gameWindow.CursorVisible = false;
            Globals.currentLevel = i;
            Globals.activeButtons.Clear();
            Globals.level = new Level();
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

        public void LevelList(int i)
        {
            if (i < 0)
            {
                currentLevelList++;
                return;
            }

            if (i > (int)Math.Floor((double)(Globals.levelsUnlocked - 1) / (float)levelsPerList))
            {
                currentLevelList--;
                return;
            }

            records.Clear();

            foreach (Button button in activeLevelButtons)
            {
                Globals.activeButtons.Remove(button);
            }

            for (int j = 0; j < levelsPerList; j++)
            {
                if (i * levelsPerList + j > Globals.levelsUnlocked - 1)
                {
                    return;
                }
                int level = i * levelsPerList + j;
                Button btn = new Button(Window.WindowSize.X / 2, Window.WindowSize.Y / 2 + 60 * j, 200, 50, 1, "Pixel", Balance.levelNames[level], Vector4.One, new Vector3(0, 0, 0), TextAlignment.CENTER, true, () => { StartGame(level); });
                Globals.activeButtons.Add(btn);
                activeLevelButtons.Add(btn);

                TimeSpan time = Globals.records[level];

                if (time.Days != 2)
                {
                    string timeString = time.ToString(@"h\:mm\:ss\.FFF");
                    Texture t = Texture.LoadFromBmp(Window.textRenderer.RenderString(timeString, Color.White), $"RecordLvl{level}", false);
                    records.Add(
                        new Sprite(t, t.Size.X, t.Size.Y, btn.posX + 300, btn.posY, btn.layer, 0, Vector4.One)
                        );
                }
            }
        }


        public void OpenLevelSelect(double time = 0)
        {
            Game.gameWindow.CursorVisible = true;
            Globals.activeButtons.Clear();

            Globals.activeButtons.Add(
                new Button(Window.WindowSize.X / 2, 300, 200, 50, 1, "Pixel", "Main Menu", Vector4.One, new Vector3(0, 0, 0), TextAlignment.CENTER, true, () => {
                    Globals.activeButtons.Clear();
                    Globals.gameState = GameState.MENU;
                    Globals.mainMenu = new MainMenu();
                })
                );

            if (time > 0)
            {
                TimeSpan ts = TimeSpan.FromSeconds(time);
                string timestring = ts.ToString(@"h\:mm\:ss\.FFF");

                Window.textRenderer.SetSize(128);
                levelTime = Texture.LoadFromBmp(Window.textRenderer.RenderString(timestring, Color.White), "levelTime", false);

                Globals.activeButtons.Add(
                    new Button(Window.WindowSize.X / 2, Window.WindowSize.Y / 2 - 120, buttonWidth, 50, 1, "Pixel", "Next Level", Vector4.One, Vector3.Zero, TextAlignment.CENTER, true, () => { StartGame(++Globals.currentLevel); })
                    );
            }

            Globals.activeButtons.Add(
                new Button(Window.WindowSize.X / 2 - 50, Window.WindowSize.Y / 2 - 60, 80, 50, 1, "Pixel", "<", Vector4.One, Vector3.Zero, TextAlignment.CENTER, true, () => { LevelList(--currentLevelList); })
                );

            Globals.activeButtons.Add(
                new Button(Window.WindowSize.X / 2 + 50, Window.WindowSize.Y / 2 - 60, 80, 50, 1, "Pixel", ">", Vector4.One, Vector3.Zero, TextAlignment.CENTER, true, () => { LevelList(++currentLevelList); })
                );

            LevelList(0);

            Globals.ActivateButtons();
        }

        public void Draw()
        {
            Window.camera.Position = new Vector3(0, 0, 10);
            foreach (Sprite sprite in records)
            {
                sprite.Draw();
            }
            deathCount.Draw();
        }
    }
}
