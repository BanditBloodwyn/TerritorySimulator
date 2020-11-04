namespace Rendering.Core.Classes.Shapes
{
    // TODO: Add normal calculation
    class GLQuadrilateral : GLShape
    {
        public GLQuadrilateral(string name, float length, float width)
            : base(name)
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