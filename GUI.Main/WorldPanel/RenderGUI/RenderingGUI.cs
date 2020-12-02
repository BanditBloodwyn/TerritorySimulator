using System;
using System.Windows.Forms;
using System.Drawing;
using Core.Configuration;
using OpenTK;
using Rendering.SceneManagement;
using Rendering.SceneManagement.Components.Node;
using Rendering.SceneManagement.SceneRenderer;


namespace GUI.Main.WorldPanel.RenderGUI
{
    public partial class RenderingGUI : UserControl
    {
        private GLControl glControl;
        private readonly Renderer renderer;
        private readonly SceneManager sceneManager;


        private Point oldMousePosition;
        private Point newMousePosition;

        public RenderingGUI()
        {
            InitializeComponent();
            CreateGLControl();
            
            sceneManager = new SceneManager();
            renderer = new Renderer(glControl.Width, glControl.Height, sceneManager);

            LayerConfiguration.LayersChanged += LayersChanged;
        }

        private void LayersChanged(object sender, EventArgs e)
        {
            RefreshWindow();
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
            sceneManager.SceneChanged += SceneChanged;
            sceneManager.CreateNodes();
            renderer.InitializeCamera();
            renderer.Render();
        }

        private void SceneChanged()
        {
            renderer.Initialize();
        }

        private void GlControl_Disposed(object sender, EventArgs e)
        { 
            renderer.Dispose();
        }

        private void GlControl_MouseWheel(object sender, MouseEventArgs e)
        {
            renderer.Camera.Zoom(e.Delta);
            RefreshWindow();
        }

        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            newMousePosition = e.Location;

            if (e.Button == MouseButtons.Left)
            {
                float deltaX = (oldMousePosition.Y - newMousePosition.Y) * 0.1f;
                float deltaY = (oldMousePosition.X - newMousePosition.X) * 0.1f;

                renderer.Camera.Latitude += deltaX * 0.01f;
                renderer.Camera.Longitude -= deltaY * 0.01f;

                RefreshWindow();
            }

            oldMousePosition = e.Location;
        }

        private void GlControl_KeyDown(object sender, KeyEventArgs e)
        {

        }

        #endregion

        #region Controls

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ResetNodeShapeRotation(sceneManager.RootNode);

            renderer.ResetCamera();
            RefreshWindow();
        }

        private void ResetNodeShapeRotation(SceneNode node)
        {
            node.ResetRotation();

            foreach (SceneNode childNode in node.ChildNodes)
            {
                ResetNodeShapeRotation(childNode);
            }
        }

        #endregion

        private void RefreshWindow()
        {
            renderer.Render();
            glControl.SwapBuffers();
        }
    }
}
