using System;
using OpenTK;
using Rendering.Core.Classes;
using Rendering.Core.Classes.Shapes;
using Rendering.SceneManagement.Components.Node;

namespace Rendering.SceneManagement
{
    public class SceneManager
    {
        public SceneNode RootNode { get; set; }

        public Action SceneChanged;

        public void CreateNodes()
        {
            RootNode = new SceneNode("Root", null, Vector3.Zero, Vector3.Zero);

            GLSphere earth = new GLSphere("earth");
            earth.Radius = 20.0f;
            earth.Rasterization = 256;
            earth.SetTexture("Resources\\Textures\\earth_diffuse.jpg", TextureType.DiffuseMap);
            earth.SetTexture("Resources\\Textures\\earth_specular.png", TextureType.SpecularMap);

            GLSphere earthClouds = new GLSphere("earthClouds");
            earthClouds.Radius = 20.1f;
            earthClouds.Rasterization = 256;
            earthClouds.SetTexture("Resources\\Textures\\earth_clouds.png", TextureType.DiffuseMap);
            earthClouds.SetTexture("Resources\\Textures\\earth_clouds.png", TextureType.SpecularMap);

            GLSphere space = new GLSphere("space");
            space.Radius = 8000.0f;
            space.Rasterization = 256;
            space.SetTexture("Resources\\Textures\\milky_way.jpg", TextureType.DiffuseMap);
            space.SetTexture("Resources\\Textures\\milky_way.jpg", TextureType.SpecularMap);


            SceneNode earthNode = new SceneNode("Earth", earth, Vector3.Zero, new Vector3(90, 0, 0));
            SceneNode cloudNode = new SceneNode("EarthClouds", earthClouds, Vector3.Zero, Vector3.Zero);
            SceneNode spaceNode = new SceneNode("Space", space, Vector3.Zero, Vector3.Zero);
            earthNode.AddChildNode(cloudNode);
            spaceNode.AddChildNode(earthNode);
            RootNode.AddChildNode(spaceNode);

            SceneChanged?.Invoke();
        }
    }
}
