using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kee5Engine
{
    public class BackgroundHandler
    {
        private Sprite s1, s2, s3, s4, s5, s1b, s2b, s3b, s4b, s5b;
        private Vector2 position;

        public BackgroundHandler()
        {
            s1 = new Sprite(Window.textures.GetTexture("Background1"), Window.WindowSize.X, Window.WindowSize.Y, Window.WindowSize.X / 2, Window.WindowSize.Y / 2, 0, 0, Vector4.One);
            s2 = new Sprite(Window.textures.GetTexture("Background2"), Window.WindowSize.X, Window.WindowSize.Y, Window.WindowSize.X / 2, Window.WindowSize.Y / 2, 0.1f, 0, Vector4.One);
            s3 = new Sprite(Window.textures.GetTexture("Background3"), Window.WindowSize.X, Window.WindowSize.Y, Window.WindowSize.X / 2, Window.WindowSize.Y / 2, 0.2f, 0, Vector4.One);
            s4 = new Sprite(Window.textures.GetTexture("Background4"), Window.WindowSize.X, Window.WindowSize.Y, Window.WindowSize.X / 2, Window.WindowSize.Y / 2, 0.3f, 0, Vector4.One);
            s5 = new Sprite(Window.textures.GetTexture("Background5"), Window.WindowSize.X, Window.WindowSize.Y, Window.WindowSize.X / 2, Window.WindowSize.Y / 2, 0.4f, 0, Vector4.One);

            s1b = new Sprite(Window.textures.GetTexture("Background1"), Window.WindowSize.X, Window.WindowSize.Y, Window.WindowSize.X / 2, Window.WindowSize.Y / 2, 0, 0, Vector4.One);
            s2b = new Sprite(Window.textures.GetTexture("Background2"), Window.WindowSize.X, Window.WindowSize.Y, Window.WindowSize.X / 2, Window.WindowSize.Y / 2, 0.1f, 0, Vector4.One);
            s3b = new Sprite(Window.textures.GetTexture("Background3"), Window.WindowSize.X, Window.WindowSize.Y, Window.WindowSize.X / 2, Window.WindowSize.Y / 2, 0.2f, 0, Vector4.One);
            s4b = new Sprite(Window.textures.GetTexture("Background4"), Window.WindowSize.X, Window.WindowSize.Y, Window.WindowSize.X / 2, Window.WindowSize.Y / 2, 0.3f, 0, Vector4.One);
            s5b = new Sprite(Window.textures.GetTexture("Background5"), Window.WindowSize.X, Window.WindowSize.Y, Window.WindowSize.X / 2, Window.WindowSize.Y / 2, 0.4f, 0, Vector4.One);
        }

        public void Update()
        {
            position = Globals.level.GetPlayerPos();

            s1.posX = position.X;
            s1.posY = position.Y;
            s2.posX = position.X - (position.X * 0.1f) % Window.WindowSize.X + Window.WindowSize.X;
            s2.posY = position.Y;
            s3.posX = position.X - (position.X * 0.2f) % Window.WindowSize.X + Window.WindowSize.X;
            s3.posY = position.Y;
            s4.posX = position.X - (position.X * 0.4f) % Window.WindowSize.X + Window.WindowSize.X;
            s4.posY = position.Y;
            s5.posX = position.X - (position.X * 0.6f) % Window.WindowSize.X + Window.WindowSize.X;
            s5.posY = position.Y;

            s1b.posX = s1.posX - Window.WindowSize.X;
            s1b.posY = s1.posY;
            s2b.posX = s2.posX - Window.WindowSize.X;
            s2b.posY = s2.posY;
            s3b.posX = s3.posX - Window.WindowSize.X;
            s3b.posY = s3.posY;
            s4b.posX = s4.posX - Window.WindowSize.X;
            s4b.posY = s4.posY;
            s5b.posX = s5.posX - Window.WindowSize.X;
            s5b.posY = s5.posY;
        }

        public void Draw()
        {
            s1.Draw();
            s1b.Draw();
            s2.Draw();
            s2b.Draw();
            s3.Draw();
            s3b.Draw();
            s4.Draw();
            s4b.Draw();
            s5b.Draw();
            s5.Draw();
        }
    }
}
