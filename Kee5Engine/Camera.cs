using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kee5Engine
{
    public class Camera
    {
        // Directional Unit Vectors
        private Vector3 _front = -Vector3.UnitZ;
        private Vector3 _up = Vector3.UnitY;
        private Vector3 _right = Vector3.UnitX;

        // Rotation around the X axis
        private float _pitch;

        // Rotation around the Y axis
        private float _yaw = -MathHelper.PiOver2;

        // FOV
        private float _fov = MathHelper.PiOver2;

        /// <summary>
        /// Create a new Camera
        /// </summary>
        /// <param name="position">Start position</param>
        /// <param name="aspectRatio">Aspect Ratio</param>
        /// <param name="speed">Movement Speed</param>
        /// <param name="sensitivity">Mouse Sensitivity</param>
        public Camera(Vector3 position, float aspectRatio, float speed, float sensitivity)
        {
            Position = position;
            AspectRatio = aspectRatio;
            Speed = speed;
            Sensitivity = sensitivity;
        }

        public float Speed { get; set; }
        public float Sensitivity { get; set; }

        // Position of the camera
        public Vector3 Position { get; set; }

        // Aspect ratio of the viewport
        public float AspectRatio { private get; set; }

        public Vector3 Front => _front;
        public Vector3 Up => _up;
        public Vector3 Right => _right;

        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                // Clamp between -89 and 89 to prevent the camera from going upside down
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }

        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }

        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 45f);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }

        /// <summary>
        /// Gets the view matrix based on the Camera's position
        /// </summary>
        /// <returns><code>Matrix4</code> view Matrix</returns>
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + _front, _up);
        }

        /// <summary>
        /// Gets the projection matrix based on the window size
        /// </summary>
        /// <returns><code>Matrix4</code> projection Matrix</returns>
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreateOrthographicOffCenter(0, Globals.windowSize.X, Globals.windowSize.Y, 0, 0.01f, 100f);
        }

        // Spooky Math
        private void UpdateVectors()
        {
            _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            _front.Y = MathF.Sin(_pitch);
            _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

            _front = Vector3.Normalize(_front);

            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }

        /// <summary>
        /// Update the camera's position based on buttons pressed
        /// </summary>
        /// <param name="deltaTime">Time passed since last update call</param>
        public void Update(double deltaTime)
        {
            if (Globals.gameState == GameState.PLAYING)
            {
                Position = new Vector3(Globals.level.GetPlayerPos().X - Window.WindowSize.X / 2, Globals.level.GetPlayerPos().Y - Window.WindowSize.Y / 2, Position.Z);
            }
            //if (Window.inputHandler.IsKeyDown(Keys.W))
            //{
            //    Position -= Up * Speed * (float)deltaTime;
            //}

            //if (Window.inputHandler.IsKeyDown(Keys.S))
            //{
            //    Position += Up * Speed * (float)deltaTime;
            //}

            //if (Window.inputHandler.IsKeyDown(Keys.A))
            //{
            //    Position -= Right * Speed * (float)deltaTime;
            //}

            //if (Window.inputHandler.IsKeyDown(Keys.D))
            //{
            //    Position += Right * Speed * (float)deltaTime;
            //}
        }
    }
}
