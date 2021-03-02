using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using OpenTK.Graphics.OpenGL4;

namespace Kee5Engine.Shaders
{
    public class Shader
    {
        public int Handle;

        /// <summary>
        /// Loads the shader, you have to give both a vertex and fragment. Can't be anything else
        /// </summary>
        public Shader(string ver, string frag)
        {
            string vss;
            string fss;
            using (StreamReader reader = new StreamReader(ver, Encoding.UTF8))
            {
                vss = reader.ReadToEnd();
            }
            using (StreamReader reader = new StreamReader(frag, Encoding.UTF8))
            {
                fss = reader.ReadToEnd();
            }
            int vs = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs, vss);
            int fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs, fss);

            GL.CompileShader(vs);
            string log = GL.GetShaderInfoLog(vs);
            if (log != System.String.Empty)
            {
                System.Console.WriteLine("Error in vertex shader: \n" + log);
            }

            GL.CompileShader(fs);
            log = GL.GetShaderInfoLog(fs);
            if (log != System.String.Empty)
            {
                System.Console.WriteLine("Error in fragment shader: \n" + log);
            }
            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, vs);
            GL.AttachShader(Handle, fs);

            GL.LinkProgram(Handle);

            GL.DetachShader(Handle, vs);
            GL.DetachShader(Handle, fs);
            GL.DeleteShader(fs);
            GL.DeleteShader(vs);

            ShaderSetup();
        }

        public void Use(long previous)
        {
            GL.UseProgram(Handle);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Run(previous);
            GL.UseProgram(0);
        }

        /// <summary>
        /// For initializing buffers and shit
        /// </summary>
        protected virtual void ShaderSetup()
        {

        }

        /// <summary>
        /// Put your code for actually running the shaders in this function.
        /// Gets called by the Use function
        /// </summary>
        protected virtual void Run(long previous)
        {

        }
    }
}
