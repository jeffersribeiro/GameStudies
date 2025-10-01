using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using StbImageSharp;

namespace GameStudies.Graphics
{
    public class Skybox
    {
        private int _vao, _vbo;
        private int _cubemap;
        private Shader _shader;

        public Skybox(string[] facePaths)
        {
            Create(facePaths);
        }

        void Create(string[] facePaths)
        {
            string baseDir = AppContext.BaseDirectory;
            _cubemap = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, _cubemap);


            for (int i = 0; i < 6; i++)
            {

                string fullpath = Path.Combine(baseDir, DirPathNames.AssetsFolderName, facePaths[i]);
                using var stream = System.IO.File.OpenRead(fullpath);
                var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0,
                       PixelInternalFormat.SrgbAlpha, image.Width, image.Height, 0,
                       PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
            GL.BindTexture(TextureTarget.TextureCubeMap, 0);

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

            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, cubeVerts.Length * sizeof(float), cubeVerts, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.BindVertexArray(0);

            _shader = new Shader("skybox.vert", "skybox.frag");
            _shader.Use();
            _shader.SetInt("skybox", 0);
        }

        public void Draw(Matrix4 view, Matrix4 projection)
        {
            GL.DepthMask(false);
            _shader.Use();

            var viewNoTrans = new Matrix4(new Matrix3(view));
            _shader.SetMat4("view", viewNoTrans);
            _shader.SetMat4("projection", projection);

            GL.BindVertexArray(_vao);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, _cubemap);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(0);

            GL.DepthMask(true);
        }

        public void Dispose()
        {
            GL.DeleteTexture(_cubemap);
            GL.DeleteBuffer(_vbo);
            GL.DeleteVertexArray(_vao);
            _shader?.Delete();
        }
    }
}