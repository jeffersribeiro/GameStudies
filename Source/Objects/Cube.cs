using GameStudies.Factory;
using GameStudies.Source;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameStudies.Obects
{
    public class CubeObject
    {
        public Mesh Mesh { get; }
        public Vector3 Position = new(0.0f, 0.0f, -0.0f);
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; } = Vector3.One;
        public float Speed { get; set; } = 1.5f;

        private readonly Shader _shader;

        public CubeObject(Shader shader, Vector3 startPosition, float size = 1f)
        {
            _shader = shader;
            Position = startPosition;
            var vertices = CubeFactory.CreateVertices();

            string[] texPaths = [
                "C:/Users/Jeffe/OneDrive/Documents/Projects/GameStudies/assets/container_texture.png",
                "C:/Users/Jeffe/OneDrive/Documents/Projects/GameStudies/assets/smile_emoji.png",
            ];

            Mesh = new Mesh(_shader, vertices, texPaths);

            Mesh.Load();
            Mesh.LoadTextures();
        }

        public void Dispose()
        {
            Mesh.Dispose();
        }

        public void SetRotationX(float value) => Rotation = new(value, Rotation.Y, Rotation.Z);
        public void SetRotationY(float value) => Rotation = new(Rotation.X, value, Rotation.Z);
        public void SetRotationZ(float value) => Rotation = new(Rotation.X, Rotation.Y, value);
        public void SetScale(float scale) => Scale = new(scale);
        public void SetPositionX(float value) => Position = new(value, Position.Y, Position.Z);
        public void SetPositionY(float value) => Position = new(Position.X, value, Position.Z);
        public void SetPositionZ(float value) => Position = new(Position.X, Position.Y, value);

        public void Draw()
        {
            var model = Mesh.ModelMatrix
            * Matrix4.CreateScale(Scale)
            * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X))
            * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y))
            * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z))
            * Matrix4.CreateTranslation(Position);

            _shader.SetMatrix4("model", model);

            Mesh.Draw();
        }

        public void ProcessKeyboard(KeyboardState kb, float deltaTime)
        {
            float velocity = Speed * deltaTime;

            if (kb.IsKeyDown(Keys.Right)) Position.X += velocity;
            if (kb.IsKeyDown(Keys.Left)) Position.X -= velocity;
            if (kb.IsKeyDown(Keys.Up)) Position.Z -= velocity;
            if (kb.IsKeyDown(Keys.Down)) Position.Z += velocity;

        }
    }
}








