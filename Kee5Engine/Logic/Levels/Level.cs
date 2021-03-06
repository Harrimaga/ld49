using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Kee5Engine
{
    public class Level
    {
        protected Tile[,] grid;
        protected Player player;
        private List<Tile> removables;
        public float tileDeathPlane, entityDeathPlane;
        private BackgroundHandler background;
        protected List<Sprite> textSprites;

        private int collectablesNeeded;
        private int totalCollectablesInLevel;
        private Button keyCounterSprite, keyCounterAmount;

        private double time;

        public Level()
        {
            textSprites = new List<Sprite>();
            LoadLevel($"Logic/Levels/Levels/{Balance.levels[Globals.currentLevel]}.txt");
            removables = new List<Tile>();
            time = 0;
            tileDeathPlane = (grid.GetLength(1) + 10) * Globals.tileSize;
            entityDeathPlane = (grid.GetLength(1) + 10) * Globals.tileSize;
            background = new BackgroundHandler();

            Game.gameWindow.CursorVisible = false;

            keyCounterSprite = new Button(35, 40, 64, 64, 7, "KeyTile", Vector4.One, true, () => { });

            string text = $"{totalCollectablesInLevel - collectablesNeeded}/{totalCollectablesInLevel}";

            keyCounterAmount = new Button(100, 40, 200, 64, 7, text, (collectablesNeeded == 0 ? new Vector3(0, 1, 0) : Vector3.Zero), TextAlignment.CENTER, true, () => { });

            Globals.activeButtons.Add(keyCounterSprite);
            Globals.activeButtons.Add(keyCounterAmount);
        }

        public bool CollidesWithMap(Entity e, out Tile collission)
        {
            Vector2 beginPos = new Vector2((float)Math.Floor(e.position.X / Globals.tileSize), (float)Math.Floor(e.position.Y / Globals.tileSize));
            Vector2 amountToCheck = new Vector2((float)Math.Floor(2 + e.size.X / Globals.tileSize), (float)Math.Floor(2 + e.size.Y / Globals.tileSize));
            for (int i = (int)beginPos.X; i < beginPos.X + amountToCheck.X; i++)
            {
                for (int j = (int)beginPos.Y; j < beginPos.Y + amountToCheck.Y; j++)
                {
                    if (i >= 0 && i < grid.GetLength(0) && j >= 0 && j < grid.GetLength(1) && grid[i, j] != null && grid[i, j].Collides(e.position, e.size))
                    {
                        collission = grid[i, j];
                        return true;
                    }
                }
            }
            collission = null;
            return false;
        }

        public int AddCollectable()
        {
            collectablesNeeded--;
            string text = $"{totalCollectablesInLevel - collectablesNeeded}/{totalCollectablesInLevel}";
            Globals.activeButtons.Remove(keyCounterAmount);
            keyCounterAmount = new Button(100, 40, 200, 64, 7, text, (collectablesNeeded == 0 ? new Vector3(0, 1, 0) : Vector3.Zero), TextAlignment.CENTER, true, () => { });
            Globals.activeButtons.Add(keyCounterAmount);
            return collectablesNeeded;
        }

        public bool CanEnd()
        {
            return collectablesNeeded <= 0;
        }

        public double GetTime()
        {
            return time;
        }

        public Tile GetTileBelow(Vector2 position, Vector2 size)
        {
            Vector2 tile = position + new Vector2(0, size.Y + 1);
            int i = (int)Math.Round(tile.X / Globals.tileSize);
            int j = (int)Math.Floor(tile.Y / Globals.tileSize);

            if (i >= 0 && i < grid.GetLength(0) && j >= 0 && j < grid.GetLength(1))
            {
                return grid[i, j];
            }

            return null;
        }

        public void LoadLevel(string path)
        {
            string line;

            // Read the file and display it line by line.  
            System.IO.StreamReader file =
                new System.IO.StreamReader(path);

            List<Vector2> texts = new List<Vector2>();

            int width = int.Parse(file.ReadLine());
            int height = int.Parse(file.ReadLine());

            grid = new Tile[width, height];

            int y = 0;
            while (y < height && (line = file.ReadLine()) != null)
            {

                for (int x = 0; x < line.Length; x++)
                {
                    char c = line[x];
                    switch (c)
                    {
                        case ' ':
                            grid[x, y] = null;
                            break;
                        case '#':
                            grid[x, y] = new UnstableTile(new Vector2(x * Globals.tileSize, y * Globals.tileSize));
                            break;
                        case '0':
                            grid[x, y] = null;
                            player = new Player(new Vector2(Globals.tileSize * x, Globals.tileSize * y), Window.textures.GetTexture("Player"));
                            break;
                        case '1':
                            grid[x, y] = new LevelEndTile(new Vector2(x * Globals.tileSize, y * Globals.tileSize));
                            break;
                        case 'S':
                            grid[x, y] = new Tile(new Vector2(x * Globals.tileSize, y * Globals.tileSize), true, true, Window.textures.GetTexture("SafeTile"));
                            break;
                        case 'I':
                            grid[x, y] = new IceTile(new Vector2(x * Globals.tileSize, y * Globals.tileSize));
                            break;
                        case '^':
                            grid[x, y] = new SpikeTile(new Vector2(x * Globals.tileSize, y * Globals.tileSize));
                            break;
                        case 'H':
                            grid[x, y] = new HiddenSolid(new Vector2(x * Globals.tileSize, y * Globals.tileSize));
                            break;
                        case 'F':
                            grid[x, y] = new FakeSolid(new Vector2(x * Globals.tileSize, y * Globals.tileSize));
                            break;
                        case 'C':
                            grid[x, y] = new Collectable(new Vector2(x * Globals.tileSize, y * Globals.tileSize));
                            collectablesNeeded++;
                            break;
                        case 'T':
                            texts.Add(new Vector2(x, y));
                            break;
                        case 'L':
                            grid[x, y] = new Arrow(new Vector2(x * Globals.tileSize, y * Globals.tileSize), "ArrowLeft");
                            break;
                        case 'R':
                            grid[x, y] = new Arrow(new Vector2(x * Globals.tileSize, y * Globals.tileSize), "ArrowRight");
                            break;
                        case 'D':
                            grid[x, y] = new Arrow(new Vector2(x * Globals.tileSize, y * Globals.tileSize), "ArrowDown");
                            break;
                        case 'U':
                            grid[x, y] = new Arrow(new Vector2(x * Globals.tileSize, y * Globals.tileSize), "ArrowUp");
                            break;
                    }
                }
                y++;
            }

            totalCollectablesInLevel = collectablesNeeded;

            int i = 0;
            while ((line = file.ReadLine()) != null)
            {
                Window.textRenderer.SetSize(32);
                Texture text = Texture.LoadFromBmp(
                    Window.textRenderer.RenderString(line, Color.Black),
                    "TutText",
                    false
                    );
                textSprites.Add(new Sprite(text, text.Size.X, text.Size.Y, texts[i].X * Globals.tileSize, texts[i].Y * Globals.tileSize, 9, 0, Vector4.One));
                i++;
            }

            file.Close();

        }

        public void Draw()
        {
            background.Draw();
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y] != null)
                        grid[x, y].Draw();
                }
            }
            player.Draw();

            foreach (Sprite text in textSprites)
            {
                text.Draw();
            }
        }

        public void Update()
        {
            time += Globals.deltaTime;
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y] != null)
                    {
                        grid[x, y].Update();
                        if (removables.Contains(grid[x, y]))
                        {
                            grid[x, y] = null;
                        }
                    }
                }
            }

            player.Update();
            background.Update();
        }

        public void deleteTile(Tile tile)
        {
            removables.Add(tile);
        }

        public Vector2 GetPlayerPos()
        {
            return player.position;
        }

        public Vector2 GetPlayerSize()
        {
            return player.size;
        }

        public Vector2 GetPlayerVelocity()
        {
            return player.velocity;
        }
    }
}
