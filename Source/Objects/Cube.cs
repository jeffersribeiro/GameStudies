using GameStudies.Factory;
using GameStudies.Source;
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

        private readonly Shader _shader;

        public CubeObject(Shader shader, Vector3 startPosition, float size = 1f)
        {
            _shader = shader;
            Position = startPosition;
            Vertex[] vertices = CubeFactory.CreateVertices();
            uint[] indices =
            [
                0, 1, 2,
                2, 3, 0,
                4, 5, 6,
                6, 7, 4,
                4, 0, 3,
                3, 7, 4,
                1, 5, 6,
                6, 2, 1,
                4, 5, 1,
                1, 0, 4,
                3, 2, 6,
                6, 7, 3
            ];

            Texture[] texPaths =
            [
                new()
                {
                    Id= 1,
                    Type= TextureType.Diffuse,
                    Path= "C:/Users/Jeffe/OneDrive/Documents/Projects/Estudos/GameStudies/assets/wood_container_texture.png"
                },
                new ()
                {
                    Id= 1,
                    Type= TextureType.Specular,
                    Path= "C:/Users/Jeffe/OneDrive/Documents/Projects/Estudos/GameStudies/assets/container_steel_border.png"
                },
            ];

            Mesh = new Mesh(vertices, indices, texPaths);

            Mesh.Load();
            Mesh.LoadTextures(_shader);
        }

        public void Dispose()
        {
            Mesh.Dispose(_shader);
        }

        public void Draw()
        {
            var model = Mesh.ModelMatrix
            * Matrix4.CreateScale(Scale)
            * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X))
            * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y))
            * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z))
            * Matrix4.CreateTranslation(Position);

            _shader.SetMat4("model", model);

            Mesh.Draw(_shader);
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








