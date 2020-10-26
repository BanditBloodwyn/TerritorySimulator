using System;
using System.IO;
using System.Linq;
using Core.Configuration;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Rendering.Core.Classes;
using Rendering.Core.Classes.Shaders;
using Rendering.Core.Classes.Shapes;
using Rendering.Core.Classes.Utilities;

namespace Rendering.Core.Rendering
{
    public class Renderer
    {
        private Shader objectShader;
        private int vertexBufferObject;
        private int elementBufferObject;

        private float screenWidth;
        private float screenHeight;

        public Camera Camera { get; private set; }
        public GLShape[] Shapes { get; private set; }

        public Renderer(float screenWidth, float screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
        }

        public void Initialize(GLShape[] shapeArray)
        {
            Shapes = shapeArray;

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.ClearColor(0.0f, 0.0f, 0.10f, 1.0f);

            InitializeBuffers(Shapes);
            InitializeVertexArrayObject(Shapes);
            SetupShaders();
            BindBuffers();
        }

        private void InitializeBuffers(GLShape[] shapeArray)
        {
            int vertexBufferSize = shapeArray.Sum(shape => shape.VertexBufferSize);
            int indexBufferSize = shapeArray.Sum(shape => shape.IndexBufferSize);

            // Vertex buffer
            vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexBufferSize, (IntPtr)0, BufferUsageHint.StaticDraw);

            IntPtr offset = (IntPtr)0;
            foreach (GLShape shape in shapeArray)
            {
                GL.BufferSubData(BufferTarget.ArrayBuffer, offset, shape.VertexBufferSize, shape.Vertices);
                offset += shape.VertexBufferSize;
            }

            // Element buffer
            elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexBufferSize, (IntPtr)0, BufferUsageHint.StaticDraw);

            offset = (IntPtr)0;
            uint firstVertexIndex = 0;
            foreach (GLShape shape in shapeArray)
            {
                var indexArray = shape.Indices.Select(index => index + firstVertexIndex).ToArray();

                GL.BufferSubData(BufferTarget.ElementArrayBuffer, offset, shape.IndexBufferSize, indexArray);
                offset += shape.IndexBufferSize;
                firstVertexIndex += (uint)(shape.VertexBufferSize / (8 * sizeof(float)));
            }
        }

        private void InitializeVertexArrayObject(GLShape[] shapeArray)
        {
            foreach (GLShape shape in shapeArray)
            {
                shape.VertexArrayObject = GL.GenVertexArray();
                GL.BindVertexArray(shape.VertexArrayObject);
            }
        }

        private void SetupShaders()
        {
            SetupObjectShader();
            SetupLightingShader();
        }

        private void SetupLightingShader()
        {
        }

        private void SetupObjectShader()
        {
            string vertexPath = Path.Combine(Environment.CurrentDirectory, @"GLSL\", "Vertex.vert");
            string fragmentPath = Path.Combine(Environment.CurrentDirectory, @"GLSL\", "Fragment.frag");

            objectShader = new Shader(vertexPath, fragmentPath);
            objectShader.Use();

            int vertexLocation = objectShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(
                vertexLocation,
                3,
                VertexAttribPointerType.Float,
                false,
                8 * sizeof(float),
                0);

            int normCoordLocation = objectShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normCoordLocation);
            GL.VertexAttribPointer(
                normCoordLocation,
                3,
                VertexAttribPointerType.Float,
                false,
                8 * sizeof(float),
                3 * sizeof(float));


            int texCoordLocation = objectShader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(
                texCoordLocation,
                2,
                VertexAttribPointerType.Float,
                false,
                8 * sizeof(float),
                6 * sizeof(float));

            objectShader.SetInt("texture0", 0);
            objectShader.SetInt("texture1", 1);
        }

        private void BindBuffers()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
        }

        public void InitializeCamera()
        {
            Camera = new Camera(0, 0, 50, screenWidth / screenHeight);
            Camera.MinHeight = 05.0f;
            Camera.MaxHeight = 70.0f;
            ResetCamera();
        }

        public void Resize(float width, float height)
        {
            screenWidth = width;
            screenHeight = height;

            GL.Viewport(0, 0, (int)width, (int)height);

            if (Camera != null)
                Camera.AspectRatio = width / height;
        }

        public void ResetCamera()
        {
            Camera.Longitude = 0;
            Camera.Latitude = 0;
            Camera.Height = 50;
        }

        public void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (Shapes == null || Shapes.Length == 0)
                return;

            IntPtr offset = (IntPtr)0;
            foreach (GLShape shape in Shapes)
            {
                ApplyTextures(shape);

                ApplyModelTransforms(shape, out Matrix4 model);
                objectShader.SetMatrix4("model", model);

                GL.DrawElements(PrimitiveType.Triangles, shape.Indices.Length, DrawElementsType.UnsignedInt, offset);
                offset += shape.IndexBufferSize;
            }
           
            objectShader.SetVector3("material.ambient", new Vector3(1.0f, 0.5f, 0.31f));
            objectShader.SetVector3("material.diffuse", new Vector3(1.0f, 0.5f, 0.31f));
            objectShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));

            objectShader.SetVector3("light.position", new Vector3(100f, 1.0f, 2.0f));
            objectShader.SetVector3("light.ambient", new Vector3(1.0f));
            objectShader.SetVector3("light.diffuse", new Vector3(0.5f));
            objectShader.SetVector3("light.specular", new Vector3(1.0f));

            objectShader.SetMatrix4("view", Camera.GetViewMatrix());
            objectShader.SetMatrix4("projection", Camera.GetProjectionMatrix());
            objectShader.Use();
        }

        private void ApplyTextures(GLShape shape)
        {
            switch (shape.Name)
            {
                case "earth":
                {
                    if (LayerConfiguration.ShowEarthTexture)
                        shape.Textures[TextureType.Visible].Use();
                    else
                        shape.Textures[TextureType.Invisible].Use();
                    break;
                }
                case "earthClouds":
                {
                    if (LayerConfiguration.ShowCloudTexture)
                        shape.Textures[TextureType.Visible].Use();
                    else
                        shape.Textures[TextureType.Invisible].Use();
                    break;
                }
                default:
                    shape.Textures[TextureType.Visible].Use();
                    break;
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

        public void Dispose()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(vertexBufferObject);

            foreach (GLShape shape in Shapes)
            {
                GL.DeleteVertexArray(shape.VertexArrayObject);
            }

            GL.DeleteProgram(objectShader.Handle);
            objectShader.Dispose();
        }
    }
}
