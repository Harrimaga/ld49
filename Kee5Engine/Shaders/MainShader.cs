using System;
using System.Collections.Generic;
using System.Text;

using OpenTK.Graphics.OpenGL4;

namespace Kee5Engine.Shaders
{
    public class MainShader : Shader
    {
        private int vao, vbo, ssbo;
        bool late = false;
        PassThroughShader pts = new PassThroughShader();
        float[] data = {
                0.0f, 1.0f, 0,
                1.0f, 0.0f, 0,
                0.0f, 0.0f, 0,
                1.0f, 1.0f, 0,
                0.0f, 1.0f, 0,
                1.0f, 0.0f, 0
            };

        public MainShader(bool amd) : base(amd ? "Shaders/vsAMD.glsl" : "Shaders/vs.glsl", amd ? "Shaders/fsAMD.glsl" : "Shaders/fs.glsl")
        {

        }

        protected override void ShaderSetup()
        {
            base.ShaderSetup();

            vbo = GL.GenBuffer();
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            ssbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, ssbo);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, ssbo);
        }

        protected override void Run(long previous)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            SData[] sdd = Window.drawList.getData();
            int num = Window.drawList.Count();

            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, ssbo);
            GL.BufferData<SData>(BufferTarget.ShaderStorageBuffer, (sizeof(int) * 5 + 1 * sizeof(long) + 9 * sizeof(float)) * num, sdd, BufferUsageHint.DynamicDraw);
            GL.Uniform2(GL.GetUniformLocation(Handle, "screenSize"), Globals.windowSize.X, Globals.windowSize.Y);
            GL.BindVertexArray(vao);
            GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, num);
            GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
        }

        public void RunLate(long previous)
        {
            late = true;
            pts.Use(previous);
            GL.UseProgram(Handle);
            Run(previous);
            GL.UseProgram(0);
            late = false;
        }
    }

    public class PassThroughShader : Shader
    {

        private int prev, screenSize;

        public PassThroughShader() : base("Shaders/vsPost.glsl", "Shaders/fsPass.glsl")
        {

        }

        protected override void ShaderSetup()
        {
            base.ShaderSetup();
            prev = GL.GetUniformLocation(Handle, "prev");
            screenSize = GL.GetUniformLocation(Handle, "screenSize");
        }

        protected override void Run(long previous)
        {
            if (previous == -1)
            {
                throw new ArgumentException("Previous was a non existing framebuffer");
            }
            GL.Arb.MakeImageHandleResident(previous, All.ReadOnly);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.Arb.UniformHandle(prev, previous);
            GL.Uniform2(screenSize, Globals.windowSize.X, Globals.windowSize.Y);
            GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
            GL.Arb.MakeImageHandleNonResident(previous);
        }

    }
}
