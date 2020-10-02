using OpenTK;
using System;

namespace Rendering.Core.Classes.Utilities
{
    public class Camera
    {
        private float fovRadian;
        private float pitchRadian;
        private float yawRadian;
       
        // Those vectors are directions pointing outwards from the camera to define how it rotated
        private Vector3 _front = -Vector3.UnitZ;
        private Vector3 _up = Vector3.UnitY;
        private Vector3 _right = Vector3.UnitX;

        public float MaxHeight { get; set; }
        public float MinHeight { get; set; }

        public float Height => (float)Math.Sqrt(Position[0] * Position[0] + Position[1] * Position[1] + Position[2] * Position[2]);

        // The position of the camera
        public Vector3 Position { get; set; }

        // Aspect ratio of the viewport, used for the projection matrix
        public float AspectRatio { private get; set; }

        public Vector3 Front => _front;

        // Vector pointing upwards from the camera
        public Vector3 Up => _up;

        // Vector pointing to the right from the camera
        public Vector3 Right => _right;

        // convert from degrees to radians as soon as the property is set to improve performance
        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(pitchRadian);
            set
            {
                // We clamp the pitch value between -89 and 89 to prevent the camera from going upside down, and a bunch
                // of weird "bugs" when you are using euler angles for rotation.
                // If you want to read more about this you can try researching a topic called gimbal lock
                var angle = MathHelper.Clamp(value, -89f, 89f);
                pitchRadian = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }

        // We convert from degrees to radians as soon as the property is set to improve performance
        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(yawRadian);
            set
            {
                yawRadian = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }
        // convert from degrees to radians as soon as the property is set to improve performance
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(fovRadian);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 90f);
                fovRadian = MathHelper.DegreesToRadians(angle);
            }
        }

        public Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;

            Fov = 45.0f;
        }

        public Matrix4 GetViewMatrix()
        {
            Matrix4 view = Matrix4.LookAt(Position, Position + _front, _up);
            return view;
        }

        // Get the projection matrix using the same method we have used up until this point
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(fovRadian, AspectRatio, 0.01f, 1000f);
        }

        // This function is going to update the direction vertices using some of the math learned in the web tutorials
        private void UpdateVectors()
        {
            // First the front matrix is calculated using some basic trigonometry
            _front.X = (float)Math.Cos(pitchRadian) * (float)Math.Cos(yawRadian);
            _front.Y = (float)Math.Sin(pitchRadian);
            _front.Z = (float)Math.Cos(pitchRadian) * (float)Math.Sin(yawRadian);

            // We need to make sure the vectors are all normalized, as otherwise we would get some funky results
            _front = Vector3.Normalize(_front);

            // Calculate both the right and the up vector using cross product
            // Note that we are calculating the right from the global up, this behaviour might
            // not be what you need for all cameras so keep this in mind if you do not want a FPS camera
            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }

        public void Zoom(int delta)
        {
            float difference = 0;

            if (delta > 0)
                difference = Math.Abs(Height - MinHeight);
            if (delta < 0)
                difference = Math.Abs(Height - MaxHeight);

            if (difference > 1)
                difference = 1;

            Vector3 moving = Front * delta * 0.002f * difference;

            Position += moving;
        }
    }
}
