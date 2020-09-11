using System;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Rendering.Core.Classes;
using Rendering.Core.Classes.Shaders;
using System.IO;
using System.Drawing;

namespace Rendering.Core.RenderGUI
{
    public partial class RenderGUI : UserControl
    {
        private GLControl glControl;
        private Shader shader;
        private Texture texture1;
        private Texture texture2;
        private int VertexBufferObject;
        private int VertexArrayObject;
        private int ElementBufferObject;

        private float angleX;
        private float angleY;
        private float angleZ;

        private Point oldMousePosition;
        private Point newMousePosition;

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
            glControl.MouseWheel += GlControl_MouseWheel;
            glControl.MouseMove += GlControl_MouseMove;
            glControl.MouseUp += GlControl_MouseUp;


            pnlGL.Controls.Add(glControl);
        }


        #region GlControl

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
           
            shader.SetInt("texture0", 0);
            shader.SetInt("texture1", 1);

            angleX = 0.0f;
            angleY = 0.0f;
            angleZ = 0.0f;
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

        private void GlControl_MouseWheel(object sender, MouseEventArgs e)
        {
            angleX += e.Delta / 30;

            RefreshWindow();
        }

        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            newMousePosition = e.Location;

            if (e.Button == MouseButtons.Middle)
            {

                angleY += (newMousePosition.X - oldMousePosition.X) * 0.1f;
                angleX += (newMousePosition.Y - oldMousePosition.Y) * 0.1f;
                
                oldMousePosition = e.Location;
            }

            RefreshWindow();
        }

        private void GlControl_MouseUp(object sender, MouseEventArgs e)
        {
            oldMousePosition = e.Location;
            newMousePosition = e.Location;
        }

        #endregion


        private void InitTextures()
        {
            texture1 = new Texture("Resources\\Textures\\container.png");
            texture2 = new Texture("Resources\\Textures\\awesomeface.png"); 
            
            texture1.Use(TextureUnit.Texture0);
            texture2.Use(TextureUnit.Texture1);
        }

        private void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            ApplyTransforms(out Matrix4 transform);

            texture1.Use(TextureUnit.Texture0);
            texture2.Use(TextureUnit.Texture1);

            shader.SetMatrix4("transform", transform);
            shader.Use();

            GL.BindVertexArray(VertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            
            glControl.SwapBuffers();
        }

        private void RefreshWindow()
        {
            Render();
        }

        private void ApplyTransforms(out Matrix4 transform)
        {
            transform = Matrix4.Identity;
            transform *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(angleX));
            transform *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(angleY));
            transform *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(angleZ));
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            angleX = angleY = angleZ = 0;
            RefreshWindow();
        }
    }
}
