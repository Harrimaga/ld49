using Kee5Engine.Shaders;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

using OpenTK.Graphics.OpenGL4;
using SharpFont;

namespace Kee5Engine
{

    public struct Character
    {
        public int TextureID;
        public Vector2 Size, Bearing;
        public Fixed26Dot6 Advance;
    }

    public class TextRenderer
    {
        Dictionary<uint, Character> Characters;
        private Shader _shader;
        private int _vertexArrayObject, _vertexBufferObject;

        public TextRenderer(uint width, uint height)
        {
            _shader = new Shader("Shaders/text.vert", "Shaders/text.frag");
            _shader.SetMatrix4("projection", Matrix4.CreateOrthographic(width, height, 0.0f, 100f));
            _shader.SetInt("text", 0);

            _vertexArrayObject = GL.GenVertexArray();
            _vertexBufferObject = GL.GenBuffer();

            GL.BindVertexArray(_vertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);


            Characters = new Dictionary<uint, Character>();
        }

        public void Load(string font, uint fontSize)
        {
            Characters.Clear();
            Library ft = new Library();
            Face face = new Face(ft, font);
            face.SetPixelSizes(0, fontSize);

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            for (uint i = 0; i < 128; i++)
            {
                face.LoadChar(i, LoadFlags.Default, LoadTarget.Normal);
                int texture = GL.GenTexture();

                GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.CompressedRed,
                    face.Glyph.Bitmap.Width,
                    face.Glyph.Bitmap.Rows,
                    0,
                    PixelFormat.Red,
                    PixelType.UnsignedByte,
                    face.Glyph.Bitmap.Buffer
                );

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                Character character = new Character
                {
                    TextureID = texture,
                    Size = new Vector2(face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows),
                    Bearing = new Vector2(face.Glyph.BitmapLeft, face.Glyph.BitmapTop),
                    Advance = face.Glyph.Advance.X
                };

                Characters.Add(i, character);
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);
            face.Dispose();
            ft.Dispose();

        }

        public void RenderText(string text, float x, float y, float scale, Vector4 color)
        {
            _shader.Use();
            _shader.SetVector4("textColor", color);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindVertexArray(_vertexArrayObject);

            for (uint i = 0; i < text.Length; i++)
            {
                Character c = Characters[i];

                float xpos = x + c.Bearing.X * scale;
                float ypos = y + (Characters[7].Bearing.Y - c.Bearing.Y) * scale;

                float w = c.Size.X * scale;
                float h = c.Size.Y * scale;

                float[] vertices = new float[]
                {
                    xpos,     ypos + h, 0.0f, 1.0f,
                    xpos + w, ypos,     1.0f, 0.0f,
                    xpos,     ypos,     0.0f, 0.0f,
                    xpos,     ypos + h, 0.0f, 1.0f,
                    xpos + w, ypos + h, 1.0f, 1.0f,
                    xpos + w, ypos,     1.0f, 0.0f
                };

                GL.BindTexture(TextureTarget.Texture2D, c.TextureID);
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
                GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, sizeof(float) * 24, vertices);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

                x += (c.Advance.ToInt32() >> 6) * scale;
            }

            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

    }
}
