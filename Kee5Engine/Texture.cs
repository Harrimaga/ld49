using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Kee5Engine
{
    public class Texture
    {
        public readonly int Handle;

        public static Texture LoadFromFile(string path)
        {
            // Generate Handle
            int handle = GL.GenTexture();

            // Bind the handle
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, handle);

            // Load the image
            using (var image = new Bitmap(path))
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
            }

            // Set min and mag filter. These are used for scaling down and up, respectively
            // Nearest is used, as it just grabs the nearest pixel, giving the pixellated feel
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            // Set wrapping mode, this is how the texture wraps
            // S is for X axis, T is for Y axis
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            // Generate mipmaps
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return new Texture(handle);
        }

        public Texture(int glhandle)
        {
            Handle = glhandle;
        }

        // Activate texture
        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
    }
}
