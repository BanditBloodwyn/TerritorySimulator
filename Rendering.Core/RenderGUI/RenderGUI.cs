using System;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Rendering.Core.Shaders;
using System.IO;


namespace Rendering.Core.RenderGUI
{
    public partial class RenderGUI : UserControl
    {
        private GLControl glControl;
        private Shader shader;
        private int VertexBufferObject;
        private int VertexArrayObject;


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

            pnlGL.Controls.Add(glControl);
        }

        private void GlControl_Paint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Render();
            
            glControl.SwapBuffers();
        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
        }

        private void GlControl_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.0f, 0.0f, 0.2f, 1.0f);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            // shader
            string vertexPath = Path.Combine(Environment.CurrentDirectory, @"GLSL\", "Vertex.vert");
            string fragmentPath = Path.Combine(Environment.CurrentDirectory, @"GLSL\", "Fragment.frag");
            shader = new Shader(vertexPath, fragmentPath);
            
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);
            
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            Render();
        }

        private void GlControl_Disposed(object sender, EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertexBufferObject);

            shader.Dispose();
        }

        private void RandomizeBackgroudColor()
        {
            Random random = new Random();
            GL.ClearColor((float)random.NextDouble()/4, (float)random.NextDouble()/4, (float)random.NextDouble()/4, 1.0f);
        }

        private void Render()
        {
            float[] vertices =
                { -0.5f, -0.5f, 0.0f,   //Bottom-left vertex
                   0.5f, -0.5f, 0.0f,   //Bottom-right vertex
                   0.0f,  0.5f, 0.0f }; //Top vertex

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            shader.Use();
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            RandomizeBackgroudColor();    // Just for fun & testing
        }
    }
}
