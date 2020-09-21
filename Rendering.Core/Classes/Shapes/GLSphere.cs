using OpenTK;
using System;
using System.Collections.Generic;


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
                RecreateIndices();
            }
        }

        public float Radius
        {
            get => radius;
            set
            {
                radius = value;
                RecreateVertices();
                RecreateIndices();
            }
        }

        public GLSphere()
        {
            radius = 1;
            rasterization = 10;
        }

        private void RecreateVertices()
        {
            List<float> vertices = new List<float>();

            float alpha = 2 * (float)Math.PI / rasterization;

            for (int i = 0; i < rasterization + 1; i++)
            {
                for (int j = 0; j < rasterization + 1; j++)
                {
                    Vector3 vertex = new Vector3(
                        radius * (float)Math.Sin(i * alpha * 1.0) * (float)Math.Sin(j * alpha),
                        radius * (float)Math.Sin(i * alpha * 1.0) * (float)Math.Cos(j * alpha),
                        radius * (float)Math.Cos(i * alpha * 1.0));
                    vertices.AddRange(new[] { vertex[0], vertex[1], vertex[2], 1.0f, 0.0f});
                }
            }

            Vertices = vertices.ToArray();
        }

        private void RecreateIndices()
        {
            List<uint> indices = new List<uint>();

            for (uint i = 0; i < rasterization; ++i)
            {
                uint k1 = i * ((uint)rasterization + 1);     // beginning of current stack
                uint k2 = k1 + (uint)rasterization + 1;      // beginning of next stack

                for (int j = 0; j < rasterization; ++j, ++k1, ++k2)
                {
                    // 2 triangles per sector excluding first and last stacks
                    // k1 => k2 => k1+1
                    if (i != 0)
                    {
                        indices.Add(k1);
                        indices.Add(k2);
                        indices.Add(k1 + 1);
                    }

                    // k1+1 => k2 => k2+1
                    if (i != (rasterization - 1))
                    {
                        indices.Add(k1 + 1);
                        indices.Add(k2);
                        indices.Add(k2 + 1);
                    }
                }
            }

            Indices = indices.ToArray();
        }
    }
}
