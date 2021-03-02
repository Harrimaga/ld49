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
        private static InputHandler inputHandler;
        public static DrawList drawList = new DrawList();
        public static List<Sprite> spriteList = new List<Sprite>();
        public static float screenScaleX, screenScaleY;

        private MainShader mainShader;
        private List<Shader> shaders = new List<Shader>();
        private List<bool> enabledShaders = new List<bool>();

        // Test List of Textures
        public static List<Texture2D> texs = new List<Texture2D>();

        private int numOfShaders = 0, frameBuffer0 = -1, frameBuffer1 = -1;
        private long frameBufferTexture0 = -1, frameBufferTexture1 = -1;

        // Test Sprite
        private Sprite testSprite;

        public Window(int width, int height, string title) : base(
            new GameWindowSettings { RenderFrequency = 60, UpdateFrequency = 60 },
            new NativeWindowSettings { Size = new Vector2i(width, height), Title = title})
        {
            // Create InputHandler
            inputHandler = new InputHandler();

            // Determine Screen Scale
            screenScaleX = width / 1920;
            screenScaleY = height / 1080;
            Globals.windowSize = Size;
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0.05f, 0.05f, 0.05f, 1.0f);


            // Loading the Shaders
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            string s = GL.GetString(StringName.Vendor);
            mainShader = new MainShader(s.Equals("ATI Technologies Inc."));
            AddShader(mainShader);            

            // Create Frame Buffers
            frameBuffer0 = createFrameBuffer(FramebufferAttachment.ColorAttachment0, ref frameBufferTexture0);
            frameBuffer1 = createFrameBuffer(FramebufferAttachment.ColorAttachment1, ref frameBufferTexture1);

            GL.DrawBuffers(2, new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1 });

            texs.Add(new Texture2D("Sprites/Test/Test.png", 1920, 1080, 1920, 1080));
            testSprite = new Sprite(1920, 1080, 0, texs[0] );

            base.OnLoad();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            Size = e.Size;
            GL.Viewport(0, 0, Size.X, Size.Y);
            screenScaleX = Size.X / 1920.0f;
            screenScaleY = Size.Y / 1080.0f;

            Globals.windowSize = Size;

            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            // Will be executed every update tick
            // Updates are every 1/UpdateFrequency (60) seconds

            // Update the inputHandler first
            inputHandler.Update(KeyboardState);

            // Quit the Game
            if (inputHandler.IsKeyDown(Keys.Escape))
            {
                Close();
            }


            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            // Will be executed every Render Frame
            // Render frames are every 1/RenderFrequency (60) seconds

            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Draw test Sprite
            testSprite.Draw(0, 0);

            //foreach (Sprite sprite in spriteList)
            //{
            //    sprite.MakeImageHandleResident();
            //}

            foreach (Texture2D texture in texs)
            {
                GL.Arb.MakeImageHandleResident(texture.Handle, All.ReadOnly);
            }

            GL.Viewport(0, 0, Size.X, Size.Y);

            int shadersPassed = 0;
            long prev = -1;

            for (int i = 0; i < shaders.Count; i++)
            {
                if (enabledShaders[i])
                {
                    shadersPassed++;
                    int frameBuffer;
                    long frameBufferTexture;

                    if (shadersPassed %2 == 0)
                    {
                        frameBuffer = frameBuffer1;
                        frameBufferTexture = frameBufferTexture1;
                    }
                    else
                    {
                        frameBuffer = frameBuffer0;
                        frameBufferTexture = frameBufferTexture0;
                    }
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
                    shaders[i].Use(prev);
                    prev = frameBufferTexture;
                }
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            Context.SwapBuffers();

            drawList.Clear();

            //foreach (Sprite sprite in spriteList)
            //{
            //    sprite.MakeImageHandleNonResident(sprite.texture);
            //}

            foreach (Texture2D texture in texs)
            {
                GL.Arb.MakeImageHandleNonResident(texture.Handle);
            }

            base.OnRenderFrame(args);
        }

        private int createFrameBuffer(FramebufferAttachment fba, ref long rtHandler)
        {
            int fb = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fb);

            int rt = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, rt);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Size.X, Size.Y, 0, PixelFormat.Rgba, PixelType.UnsignedByte, new IntPtr());

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 0x2601);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 0x2601);

            GL.FramebufferTexture(FramebufferTarget.Framebuffer, fba, rt, 0);

            rtHandler = GL.Arb.GetImageHandle(rt, 0, false, 0, (PixelFormat)0x8058);

            return fb;
        }

        public void AddShader(Shader s, bool enabled = true)
        {
            shaders.Add(s);
            enabledShaders.Add(enabled);
            numOfShaders += enabled ? 1 : 0;
        }

        public void SetShaderEnabled(int shaderNum, bool enabled)
        {
            if (shaderNum >= enabledShaders.Count) return;
            numOfShaders -= enabledShaders[shaderNum] ? 1 : 0;
            numOfShaders += enabled ? 1 : 0;
            enabledShaders[shaderNum] = enabled;
        }
    }

    public class DrawList
    {

        public SData[] data;
        private int size = 0, max = 10;

        public DrawList()
        {
            data = new SData[max];
        }

        public void Add(SData s)
        {
            if (size == max)
            {
                SData[] n = new SData[max * 2];
                for (int i = 0; i < max; i++)
                {
                    n[i] = data[i];
                }
                data = n;
                max *= 2;
            }
            data[size] = s;
            size++;
        }

        public SData[] getData()
        {
            return data;
        }

        public void Clear()
        {
            size = 0;
        }

        public int Count()
        {
            return size;
        }

    }
}
