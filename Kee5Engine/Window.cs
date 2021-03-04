using System;
using System.Collections.Generic;
using System.Text;

using OpenTK;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

using Kee5Engine.IO;
using Kee5Engine.Shaders;

namespace Kee5Engine
{
    public class Window : GameWindow
    {
        // (0, 0) is the center of the screen
        private readonly float[] _vertices =
        {
            // Position         Texture coordinates
            -1f, -1f, 0.0f, 1.0f, 1.0f, // Bottom-left vertex
             1f, -1f, 0.0f, 1.0f, 0.0f, // Bottom-right vertex
             1f,  1f, 0.0f, 0.0f, 0.0f, // Top-right vertex
            -1f,  1f, 0.0f, 0.0f, 1.0f  // Top-left vertex
        };

        private readonly uint[] _indices =
        {
            0, 1, 3, // Triangle 1 
            1, 2, 3  // Triangle 2
        };

        private int _vertexBufferObject;

        private int _vertexArrayObject;

        private Shader _shader;

        private Texture _texture;

        public static TextureList textures;

        private int _elementBufferObject;

        private double _time;

        public static Camera camera;

        private bool _firstMove = true;

        private Vector2 _lastPost;

        public SpriteRenderer spriteRenderer;

        public static InputHandler inputHandler;
        public static float screenScaleX, screenScaleY;


        public Window(int width, int height, string title) : base(
            new GameWindowSettings { RenderFrequency = 60, UpdateFrequency = 60 },
            new NativeWindowSettings { Size = new Vector2i(width, height), Title = title })
        {
            // Create InputHandler
            inputHandler = new InputHandler();

            textures = new TextureList();

            // Determine Screen Scale
            screenScaleX = width / 1920;
            screenScaleY = height / 1080;
            Globals.windowSize = Size;
        }

        // Initialize OpenGL
        protected override void OnLoad()
        {
            // Set the background colour after we clear it
            GL.ClearColor(0.05f, 0.05f, 0.05f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            // Create vbo
            _vertexBufferObject = GL.GenBuffer();

            // Bind the buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            // Upload the vertices to the buffer
            GL.BufferData(
                BufferTarget.ArrayBuffer,               // Which buffer the data should be sent to
                _vertices.Length * sizeof(float),       // How much data is being sent, in bytes
                _vertices,                              // The vertices that are sent
                BufferUsageHint.StaticDraw              // Bufferusage (Static, Dynamic, Stream)
                );

            // To let OpenGL know how to use the gibberish mess of bytes that is a vbo, we use a vao (VertexArrayObject)

            // Generate and bind a vao
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);


            // Create EBO (element buffer object)
            _elementBufferObject = GL.GenBuffer();

            // Bind EBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);

            // Upload data to the EBO
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            // Create the shaders
            _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            // Enable the shader
            _shader.Use();

            spriteRenderer = new SpriteRenderer(_shader);

            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);

            // Setup how the vertex shader interprets the VBO data
            GL.VertexAttribPointer(
                vertexLocation,                     // Location of the input variable in the shader (The layout(location = x) line)
                3,                                  // How many elements will be sent
                VertexAttribPointerType.Float,      // The data type of the elements
                false,                              // Whether or not the data should be converted to normalized device coordinates
                5 * sizeof(float),                  // How many bytes are between the last element of one vertex, and the first element of the next
                0                                   // How many bytes to skip to find the first element of the first vertex
                );

            // Setup texture coordinates
            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            
            // Skip 3 * sizeof(float) bytes as the texture coordinates come after the position coordinates
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            textures.LoadTexture("Sprites/Test/Test.png", "Test");
            textures.GetTexture("Test").Use(TextureUnit.Texture0);

            // Initiate the camera
            camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y, 1.5f, 0.2f);

            CursorGrabbed = true;

            base.OnLoad();
        }

        // Create Render loop
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            Title = $"Game | FPS: {Math.Round(1 / args.Time)}";
            // Clear the image
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Bind the VAO
            // Because the EBO is a proberty of the currently bound VAO
            // the EBO will change with the VAO
            GL.BindVertexArray(_vertexArrayObject);

            // Bind the shader
            _shader.Use();
            _shader.SetMatrix4("view", camera.GetViewMatrix());
            _shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            spriteRenderer.DrawSprite(textures.GetTexture("Test"), new Vector2(-0.5f, 0.5f), new Vector2(0.2f, 0.2f), 0f, new Vector4(1, 1, 1, 1));
            spriteRenderer.DrawSprite(textures.GetTexture("Test"), new Vector2(0.5f, 0.5f), new Vector2(0.2f, 0.2f), 0f, new Vector4(1, 1, 1, 1));

            spriteRenderer.DrawSprite(textures.GetTexture("Test"), new Vector2(0f, -0.5f), new Vector2(0.7f, 0.2f), 0f, new Vector4(1, 1, 1, 1));
            spriteRenderer.DrawSprite(textures.GetTexture("Test"), new Vector2(0.5f, -0.1f), new Vector2(0.2f, 0.2f), 0f, new Vector4(1, 1, 1, 1));
            spriteRenderer.DrawSprite(textures.GetTexture("Test"), new Vector2(-0.5f, -0.1f), new Vector2(0.2f, 0.2f), 0f, new Vector4(1, 1, 1, 1));

            // Call Drawing function
            GL.DrawElements(
                PrimitiveType.Triangles,        // Primitive type
                _indices.Length,                // How many indices should be drawn
                DrawElementsType.UnsignedInt,   // Data type of the indices
                0                               // Offset in the EBO
                );

            // Swap the buffers to render on screen
            SwapBuffers();

            base.OnRenderFrame(args);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (!IsFocused)
            {
                return;
            }

            // Update the InputHandler
            inputHandler.Update(KeyboardState);

            camera.Update(args.Time);

            // Check if the Escape button is pressed
            if (inputHandler.IsKeyDown(Keys.Escape))
            {
                // Close the window
                Close();
            }

            base.OnUpdateFrame(args);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            // Call GL.viewport to resize OpenGL's viewport to match the new size
            GL.Viewport(0, 0, Size.X, Size.Y);
            camera.AspectRatio = Size.X / (float)Size.Y;
            base.OnResize(e);
        }

        protected override void OnUnload()
        {
            // Unload all the resources
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all the resources
            GL.DeleteBuffer(_vertexArrayObject);
            GL.DeleteVertexArray(_vertexArrayObject);

            GL.DeleteProgram(_shader.Handle);
            textures.UnLoad();

            base.OnUnload();
        }
    }
}
