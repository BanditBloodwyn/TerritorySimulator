using System;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Rendering.Core.Classes;
using Rendering.Core.Classes.Shaders;
using System.IO;
using System.Drawing;
using Rendering.Core.Classes.Shapes;
using System.Collections.Generic;

namespace Rendering.Core.RenderGUI
{
    public partial class RenderGUI : UserControl
    {
        private GLControl glControl;
        private Shader shader;
        private int VertexBufferObject;
        private int VertexArrayObject;
        private int ElementBufferObject;

        private Point oldMousePosition;
        private Point newMousePosition;

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
            GL.Enable(EnableCap.DepthTest);
            
            CreateShapes();

            GL.ClearColor(0.0f, 0.0f, 0.15f, 1.0f);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);

            foreach (GLShape shape in shapes)
            {
                GL.BufferData(BufferTarget.ArrayBuffer, shape.Vertices.Length * sizeof(float), shape.Vertices, BufferUsageHint.StaticDraw);
                GL.BufferData(BufferTarget.ElementArrayBuffer, shape.Indices.Length * sizeof(uint), shape.Indices, BufferUsageHint.StaticDraw);
            }

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
           
            shader.SetInt("texture0", 0);
            shader.SetInt("texture1", 1);

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

        private void GlControl_MouseWheel(object sender, MouseEventArgs e)
        {
            // TODO: implement zooming here
            RefreshWindow();
        }

        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            newMousePosition = e.Location;

            if (e.Button == MouseButtons.Left)
            {
                foreach(GLShape shape in shapes)
                {
                    shape.Rotate(
                        (newMousePosition.Y - oldMousePosition.Y) * 0.1f,
                        (newMousePosition.X - oldMousePosition.X) * 0.1f, 
                        0);
                }
               
                RefreshWindow();
            }

            oldMousePosition = e.Location;
        }

        private void GlControl_MouseUp(object sender, MouseEventArgs e)
        {
            oldMousePosition = e.Location;
            newMousePosition = e.Location;
        }

        #endregion


        private void CreateShapes()
        {
            shapes = new List<GLShape>();

            GLRectangle rectangle = new GLRectangle(
                new Vector3(0.5f,  0.5f, 0.0f),
                new Vector3(0.5f, -0.5f, 0.0f),
                new Vector3(-0.5f, -0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f));
            rectangle.SetTexture("Resources\\Textures\\container.png", TextureUnit.Texture0);
            rectangle.SetTexture("Resources\\Textures\\awesomeface.png", TextureUnit.Texture1);
            shapes.Add(rectangle);

            rectangle = new GLRectangle(
                new Vector3(0.2f, 0.2f, 0.5f),
                new Vector3(0.2f, -0.2f, -0.5f),
                new Vector3(-0.2f, -0.2f, -0.5f),
                new Vector3(-0.2f, 0.2f, 0.5f));
            shapes.Add(rectangle);
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

                GL.DrawElements(PrimitiveType.Triangles, shape.Indices.Length, DrawElementsType.UnsignedInt, 0);
                GL.BufferData(BufferTarget.ArrayBuffer, shape.Vertices.Length * sizeof(float), shape.Vertices, BufferUsageHint.StaticDraw);
                
                ApplyTransforms(shape, out Matrix4 transform);
                shader.SetMatrix4("transform", transform);
                shader.Use();
            }

            GL.BindVertexArray(VertexArrayObject);

            glControl.SwapBuffers();
        }

        private void RefreshWindow()
        {
            Render();
        }

        private void ApplyTransforms(GLShape shape, out Matrix4 transform)
        {
            transform = Matrix4.Identity;
            transform *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-shape.AngleX));
            transform *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-shape.AngleY));
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            foreach(GLShape shape in shapes)
            {
                shape.ResetRotation();
            }

            RefreshWindow();
        }
    }
}
