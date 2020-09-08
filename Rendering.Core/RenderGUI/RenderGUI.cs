using System;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Rendering.Core.Classes;
using Rendering.Core.Classes.Shaders;
using System.IO;


namespace Rendering.Core.RenderGUI
{
    public partial class RenderGUI : UserControl
    {
        private GLControl glControl;
        private Shader shader;
        private Texture texture;
        private int VertexBufferObject;
        private int VertexArrayObject;
        private int ElementBufferObject;

        private float[] vertices =
            {   
                //Position         Texture coordinates
                0.5f,  0.5f, 0.0f, 1.0f, 1.0f,  // top right
                0.5f, -0.5f, 0.0f, 1.0f, 0.0f,  // bottom right
               -0.5f, -0.5f, 0.0f, 0.0f, 0.0f,  // bottom left
               -0.5f,  0.5f, 0.0f, 0.0f, 1.0f   // top left
            };

        private uint[] indices = 
            {               // note that we start from 0!
                0, 1, 3,    // first triangle
                1, 2, 3     // second triangle
            };

        private float[] texCoords =
            {
                0.0f, 0.0f,  // lower-left corner  
                1.0f, 0.0f,  // lower-right corner
                0.5f, 1.0f   // top-center corner
            };


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


        #region GlControl

        private void GlControl_Paint(object sender, PaintEventArgs e)
        {
            RefreshWindow();
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

            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

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
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);

            InitTextures();

            Render();
        }

        private void GlControl_Disposed(object sender, EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteBuffer(ElementBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);

            GL.DeleteProgram(shader.Handle);
            shader.Dispose();
        }

        private void InitTextures()
        {
            texture = new Texture("Resources\\Textures\\container.png");
            texture.Use();
        }

        #endregion


        private void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            shader.Use();
            texture.Use();

            GL.BindVertexArray(VertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            
            glControl.SwapBuffers();
        }

        private void RandomizeBackgroudColor()
        {
            Random random = new Random();
            GL.ClearColor((float)random.NextDouble() / 4, (float)random.NextDouble() / 4, (float)random.NextDouble() / 4, 1.0f);
        }

        private void RandomizeTriangles()
        {
            Random random = new Random();

            for (int i = 0; i < vertices.Length; i++)
            {
                if (i == 3 || i == 8 || i == 13)
                    i += 2;
                vertices[i] = (float)random.NextDouble() * 2 - 1;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshWindow();
        }

        private void RefreshWindow()
        {
            RandomizeBackgroudColor();
            RandomizeTriangles();

            Render();
        }
    }
}
