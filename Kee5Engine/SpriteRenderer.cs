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

        private float[] _vertices =
        {
            // Position     Texture     Color
            0.5f, 0.5f, 1f, 1.0f, 1.0f, 1f, 1f, 1f, 1f, // Bottom-right vertex
            0.5f, 0f, 1f,   1.0f, 0.0f, 1f, 1f, 1f, 1f, // Top-right vertex
            0f, 0f, 1f,     0.0f, 0.0f, 1f, 1f, 1f, 1f, // Top-left vertex
            
            0.5f, 0.5f, 1f, 1.0f, 1.0f, 1f, 1f, 1f, 1f, // Bottom-right vertex
            0f, 0f, 1f,     0.0f, 0.0f, 1f, 1f, 1f, 1f, // Top-left vertex
            0f, 0.5f, 1f,   0.0f, 1.0f, 1f, 1f, 1f, 1f, // Bottom-left vertex

            1f, 0.5f, 1f,   1.0f, 1.0f, 1f, 1f, 1f, 1f, // Bottom-right vertex
            1f, 0f, 1f,     1.0f, 0.0f, 1f, 1f, 1f, 1f, // Top-right vertex
            0.5f, 0f, 1f,   0.0f, 0.0f, 1f, 1f, 1f, 1f, // Top-left vertex
            
            1f, 0.5f, 1f,   1.0f, 1.0f, 1f, 1f, 1f, 1f, // Bottom-right vertex
            0.5f, 0f, 1f,   0.0f, 0.0f, 1f, 1f, 1f, 1f, // Top-left vertex
            0.5f, 0.5f, 1f, 0.0f, 1.0f, 1f, 1f, 1f, 1f  // Bottom-left vertex
        };

        public float[] GetVertices()
        {

            float[] color = new float[] { 1f, 1f, 1f, 1f };

            int SpriteCount = 5;

            float[] vertices = new float[SpriteCount * 54];

            Random rng = new Random();

            for (int i = 0; i < SpriteCount; i++)
            {
                float startPosX = (float)rng.NextDouble();
                float startPosY = (float)rng.NextDouble();
                vertices[i * 54 + 0] = startPosX;
                vertices[i * 54 + 1] = startPosY;
                vertices[i * 54 + 2] = 1f;
                             
                vertices[i * 54 + 3] = 1.0f;
                vertices[i * 54 + 4] = 1.0f;
                             
                vertices[i * 54 + 5] = 1.0f;
                vertices[i * 54 + 6] = 1.0f;
                vertices[i * 54 + 7] = 1.0f;
                vertices[i * 54 + 8] = 1.0f;


                vertices[i * 54 + 9]  = startPosX;
                vertices[i * 54 + 10] = startPosY - 0.2f;
                vertices[i * 54 + 11] = 1.0f;
                             
                vertices[i * 54 + 12] = 1.0f;
                vertices[i * 54 + 13] = 0.0f;
                             
                vertices[i * 54 + 14] = 1.0f;
                vertices[i * 54 + 15] = 1.0f;
                vertices[i * 54 + 16] = 1.0f;
                vertices[i * 54 + 17] = 1.0f;
                             
                             
                vertices[i * 54 + 18] = startPosX - 0.2f;
                vertices[i * 54 + 19] = startPosY - 0.2f;
                vertices[i * 54 + 20] = 1.0f;
                             
                vertices[i * 54 + 21] = 0.0f;
                vertices[i * 54 + 22] = 0.0f;
                             
                vertices[i * 54 + 23] = 1.0f;
                vertices[i * 54 + 24] = 1.0f;
                vertices[i * 54 + 25] = 1.0f;
                vertices[i * 54 + 26] = 1.0f;
                             
                             
                vertices[i * 54 + 27] = startPosX;
                vertices[i * 54 + 28] = startPosY;
                vertices[i * 54 + 29] = 1f;
                             
                vertices[i * 54 + 30] = 1.0f;
                vertices[i * 54 + 31] = 1.0f;
                             
                vertices[i * 54 + 32] = 1.0f;
                vertices[i * 54 + 33] = 1.0f;
                vertices[i * 54 + 34] = 1.0f;
                vertices[i * 54 + 35] = 1.0f;
                             
                             
                vertices[i * 54 + 36] = startPosX - 0.2f;
                vertices[i * 54 + 37] = startPosY - 0.2f;
                vertices[i * 54 + 38] = 1.0f;
                             
                vertices[i * 54 + 39] = 0.0f;
                vertices[i * 54 + 40] = 0.0f;
                             
                vertices[i * 54 + 41] = 1.0f;
                vertices[i * 54 + 42] = 1.0f;
                vertices[i * 54 + 43] = 1.0f;
                vertices[i * 54 + 44] = 1.0f;
                             
                             
                vertices[i * 54 + 45] = startPosX - 0.2f;
                vertices[i * 54 + 46] = startPosY;
                vertices[i * 54 + 47] = 1.0f;
                             
                vertices[i * 54 + 48] = 0.0f;
                vertices[i * 54 + 49] = 1.0f;
                             
                vertices[i * 54 + 50] = 1.0f;
                vertices[i * 54 + 51] = 1.0f;
                vertices[i * 54 + 52] = 1.0f;
                vertices[i * 54 + 53] = 1.0f;
            }
            return vertices;
        }

        private Shader _shader;

        private int _vertexArrayObject, _vertexBufferObject;

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

            GL.ActiveTexture(TextureUnit.Texture0);
            texture.Use(TextureUnit.Texture0);

            

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, _vertices.Length * sizeof(float), _vertices);

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 12);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        private void initRenderData()
        {

            _vertices = GetVertices();

            _vertexArrayObject = GL.GenVertexArray();
            _vertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, 1000 * sizeof(float), IntPtr.Zero, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(_vertexArrayObject);

            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 9 * sizeof(float), 3 * sizeof(float));

            var colorLocation = _shader.GetAttribLocation("aColor");
            GL.EnableVertexAttribArray(colorLocation);
            GL.VertexAttribPointer(colorLocation, 4, VertexAttribPointerType.Float, false, 9 * sizeof(float), 5 * sizeof(float));

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
    }
}
