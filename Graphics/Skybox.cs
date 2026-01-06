using Matrix4 = System.Numerics.Matrix4x4;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace GameStudies.Graphics
{
    public class Skybox
    {
        private GL _gl;

        private uint _vao, _vbo;
        private uint _cubemap;
        private Shader _shader;

        public Skybox(GL gl, Shader shader, string[] facePaths)
        {
            _gl = gl;
            _shader = shader;
            Create(facePaths);
        }

        void Create(string[] facePaths)
        {
            string baseDir = AppContext.BaseDirectory;
            _cubemap = _gl.GenTexture();
            _gl.BindTexture(TextureTarget.TextureCubeMap, _cubemap);


            for (int i = 0; i < 6; i++)
            {

                string fullpath = Path.Combine(baseDir, DirPathNames.AssetsFolderName, facePaths[i]);
                using var stream = System.IO.File.OpenRead(fullpath);
                var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                _gl.TexImage2D<byte>(GLEnum.TextureCubeMapPositiveX + i, 0, (int)InternalFormat.SrgbAlpha, (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }

            _gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            _gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            _gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            _gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            _gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
            _gl.BindTexture(TextureTarget.TextureCubeMap, 0);

            float[] cubeVerts = {
            -1.0f,  1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

            1.0f, -1.0f, -1.0f,
            1.0f, -1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

            -1.0f,  1.0f, -1.0f,
            1.0f,  1.0f, -1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
            1.0f, -1.0f,  1.0f
            };

            _vao = _gl.GenVertexArray();
            _vbo = _gl.GenBuffer();
            _gl.BindVertexArray(_vao);
            _gl.BindBuffer(GLEnum.ArrayBuffer, _vbo);
            _gl.BufferData<float>(GLEnum.ArrayBuffer, (uint)(cubeVerts.Length * sizeof(float)), cubeVerts, GLEnum.StaticDraw);
            _gl.EnableVertexAttribArray(0);
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            _gl.BindVertexArray(0);

            _shader.Use();
            _shader.SetInt("skybox", 0);
        }

        public void Draw(Matrix4 view, Matrix4 projection)
        {
            _gl.DepthMask(false);
            _shader.Use();

            var viewNoTrans = view;
            _shader.SetMat4("view", viewNoTrans);
            _shader.SetMat4("projection", projection);

            _gl.BindVertexArray(_vao);
            _gl.ActiveTexture(TextureUnit.Texture0);
            _gl.BindTexture(TextureTarget.TextureCubeMap, _cubemap);
            _gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
            _gl.BindVertexArray(0);

            _gl.DepthMask(true);
        }

        public void Dispose()
        {
            _gl.DeleteTexture(_cubemap);
            _gl.DeleteBuffer(_vbo);
            _gl.DeleteVertexArray(_vao);
            _shader?.Delete();
        }
    }
}