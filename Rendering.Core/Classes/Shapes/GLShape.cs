using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;


namespace Rendering.Core.Classes.Shapes
{
    public class GLShape
    {
        public string Name { get; protected set; }

        public float[] Vertices { get; protected set; }
        public uint[] Indices { get; protected set; }
        public float[] Normals { get; protected set; }

        public float AngleX { get; protected set; }
        public float AngleY { get; protected set; }
        public float AngleZ { get; protected set; }

        public float PositionX { get; protected set; }
        public float PositionY { get; protected set; }
        public float PositionZ { get; protected set; }

        public int VertexBufferSize => Vertices.Length * sizeof(float);
        public int IndexBufferSize => Indices.Length * sizeof(int);

        public int VertexArrayObject { get; set; }


        public Dictionary<TextureType, Texture> Textures { get; }

        public GLShape(string name)
        {
            Name = name;
            Textures = new Dictionary<TextureType, Texture>();
            ResetRotation();
            ResetTranslation();

            SetTexture("Resources\\Textures\\transparent.png", TextureType.Invisible);
        }


        public void SetTexture(string path, TextureType type = TextureType.Visible, TextureUnit unit = TextureUnit.Texture0)
        {
            Texture texture = new Texture(path, unit);
            texture.Use();

            Textures.Add(type, texture);
        }

        public void Rotate(float deltaX, float deltaY, float deltaZ)
        {
            AngleX += deltaX;
            AngleY += deltaY;
            AngleZ += deltaZ;
        }

        public void Translate(float deltaX, float deltaY, float deltaZ)
        {
            PositionX += deltaX;
            PositionY += deltaY;
            PositionZ += deltaZ;
        }

        public void ResetRotation()
        {
            AngleX = 90.0f;
            AngleY = 0.0f;
            AngleZ = 0.0f;
        }

        public void ResetTranslation()
        {
            PositionX = 0.0f;
            PositionY = 0.0f;
            PositionZ = 0.0f;
        }
    }
}
