using System;
using System.IO;
using System.Linq;
using Core.Configuration;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Rendering.Core.Classes;
using Rendering.Core.Classes.Shaders;
using Rendering.Core.Classes.Utilities;
using Rendering.SceneManagement.Components.Node;


namespace Rendering.SceneManagement.SceneRenderer
{
    public class Renderer
    {
        private Shader objectShader;
        private int vertexBufferObject;
        private int elementBufferObject;

        private IntPtr arrayOffset;
        private IntPtr elementArrayOffset;
        private IntPtr drawOffset;

        private float screenWidth;
        private float screenHeight;

        public Camera Camera { get; private set; }

        public SceneManager Manager { get; set; }

        public Renderer(float screenWidth, float screenHeight, SceneManager sceneManager)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            Manager = sceneManager;
        }

        public void Initialize()
        {
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.ClearColor(0.0f, 0.0f, 0.00f, 0.0f);

            InitializeBuffers(Manager.RootNode);
            InitializeVertexArrayObject(Manager.RootNode);
            SetupShaders();
            BindBuffers();
        }

        private void InitializeBuffers(SceneNode node)
        {
            int vertexBufferSize = node.GetVertexBufferSize();
            int indexBufferSize = node.GetIndexBufferSize();

            // Vertex buffer
            vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexBufferSize, (IntPtr)0, BufferUsageHint.StaticDraw);

            arrayOffset = (IntPtr)0;
            ArrayBufferSubData(node);

            // Element buffer
            elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexBufferSize, (IntPtr)0, BufferUsageHint.StaticDraw);

            elementArrayOffset = (IntPtr)0;
            ElementArrayBufferSubData(node, 0);
        }

        private void ArrayBufferSubData(SceneNode node)
        {
            if (node.NodeShape != null)
            {
                GL.BufferSubData(BufferTarget.ArrayBuffer, arrayOffset, node.NodeShape.VertexBufferSize, node.NodeShape.Vertices);
                arrayOffset += node.NodeShape.VertexBufferSize;
            }

            foreach (SceneNode childNode in node.ChildNodes)
            {
                ArrayBufferSubData(childNode);
            }
        }

        private void ElementArrayBufferSubData(SceneNode node, uint firstVertexIndex)
        {
            if (node.NodeShape != null)
            {
                uint[] indexArray = node.NodeShape.Indices.Select(index => index + firstVertexIndex).ToArray();

                GL.BufferSubData(BufferTarget.ElementArrayBuffer, elementArrayOffset, node.NodeShape.IndexBufferSize, indexArray);
                elementArrayOffset += node.NodeShape.IndexBufferSize;
                firstVertexIndex += (uint) (node.NodeShape.VertexBufferSize / (8 * sizeof(float)));
            }

            foreach (SceneNode childNode in node.ChildNodes)
            {
                ElementArrayBufferSubData(childNode, firstVertexIndex);
            }
        }

        private void InitializeVertexArrayObject(SceneNode node)
        {
            if (node.NodeShape != null)
            {
                node.NodeShape.VertexArrayObject = GL.GenVertexArray();
                GL.BindVertexArray(node.NodeShape.VertexArrayObject);
            }

            foreach (SceneNode childNode in node.ChildNodes)
            {
                InitializeVertexArrayObject(childNode);
            }
        }

        private void SetupShaders()
        {
            SetupObjectShader();
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
            Camera = new Camera(0, 0, 50, screenWidth / screenHeight)
            {
                MinHeight = 25.0f,
                MaxHeight = 70.0f
            };
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
            if (Manager.RootNode == null)
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            drawOffset = (IntPtr)0;

            DrawNode(Manager.RootNode);

            objectShader.Use();

            objectShader.SetVector3("viewPos", Camera.Position);

            objectShader.SetInt("material.diffuse", 0);
            objectShader.SetInt("material.specular", 1);
            objectShader.SetVector3("material.specular", new Vector3(1.0f, 1.0f, 1.0f));
            objectShader.SetFloat("material.shininess", 32.0f);

            objectShader.SetVector3("light.position", new Vector3(-1000.0f, 1.0f, 0.0f));
            objectShader.SetVector3("light.ambient", new Vector3(0.02f));
            objectShader.SetVector3("light.diffuse", new Vector3(2.0f));
            objectShader.SetVector3("light.specular", new Vector3(0.5f));

            objectShader.SetMatrix4("view", Camera.GetViewMatrix());
            objectShader.SetMatrix4("projection", Camera.GetProjectionMatrix());
        }

        private void DrawNode(SceneNode node)
        {
            if (node.NodeShape != null)
            {
                ApplyTextures(node);

                ApplyModelTransforms(node, out Matrix4 model);
                objectShader.SetMatrix4("model", model);

                GL.DrawElements(PrimitiveType.Triangles, node.NodeShape.Indices.Length, DrawElementsType.UnsignedInt,
                    drawOffset);
                drawOffset += node.NodeShape.IndexBufferSize;
            }

            foreach (SceneNode childNode in node.ChildNodes)
            {
                DrawNode(childNode);
            }
        }

        private void ApplyTextures(SceneNode node)
        {
            switch (node.NodeShape.Name)
            {
                case "earth":
                    {
                        if (LayerConfiguration.ShowEarthTexture)
                        {
                            node.NodeShape.Textures[TextureType.DiffuseMap].Use();
                            node.NodeShape.Textures[TextureType.SpecularMap].Use();
                        }
                        else
                            node.NodeShape.Textures[TextureType.Transparent].Use();
                        break;
                    }
                case "earthClouds":
                    {
                        if (LayerConfiguration.ShowCloudTexture)
                        {
                            node.NodeShape.Textures[TextureType.DiffuseMap].Use();
                            node.NodeShape.Textures[TextureType.SpecularMap].Use();
                        }
                        else
                            node.NodeShape.Textures[TextureType.Transparent].Use();
                        break;
                    }
                default:
                    node.NodeShape.Textures[TextureType.DiffuseMap].Use();
                    node.NodeShape.Textures[TextureType.SpecularMap].Use();
                    break;
            }
        }

        private void ApplyModelTransforms(SceneNode node, out Matrix4 model)
        {
            model = Matrix4.Identity;
            model *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(node.NodeShape.AngleX));
            model *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(node.NodeShape.AngleY));
            model *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(node.NodeShape.AngleZ));
            model *= Matrix4.CreateTranslation(node.NodeShape.PositionX, node.NodeShape.PositionY, node.NodeShape.PositionZ);
        }

        public void Dispose()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteBuffer(elementBufferObject);

            DisposeVertexArray(Manager.RootNode);

            GL.DeleteProgram(objectShader.Handle);
            objectShader.Dispose();
        }

        private void DisposeVertexArray(SceneNode node)
        {
            if(node.NodeShape != null) 
                GL.DeleteVertexArray(node.NodeShape.VertexArrayObject);

            foreach (SceneNode childNode in node.ChildNodes)
            {
                DisposeVertexArray(childNode);
            }
        }
    }
}
