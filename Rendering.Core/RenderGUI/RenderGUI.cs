using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using Core.Configuration;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Rendering.Core.Classes.Shaders;
using Rendering.Core.Classes.Shapes;
using Rendering.Core.Classes.Utilities;
using Rendering.Core.Rendering;

namespace Rendering.Core.RenderGUI
{
    public partial class RenderGUI : UserControl
    {
        private GLControl glControl;

        private Point oldMousePosition;
        private Point newMousePosition;

        private Camera camera;

        private List<GLShape> shapes;

        public Func<string, bool> RasterizationChanged;

        private Renderer renderer;


        public RenderGUI()
        {
            InitializeComponent();
            CreateGLControl();
        }

        private void CreateGLControl()
        {
            glControl = new GLControl();
            glControl.Dock = DockStyle.Fill;
            glControl.Load += GlControl_Load;
            glControl.Disposed += GlControl_Disposed;
            glControl.Resize += GlControl_Resize;
            glControl.Paint += GlControl_Paint;
            glControl.MouseWheel += GlControl_MouseWheel;
            glControl.MouseMove += GlControl_MouseMove;
            glControl.KeyDown += GlControl_KeyDown;

            pnlGL.Controls.Add(glControl);
        }


        #region GlControl

        private void GlControl_Paint(object sender, PaintEventArgs e)
        {
            RefreshWindow();
        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);

            if (shapes == null || shader == null || camera == null)
                return;

            camera.AspectRatio = (float)glControl.Width / glControl.Height;
            RefreshWindow();
        }

        private void GlControl_Load(object sender, EventArgs e)
        {
            CreateShapes();

            renderer = new Renderer();
            renderer.Initialize(glControl.Width, glControl.Height, shapes.ToArray());
        }

        private void GlControl_Disposed(object sender, EventArgs e)
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

        private void GlControl_MouseWheel(object sender, MouseEventArgs e)
        {
            camera.Position += camera.Front * e.Delta * 0.001f;

            RefreshWindow();
        }

        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            newMousePosition = e.Location;

            if (e.Button == MouseButtons.Left)
            {
                foreach (GLShape shape in shapes)
                {
                    float deltaX = (oldMousePosition.Y - newMousePosition.Y) * 0.1f;
                    float deltaY = (oldMousePosition.X - newMousePosition.X) * 0.1f;

                    shape.Rotate(deltaX, deltaY, 0);
                }
                RefreshWindow();
            }

            if (e.Button == MouseButtons.Right)
            {
                float deltaPitch = (newMousePosition.Y - oldMousePosition.Y) * 0.05f;
                float deltaYaw = (newMousePosition.X - oldMousePosition.X) * 0.05f;

                camera.Yaw -= deltaYaw;
                camera.Pitch += deltaPitch;

                RefreshWindow();
            }


            if (e.Button == MouseButtons.Middle)
            {
                camera.Position += camera.Up * (newMousePosition.Y - oldMousePosition.Y) * 0.001f;
                camera.Position -= camera.Right * (newMousePosition.X - oldMousePosition.X) * 0.001f;

                RefreshWindow();
            }

            oldMousePosition = e.Location;
        }

        private void GlControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                foreach (GLSphere glShape in shapes)
                {
                    var sphere = glShape;
                    sphere.Rasterization += 1;

                    RasterizationChanged?.Invoke(sphere.Rasterization.ToString());

                    GL.BufferData(BufferTarget.ArrayBuffer, sphere.Vertices.Length * sizeof(float), sphere.Vertices, BufferUsageHint.StaticDraw);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, sphere.Indices.Length * sizeof(uint), sphere.Indices, BufferUsageHint.StaticDraw);
                }

            if (e.KeyCode == Keys.Space)
                foreach (GLSphere glShape in shapes)
                {
                    var sphere = glShape;
                    sphere.Rasterization -= 1;

                    RasterizationChanged?.Invoke(sphere.Rasterization.ToString());

                    GL.BufferData(BufferTarget.ArrayBuffer, sphere.Vertices.Length * sizeof(float), sphere.Vertices, BufferUsageHint.StaticDraw);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, sphere.Indices.Length * sizeof(uint), sphere.Indices, BufferUsageHint.StaticDraw);
                }

            RefreshWindow();
        }

        #endregion

        #region Controls

        private
            void btnRefresh_Click(object sender, EventArgs e)
        {
            foreach (GLShape shape in shapes)
            {
                shape.ResetRotation();
            }

            ResetCamera();
            RefreshWindow();
        }

        #endregion


        private void CreateShapes()
        {
            shapes = new List<GLShape>();

            var sphere = new GLSphere();
            sphere.Radius = 1.0f;
            sphere.Rasterization = 128;
            sphere.SetTexture("Resources\\Textures\\earth.jpg");
            sphere.Rotate(90, 0, 0);
            shapes.Add(sphere);
            
            var stars = new GLSphere();
            stars.Radius = 3.0f;
            stars.Rasterization = 128;
            stars.SetTexture("Resources\\Textures\\stars.png");
            stars.Rotate(90, 0, 0);
            stars.Translate(2, 0, 0);
            shapes.Add(stars);

            RasterizationChanged?.Invoke(sphere.Rasterization.ToString());
        }

        private void InitializeCamera()
        {
            camera = new Camera(Vector3.UnitZ * 3, (float)glControl.Width / glControl.Height);
            ResetCamera();
        }

        private void ResetCamera()
        {
            camera.Position = new Vector3(0.0f, 0.0f, 3.0f);
            camera.Pitch = 0;
            camera.Yaw = -90;
        }

        private void Render()
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


                //GL.BindBuffer(BufferTarget.ArrayBuffer, shape.vertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, shape.Vertices.Length * sizeof(float), shape.Vertices, BufferUsageHint.StaticDraw);
                //GL.BindBuffer(BufferTarget.ElementArrayBuffer, shape.elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, shape.Indices.Length * sizeof(uint), shape.Indices, BufferUsageHint.StaticDraw);

                GL.DrawElements(PrimitiveType.Triangles, shape.Indices.Length, DrawElementsType.UnsignedInt, 0);
            }

            glControl.SwapBuffers();
        }

        private void RefreshWindow()
        {
            Render();
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
