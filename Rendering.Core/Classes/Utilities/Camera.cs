using OpenTK;
using System;


namespace Rendering.Core.Classes.Utilities
{
    public class Camera
    {
        private float fovRadian;
       
        private readonly Vector3 _up = Vector3.UnitY;

        private float _latitude;
        private float _longitude;
        private float _height;

        public float MaxHeight { get; set; }
        public float MinHeight { get; set; }

        public float Height
        {
            get => _height;
            set
            {
                _height = value;
                RecalculatePosition();
            }
        }
        public float Latitude
        {
            get => _latitude;
            set
            {
                var angle = MathHelper.Clamp(value, -1.5f, 1.5f);
                _latitude = angle;
                RecalculatePosition();
            }
        }
        public float Longitude
        {
            get => _longitude;
            set
            {
                _longitude = value;
                RecalculatePosition();
            }
        }
        // The real position of the camera
        public Vector3 Position { get; set; }

        // Aspect ratio of the viewport, used for the projection matrix
        public float AspectRatio { private get; set; }

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

        public Camera(float longitude, float latitude, float height, float aspectRatio)
        {
            _longitude = longitude;
            _latitude = latitude;
            _height = height;

            AspectRatio = aspectRatio;

            Fov = 45.0f;

            RecalculatePosition();
        }

        public Matrix4 GetViewMatrix()
        {
            Matrix4 view = Matrix4.LookAt(Position, new Vector3(0, 0, 0), _up);
            return view;
        }

        // Get the projection matrix using the same method we have used up until this point
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(fovRadian, AspectRatio, 0.01f, 10000f);
        }

        public void Zoom(int delta)
        {
            float difference = 0;

            if (delta > 0)
                difference = Math.Abs(_height - MinHeight);
            if (delta < 0)
                difference = Math.Abs(_height - MaxHeight);

            if (difference > 1)
                difference = 1;

            float moving = delta * 0.005f * difference;

            _height -= moving;

            RecalculatePosition();
        }

        private void RecalculatePosition()
        {
            float x = Height * (float)Math.Cos(_latitude) * (float)Math.Sin(_longitude);
            float y = Height * (float)Math.Sin(_latitude);
            float z = Height * (float)Math.Cos(_latitude) * (float)Math.Cos(_longitude);

            Position = new Vector3(x, y, z);
        }
    }
}
