using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rendering.Core.Classes.Shapes
{
    class GLCube : GLShape
    {
        public GLCube(float length, float width, float height)
        {
            Vertices = new float[]
            {
                -length/2, -height/2, -width/2,  0.0f, 0.0f,
                length/2, -height/2, -width/2,  1.0f, 0.0f,
                length/2,  height/2, -width/2,  1.0f, 1.0f,
                length/2,  height/2, -width/2,  1.0f, 1.0f,
                -length/2,  height/2, -width/2,  0.0f, 1.0f,
                -length/2, -height/2, -width/2,  0.0f, 0.0f,
                
                -length/2, -height/2,  width/2,  0.0f, 0.0f,
                length/2, -height/2,  width/2,  1.0f, 0.0f,
                length/2,  height/2,  width/2,  1.0f, 1.0f,
                length/2,  height/2,  width/2,  1.0f, 1.0f,
                -length/2,  height/2,  width/2,  0.0f, 1.0f,
                -length/2, -height/2,  width/2,  0.0f, 0.0f,
                
                -length/2,  height/2,  width/2,  1.0f, 0.0f,
                -length/2,  height/2, -width/2,  1.0f, 1.0f,
                -length/2, -height/2, -width/2,  0.0f, 1.0f,
                -length/2, -height/2, -width/2,  0.0f, 1.0f,
                -length/2, -height/2,  width/2,  0.0f, 0.0f,
                -length/2,  height/2,  width/2,  1.0f, 0.0f,

                length/2,  height/2,  width/2,  1.0f, 0.0f,
                length/2,  height/2, -width/2,  1.0f, 1.0f,
                length/2, -height/2, -width/2,  0.0f, 1.0f,
                length/2, -height/2, -width/2,  0.0f, 1.0f,
                length/2, -height/2,  width/2,  0.0f, 0.0f,
                length/2,  height/2,  width/2,  1.0f, 0.0f,
                
                -length/2, -height/2, -width/2,  0.0f, 1.0f,
                length/2, -height/2, -width/2,  1.0f, 1.0f,
                length/2, -height/2,  width/2,  1.0f, 0.0f,
                length/2, -height/2,  width/2,  1.0f, 0.0f,
                -length/2, -height/2,  width/2,  0.0f, 0.0f,
                -length/2, -height/2, -width/2,  0.0f, 1.0f,
                
                -length/2,  height/2, -width/2,  0.0f, 1.0f,
                length/2,  height/2, -width/2,  1.0f, 1.0f,
                length/2,  height/2,  width/2,  1.0f, 0.0f,
                length/2,  height/2,  width/2,  1.0f, 0.0f,
                -length/2,  height/2,  width/2,  0.0f, 0.0f,
                -length/2,  height/2, -width/2,  0.0f, 1.0f
            };

            Indices = new uint[]
            {

            };
        }
    }
}
