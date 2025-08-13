using GameStudies.Source;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GameStudies.Objects
{
    public class CubeLight
    {

        private int _vao, _vbo;
        public Vector3 Position { get; set; } = new(1.2f, 1.0f, 2.0f);
        public Vector3 Color { get; set; } = Vector3.One;
        public float Scale { get; set; } = 0.2f;

        private static readonly float[] _cubeVerts = {
            // back
            -0.5f,-0.5f,-0.5f,  0.5f,-0.5f,-0.5f,  0.5f, 0.5f,-0.5f,
             0.5f, 0.5f,-0.5f, -0.5f, 0.5f,-0.5f, -0.5f,-0.5f,-0.5f,
            // front
            -0.5f,-0.5f, 0.5f,  0.5f,-0.5f, 0.5f,  0.5f, 0.5f, 0.5f,
             0.5f, 0.5f, 0.5f, -0.5f, 0.5f, 0.5f, -0.5f,-0.5f, 0.5f,
            // left
            -0.5f, 0.5f, 0.5f, -0.5f, 0.5f,-0.5f, -0.5f,-0.5f,-0.5f,
            -0.5f,-0.5f,-0.5f, -0.5f,-0.5f, 0.5f, -0.5f, 0.5f, 0.5f,
            // right
             0.5f, 0.5f, 0.5f,  0.5f, 0.5f,-0.5f,  0.5f,-0.5f,-0.5f,
             0.5f,-0.5f,-0.5f,  0.5f,-0.5f, 0.5f,  0.5f, 0.5f, 0.5f,
            // bottom
            -0.5f,-0.5f,-0.5f,  0.5f,-0.5f,-0.5f,  0.5f,-0.5f, 0.5f,
             0.5f,-0.5f, 0.5f, -0.5f,-0.5f, 0.5f, -0.5f,-0.5f,-0.5f,
            // top
            -0.5f, 0.5f,-0.5f,  0.5f, 0.5f,-0.5f,  0.5f, 0.5f, 0.5f,
             0.5f, 0.5f, 0.5f, -0.5f, 0.5f, 0.5f, -0.5f, 0.5f,-0.5f
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

            // layout(location = 0) => vec3 position
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.EnableVertexAttribArray(0);
        }

        public void Draw()
        {

            _shader.Use();

            // model = translate * scale
            var model = Matrix4.CreateScale(Scale) * Matrix4.CreateTranslation(Position);

            _shader.SetMat4("model", model);
            _shader.SetVec3("uColor", Color);

            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(_vao);
            GL.DeleteBuffer(_vbo);
            _shader.Delete();
        }
    }
}