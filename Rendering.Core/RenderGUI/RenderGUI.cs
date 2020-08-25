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

        private readonly float[] vertices =
            { -0.5f, -0.5f, 1.0f,   //Bottom-left vertex
               0.5f, -0.5f, 0.5f,   //Bottom-right vertex
               0.0f,  0.5f, 0.0f }; //Top vertex


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
            Render();
            
        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
        }

        private void GlControl_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.0f, 0.0f, 0.15f, 1.0f);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // shader
            string vertexPath = Path.Combine(Environment.CurrentDirectory, @"GLSL\", "Vertex.vert");
            string fragmentPath = Path.Combine(Environment.CurrentDirectory, @"GLSL\", "Fragment.frag");
            shader = new Shader(vertexPath, fragmentPath);
            
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);
            
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            Render();
        }

        private void GlControl_Disposed(object sender, EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0); 
            
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);

            GL.DeleteProgram(shader.Handle);
            shader.Dispose();
        }

        private void RandomizeBackgroudColor()
        {
            Random random = new Random();
            GL.ClearColor((float)random.NextDouble()/4, (float)random.NextDouble()/4, (float)random.NextDouble()/4, 1.0f);
        }

        private void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            shader.Use();
            GL.BindVertexArray(VertexArrayObject);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            glControl.SwapBuffers();
            
            //RandomizeBackgroudColor();    // Just for fun & testing
        }
    }
}
