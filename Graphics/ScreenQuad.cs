using System;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace GameStudies.Graphics
{
    public sealed class ScreenQuad : IDisposable
    {
        private readonly GL _gl;
        private uint _vao, _vbo, _ebo;

        private static readonly float[] s_Vertices =
        {
            // pos      // uv
            -1f, -1f,   0f, 0f,
             1f, -1f,   1f, 0f,
             1f,  1f,   1f, 1f,
            -1f,  1f,   0f, 1f,
        };

        private static readonly uint[] s_Indices =
        {
            0, 1, 2,
            0, 2, 3
        };

        public ScreenQuad(GL gl)
        {
            _gl = gl;
            Create();
        }

        private unsafe void Create()
        {
            _vao = _gl.GenVertexArray();
            _vbo = _gl.GenBuffer();
            _ebo = _gl.GenBuffer();

            _gl.BindVertexArray(_vao);

            _gl.BindBuffer(GLEnum.ArrayBuffer, _vbo);
            _gl.BufferData<float>(GLEnum.ArrayBuffer, s_Vertices.AsSpan(), GLEnum.StaticDraw);

            _gl.BindBuffer(GLEnum.ElementArrayBuffer, _ebo);
            _gl.BufferData<uint>(GLEnum.ElementArrayBuffer, s_Indices.AsSpan(), GLEnum.StaticDraw);

            const int floatsPerVertex = 4;
            uint stride = (uint)(floatsPerVertex * sizeof(float));

            _gl.EnableVertexAttribArray(0);
            _gl.VertexAttribPointer(0, 2, GLEnum.Float, false, stride, 0);

            _gl.EnableVertexAttribArray(1);
            _gl.VertexAttribPointer(1, 2, GLEnum.Float, false, stride, 2 * sizeof(float));

            _gl.BindVertexArray(0);
            _gl.BindBuffer(GLEnum.ArrayBuffer, 0);
        }

        public void Draw()
        {
            _gl.BindVertexArray(_vao);
            _gl.DrawElements(GLEnum.Triangles, (uint)s_Indices.Length, GLEnum.UnsignedInt, 0);
            _gl.BindVertexArray(0);
        }

        public void Dispose()
        {
            if (_ebo != 0) _gl.DeleteBuffer(_ebo);
            if (_vbo != 0) _gl.DeleteBuffer(_vbo);
            if (_vao != 0) _gl.DeleteVertexArray(_vao);

            _ebo = _vbo = _vao = 0;
        }
    }
}
