using OpenTK.Graphics.OpenGL;

namespace GameStudies.Graphics
{
    public sealed class ScreenQuad : IDisposable
    {
        private int _vao, _vbo, _ebo;

        private
        static readonly float[] s_Vertices =
        {
            -1f, -1f, 0f, 0f,
             1f, -1f, 1f, 0f,
             1f,  1f, 1f, 1f,
            -1f,  1f, 0f, 1f,
        };

        private static readonly uint[] s_Indices =
        {
            0, 1, 2,
            0, 2, 3
        };

        public ScreenQuad()
        {
            Create();
        }

        private void Create()
        {
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, s_Vertices.Length * sizeof(float), s_Vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, s_Indices.Length * sizeof(uint), s_Indices, BufferUsageHint.StaticDraw);

            int stride = 4 * sizeof(float);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 2 * sizeof(float));

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Draw()
        {
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, s_Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            if (_ebo != 0) GL.DeleteBuffer(_ebo);
            if (_vbo != 0) GL.DeleteBuffer(_vbo);
            if (_vao != 0) GL.DeleteVertexArray(_vao);
            _ebo = _vbo = _vao = 0;

        }
    }
}