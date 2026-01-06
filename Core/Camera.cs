
using Silk.NET.Input;
using Matrix4 = System.Numerics.Matrix4x4;
using Vector3 = System.Numerics.Vector3;
using Vector2 = System.Numerics.Vector2;

namespace GameStudies.Core
{
    public class Camera
    {
        public Vector3 Position = new(1.0f, 0.0f, 7.0f);
        public Vector3 Target;
        public Vector3 Front { get; private set; } = -Vector3.UnitZ;
        public Vector3 Up { get; private set; } = Vector3.UnitY;

        public float Yaw { get; private set; } = -90f;
        public float Pitch { get; private set; } = 0f;

        public float Speed { get; set; } = 2.5f;
        public float Sensitvity { get; set; } = 0.09f;

        public float AspectRatio = 800.0f / 600.0f;
        public float Fov { get; private set; } = 45f;

        public float Degress = 45f;

        public Matrix4 ViewMatrix => Matrix4.CreateLookAt(Position, Position + Front, Up);

        public Matrix4 ProjectionMatrix =>
        Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(Fov),
            AspectRatio,
            0.1f,
            100f);

        public Camera() { }

        public void ProcessKeyboard(IKeyboard kb, float deltaTime)
        {
            float velocity = Speed * deltaTime;

            var right = Vector3.Normalize(Vector3.Cross(Front, Up));
            var up = Vector3.Normalize(Vector3.Cross(right, Front));

            if (kb.IsKeyPressed(Key.W)) Position += Front * velocity;
            if (kb.IsKeyPressed(Key.S)) Position -= Front * velocity;

            if (kb.IsKeyPressed(Key.A)) Position -= right * velocity;
            if (kb.IsKeyPressed(Key.D)) Position += right * velocity;

            if (kb.IsKeyPressed(Key.Space)) Position += up * velocity;
            if (kb.IsKeyPressed(Key.ControlLeft)) Position -= up * velocity;
        }
        public void ProcessMouseMovement(Vector2 delta)
        {
            delta.X *= Sensitvity;
            delta.Y *= Sensitvity;

            Yaw += delta.X;
            Pitch = Math.Clamp(Pitch + delta.Y, -89f, 89f);

            UpdateVectors();
        }

        public void ProcessMouseScroll(ScrollWheel wheel)
        {
            Fov -= wheel.Y;
            Fov = Math.Clamp(Fov, 1f, 90f);
        }

        private void UpdateVectors()
        {
            Vector3 front;
            front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
            front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            Front = Vector3.Normalize(front);

        }

    }
}