using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Text;


namespace Kee5Engine
{
    public class Tile
    {

        protected Vector2 position;
        protected bool solid, draw;
        protected Sprite sprite;
        public float resistanceFactor;

        public Tile(Vector2 position, bool solid, bool draw, Texture t)
        {
            this.position = position;
            this.solid = solid;
            this.draw = draw;
            resistanceFactor = 1f;
            if (solid)
            {
                sprite = new Sprite(t, Globals.tileSize, Globals.tileSize, position.X, position.Y, 1, 0, new Vector4(1, 1, 1, 1));
            }
            else
            {
                sprite = new Sprite(t, Globals.tileSize, Globals.tileSize, position.X, position.Y, 1, 0, new Vector4(0, 1, 1, 1));
            }
        }

        public Tile(Vector2 position, bool solid, Sprite s)
        {
            this.position = position;
            this.solid = solid;
            draw = true;
            resistanceFactor = 1f;
            sprite = s;
        }

        public void Draw()
        {
            if (!draw) return;
            sprite.Draw();
        }

        public virtual void Update()
        {
            
        }

        public bool Collides(Vector2 pos, Vector2 size)
        {
            return solid && pos.X < position.X + Globals.tileSize && pos.X + size.X > position.X &&
                pos.Y < position.Y + Globals.tileSize && pos.Y + size.Y > position.Y;
        }

        public bool CollidesEnd(Vector2 pos, Vector2 size)
        {
            return pos.X < position.X + Globals.tileSize && pos.X + size.X > position.X &&
                pos.Y < position.Y + Globals.tileSize && pos.Y + size.Y > position.Y;
        }

    }

    public class LevelEndTile : Tile
    {
        public LevelEndTile(Vector2 position) : base(position, false, true, Window.textures.GetTexture("Pixel"))
        {

        }

        public override void Update()
        {
            if (CollidesEnd(Globals.level.GetPlayerPos(), Globals.level.GetPlayerSize()) && (Window.inputHandler.IsKeyPressed(Keys.E) || Window.inputHandler.IsButtonPressed(IO.ControllerKeys.Y)) && Globals.level.CanEnd())
            {
                // End level
                Globals.gameState = GameState.MENU;
                Globals.levelsUnlocked = Math.Max(Math.Min(Balance.levels.Length, Globals.currentLevel + 2), Globals.levelsUnlocked);
                Globals.mainMenu.OpenLevelSelect(Globals.level.GetTime());
            }
        }
    }

    public class UnstableTile : Tile
    {
        private double stoodOnTime = 0;
        private bool stoodOn = false;
        private Vector2 velocity = new Vector2(0, 0);
        public UnstableTile(Vector2 position) : base(position, true, true, Window.textures.GetTexture("BasicSolid"))
        {

        }

        public UnstableTile(Vector2 position, Sprite s) : base(position, true, s)
        {

        }

        public override void Update()
        {
            if (Collides(Globals.level.GetPlayerPos() + new Vector2(0, 1), Globals.level.GetPlayerSize()))
            {
                stoodOn = true;
            }

            if (stoodOn)
            {
                stoodOnTime += Globals.deltaTime;
            }

            if (stoodOnTime >= Balance.decayTime)
            {
                // Decay
                velocity.Y += Balance.tileGravity * (float)Globals.deltaTime;
            }

            position += velocity;
            sprite.posX = position.X;
            sprite.posY = position.Y;

            if (position.Y > Globals.level.tileDeathPlane)
            {
                Globals.level.deleteTile(this);
            }
        }
    }

    public class IceTile : UnstableTile
    {
        public IceTile(Vector2 position) : base(position, new Sprite(Window.textures.GetTexture("IceTile"), Globals.tileSize, Globals.tileSize, position.X, position.Y, 1, 0, Vector4.One, 3, 0, 0, 1))
        {
            resistanceFactor = Balance.iceResistanceFactor;
        }

        public override void Update()
        {
            base.Update();
        }
    }

    public class SpikeTile : Tile
    {
        public SpikeTile(Vector2 position) : base(position, true, true, Window.textures.GetTexture("Pixel"))
        {
            sprite.color = new Vector4(0.2f, 0.2f, 0.2f, 1);
        }
    }

    public class HiddenSolid : Tile
    {
        public HiddenSolid(Vector2 position) : base(position, true, false, Window.textures.GetTexture("Pixel"))
        {

        }
    }

    public class FakeSolid : Tile
    {
        public FakeSolid(Vector2 position) : base(position, false, true, Window.textures.GetTexture("BasicSolid"))
        {
            sprite.color = new Vector4(1, 1, 1, 1);
        }
    }

    public class Collectable : Tile
    {
        
        public Collectable(Vector2 position) : base(position, false, true, Window.textures.GetTexture("Pixel"))
        {
            
        }

        public override void Update()
        {
            if (CollidesEnd(Globals.level.GetPlayerPos(), Globals.level.GetPlayerSize()))
            {
                Globals.level.AddCollectable();
                Globals.level.deleteTile(this);
            }
        }
    }

}
