namespace Rendering.Core.Classes.Shapes
{
    class GLQuadrilateral : GLShape
    {
        public GLQuadrilateral(float length, float width)
        {
            Vertices = new[]
            {
                //Position              Texture coordinates
                -length/2, -width/2, 0, 1.0f, 1.0f,
                length/2, -width/2, 0, 1.0f, 0.0f,
                length/2, width/2, 0, 0.0f, 0.0f,
                -length/2, -width/2, 0, 1.0f, 1.0f,
                length/2, width/2, 0, 0.0f, 0.0f,
                -length/2, width/2, 0, 0.0f, 1.0f,
            };
        }
    }
}