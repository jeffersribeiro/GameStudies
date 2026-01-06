using GameStudies.Factories;
using GameStudies.Graphics;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Matrix4 = System.Numerics.Matrix4x4;
using Vector3 = System.Numerics.Vector3;


namespace GameStudies.Objects
{

    public class SquareObject
    {
        private readonly GL _gl;
        public Mesh Mesh { get; }
        public Vector3 Position = new(0.0f, 0.0f, -0.0f);
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 Scale { get; set; } = Vector3.One;
        public float Speed { get; set; } = 1.5f;

        public SquareObject(GL gl, Vector3 startPosition, float size = 1f)
        {
            _gl = gl;
            Position = startPosition;
            Vertex[] vertices = VerticesFactory.CreateSquare();
            uint[] indices = VerticesFactory.Indices;

            Graphics.Texture[] texPaths =
            [
                new()
                {
                    Id = (uint)TextureLoader.Load2D(_gl, Path.Combine("Assets", "asphat.png")),
                    Type = TextureType.Diffuse,
                    Path = Path.Combine("asphat.png")
                },
            ];

            Mesh = new Mesh(_gl, vertices, indices, texPaths);
        }

        public void Dispose()
        {
            Mesh.Dispose();
        }

        public void Draw(Graphics.Shader shader)
        {
            var model =
            Matrix4.CreateScale(Scale)
            * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X))
            * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y))
            * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z))
            * Matrix4.CreateTranslation(Position);

            Mesh.Draw(shader, in model);
        }

        public void ProcessKeyboard(IKeyboard kb, float deltaTime)
        {
            float velocity = Speed * deltaTime;

            if (kb.IsKeyPressed(Key.Right)) Position.X += velocity;
            if (kb.IsKeyPressed(Key.Left)) Position.X -= velocity;
            if (kb.IsKeyPressed(Key.Up)) Position.Z -= velocity;
            if (kb.IsKeyPressed(Key.Down)) Position.Z += velocity;
            if (kb.IsKeyPressed(Key.Keypad1)) Position.Y += velocity;
            if (kb.IsKeyPressed(Key.Keypad0)) Position.Y -= velocity;
            if (kb.IsKeyPressed(Key.Keypad8)) Rotation.X -= velocity * 10;
            if (kb.IsKeyPressed(Key.Keypad2)) Rotation.Y -= velocity * 10;
            if (kb.IsKeyPressed(Key.Keypad6)) Rotation.Z -= velocity * 10;
        }
    }
}
