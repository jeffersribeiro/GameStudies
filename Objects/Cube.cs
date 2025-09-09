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
            Vertex[] vertices = CubeFactory.CreateVertices();
            uint[] indices = CubeFactory.Indices;


            Texture[] texPaths =
            [
                new()
                {
                    Id= 1,
                    Type= TextureType.Diffuse,
                    Path= Path.Combine("wood_container_texture.png")
                },
                new ()
                {
                    Id= 2,
                    Type= TextureType.Specular,
                    Path= Path.Combine("container_steel_border.png")
                },
            ];

            Mesh = new Mesh(vertices, indices, texPaths);
        }

        public void Dispose()
        {
            Mesh.Dispose();
        }

        public void Draw(Shader shader)
        {
            Mesh.Draw(shader);
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
