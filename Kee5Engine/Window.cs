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
using Kee5Engine.Audio;

namespace Kee5Engine
{
    public class Window : GameWindow
    {
        public static double timeElapsed = 0;

        private Shader _shader;
        public static TextureList textures;
        public static Camera camera;
        public static SpriteRenderer spriteRenderer;
        public static InputHandler inputHandler;
        public static float screenScaleX, screenScaleY;
        public static int drawCalls = 0, spritesDrawn = 0;
        public static Vector2 WindowSize { get; private set; }
        public static TextRenderer2D textRenderer;
        private Sprite _player, _backGround;

        /// <summary>
        /// Create an OpenGL GameWindow
        /// </summary>
        /// <param name="width">Width of the Window</param>
        /// <param name="height">Height of the Window</param>
        /// <param name="title">Title of the window</param>
        public Window(int width, int height, string title) : base(
            new GameWindowSettings { RenderFrequency = 60, UpdateFrequency = 60 },
            new NativeWindowSettings { Size = new Vector2i(width, height), Title = title })
        {
            // Create InputHandler
            inputHandler = new InputHandler();

            // Create texture List. All textures are loaded in here
            textures = new TextureList();

            WindowSize = Size;

            // Determine Screen Scale
            screenScaleX = width / 1920;
            screenScaleY = height / 1080;
            Globals.windowSize = Size;
        }

        /// <summary>
        /// Is called when the Window is loaded
        /// </summary>
        protected override void OnLoad()
        {
            // Set the background colour after we clear it
            GL.ClearColor(0.05f, 0.05f, 0.05f, 1f);

            // Enable DepthTest, if the depth of fragments is too deep, they are discarded
            GL.Enable(EnableCap.DepthTest);

            // Initiate Text Renderer
            textRenderer = new TextRenderer2D();

            // Set TextRenderer Font
            // This needs to be called every time you want to change fonts (including italics and bold)
            textRenderer.SetFont("Fonts/arial.ttf");

            // Set Font Size, also needs to be called every time you want to change it
            textRenderer.SetSize(128);

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

            // Create 2 test Sprites, 1 animated one, 1 non animated one
            _player = new Sprite(textures.GetTexture("PlayerIdle"), 128, 128, 960, 540, 1f, 0, Vector4.One, 4, 0.1);
            _backGround = new Sprite(textures.GetTexture("Test"), 1920, 1080, 960, 540, -1f, 0, Vector4.One);

            // Create a test button
            Globals.activeButtons.Add(new Button(200, 980, 400, 200, 2, "Test", "Button", new Vector4(0.2f, 0, 0, 1), Vector3.One, TextAlignment.CENTER, true, () => { Console.WriteLine("Clicked!"); }));

            base.OnLoad();
        }

        /// <summary>
        /// Is called on every render frame
        /// </summary>
        /// <param name="args">FrameEventArgs mainly keep track of the time between render frames</param>
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            // Reset Debug Data
            drawCalls = 0;
            spritesDrawn = 0;
            
            // Clear the image
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Begin the Sprite Batch
            spriteRenderer.Begin();

            // Bind the shader
            _shader.Use();
            _shader.SetMatrix4("view", camera.GetViewMatrix());
            _shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            // Draw Sprites:
            _backGround.Draw();
            _player.Draw();

            // Call Globals' draw method. This Draws all active buttons
            Globals.Draw();

            // End the spriteBatch and Flush all remaining data to the window buffer
            spriteRenderer.End();

            // Set the title to reflect debug data
            Title = $"Game | FPS: {Math.Round(1 / args.Time)} | Draw Calls: {drawCalls} | Sprites Drawn: {spritesDrawn}";

            // Swap the buffers to render on screen
            SwapBuffers();

            base.OnRenderFrame(args);
        }

        /// <summary>
        /// Is called every update frame
        /// </summary>
        /// <param name="args">FrameEventArgs mainly keeps track of the time between update frames</param>
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            // Increment the total elapsed time
            timeElapsed += args.Time;

            // Return if the window isn't focussed
            // If running with break points, disable this, or this will always return
            if (!IsFocused)
            {
                return;
            }

            // Update the InputHandler
            inputHandler.Update(KeyboardState.GetSnapshot(), MouseState.GetSnapshot());

            // Update the Camera
            camera.Update(args.Time);

            // Check if the Escape button is pressed
            if (inputHandler.IsKeyDown(Keys.Escape))
            {
                // Close the window
                Close();
            }

            // Update a Sprite
            _player.Update(args.Time);

            // Call Globals' update, this updates the active Buttons as well as the AudioManager
            Globals.Update();

            base.OnUpdateFrame(args);
        }

        /// <summary>
        /// Called whenever the mousebutton is clicked
        /// </summary>
        /// <param name="e">Information about the click</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            for (int i = Globals.activeButtons.Count - 1; i >= 0; i--)
            {
                if (Globals.activeButtons[i].IsInButton(MousePosition.X / screenScaleX, MousePosition.Y / screenScaleY))
                {
                    Globals.activeButtons[i].OnClick();
                }
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Called whenever the window is resized
        /// </summary>
        /// <param name="e">Information about the new window</param>
        protected override void OnResize(ResizeEventArgs e)
        {
            // Call GL.viewport to resize OpenGL's viewport to match the new size
            GL.Viewport(0, 0, Size.X, Size.Y);
            camera.AspectRatio = Size.X / (float)Size.Y;
            base.OnResize(e);
        }

        /// <summary>
        /// Called whenever the window is Quit.
        /// Unloads and deletes all resources
        /// </summary>
        protected override void OnUnload()
        {
            // Unload all the resources
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all the resources
            spriteRenderer.UnLoad();

            GL.DeleteProgram(_shader.Handle);
            textures.UnLoad();

            foreach (Button button in Globals.activeButtons)
            {
                button.UnLoad();
            }

            Console.WriteLine($"{Globals.unloaded} textures unloaded");

            base.OnUnload();
        }
    }
}
