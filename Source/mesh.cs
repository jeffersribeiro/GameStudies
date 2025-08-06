using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace GameStudies.Source
{
    public class Mesh
    {
        public Vector3 Position;
        public float Rotation;
        public Vector3 Scale = Vector3.One;
        public Matrix4 ModelMatrix =>
        Matrix4.CreateTranslation(Position)
        * Matrix4.CreateRotationZ(Rotation)
        * Matrix4.CreateScale(Scale);

        private readonly Shader _shader;
        private readonly float[] _vertices;
        private readonly uint[] _indices = new uint[36];
        private int _vao, _vbo, _ebo;
        private List<string> _texPaths = new();
        private int[] _textures = Array.Empty<int>();



        public Mesh(Shader shader, float[] vertices, string[] texPaths)
        {
            _shader = shader;
            _vertices = vertices;
            _texPaths = new List<string>(texPaths);
        }

        public void Load()
        {

            for (uint i = 0; i < 36; i++) _indices[i] = i;


            // Generate VAO and VBO
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            // aPos @ location 0 (vec3)
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            // // aColor @ location 1 (vec3)
            // GL.EnableVertexAttribArray(1);
            // GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            // aTexCoord @ location 2 (vec2)
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            GL.BindVertexArray(0);
        }

        public void LoadTextures()
        {
            if (_texPaths.Count == 0)
            {
                throw new Exception("please set any texture path");
            }

            _textures = new int[_texPaths.Count];
            GL.GenTextures(_textures.Length, _textures);

            for (int i = 0; i < _texPaths.Count; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i);
                GL.BindTexture(TextureTarget.Texture2D, _textures[i]);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapNearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                using var stream = File.OpenRead(_texPaths[i]);
                var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

                // upload
                GL.TexImage2D(TextureTarget.Texture2D,
                              level: 0,
                              internalformat: PixelInternalFormat.Rgba,
                              width: image.Width,
                              height: image.Height,
                              border: 0,
                              format: PixelFormat.Rgba,
                              type: PixelType.UnsignedByte,
                              pixels: image.Data);

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                // tell shader which sampler to use
                _shader.Use();
                _shader.SetInt($"texture{i}", i);

            }
        }

        public void Draw()
        {
            for (int i = 0; i < _textures.Length; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i);
                GL.BindTexture(TextureTarget.Texture2D, _textures[i]);
            }

            _shader.Use();

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            _shader.Delete();

            if (_textures.Length > 0) GL.DeleteTextures(_textures.Length, _textures);
            GL.DeleteVertexArrays(1, ref _vao);
            GL.DeleteBuffers(1, ref _vbo);
            GL.DeleteBuffers(1, ref _ebo);
        }
    }
}