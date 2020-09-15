using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Rendering.Core.Classes.Shaders;
using Rendering.Core.Classes.Shapes;
using Rendering.Core.Classes.Utilities;
using System.Linq;

namespace Rendering.Core.RenderGUI
{
    public partial class RenderGUI : UserControl
    {
        private GLControl glControl;
        private Shader shader;
        private int VertexBufferObject;
        private int VertexArrayObject;

        private Point oldMousePosition;
        private Point newMousePosition;

        private Camera camera;

        private List<GLShape> shapes;


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

            camera.AspectRatio = (float)glControl.Width / (float)glControl.Height;
            RefreshWindow();
        }

        private void GlControl_Load(object sender, EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.0f, 0.0f, 0.10f, 1.0f);

            CreateShapes();
            List<float> allVertices = new List<float>();
            foreach (GLShape shape in shapes)
                allVertices.AddRange(shape.Vertices);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, allVertices.ToArray().Length * sizeof(float), allVertices.ToArray(), BufferUsageHint.StaticDraw);

            foreach (GLShape shape in shapes)
                GL.BufferData(BufferTarget.ArrayBuffer, shape.Vertices.Length * sizeof(float), shape.Vertices, BufferUsageHint.StaticDraw);

            // shader
            string vertexPath = Path.Combine(Environment.CurrentDirectory, @"GLSL\", "Vertex.vert");
            string fragmentPath = Path.Combine(Environment.CurrentDirectory, @"GLSL\", "Fragment.frag");
            shader = new Shader(vertexPath, fragmentPath);
            shader.Use();

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            int vertexLocation = shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            shader.SetInt("texture0", 0);
            shader.SetInt("texture1", 1);

            InitializeCamera();

            Render();
        }

        private void GlControl_Disposed(object sender, EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);

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
                    shape.Rotate(
                        (newMousePosition.Y - oldMousePosition.Y) * 0.1f,
                        (newMousePosition.X - oldMousePosition.X) * 0.1f,
                        0);
                }
                RefreshWindow();
            }

            if (e.Button == MouseButtons.Right)
            {
                float deltaPitch = (newMousePosition.Y - oldMousePosition.Y) * 0.05f;
                float deltaYaw  = (newMousePosition.X - oldMousePosition.X) * 0.05f;

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

        #endregion

        #region Controls

        private
            void btnRefresh_Click(object sender, EventArgs e)
        {
            foreach (GLShape shape in shapes)
            {
                shape.ResetRotation();
            }

            camera.Pitch = 0;
            camera.Yaw = -90;
            camera.Position = new Vector3(0.0f, 0.0f, 3.0f);

            RefreshWindow();
        }

        #endregion


        private void CreateShapes()
        {
            shapes = new List<GLShape>();

            GLCube cube = new GLCube(1, 1, 1);
            cube.SetTexture("Resources\\Textures\\container.png", TextureUnit.Texture0);
            cube.SetTexture("Resources\\Textures\\awesomeface.png", TextureUnit.Texture1);
            shapes.Add(cube);

            cube = new GLCube(0.5f, 0.5f, 0.5f);
            cube.Translate(1, 0, 0);
            shapes.Add(cube);

            cube = new GLCube(0.5f, 0.5f, 0.5f);
            cube.Translate(-1, 0, 0);
            shapes.Add(cube);
            
            cube = new GLCube(0.5f, 0.5f, 0.5f);
            cube.Translate(0, 1, 0);
            shapes.Add(cube);

            cube = new GLCube(0.5f, 0.5f, 0.5f);
            cube.Translate(0, -1, 0);
            shapes.Add(cube);
        }

        private void InitializeCamera()
        {
            camera = new Camera(Vector3.UnitZ * 3, (float)glControl.Width / (float)glControl.Height);
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
                    texture.Key.Use(texture.Value);
                }

                ApplyModelTransforms(shape, out Matrix4 model);
                shader.SetMatrix4("model", model);
                shader.SetMatrix4("view", camera.GetViewMatrix());

                GL.BindVertexArray(VertexArrayObject);

                GL.BufferData(BufferTarget.ArrayBuffer, shape.Vertices.Length * sizeof(float), shape.Vertices, BufferUsageHint.StaticDraw);
                GL.DrawArrays(PrimitiveType.Triangles, 0, shape.Vertices.Length / 5);
            }

            shader.SetMatrix4("projection", camera.GetProjectionMatrix());
            shader.Use();


            glControl.SwapBuffers();
        }

        private void RefreshWindow()
        {
            Render();
        }

        private void ApplyModelTransforms(GLShape shape, out Matrix4 model)
        {
            model = Matrix4.Identity;
            model *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-shape.AngleX));
            model *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-shape.AngleY));
            model *= Matrix4.CreateTranslation(shape.PositionX, shape.PositionY, shape.PositionZ);
        }
    }
}
