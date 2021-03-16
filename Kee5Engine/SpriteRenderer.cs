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
            960f, 540f, 1f, 1.0f, 1.0f, 1f, 1f, 1f, 1f, // Bottom-right vertex
            960f, 0f, 1f,   1.0f, 0.0f, 1f, 1f, 1f, 1f, // Top-right vertex
            0f, 0f, 1f,     0.0f, 0.0f, 1f, 1f, 1f, 1f, // Top-left vertex
            0f, 540f, 1f,   0.0f, 1.0f, 1f, 1f, 1f, 1f, // Bottom-left vertex

            1920f, 540f, 1f,   1.0f, 1.0f, 1f, 1f, 1f, 1f, // Bottom-right vertex
            1920f, 0f, 1f,     1.0f, 0.0f, 1f, 1f, 1f, 1f, // Top-right vertex
            960f, 0f, 1f,   0.0f, 0.0f, 1f, 1f, 1f, 1f, // Top-left vertex
            960f, 540f, 1f, 0.0f, 1.0f, 1f, 1f, 1f, 1f  // Bottom-left vertex
        };

        private uint[] _indices =
        {
            0, 1, 2,
            0, 2, 3,
            4, 5, 6,
            4, 6, 7
        };

        public float[] GetVertices(List<Sprite> drawList)
        {
            int SpriteCount = drawList.Count;

            float[] vertices = new float[SpriteCount * 36];

            Random rng = new Random();

            for (int i = 0; i < SpriteCount; i++)
            {
                Sprite s = drawList[i];
                vertices[i * 36 + 0] = s.posX + s.width / 2;
                vertices[i * 36 + 1] = s.posY + s.height / 2;
                vertices[i * 36 + 2] = 1f;
                             
                vertices[i * 36 + 3] = 1.0f;
                vertices[i * 36 + 4] = 1.0f;
                             
                vertices[i * 36 + 5] = 1.0f;
                vertices[i * 36 + 6] = 1.0f;
                vertices[i * 36 + 7] = 1.0f;
                vertices[i * 36 + 8] = 1.0f;
                             
                             
                vertices[i * 36 + 9]  = s.posX + s.width / 2;
                vertices[i * 36 + 10] = s.posY - s.height / 2;
                vertices[i * 36 + 11] = 1.0f;
                             
                vertices[i * 36 + 12] = 1.0f;
                vertices[i * 36 + 13] = 0.0f;
                             
                vertices[i * 36 + 14] = 1.0f;
                vertices[i * 36 + 15] = 1.0f;
                vertices[i * 36 + 16] = 1.0f;
                vertices[i * 36 + 17] = 1.0f;
                             
                             
                vertices[i * 36 + 18] = s.posX - s.width / 2;
                vertices[i * 36 + 19] = s.posY - s.height / 2;
                vertices[i * 36 + 20] = 1.0f;
                             
                vertices[i * 36 + 21] = 0.0f;
                vertices[i * 36 + 22] = 0.0f;
                             
                vertices[i * 36 + 23] = 1.0f;
                vertices[i * 36 + 24] = 1.0f;
                vertices[i * 36 + 25] = 1.0f;
                vertices[i * 36 + 26] = 1.0f;
                             
                             
                vertices[i * 36 + 27] = s.posX - s.width / 2;
                vertices[i * 36 + 28] = s.posY + s.height / 2;
                vertices[i * 36 + 29] = 1.0f;
                             
                vertices[i * 36 + 30] = 0.0f;
                vertices[i * 36 + 31] = 1.0f;
                             
                vertices[i * 36 + 32] = 1.0f;
                vertices[i * 36 + 33] = 1.0f;
                vertices[i * 36 + 34] = 1.0f;
                vertices[i * 36 + 35] = 1.0f;
            }
            return vertices;
        }

        private Shader _shader;

        private int _vertexArrayObject, _vertexBufferObject, _elementBufferObject;

        private int _maxQuadCount, _maxVertexCount, _maxIndicesCount;

        private List<Sprite> _drawList;


        public SpriteRenderer(Shader shader)
        {
            _shader = shader;
            initRenderData();
        }

        public void Begin()
        {
            _drawList.Clear();
        }

        public void End()
        {
            Flush();
            _drawList.Clear();
        }

        public void Flush()
        {
            Window.drawCalls += 1;

            _vertices = GetVertices(_drawList);

            GL.ActiveTexture(TextureUnit.Texture0);
            Window.textures.GetTexture("text").Use(TextureUnit.Texture0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, _vertices.Length * sizeof(float), _vertices);

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
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

            Window.spritesDrawn += 1;

            _drawList.Add(new Sprite(texture, size.X, size.Y, position.X, position.Y, rotation, color));

            if (_drawList.Count > _maxQuadCount)
            {
                Flush();
                Begin();
            }
            
        }

        private void initRenderData()
        {
            _maxQuadCount = 1000;
            _maxVertexCount = _maxQuadCount * 4;
            _maxIndicesCount = _maxQuadCount * 6;

            _drawList = new List<Sprite>();

            _indices = GetIndices();

            _vertexArrayObject = GL.GenVertexArray();
            _vertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _maxVertexCount * 9 * sizeof(float), IntPtr.Zero, BufferUsageHint.StaticDraw);

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

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public uint[] GetIndices()
        {
            uint offset = 0;
            uint[] indices = new uint[_maxIndicesCount];
            for (int i = 0; i < _maxIndicesCount; i += 6)
            {
                indices[i + 0] = 0 + offset;
                indices[i + 1] = 1 + offset;
                indices[i + 2] = 2 + offset;
                indices[i + 3] = 0 + offset;
                indices[i + 4] = 2 + offset;
                indices[i + 5] = 3 + offset;

                offset += 4;
            }

            return indices;
        }
    }
}
