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

        public float[] GetVertices(SortedList<int, Sprite> drawList)
        {
            int SpriteCount = drawList.Count;

            float[] vertices = new float[SpriteCount * 40];

            Random rng = new Random();

            for (int i = 0; i < SpriteCount; i++)
            {
                Sprite s = drawList.Values[i];
                vertices[i * 40 + 0] = s.posX + s.width / 2;
                vertices[i * 40 + 1] = s.posY + s.height / 2;
                vertices[i * 40 + 2] = s.posZ;
                           
                vertices[i * 40 + 3] = 1.0f;
                vertices[i * 40 + 4] = 1.0f;
                             
                vertices[i * 40 + 5] = s.color[0];
                vertices[i * 40 + 6] = s.color[1];
                vertices[i * 40 + 7] = s.color[2];
                vertices[i * 40 + 8] = s.color[3];

                vertices[i * 40 + 9] = drawList.Keys[i];
                             
                             
                vertices[i * 40 + 10]  = s.posX + s.width / 2;
                vertices[i * 40 + 11] = s.posY - s.height / 2;
                vertices[i * 40 + 12] = s.posZ;
                            
                vertices[i * 40 + 13] = 1.0f;
                vertices[i * 40 + 14] = 0.0f;
                            
                vertices[i * 40 + 15] = s.color[0];
                vertices[i * 40 + 16] = s.color[1];
                vertices[i * 40 + 17] = s.color[2];
                vertices[i * 40 + 18] = s.color[3];

                vertices[i * 40 + 19] = drawList.Keys[i];


                vertices[i * 40 + 20] = s.posX - s.width / 2;
                vertices[i * 40 + 21] = s.posY - s.height / 2;
                vertices[i * 40 + 22] = s.posZ;
                             
                vertices[i * 40 + 23] = 0.0f;
                vertices[i * 40 + 24] = 0.0f;
                             
                vertices[i * 40 + 25] = s.color[0];
                vertices[i * 40 + 26] = s.color[1];
                vertices[i * 40 + 27] = s.color[2];
                vertices[i * 40 + 28] = s.color[3];

                vertices[i * 40 + 29] = drawList.Keys[i];


                vertices[i * 40 + 30] = s.posX - s.width / 2;
                vertices[i * 40 + 31] = s.posY + s.height / 2;
                vertices[i * 40 + 32] = s.posZ;
                             
                vertices[i * 40 + 33] = 0.0f;
                vertices[i * 40 + 34] = 1.0f;
                             
                vertices[i * 40 + 35] = s.color[0];
                vertices[i * 40 + 36] = s.color[1];
                vertices[i * 40 + 37] = s.color[2];
                vertices[i * 40 + 38] = s.color[3];

                vertices[i * 40 + 39] = drawList.Keys[i];
            }
            return vertices;
        }

        private Shader _shader;

        private int _vertexArrayObject, _vertexBufferObject, _elementBufferObject;

        private int _maxQuadCount, _maxVertexCount, _maxIndicesCount, _maxTextureCount;

        private List<Texture> _texList;

        private SortedList<int, Sprite> _drawList;


        public SpriteRenderer(Shader shader)
        {
            _shader = shader;
            initRenderData();
        }

        public void Begin()
        {
            _drawList.Clear();
            _texList.Clear();
        }

        public void End()
        {
            Flush();
            _drawList.Clear();
            _texList.Clear();
        }

        public void Flush()
        {
            Window.drawCalls += 1;

            _vertices = GetVertices(_drawList);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            int[] handles = new int[_texList.Count];

            for (int i = 0; i < _texList.Count; i++)
            {
                handles[i] = _texList[i].Handle;
            }

            GL.BindTextures(0, _texList.Count, handles);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, _vertices.Length * sizeof(float), _vertices);

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public void DrawSprite(Texture texture, Vector2 position, Vector2 size, float layer, float rotation, Vector4 color)
        {
            if (position.X - Window.camera.Position.X > Window.WindowSize.X + 100 + size.X / 2
                || position.Y - Window.camera.Position.Y > Window.WindowSize.Y + 100 + size.Y / 2
                || position.X - Window.camera.Position.X < -100 - size.X / 2
                || position.Y - Window.camera.Position.Y < -100 - size.Y / 2)
            {
                return;
            }

            Window.spritesDrawn += 1;
            
            if (!_texList.Contains(texture))
            {
                _texList.Add(texture);
            }

            _drawList.Add(_texList.Count - 1, new Sprite(texture, size.X, size.Y, position.X, position.Y, layer, rotation, color));

            if (_drawList.Count > _maxQuadCount || _texList.Count > _maxTextureCount - 1)
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

            _maxTextureCount = GL.GetInteger(GetPName.MaxTextureImageUnits);

            _drawList = new SortedList<int, Sprite>();
            _texList = new List<Texture>();

            int[] samplers = new int[_maxTextureCount];

            for (int i = 0; i < _maxTextureCount; i++)
            {
                samplers[i] = i;
            }

            _shader.SetIntArray("uTextures", samplers);

            _indices = GetIndices();

            _vertexArrayObject = GL.GenVertexArray();
            _vertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _maxVertexCount * 10 * sizeof(float), IntPtr.Zero, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(_vertexArrayObject);

            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 10 * sizeof(float), 0);

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 10 * sizeof(float), 3 * sizeof(float));

            var colorLocation = _shader.GetAttribLocation("aColor");
            GL.EnableVertexAttribArray(colorLocation);
            GL.VertexAttribPointer(colorLocation, 4, VertexAttribPointerType.Float, false, 10 * sizeof(float), 5 * sizeof(float));

            var texIDLocation = _shader.GetAttribLocation("aTexID");
            GL.EnableVertexAttribArray(texIDLocation);
            GL.VertexAttribPointer(texIDLocation, 1, VertexAttribPointerType.Float, false, 10 * sizeof(float), 9 * sizeof(float));

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
