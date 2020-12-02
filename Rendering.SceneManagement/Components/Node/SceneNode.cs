using System.Linq;
using OpenTK;
using Rendering.Core.Classes.Shapes;

namespace Rendering.SceneManagement.Components.Node
{
    public class SceneNode
    {
        private int vertexBufferSizeBuffer;
        private int indexBufferSizeBuffer;
        public string Name { get; set; }
        public SceneNode ParentNode { get; set; }
        public SceneNode[] ChildNodes { get; set; }
        public GLShape NodeShape { get; set; }

        public Matrix4 WorldTransform { get; set; }

        public SceneNode(string name, GLShape shape, Vector3 position, Vector3 rotation)
        {
            Name = name;
            NodeShape = shape;
            ChildNodes = new SceneNode[] {};
            WorldTransform = CreateTransformMatrix(position, rotation);

            vertexBufferSizeBuffer = 0;
            indexBufferSizeBuffer = 0;
        }

        private Matrix4 CreateTransformMatrix(Vector3 position, Vector3 rotation)
        {
            Matrix4 model = Matrix4.Identity;
            model *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotation.X));
            model *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation.Y));
            model *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation.Z));
            model *= Matrix4.CreateTranslation(position.X, position.Y, position.Z);
            return model;
        }

        public void AddChildNode(SceneNode child)
        {
            child.ParentNode = this;

            var childList = ChildNodes.ToList();
            childList.Add(child);
            ChildNodes = childList.ToArray();
        }

        public void Rotate(float deltaX, float deltaY, float deltaZ)
        {
            Matrix4 model = Matrix4.Identity;
            model *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(deltaX));
            model *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(deltaY));
            model *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(deltaZ));

            WorldTransform *= model;
        }

        public void RotateTo(float x, float y, float z)
        {
            Matrix4 model = Matrix4.Identity;
            model *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(x));
            model *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(y));
            model *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(z));

            WorldTransform = model;
        }

        public void Translate(float deltaX, float deltaY, float deltaZ)
        {
            Matrix4 model = Matrix4.Identity;
            model *= Matrix4.CreateTranslation(deltaX, deltaY, deltaZ);

            WorldTransform *= model;
        }

        public void TranslateTo(float x, float y, float z)
        {
            Matrix4 model = Matrix4.Identity;
            model *= Matrix4.CreateTranslation(x, y, z);

            WorldTransform = model;
        }

        public int GetVertexBufferSize()
        {
            if (NodeShape != null)
                vertexBufferSizeBuffer += NodeShape.VertexBufferSize;

            foreach (SceneNode childNode in ChildNodes)
            {
                vertexBufferSizeBuffer += childNode.GetVertexBufferSize();
            }

            return vertexBufferSizeBuffer;
        }

        public int GetIndexBufferSize()
        {
            if (NodeShape != null)
                indexBufferSizeBuffer += NodeShape.IndexBufferSize;

            foreach (SceneNode childNode in ChildNodes)
            {
                indexBufferSizeBuffer += childNode.GetIndexBufferSize();
            }

            return indexBufferSizeBuffer;
        }

        public void ResetRotation()
        {
            RotateTo(0, 0, 0);
        }
    }
}
