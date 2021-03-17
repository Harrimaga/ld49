using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Kee5Engine.Shaders
{
    public class Shader
    {
        public readonly int Handle;

        private readonly Dictionary<string, int> _uniformLocations;

        public Shader(string vertPath, string fragPath)
        {
            // Vertex shader moves around vertices
            // Fragment shader converts vertices to fragmens, which is data OpenGL uses to draw a pixel

            //Load vertex shader and compile
            var shaderSource = File.ReadAllText(vertPath);

            // Create the shader of type vertexShader
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);

            // Bind GLSL source code
            GL.ShaderSource(vertexShader, shaderSource);

            // Compile Shader
            CompileShader(vertexShader);

            // Same for Fragment Shader
            shaderSource = File.ReadAllText(fragPath);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, shaderSource);
            CompileShader(fragmentShader);

            // Merge the shaders into a shader program, which can then be used by OpenGL
            // Create a Program
            Handle = GL.CreateProgram();

            // Attach the shaders
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);

            // Link the shaders together
            LinkProgram(Handle);

            // After linking the shader program, the individual shaders are no longer needed
            // Detach and delete them
            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);

            // Cache shader uniform locations

            // Get the number of active uniforms in the shader
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            // Allocate the dictionary to hold the locations
            _uniformLocations = new Dictionary<string, int>();

            // Loop over all uniforms
            for (int i = 0; i < numberOfUniforms; i++)
            {
                // Get the name of the uniform
                var key = GL.GetActiveUniform(Handle, i, out _, out _);

                // Get the location
                var location = GL.GetUniformLocation(Handle, key);

                // Add to dictionary
                _uniformLocations.Add(key, location);
            }
        }

        private static void CompileShader(int shader)
        {
            // Try to compile the shader
            GL.CompileShader(shader);

            // Check for errors
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                // Something happened
                var infoLog = GL.GetShaderInfoLog(shader);
                Console.WriteLine($"Error occured whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }

        private static void LinkProgram(int program)
        {
            // Try to link the program
            GL.LinkProgram(program);

            // Check for errors
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                // Something happened
                var infoLog = GL.GetProgramInfoLog(program);
                Console.WriteLine($"Error occured whilst linking Program({program}).\n\n{infoLog}");
            }
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }

        // Uniform setters
        // Use VBOs for vertex-related data, uniforms for almost anything else

        public void SetInt(string name, int data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[name], data);
        }

        public void SetFloat(string name, float data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[name], data);
        }

        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(Handle);
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }

        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(Handle);
            GL.Uniform3(_uniformLocations[name], data);
        }

        public void SetVector4(string name, Vector4 data)
        {
            GL.UseProgram(Handle);
            GL.Uniform4(_uniformLocations[name], data);
        }

        public void SetIntArray(string name, int[] data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[name + "[0]"], data.Length, data);
        }
    }
}
