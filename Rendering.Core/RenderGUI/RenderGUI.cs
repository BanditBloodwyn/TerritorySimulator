using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Rendering.Core.Classes.Shapes;
using Rendering.Core.Rendering;


namespace Rendering.Core.RenderGUI
{
    public partial class RenderGUI : UserControl
    {
        private GLControl glControl;
        private readonly Renderer renderer;

        private Point oldMousePosition;
        private Point newMousePosition;

        private List<GLShape> shapes;

        public Func<string, bool> RasterizationChanged;


        public RenderGUI()
        {
            InitializeComponent();
            CreateGLControl();

            renderer = new Renderer(glControl.Width, glControl.Height);
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
            renderer.Resize(glControl.Width, glControl.Height);
            RefreshWindow();
        }

        private void GlControl_Load(object sender, EventArgs e)
        {
            CreateShapes();

            renderer.Initialize(shapes);
            renderer.InitializeCamera();

            renderer.Render();
        }

        private void GlControl_Disposed(object sender, EventArgs e)
        { 
            renderer.Dispose();
        }

        private void GlControl_MouseWheel(object sender, MouseEventArgs e)
        {
            renderer.Camera.Position += renderer.Camera.Front * e.Delta * 0.001f;

            RefreshWindow();
        }

        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            newMousePosition = e.Location;

            if (e.Button == MouseButtons.Left)
            {
                foreach (GLShape shape in renderer.Shapes)
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

                renderer.Camera.Yaw -= deltaYaw;
                renderer.Camera.Pitch += deltaPitch;

                RefreshWindow();
            }


            if (e.Button == MouseButtons.Middle)
            {
                renderer.Camera.Position += renderer.Camera.Up * (newMousePosition.Y - oldMousePosition.Y) * 0.001f;
                renderer.Camera.Position -= renderer.Camera.Right * (newMousePosition.X - oldMousePosition.X) * 0.001f;

                RefreshWindow();
            }

            oldMousePosition = e.Location;
        }

        private void GlControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                foreach (var glShape in renderer.Shapes)
                {
                    var sphere = (GLSphere)glShape;
                    sphere.Rasterization += 1;

                    RasterizationChanged?.Invoke(sphere.Rasterization.ToString());

                    GL.BufferData(BufferTarget.ArrayBuffer, sphere.Vertices.Length * sizeof(float), sphere.Vertices, BufferUsageHint.StaticDraw);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, sphere.Indices.Length * sizeof(uint), sphere.Indices, BufferUsageHint.StaticDraw);
                }

            if (e.KeyCode == Keys.Space)
                foreach (var glShape in renderer.Shapes)
                {
                    var sphere = (GLSphere)glShape;
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
            foreach (GLShape shape in renderer.Shapes)
            {
                shape.ResetRotation();
            }

            renderer.ResetCamera();
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

            RasterizationChanged?.Invoke(sphere.Rasterization.ToString());
        }

        private void RefreshWindow()
        {
            renderer.Render();
            glControl.SwapBuffers();
        }
    }
}
