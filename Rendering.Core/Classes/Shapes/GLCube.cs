namespace Rendering.Core.Classes.Shapes
{
    class GLCube : GLShape
    {
        public GLCube(string name, float length, float width, float height) 
            : base(name)
        {
            Vertices = new[]
            {
                //Position                      Texture coordinates
                -length/2, -height/2, width/2,  0.0f, 0.0f,
                length/2, -height/2, width/2,   1.0f, 0.0f,
                length/2, height/2, width/2,    1.0f, 1.0f,
                -length/2, height/2, width/2,   0.0f, 1.0f,

                -length/2, -height/2, -width/2, 0.0f, 1.0f,
                length/2, -height/2, -width/2,  1.0f, 1.0f,
                length/2, height/2, -width/2,   1.0f, 0.0f,
                -length/2, height/2, -width/2,  0.0f, 0.0f,
            };

            Indices = new uint[]
            {
                0, 1, 3,
                1, 2, 3,
                
                5, 1, 2,
                5, 6, 2,

                4, 5, 6,
                4, 7, 6,

                0, 3, 7,
                0, 4, 7,

                0, 1, 5,
                0, 4, 5,

                3, 2, 6,
                3, 7, 6
            };
        }
    }
}
