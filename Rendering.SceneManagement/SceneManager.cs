using System.Collections.Generic;
using Rendering.Core.Classes;
using Rendering.Core.Classes.Shapes;
using Rendering.SceneManagement.Components.Scene;

namespace Rendering.SceneManagement
{
    public class SceneManager
    {
        public Scene[] Scenes { get; set; }

        public SceneManager()
        {

        }

        public GLShape[] CreateShapes()
        {
            List<GLShape> shapes = new List<GLShape>();

            var earth = new GLSphere("earth");
            earth.Radius = 20.0f;
            earth.Rasterization = 256;
            earth.SetTexture("Resources\\Textures\\earth_diffuse.jpg", TextureType.DiffuseMap);
            earth.SetTexture("Resources\\Textures\\earth_specular.png", TextureType.SpecularMap);
            shapes.Add(earth);

            var earthClouds = new GLSphere("earthClouds");
            earthClouds.Radius = 20.1f;
            earthClouds.Rasterization = 256;
            earthClouds.SetTexture("Resources\\Textures\\earth_clouds.png", TextureType.DiffuseMap);
            earthClouds.SetTexture("Resources\\Textures\\earth_clouds.png", TextureType.SpecularMap);
            shapes.Add(earthClouds);

            var stars = new GLSphere("space");
            stars.Radius = 8000.0f;
            stars.Rasterization = 256;
            stars.SetTexture("Resources\\Textures\\milky_way.jpg", TextureType.DiffuseMap);
            stars.SetTexture("Resources\\Textures\\milky_way.jpg", TextureType.SpecularMap);
            shapes.Add(stars);

            return shapes.ToArray();

        }
    }
}
