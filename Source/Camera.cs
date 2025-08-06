using OpenTK.Mathematics;

namespace GameStudies.Source
{
    public class Camera
    {
        public Vector3 Position;
        public Vector3 Target;
        public float AspectRatio = 800.0f / 600.0f;

        public Matrix4 ViewMatrix => Matrix4.CreateTranslation(1.0f, 0.0f, -3.0f);

        public Matrix4 ProjectionMatrix =>
        Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45f),
            AspectRatio,
            0.1f,
            100f);

        public Camera() { }

    }
}