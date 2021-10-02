using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Kee5Engine
{
    public class Texture
    {
        public readonly int Handle;
        public readonly Vector2 Size;
        public readonly string name;

        /// <summary>
        /// Create a texture from a filepath
        /// </summary>
        /// <param name="path">Path to the texture</param>
        /// <param name="name">Name of the texture</param>
        /// <returns><code>Texture</code></returns>
        public static Texture LoadFromFile(string path, string name)
        {
            return LoadFromBmp(new Bitmap(path), name, true);
        }

        /// <summary>
        /// Create a texture from a Bitmap
        /// </summary>
        /// <param name="bmp">Bitmap</param>
        /// <param name="name">Name of the texture</param>
        /// <returns><code>Texture</code></returns>
        public static Texture LoadFromBmp(Bitmap bmp, string name, bool pixelart)
        {
            // Generate Handle
            int handle = GL.GenTexture();

            // Bind the texture
            GL.BindTexture(TextureTarget.Texture2D, handle);
            Vector2 size;

            // Load the image
            using (var image = bmp)
            {
                // Get the pixels from the loaded bitmap
                var data = image.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height),         // The pixel area
                    ImageLockMode.ReadOnly,                                 // Locking mode. Need ReadOnly as we're only passing them to OpenGL
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb      // Pixelformat for the pixels
                    );

                // Generate a texture
                GL.TexImage2D(
                    TextureTarget.Texture2D,    // Type of generated texture
                    0,                          // Level of detail
                    PixelInternalFormat.Rgba,   // Target format of the pixels
                    image.Width,                // Width
                    image.Height,               // Height
                    0,                          // Border. Must always be 0, lazy Khronos never removed it
                    PixelFormat.Bgra,           // Format of the pixels
                    PixelType.UnsignedByte,     // Data type of the pixels
                    data.Scan0                  // The actual pixels
                    );
                size = new Vector2(image.Width, image.Height);
            }

            if (pixelart)
            {
                // Set min and mag filter. These are used for scaling down and up, respectively
                // Nearest is used, as it just grabs the nearest pixel, giving the pixellated feel
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            }
            else
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            }

            // Set wrapping mode, this is how the texture wraps
            // S is for X axis, T is for Y axis
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            // Generate mipmaps
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // Unbind the Texture
            GL.BindTexture(TextureTarget.Texture2D, 0);

            return new Texture(handle, size, name);
        }

        public Texture(int glhandle, Vector2 size, string name)
        {
            Handle = glhandle;
            Size = size;
            this.name = name;
        }

        // Activate texture
        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
    }
}
