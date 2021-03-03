using System;
using System.Collections.Generic;
using System.Text;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Kee5Engine
{

    public struct Vertex
    {
        public const int Size = (4 + 4) * 4;

        private readonly Vector4 _position;
        private readonly Color4 _color;

        public Vertex(Vector4 position, Color4 color)
        {
            _position = position;
            _color = color;
        }
    }

    // Creates VBO and attrib pointers from sprites
    public class SpriteBatch
    {

        private List<Sprite> spriteBatch;

        private bool _initialized;
        private int _vertexBufferObject;

        public SpriteBatch()
        {
            spriteBatch = new List<Sprite>();
        }

        public void Add(Sprite sprite)
        {
            spriteBatch.Add(sprite);
        }

        public void Draw()
        {
            //    float[] vertices = new float[20 * spriteBatch.Count];
            //    // Create Vertices array
            //    for (int i = 0; i < spriteBatch.Count; i++)
            //    {
            //        Sprite sprite = spriteBatch[i];
            //    }

            //    // Create vbo
            //    _vertexBufferObject = GL.GenBuffer();

            //    // Bind the buffer
            //    GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            //    // Upload the vertices to the buffer
            //    GL.BufferData(
            //        BufferTarget.ArrayBuffer,                                   // Which buffer the data should be sent to
            //        _vertices.Length * sizeof(float),                           // How much data is being sent, in bytes
            //        _vertices,                                                  // The vertices that are sent
            //        BufferUsageHint.StaticDraw                                  // Bufferusage (Static, Dynamic, Stream)
            //        );

            //    // Add the vertices and tex coords of each sprite to a VBO
            //    foreach (Sprite sprite in spriteBatch)
            //    {

            //    }
            //}
        }
    }
}
