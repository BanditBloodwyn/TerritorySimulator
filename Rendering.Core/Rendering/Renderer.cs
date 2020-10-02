using System;
using System.IO;
using System.Linq;
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
            GL.ClearColor(0.0f, 0.0f, 0.10f, 1.0f);

            InitializeBuffers(Shapes);
            InitializeVertexArrayObject(Shapes);
            SetupShader();
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
                firstVertexIndex += (uint)(shape.VertexBufferSize / (5 * sizeof(float)));
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

        private void SetupShader()
        {
            // shader
            string vertexPath = Path.Combine(Environment.CurrentDirectory, @"GLSL\", "Vertex.vert");
            string fragmentPath = Path.Combine(Environment.CurrentDirectory, @"GLSL\", "Fragment.frag");
            shader = new Shader(vertexPath, fragmentPath);
            shader.Use();

            int vertexLocation = shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(
                vertexLocation, 
                3, 
                VertexAttribPointerType.Float, 
                false, 
                5 * sizeof(float), 
                0);

            int texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(
                texCoordLocation, 
                2, 
                VertexAttribPointerType.Float, 
                false, 
                5 * sizeof(float), 
                3 * sizeof(float));

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
            Camera = new Camera(0, 0, 3, screenWidth / screenHeight);
            Camera.MinHeight = 1.3f;
            Camera.MaxHeight = 5.0f;
            ResetCamera();
        }

        public void Resize(float width, float height)
        {
            screenWidth = width;
            screenHeight = height;

            GL.Viewport(0, 0, (int)width, (int)height);

            if(Camera != null) 
                Camera.AspectRatio = width / height;
        }

        public void ResetCamera()
        {
            Camera.Longitude = 0;
            Camera.Latitude = 0;
            Camera.Height = 3;
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
                shader.SetMatrix4("model", model);
                shader.SetMatrix4("view", Camera.GetViewMatrix());

                GL.DrawElements(PrimitiveType.Triangles, shape.Indices.Length, DrawElementsType.UnsignedInt, offset);
                offset += shape.IndexBufferSize;
            }

            shader.SetMatrix4("projection", Camera.GetProjectionMatrix());
            shader.Use();
        }

        private void ApplyTextures(GLShape shape)
        {
            foreach (var texture in shape.Textures)
            {
                if (shape.Name != "earth" || LayerConfiguration.ShowEarthTexture)
                    texture.Key.Use(texture.Value);
                else
                    texture.Key.MakeTransparent(texture.Value);
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

            GL.DeleteProgram(shader.Handle);
            shader.Dispose();
        }
    }
}
