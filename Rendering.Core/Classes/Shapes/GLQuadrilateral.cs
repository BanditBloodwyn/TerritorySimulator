using OpenTK;


namespace Rendering.Core.Classes.Shapes
{
    class GLQuadrilateral : GLShape
    {
        public GLQuadrilateral(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Vector3 vertex4)
        {
            Vertices = new float[]
            {
                //Position                          Texture coordinates
                vertex1[0], vertex1[1], vertex1[2], 1.0f, 1.0f,  // top right
                vertex2[0], vertex2[1], vertex2[2], 1.0f, 0.0f,  // bottom right
                vertex3[0], vertex3[1], vertex3[2], 0.0f, 0.0f,  // bottom left
                vertex4[0], vertex4[1], vertex4[2], 0.0f, 1.0f   // top left
            };

            Indices = new uint[]
            {               // note that we start from 0!
                0, 1, 3,    // first triangle
                1, 2, 3     // second triangle
            };
        }
    }
}