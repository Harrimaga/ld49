using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using System;
using System.Collections.Generic;
using System.Text;

using OpenTK.Graphics.OpenGL4;

namespace Kee5Engine
{    
    public class Texture2D
    {
        public long Handle;
        int spriteSheetWidth, spriteSheetHeight, spriteWidth, spriteHeight, columns, rows;

        public Texture2D(string file, int spriteSheetWidth, int spriteSheetHeight, int spriteWidth, int spriteHeight)
        {
            this.spriteSheetWidth = spriteSheetWidth;
            this.spriteSheetHeight = spriteSheetHeight;
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;
            columns = spriteSheetWidth / spriteWidth;
            rows = spriteSheetHeight / spriteHeight;

            Image<Rgba32> image = (Image<Rgba32>)Image.Load(file);
            image.Mutate(x => x.Flip(FlipMode.Vertical));
            image.TryGetSinglePixelSpan(out Span<Rgba32> tp);
            Rgba32[] tempPixels = tp.ToArray();
            List<byte> pixels = new List<byte>();

            foreach (Rgba32 p in tempPixels)
            {
                pixels.Add(p.R);
                pixels.Add(p.G);
                pixels.Add(p.B);
                pixels.Add(p.A);
            }

            int h = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, h);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 0x2601);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 0x2601);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels.ToArray());
            Handle = GL.Arb.GetImageHandle(h, 0, false, 0, (PixelFormat)0x8058);
        }

        public void AddToList(float x, float y, float r, float g, float b, float a, float rot, int num, int w, int h, bool cam)
        {
            int sX = num * spriteWidth % spriteSheetWidth;
            int sY = (rows - 1) - num * spriteWidth / spriteSheetWidth;
            sY *= spriteHeight;
            float scaleX = (float)(w) / spriteWidth;
            float scaleY = (float)(h) / spriteHeight;

            if (cam)
            {
                Window.drawList.Add(new SData(Handle, (int)(w * Window.screenScaleX), (int)(h * Window.screenScaleY), (x /*- Window.camX*/) * Window.screenScaleX, (y /*- Window.camY*/) * Window.screenScaleY, scaleX * Window.screenScaleX, scaleY * Window.screenScaleY, sX, sY, r, g, b, a, rot));
            }
            else
            {
                Window.drawList.Add(new SData(Handle, (int)(w * Window.screenScaleX), (int)(h * Window.screenScaleY), x * Window.screenScaleX, y * Window.screenScaleY, scaleX * Window.screenScaleX, scaleY * Window.screenScaleY, sX, sY, r, g, b, a, rot));
            }

        }
    }
}
