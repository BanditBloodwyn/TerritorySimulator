using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Rendering.Core.Classes.Shapes
{
    class GLSphere : GLShape
    {
        private int rasterization;
        private float radius;

        public int Rasterization
        {
            get => rasterization;
            set
            {
                rasterization = value;
                RecreateVertices();
            }
        }

        public float Radius
        {
            get => radius;
            set
            {
                radius = value;
                RecreateVertices();
            }
        }

        public GLSphere()
        {
            radius = 1;
            rasterization = 10;
        }

        private void RecreateVertices()
        {
            List<float> vertices = new List<float> { };

            float alpha = 2 * (float)Math.PI / rasterization;

            for (int i = 0; i < rasterization - 1; i++)
            {
                for (int j = 0; j < rasterization - 1; j++)
                {
                    Vector3 vertex = new Vector3(
                        radius * (float)Math.Sin(i * alpha * 0.5) * (float)Math.Sin(j * alpha),
                        radius * (float)Math.Sin(i * alpha * 0.5) * (float)Math.Cos(j * alpha),
                        radius * (float)Math.Cos(i * alpha * 0.5));
                    vertices.AddRange(new float[] { vertex[0], vertex[1], vertex[2], 1, 0});
                }
            }

            Vertices = vertices.ToArray();
        }
    }
}
