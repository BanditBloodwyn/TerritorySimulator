using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rendering.Core.Classes.Shapes;

namespace Rendering.SceneManagement.Components.Group
{
    public class SceneGroup
    {
        public SceneGroup[] SubGroups { get; set; }

        public GLShape[] GroupShapes { get; set; }

        public SceneGroup()
        {

        }
    }
}
