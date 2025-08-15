using GameStudies.Source;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameStudies.Objects
{
    public class CubeLight
    {

        private int _vao, _vbo;
        public Vector3 Position = new(0.0f, 0.0f, -0.0f);
        public Vector3 Color { get; set; } = new(1.0f, 1.0f, 1.0f);
        public float Scale { get; set; } = 0.2f;
        public float Speed { get; set; } = 1.5f;


        private static readonly float[] _cubeVerts = {
                -0.5f, -0.5f, -0.5f,  0.0f, 0.0f, -1.0f,  1.0f, 1.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, 0.0f, -1.0f,  1.0f, 1.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  0.0f, 0.0f, -1.0f,  1.0f, 1.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  0.0f, 0.0f, -1.0f,  1.0f, 1.0f, 1.0f,
                -0.5f,  0.5f, -0.5f,  0.0f, 0.0f, -1.0f,  1.0f, 1.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 0.0f, -1.0f,  1.0f, 1.0f, 1.0f,

                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 1.0f,   1.0f, 1.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 1.0f,   1.0f, 1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 1.0f,   1.0f, 1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 1.0f,   1.0f, 1.0f, 1.0f,
                -0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 1.0f,   1.0f, 1.0f, 1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 1.0f,   1.0f, 1.0f, 1.0f,

                -0.5f,  0.5f,  0.5f, -1.0f,  0.0f, 0.0f,  1.0f, 1.0f, 1.0f,
                -0.5f,  0.5f, -0.5f, -1.0f,  0.0f, 0.0f,  1.0f, 1.0f, 1.0f,
                -0.5f, -0.5f, -0.5f, -1.0f,  0.0f, 0.0f,  1.0f, 1.0f, 1.0f,
                -0.5f, -0.5f, -0.5f, -1.0f,  0.0f, 0.0f,  1.0f, 1.0f, 1.0f,
                -0.5f, -0.5f,  0.5f, -1.0f,  0.0f, 0.0f,  1.0f, 1.0f, 1.0f,
                -0.5f,  0.5f,  0.5f, -1.0f,  0.0f, 0.0f,  1.0f, 1.0f, 1.0f,

                0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f, 1.0f,
                0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f, 1.0f,
                0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f, 1.0f,
                0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f, 1.0f,
                0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f, 1.0f,
                0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f, 1.0f,

                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f, 0.0f,  1.0f, 1.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, -1.0f, 0.0f,  1.0f, 1.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, -1.0f, 0.0f,  1.0f, 1.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, -1.0f, 0.0f,  1.0f, 1.0f, 1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, -1.0f, 0.0f,  1.0f, 1.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f, 0.0f,  1.0f, 1.0f, 1.0f,

                -0.5f,  0.5f, -0.5f,  0.0f,  1.0f, 0.0f,  1.0f, 1.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  1.0f, 0.0f,  1.0f, 1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  1.0f, 0.0f,  1.0f, 1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  1.0f, 0.0f,  1.0f, 1.0f, 1.0f,
                -0.5f,  0.5f,  0.5f,  0.0f,  1.0f, 0.0f,  1.0f, 1.0f, 1.0f,
                -0.5f,  0.5f, -0.5f,  0.0f,  1.0f, 0.0f,  1.0f, 1.0f, 1.0f,
        };

        private readonly Shader _shader;

        public CubeLight(Shader shader)
        {
            _shader = shader;
        }


        public void Load()
        {
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _cubeVerts.Length * sizeof(float), _cubeVerts, BufferUsageHint.StaticDraw);


            // aPos @ location 0 (vec3)
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // aNormal @ location 1 (vec3)
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // aColor @ location 1 (vec3)
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            GL.EnableVertexAttribArray(0);
        }

        public void Draw(float dt)
        {

            _shader.Use();


            var model = Matrix4.CreateScale(Scale) * Matrix4.CreateTranslation(Position);


            _shader.SetMat4("model", model);
            // _shader.SetVec3("aColor", Color);

            _shader.SetVec3("material.ambient", new(1.0f, 1.0f, 0.5f));
            _shader.SetVec3("material.diffuse", new(1.0f, 0.36f, 0.0f));
            _shader.SetVec3("material.specular", new(1.0f, 1.0f, 1.0f));
            _shader.SetFloat("material.shininess", 64.0f);

            Vector3 lightColor;


            lightColor.X = (float)Math.Sin(dt * 2.0f);
            lightColor.Y = (float)Math.Sin(dt * 0.7f);
            lightColor.Z = (float)Math.Sin(dt * 1.3f);

            Vector3 diffuseColor = lightColor * new Vector3(0.5f);
            Vector3 ambientColor = diffuseColor * new Vector3(1.0f);

            _shader.SetVec3("light.position", Position);
            _shader.SetVec3("light.ambient", new(0.5f, 0.9f, 0.2f));
            _shader.SetVec3("light.diffuse", ambientColor);
            _shader.SetVec3("light.specular", new(1.0f, 1.0f, 1.0f));

            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }

        public void ProcessKeyboard(KeyboardState kb, float deltaTime)
        {
            float velocity = Speed * deltaTime;

            if (kb.IsKeyDown(Keys.Right)) Position.X += velocity;
            if (kb.IsKeyDown(Keys.Left)) Position.X -= velocity;
            if (kb.IsKeyDown(Keys.Up)) Position.Z -= velocity;
            if (kb.IsKeyDown(Keys.Down)) Position.Z += velocity;
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(_vao);
            GL.DeleteBuffer(_vbo);
            _shader.Delete();
        }
    }
}