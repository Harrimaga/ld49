using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Kee5Engine.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Kee5Engine
{
    // Struct memory should be in the same order as is written down
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex
    {
        float posX, posY, posZ, texX, texY, R, G, B, A;
        int texId;
        public Vertex(float posX, float posY, float posZ, float texX, float texY, float r, float g, float b, float a, int texId)
        {
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;
            this.texX = texX;
            this.texY = texY;
            R = r;
            G = g;
            B = b;
            A = a;
            this.texId = texId;
        }
    }
    public class SpriteRenderer
    {

        private Vertex[] _vertices =
        {
           
        };

        private uint[] _indices =
        {
            0, 1, 2,
            0, 2, 3,
            4, 5, 6,
            4, 6, 7
        };

        /// <summary>
        /// Get the Vertices array from the drawlist
        /// </summary>
        /// <param name="drawList">List of sprites to be drawn this batch</param>
        /// <returns><code>float[]</code> of vertices</returns>
        public Vertex[] GetVertices(List<Sprite> drawList)
        {
            int SpriteCount = drawList.Count;
            Vertex[] vertices = new Vertex[SpriteCount * 4];

            // For each Sprite, get the 40 required vertices and put them in the array
            for (int i = 0; i < SpriteCount; i++)
            {
                Sprite s = drawList[i];

                Matrix3 rotation = Matrix3.CreateRotationZ(-s.rotation);


                Vector3 p1 = new Vector3(s.posX + s.width / 2, s.posY + s.height / 2, 1);
                Vector3 p2 = new Vector3(s.posX + s.width / 2, s.posY - s.height / 2, 1);
                Vector3 p3 = new Vector3(s.posX - s.width / 2, s.posY - s.height / 2, 1);
                Vector3 p4 = new Vector3(s.posX - s.width / 2, s.posY + s.height / 2, 1);

                Vector3 center = new Vector3(s.posX, s.posY, 1);

                p1 = rotation * (p1 - center) + center;
                p2 = rotation * (p2 - center) + center;
                p3 = rotation * (p3 - center) + center;
                p4 = rotation * (p4 - center) + center;

                vertices[i * 4 + 0] = new Vertex(p1.X, p1.Y, s.posZ, s.texX + (float)s.currentFrame / (float)s.frames, 1.0f,
                                                s.color[0], s.color[1], s.color[2], s.color[3], s.texID);
                vertices[i * 4 + 1] = new Vertex(p2.X, p2.Y, s.posZ, s.texX + (float)s.currentFrame / (float)s.frames, 0.0f,
                                                s.color[0], s.color[1], s.color[2], s.color[3], s.texID);
                vertices[i * 4 + 2] = new Vertex(p3.X, p3.Y, s.posZ, (float)s.currentFrame / (float)s.frames, 0.0f,
                                                s.color[0], s.color[1], s.color[2], s.color[3], s.texID);
                vertices[i * 4 + 3] = new Vertex(p4.X, p4.Y, s.posZ, (float)s.currentFrame / (float)s.frames, 1.0f,
                                                s.color[0], s.color[1], s.color[2], s.color[3], s.texID);
            }
            return vertices;
        }

        private Shader _shader;

        private int _vertexArrayObject, _vertexBufferObject, _elementBufferObject;

        private int _maxQuadCount, _maxVertexCount, _maxIndicesCount, _maxTextureCount;

        private List<Texture> _texList;

        private List<Sprite> _drawList;

        /// <summary>
        /// Create a new SpriteRenderer
        /// </summary>
        /// <param name="shader">The Shader that is used</param>
        public SpriteRenderer(Shader shader)
        {
            _shader = shader;
            initRenderData();
        }

        /// <summary>
        /// Begin spritebatch
        /// </summary>
        public void Begin()
        {
            _drawList.Clear();
            _texList.Clear();
        }

        /// <summary>
        /// End spritebatch. This calls Flush first.
        /// </summary>
        public void End()
        {
            Flush();
            _drawList.Clear();
            _texList.Clear();
        }

        /// <summary>
        /// Flush the spritebatch to the window buffer
        /// </summary>
        public void Flush()
        {
            // Increment debug data
            Window.drawCalls += 1;

            // Get the vertices of the current drawlist
            _vertices = GetVertices(_drawList);

            // Enable transparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Get the handles for all textures in the batch
            int[] handles = new int[_texList.Count];

            for (int i = 0; i < _texList.Count; i++)
            {
                handles[i] = _texList[i].Handle;
            }

            // Bind the textures to the buffer
            GL.BindTextures(0, _texList.Count, handles);

            // Bind the vertex data to the buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, _vertices.Length * (9 * sizeof(float) + sizeof(int)), _vertices);

            // Bind the vertex array
            GL.BindVertexArray(_vertexArrayObject);

            // Draw the quads in the batch
            GL.DrawElements(PrimitiveType.Triangles, _drawList.Count * 6, DrawElementsType.UnsignedInt, 0);

            // Unbind the buffers
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Draw a Sprite
        /// </summary>
        /// <param name="sprite">The sprite to be drawn</param>
        public void DrawSprite(Sprite sprite)
        {
            // If the sprite is too far out of the viewport, don't draw it
            if (sprite.posX - Window.camera.Position.X > Window.WindowSize.X + 100 + sprite.width / 2
                || sprite.posY - Window.camera.Position.Y > Window.WindowSize.Y + 100 + sprite.height / 2
                || sprite.posX - Window.camera.Position.X < -100 - sprite.width / 2
                || sprite.posY - Window.camera.Position.Y < -100 - sprite.height / 2)
            {
                return;
            }

            // Increment debug data
            Window.spritesDrawn += 1;

            // If the texture is not yet in the texture list, add it
            if (!_texList.Contains(sprite.texture))
            {
                _texList.Add(sprite.texture);
                sprite.texID = _texList.Count - 1;
            }
            
            // If it is not, let the sprite know which texture to use
            else
            {
                for (int i = 0; i < _texList.Count; i++)
                {
                    if (_texList[i] == sprite.texture)
                    {
                        sprite.texID = i;
                        break;
                    }
                }
            }

            // Add the sprite to the drawlist
            _drawList.Add(sprite);

            // If the drawlist contains more quads than the max quad count,
            // or the maximum amount of textures are used,
            // Flush the batch and start a new one
            if (_drawList.Count > _maxQuadCount - 1|| _texList.Count > _maxTextureCount - 1)
            {
                Flush();
                Begin();
            }
        }

        /// <summary>
        /// Draw a Sprite, without making a sprite first
        /// </summary>
        /// <param name="texture">Texture</param>
        /// <param name="position">Position (center-origin)</param>
        /// <param name="size">Size</param>
        /// <param name="layer">Drawing layer</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="color">Sprite color</param>
        public void DrawSprite(Texture texture, Vector2 position, Vector2 size, float layer, float rotation, Vector4 color)
        {

            DrawSprite(new Sprite(texture, size.X, size.Y, position.X, position.Y, layer, rotation, color, 1, (float)_texList.Count - 1));
        }

        /// <summary>
        /// Initialize the Buffers and Lists
        /// </summary>
        private void initRenderData()
        {
            _maxQuadCount = 1000;
            _maxVertexCount = _maxQuadCount * 4;
            _maxIndicesCount = _maxQuadCount * 6;

            // Get the maximum textures for this GPU
            _maxTextureCount = GL.GetInteger(GetPName.MaxTextureImageUnits);

            _drawList = new List<Sprite>();
            _texList = new List<Texture>();

            // Create the samplers array
            int[] samplers = new int[_maxTextureCount];

            for (int i = 0; i < _maxTextureCount; i++)
            {
                samplers[i] = i;
            }

            _shader.SetIntArray("uTextures", samplers);

            // Get the indices array
            _indices = GetIndices();

            // Create the Buffers
            _vertexArrayObject = GL.GenVertexArray();
            _vertexBufferObject = GL.GenBuffer();

            // Bind the VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            // Allocate memory for the buffer
            GL.BufferData(BufferTarget.ArrayBuffer, _maxVertexCount * 40 * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            // Bind the VAO
            GL.BindVertexArray(_vertexArrayObject);

            // Let the shader know how to read the VBO
            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float) + sizeof(int), 0);

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 9 * sizeof(float) + sizeof(int), 3 * sizeof(float));

            var colorLocation = _shader.GetAttribLocation("aColor");
            GL.EnableVertexAttribArray(colorLocation);
            GL.VertexAttribPointer(colorLocation, 4, VertexAttribPointerType.Float, false, 9 * sizeof(float) + sizeof(int), 5 * sizeof(float));

            var texIDLocation = _shader.GetAttribLocation("aTexID");
            GL.EnableVertexAttribArray(texIDLocation);
            GL.VertexAttribIPointer(texIDLocation, 1, VertexAttribIntegerType.Int, 9 * sizeof(float) + sizeof(int), (IntPtr)(9 * sizeof(float)));

            // Create the EBO
            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            // Load the indices array to the EBO
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            // Unbind the buffers
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Get the indices for the triangles in the quad
        /// </summary>
        /// <returns><code>uint[]</code> of indices (0, 1, 2, 0, 2, 3)</returns>
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

        /// <summary>
        /// Unload the buffers
        /// </summary>
        public void UnLoad()
        {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
            GL.DeleteBuffer(_elementBufferObject);
        }
    }
}
