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
        private Shader shader;

        private List<GLShape> shapes;
        
        private Camera camera;

        private int vertexBufferObject;
        private int vertexArrayObject;
        private int elementBufferObject;

        private float screenWidth;
        private float screenHeight;

        public Camera Camera { get; set; }

        public GLShape[] Shapes => shapes.ToArray();

        public Renderer(float screenWidth, float screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
        }

        public void Initialize(List<GLShape> shapeList)
        {
            shapes = shapeList;

            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.0f, 0.0f, 0.10f, 1.0f);

            InitializeBuffers();
            InitializeVertexArrayObject();
            SetupShader();
            BindBuffers();
        }

        private void InitializeBuffers()
        {
            List<float> allVertices = new List<float>();
            foreach (GLShape shape in shapes)
                allVertices.AddRange(shape.Vertices);
            List<uint> allIndices = new List<uint>();
            foreach (GLShape shape in shapes)
                allIndices.AddRange(shape.Indices);

            vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, allVertices.Count * sizeof(float), allVertices.ToArray(), BufferUsageHint.StaticDraw);

            elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, allIndices.Count * sizeof(uint), allIndices.ToArray(), BufferUsageHint.StaticDraw);

        }

        private void InitializeVertexArrayObject()
        {
            vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObject);
        }

        private void SetupShader()
        {
            // shader
            string vertexPath = Path.Combine(Environment.CurrentDirectory, @"GLSL\", "Vertex.vert");
            string fragmentPath = Path.Combine(Environment.CurrentDirectory, @"GLSL\", "Fragment.frag");
            shader = new Shader(vertexPath, fragmentPath);
            shader.Use();

            int vertexLocation = shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            shader.SetInt("texture0", 0);
            shader.SetInt("texture1", 1);
        }

        private void BindBuffers()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
        }

        public void InitializeCamera()
        {
            camera = new Camera(Vector3.UnitZ * 3, screenWidth / screenHeight);
            ResetCamera();
        }

        public void Resize(float width, float height)
        {
            screenWidth = width;
            screenHeight = height;

            GL.Viewport(0, 0, (int)width, (int)height);

            if(camera != null) 
                camera.AspectRatio = width / height;
        }

        public void ResetCamera()
        {
            camera.Position = new Vector3(0.0f, 0.0f, 3.0f);
            camera.Pitch = 0;
            camera.Yaw = -90;
        }


        public void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (shapes == null || shapes.Count == 0)
                return;

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

                GL.DrawElements(PrimitiveType.Triangles, shape.Indices.Length, DrawElementsType.UnsignedInt, 0);
            }

            shader.SetMatrix4("projection", camera.GetProjectionMatrix());
            shader.Use();
        }

        private void ApplyModelTransforms(GLShape shape, out Matrix4 model)
        {
            model = Matrix4.Identity;
            model *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(shape.AngleX));
            model *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(shape.AngleY));
            model *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(shape.AngleZ));
            model *= Matrix4.CreateTranslation(shape.PositionX, shape.PositionY, shape.PositionZ);
        }

        public void Dispose()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteVertexArray(vertexArrayObject);

            GL.DeleteProgram(shader.Handle);
            shader.Dispose();
        }
    }
}
