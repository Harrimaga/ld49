using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Kee5Engine.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Kee5Engine
{

    public struct Vertex
    {
        public float[] Position;
        public float[] Color;
        public float[] TexCoords;

        public Vertex(float x, float y, float r, float g, float b, float a, float texX, float texY)
        {
            Position = new float[]
            {
                x, y, 1
            };
            Color = new float[]
            {
                r, g, b, a
            };
            TexCoords = new float[]
            {
                texX, texY
            };
        }
    }

    public class SpriteRenderer
    {

        private readonly float[] _vertices =
        {
            // Position Texture coordinates
            0f, 0f,   0.0f, 0.0f, // Top-right vertex
            1f, 0f,   1.0f, 0.0f, // Bottom-right vertex
            1f, 1f,   1.0f, 1.0f, // Bottom-left vertex
            0f, 1f,   0.0f, 1.0f  // Top-left vertex
        };

        private float[] _verts;

        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private Shader _shader;
        private List<Sprite> _drawList;

        private int _vertexArrayObject;
        private int _vertexBufferObject;
        private int _elementBufferObject;

        public SpriteRenderer(Shader shader)
        {
            _shader = shader;
            _drawList = new List<Sprite>();
            initRenderData();
        }

        public void OnRenderFrame()
        {
            // Create the Vertices Array with length of the list * 4, because there are 4 vertices per sprite
            _verts = new float[_drawList.Count * 9 * 4];

            // For each Sprite in the DrawList, get the vertices and add them to the vertex array
            for (int i = 0; i < _drawList.Count; i++)
            {
                Sprite s = _drawList[i];
                _verts[i * 32 + 0] = s.posX;
                _verts[i * 32 + 1] = s.posY;
                _verts[i * 32 + 2] = 1;
                _verts[i * 32 + 3] = 1;
                _verts[i * 32 + 4] = 1;
                _verts[i * 32 + 5] = 1;
                _verts[i * 32 + 6] = 0;
                _verts[i * 32 + 7] = 0;
                _verts[i * 32 + 8] = s.posX + s.width;
                _verts[i * 32 + 9] = s.posY;
                _verts[i * 32 + 10] = 1;
                _verts[i * 32 + 11] = 1;
                _verts[i * 32 + 12] = 1;
                _verts[i * 32 + 13] = 1;
                _verts[i * 32 + 14] = 1;
                _verts[i * 32 + 15] = 0;
                _verts[i * 32 + 16] = s.posX + s.width;
                _verts[i * 32 + 17] = s.posY + s.height;
                _verts[i * 32 + 18] = 1;
                _verts[i * 32 + 19] = 1;
                _verts[i * 32 + 20] = 1;
                _verts[i * 32 + 21] = 1;
                _verts[i * 32 + 22] = 1;
                _verts[i * 32 + 23] = 1;
                _verts[i * 32 + 24] = s.posX;
                _verts[i * 32 + 25] = s.posY + s.height;
                _verts[i * 32 + 26] = 1;
                _verts[i * 32 + 27] = 1;
                _verts[i * 32 + 28] = 1;
                _verts[i * 32 + 29] = 1;
                _verts[i * 32 + 30] = 0;
                _verts[i * 32 + 31] = 1;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, _verts.Length * sizeof(float), _verts);

            GL.ActiveTexture(TextureUnit.Texture0);
            Window.textures.GetTexture("Test").Use(TextureUnit.Texture0);

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
        }

        public void DrawSprite(Texture texture, Vector2 position, Vector2 size, float rotation, Vector4 color)
        {
            _drawList.Add(new Sprite(texture, size.X, size.Y, position.X, position.Y));
            return;


            if (position.X - Window.camera.Position.X > Window.WindowSize.X + 100 + size.X / 2
                || position.Y - Window.camera.Position.Y > Window.WindowSize.Y + 100 + size.Y / 2
                || position.X - Window.camera.Position.X < -100 - size.X / 2
                || position.Y - Window.camera.Position.Y < -100 - size.Y / 2)
            {
                return;
            }
            Matrix4 model = Matrix4.Identity;
            model *= Matrix4.CreateScale(size.X, size.Y, 1f);
            model *= Matrix4.CreateRotationZ(rotation);
            model *= Matrix4.CreateTranslation(position.X - size.X / 2, position.Y - size.Y / 2, 1f);

            _shader.SetMatrix4("model", model);
            _shader.SetVector4("spriteColor", color);

            GL.ActiveTexture(TextureUnit.Texture0);
            texture.Use(TextureUnit.Texture0);

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
        }

        private void initRenderData()
        {
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            _vertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, 1000 * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 9, 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, sizeof(float) * 9, 3 * sizeof(float));

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, sizeof(float) * 9, 7 * sizeof(float));

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
    }
}
