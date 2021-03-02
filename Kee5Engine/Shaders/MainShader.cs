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
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);
            GL.BindVertexArray(vao);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            ssbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, ssbo);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, ssbo);
        }

        protected override void Run()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            SData[] sdd = Window.drawList.getData();
            int num = Window.drawList.Count();
            GL.BindVertexArray(vao);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.BindVertexArray(vao);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, ssbo);
            GL.BufferData<SData>(BufferTarget.ShaderStorageBuffer, (sizeof(int) * 5 + 1 * sizeof(long) + 9 * sizeof(float)) * num, sdd, BufferUsageHint.DynamicDraw);
            GL.Uniform2(GL.GetUniformLocation(Handle, "screenSize"), Globals.windowSize.X, Globals.windowSize.Y);
            GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, num);
            GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
        }
    }
}
