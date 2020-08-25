﻿using System;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL;


namespace Rendering.Core.Shaders
{
    public class Shader : IDisposable
    {
        private int Handle;
        private bool disposedValue = false;

        public Shader(string vertexPath, string fragmentPath)
        {
            // shader handles
            int VertexShader;
            int FragmentShader;

            //  load shader source code
            string VertexShaderSource;
            using (StreamReader reader = new StreamReader(vertexPath, Encoding.UTF8))
                VertexShaderSource = reader.ReadToEnd();

            string FragmentShaderSource;
            using (StreamReader reader = new StreamReader(fragmentPath, Encoding.UTF8))
                FragmentShaderSource = reader.ReadToEnd();

            // generate shaders
            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);

            // compile shaders
            GL.CompileShader(VertexShader);
            string infoLogVert = GL.GetShaderInfoLog(VertexShader);
            if (infoLogVert != string.Empty)
                Console.WriteLine(infoLogVert);

            GL.CompileShader(FragmentShader);
            string infoLogFrag = GL.GetShaderInfoLog(FragmentShader);
            if (infoLogFrag != string.Empty)
                Console.WriteLine(infoLogFrag);

            // link shaders together into a program that can be run on the GPU
            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);
            GL.LinkProgram(Handle);

            // cleanup (shaders are already copied to the shader-program, so we don't need them individually anymore
            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }
        
        ~Shader()
        {
            GL.DeleteProgram(Handle);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);

                disposedValue = true;
            }
        }
    }
}
