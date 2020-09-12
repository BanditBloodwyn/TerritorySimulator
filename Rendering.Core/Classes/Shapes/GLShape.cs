using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;


namespace Rendering.Core.Classes.Shapes
{
    class GLShape
    {
        public float[] Vertices { get; protected set; }
        public uint[] Indices { get; protected set; }
        public float[] TexCoords { get; protected set; }

        public float AngleX { get; protected set; }
        public float AngleY { get; protected set; }
        public float AngleZ { get; protected set; }

        public float PositionX { get; protected set; }
        public float PositionY { get; protected set; }
        public float PositionZ { get; protected set; }

        public Dictionary<Texture, TextureUnit> Textures
        {
            get;
            private set;
        }

        public GLShape()
        {
            Textures = new Dictionary<Texture, TextureUnit>();
            ResetRotation();  
        }


        public void SetTexture(string path, TextureUnit unit = TextureUnit.Texture0)
        {
            Texture texture = new Texture(path);
            texture.Use(unit);

            Textures.Add(texture, unit);
        }

        public void Rotate(float deltaX, float deltaY, float deltaZ)
        {
            AngleX += deltaX;
            AngleY += deltaY;
            AngleZ += deltaZ;
        }

        public void ResetRotation()
        {
            AngleX = 0.0f;
            AngleY = 0.0f;
            AngleZ = 0.0f;
        }
    }
}
