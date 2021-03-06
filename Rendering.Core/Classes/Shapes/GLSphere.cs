﻿using System;
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

            RecreateVertices();
            RecreateIndices();
        }

        private void RecreateVertices()
        {
            List<float> vertices = new List<float>();

            float alpha = 2 * (float)Math.PI / rasterization;

            for (int i = 0; i < rasterization + 1; i++)
            {
                for (int j = 0; j < rasterization + 1; j++)
                {
                    float x = radius * (float)Math.Sin(i * alpha * 1.0) * (float)Math.Sin(j * alpha);
                    float y = radius * (float)Math.Sin(i * alpha * 1.0) * (float)Math.Cos(j * alpha);
                    float z = radius * (float)Math.Cos(i * alpha * 1.0);

                    float textureX = (float)j / rasterization;
                    float textureY = (float)i / rasterization * 2;

                    vertices.AddRange(new[] { x, y, z, textureX, textureY });
                }
            }

            Vertices = vertices.ToArray();
        }

        private void RecreateIndices()
        {
            List<uint> indices = new List<uint>();

            for (uint i = 0; i < rasterization / 2; ++i)
            {
                uint k1 = i * ((uint)rasterization + 1);
                uint k2 = k1 + (uint)rasterization + 1;

                for (int j = 0; j < rasterization; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        indices.Add(k1);
                        indices.Add(k2);
                        indices.Add(k1 + 1);
                    }

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
