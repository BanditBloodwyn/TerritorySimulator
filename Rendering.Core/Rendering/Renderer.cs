using System;
using System.Collections.Generic;
using System.IO;
using Core.Configuration;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Rendering.Core.Classes.Shaders;
using Rendering.Core.Classes.Shapes;
using Rendering.Core.Classes.Utilities;

namespace Rendering.Core.Rendering
{
    public class Renderer
    {
        private float screenWidth;
        private float screenHeight;

        private int vertexBufferObject;
        private int[] vertexArrayObjects;
        private int elementBufferObject;
        
        private Camera camera;

        private Shader shader;


        public Renderer()
        {

        }

        public void Initialize(float screenWidth, float screenHeight, GLShape[] shapes)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.0f, 0.0f, 0.10f, 1.0f);

            vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, GetShapesVertexBufferSize(shapes), IntPtr.Zero, BufferUsageHint.StaticDraw);
            for (int i = 0; i < shapes.Length; i++)
            {
                GL.BufferSubData(BufferTarget.ArrayBuffer,
                    i == 0 ? IntPtr.Zero : new IntPtr(i * shapes[i - 1].Vertices.Length * sizeof(float)),
                    shapes[i].Vertices.Length * sizeof(float), shapes[i].Vertices);
            }

            elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, GetShapesIndexBufferSize(shapes), IntPtr.Zero, BufferUsageHint.StaticDraw);
            for (int i = 0; i < shapes.Length; i++)
            {
                GL.BufferSubData(BufferTarget.ElementArrayBuffer,
                    i == 0 ? IntPtr.Zero : new IntPtr(i * shapes[i - 1].Indices.Length * sizeof(float)),
                    shapes[i].Indices.Length * sizeof(float), shapes[i].Indices);
            }

            // shader
            string vertexPath = Path.Combine(Environment.CurrentDirectory, @"GLSL\", "Vertex.vert");
            string fragmentPath = Path.Combine(Environment.CurrentDirectory, @"GLSL\", "Fragment.frag");
            shader = new Shader(vertexPath, fragmentPath);
            shader.Use();

            for (int i = 0; i < shapes.Length; i++)
            {
                int vertexArrayObject = GL.GenVertexArray();
                GL.BindVertexArray(vertexArrayObject);

                int vertexLocation = shader.GetAttribLocation("aPosition");
                GL.EnableVertexAttribArray(vertexLocation);
                GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float),
                    i == 0 ? 0 : i* shapes[i-1].Vertices.Length * sizeof(float));

                int texCoordLocation = shader.GetAttribLocation("aTexCoord");
                GL.EnableVertexAttribArray(texCoordLocation);
                GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float),
                    i == 0 ? 3 : 3 + i * shapes[i - 1].Vertices.Length * sizeof(float));
            }


            shader.SetInt("texture0", 0);
            shader.SetInt("texture1", 1);

            InitializeCamera(screenWidth, screenHeight);

            Render(shapes);
        }

        private int GetShapesVertexBufferSize(GLShape[] shapes)
        {
            List<float> allVertices = new List<float>();
            foreach (GLShape shape in shapes)
                allVertices.AddRange(shape.Vertices);
            
            return allVertices.Count * sizeof(float);
        }

        private int GetShapesIndexBufferSize(GLShape[] shapes)
        {
            List<uint> allIndices = new List<uint>();
            foreach (GLShape shape in shapes)
                allIndices.AddRange(shape.Indices);

            return allIndices.Count * sizeof(uint);
        }


        public void Dispose()
        {
            GL.DeleteProgram(shader.Handle);
            shader.Dispose();
        }

        private void InitializeCamera(float screenWidth, float screenHeigth)
        {
            camera = new Camera(Vector3.UnitZ * 3, screenWidth / screenHeigth);
            ResetCamera();
        }

        public void ResetCamera()
        {
            camera.Position = new Vector3(0.0f, 0.0f, 3.0f);
            camera.Pitch = 0;
            camera.Yaw = -90;
        }

        public void SetAspectRatio(float screenWidth, float screenHeigth)
        {
            camera.AspectRatio = screenWidth / screenHeigth;
        }

        public void Render(GLShape[] shapes)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (GLShape shape in shapes)
            {
                foreach (var texture in shape.Textures)
                {
                    if (LayerConfiguration.ShowEarthTexture)
                        texture.Key.Use(texture.Value);
                    else
                        texture.Key.MakeTransparent(texture.Value);
                }

                ApplyModelTransforms(shape, out Matrix4 model);
                shader.SetMatrix4("model", model);
                shader.SetMatrix4("view", camera.GetViewMatrix());
                shader.SetMatrix4("projection", camera.GetProjectionMatrix());
                shader.Use();

                GL.DrawElements(PrimitiveType.Triangles, shape.Indices.Length, DrawElementsType.UnsignedInt, 0);
            }
        }

        private void ApplyModelTransforms(GLShape shape, out Matrix4 model)
        {
            model = Matrix4.Identity;
            model *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(shape.AngleX));
            model *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(shape.AngleY));
            model *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(shape.AngleZ));
            model *= Matrix4.CreateTranslation(shape.PositionX, shape.PositionY, shape.PositionZ);
        }

    }
}
