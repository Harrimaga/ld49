using System;
using System.Collections.Generic;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Kee5Engine
{
    public struct SData
    {
        long img;
        int w, h;
        float x, y, scalex, scaley;
        int startx, starty;
        float r, g, b, a, rot;

        public SData(long img, int w, int h, float x, float y, float scalex, float scaley, int startx, int starty, float r, float g, float b, float a, float rot)
        {
            this.img = img;
            this.w = w;
            this.h = h;
            this.x = x;
            this.y = y;
            this.scalex = scalex;
            this.scaley = scaley;
            this.startx = startx;
            this.starty = starty;
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
            this.rot = rot;
        }
    }

    public class Sprite
    {

        public int w, h, frame;
        public Texture2D texture;

        public Sprite(int width, int height, int frame, Texture2D texture)
        {
            this.w = width;
            this.h = height;
            this.frame = frame;
            this.texture = texture;
            Window.spriteList.Add(this);
        }

        public void Draw(float x, float y, bool cam = true, float rot = 0, float r = 1, float g = 1, float b = 1, float a = 1)
        {
            texture.AddToList(x, y, r, g, b, a, rot, frame, w, h, cam);
        }

        public void Unload()
        {
            Window.spriteList.Remove(this);
        }

        public void MakeImageHandleResident()
        {
            GL.Arb.MakeImageHandleResident(texture.Handle, All.ReadOnly);
        }

        public void MakeImageHandleNonResident()
        {
            GL.Arb.MakeTextureHandleNonResident(texture.Handle);
        }
    }
}
