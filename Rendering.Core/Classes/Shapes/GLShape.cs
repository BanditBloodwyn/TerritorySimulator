using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;


namespace Rendering.Core.Classes.Shapes
{
    public class GLShape
    {
        public string Name { get; protected set; }

        public float[] Vertices { get; protected set; }
        public uint[] Indices { get; protected set; }

        public int VertexBufferSize => Vertices.Length * sizeof(float);
        public int IndexBufferSize => Indices.Length * sizeof(int);

        public int VertexArrayObject { get; set; }


        public Dictionary<TextureType, Texture> Textures { get; }

        public GLShape(string name)
        {
            Name = name;
            Textures = new Dictionary<TextureType, Texture>();

            SetTexture("Resources\\Textures\\transparent.png", TextureType.Transparent);
        }


        public void SetTexture(string path, TextureType type)
        {
            Texture texture;

            switch(type)
            {
                case TextureType.Transparent:
                    texture = new Texture(path);
                    break;
                case TextureType.DiffuseMap:
                    texture = new Texture(path);
                    break;
                case TextureType.SpecularMap:
                    texture = new Texture(path, TextureUnit.Texture1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            texture.Use();

            Textures.Add(type, texture);
        }
    }
}
