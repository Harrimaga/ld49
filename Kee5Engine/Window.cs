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

        public static double timeElapsed = 0;

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

        public static SpriteRenderer spriteRenderer;

        public static InputHandler inputHandler;
        public static float screenScaleX, screenScaleY;

        public static int drawCalls = 0, spritesDrawn = 0;

        public static Vector2 WindowSize { get; private set; }

        public static TextRenderer2D textRenderer;

        public Window(int width, int height, string title) : base(
            new GameWindowSettings { RenderFrequency = 60, UpdateFrequency = 60 },
            new NativeWindowSettings { Size = new Vector2i(width, height), Title = title })
        {
            // Create InputHandler
            inputHandler = new InputHandler();

            textures = new TextureList();

            WindowSize = Size;

            // Determine Screen Scale
            screenScaleX = width / 1920;
            screenScaleY = height / 1080;
            Globals.windowSize = Size;
        }

        // Initialize OpenGL
        protected override void OnLoad()
        {
            // Set the background colour after we clear it
            GL.ClearColor(0.05f, 0.05f, 0.05f, 1f);

            GL.Enable(EnableCap.DepthTest);

            textRenderer = new TextRenderer2D();
            textRenderer.SetFont("Fonts/arial.ttf");
            textRenderer.SetSize(128);
            System.Drawing.Bitmap Text = textRenderer.RenderString("Hello", System.Drawing.Color.White, System.Drawing.Color.Transparent);
            textures.LoadTexture(Text, "text");

            textRenderer.SetFont("Fonts/ariali.ttf");
            textRenderer.SetSize(64);
            System.Drawing.Bitmap text2 = textRenderer.RenderString("This is some text", System.Drawing.Color.Black, System.Drawing.Color.Transparent);
            textures.LoadTexture(text2, "text2");

            // Create the shaders
            _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            // Enable the shader
            _shader.Use();

            // Create the sprite renderer
            spriteRenderer = new SpriteRenderer(_shader);

            // Initiate the camera
            camera = new Camera(new Vector3(0, 0, 10f), Size.X / (float)Size.Y, 100f, 0.2f);

            // Remove mouse from screen :)
            CursorGrabbed = false;

            Globals.activeButtons.Add(new Button(960, 540, 400, 200, 2, "Test", "Button", new Vector4(0.2f, 0, 0, 1), new Vector3(1, 1, 1), true, () => { Console.WriteLine("Clicked!"); }));


            base.OnLoad();
        }

        // Create Render loop
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            drawCalls = 0;
            spritesDrawn = 0;
            
            // Clear the image
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            spriteRenderer.Begin();

            // Bind the shader
            _shader.Use();
            _shader.SetMatrix4("view", camera.GetViewMatrix());
            _shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            // Draw a Sprite:
            // Window.spriteRenderer is static, so draws can be made anywhere
            //spriteRenderer.DrawSprite(
            //    textures.GetTexture("Test"),        // Texture
            //    new Vector2(1920 / 2, 1080 / 2),    // Position (center-origin)
            //    new Vector2(1920f, 1080f),          // Size
            //    1f,                                 // Layer
            //    0f,                                 // Rotation
            //    new Vector4(1, 1, 1, 1)             // Colour (r, g, b, a)
            //    );

            //spriteRenderer.DrawSprite(
            //    textures.GetTexture("text"),
            //    new Vector2(960, 540),
            //    textures.GetTexture("text").Size,
            //    2f,
            //    0f,
            //    new Vector4(1, 0, 0, 1)
            //    );

            //spriteRenderer.DrawSprite(
            //    textures.GetTexture("text2"),
            //    new Vector2(960, 700),
            //    textures.GetTexture("text2").Size,
            //    2f,
            //    0f,
            //    new Vector4(1, 1, 1, 1)
            //    );

            Globals.Draw();

            spriteRenderer.End();

            Title = $"Game | FPS: {Math.Round(1 / args.Time)} | Draw Calls: {drawCalls} | Sprites Drawn: {spritesDrawn}";

            // Swap the buffers to render on screen
            SwapBuffers();

            base.OnRenderFrame(args);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            timeElapsed += args.Time;

            if (!IsFocused)
            {
                return;
            }

            // Update the InputHandler
            inputHandler.Update(KeyboardState, MouseState);

            camera.Update(args.Time);

            // Check if the Escape button is pressed
            if (inputHandler.IsKeyDown(Keys.Escape))
            {
                // Close the window
                Close();
            }

            Globals.Update();

            base.OnUpdateFrame(args);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            foreach (Button button in Globals.activeButtons)
            {
                if (button.IsInButton(MousePosition.X, MousePosition.Y))
                {
                    button.OnClick();
                }
            }
            base.OnMouseDown(e);
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
