using GameStudies.Factories;
using GameStudies.Graphics;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameStudies.Objects
{

    public class CubeObject
    {
        public Mesh Mesh { get; }
        public Vector3 Position = new(0.0f, 0.0f, -0.0f);
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; } = Vector3.One;
        public float Speed { get; set; } = 1.5f;

        public CubeObject(Vector3 startPosition, float size = 1f)
        {
            Position = startPosition;
            Vertex[] vertices = VerticesFactory.CreateCube();
            uint[] indices = VerticesFactory.Indices;

            Texture[] texPaths =
            [
                new()
                {
                    Id = (uint)TextureLoader.Load2D(Path.Combine("Assets", "wood_container_texture.png")),
                    Type = TextureType.Diffuse,
                    Path = Path.Combine("wood_container_texture.png")
                },
                new()
                {
                    Id = (uint)TextureLoader.Load2D(Path.Combine("Assets", "container_steel_border.png")),
                    Type = TextureType.Specular,
                    Path = Path.Combine("container_steel_border.png")
                },
            ];

            Mesh = new Mesh(vertices, indices, texPaths, in Matrix4.Identity);
        }

        public void Dispose()
        {
            Mesh.Dispose();
        }

        public void Draw(Shader shader)
        {
            var model =
            Matrix4.CreateScale(Scale)
            * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X))
            * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y))
            * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z))
            * Matrix4.CreateTranslation(Position);

            Mesh.Draw(shader, in model);
        }

        public void ProcessKeyboard(KeyboardState kb, float deltaTime)
        {
            float velocity = Speed * deltaTime;

            if (kb.IsKeyDown(Keys.Right)) Position.X += velocity;
            if (kb.IsKeyDown(Keys.Left)) Position.X -= velocity;
            if (kb.IsKeyDown(Keys.Up)) Position.Z -= velocity;
            if (kb.IsKeyDown(Keys.Down)) Position.Z += velocity;
            if (kb.IsKeyDown(Keys.KeyPad1)) Position.Y += velocity;
            if (kb.IsKeyDown(Keys.KeyPad0)) Position.Y -= velocity;
        }
    }
}
