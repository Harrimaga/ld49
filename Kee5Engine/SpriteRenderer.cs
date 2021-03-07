using System;
using System.Collections.Generic;
using System.Text;
using Kee5Engine.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Kee5Engine
{
    public class SpriteRenderer
    {

        private readonly float[] _vertices =
        {
            // Position Texture coordinates
            1f, 1f,   1.0f, 1.0f, // Bottom-left vertex
            1f, 0f,   1.0f, 0.0f, // Bottom-right vertex
            0f, 0f,   0.0f, 0.0f, // Top-right vertex

            1f, 1f,   1.0f, 1.0f, // Bottom-left vertex
            0f, 0f,   0.0f, 0.0f, // Top-right vertex
            0f, 1f,   0.0f, 1.0f  // Top-left vertex
        };

        private Shader _shader;

        private int _vertexArrayObject;

        public SpriteRenderer(Shader shader)
        {
            _shader = shader;
            initRenderData();
        }

        public void DrawSprite(Texture texture, Vector2 position, Vector2 size, float rotation, Vector4 color)
        {
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
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.BindVertexArray(0);
        }

        private void initRenderData()
        {
            int vertexBufferObject;

            _vertexArrayObject = GL.GenVertexArray();
            vertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(_vertexArrayObject);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
    }
}
