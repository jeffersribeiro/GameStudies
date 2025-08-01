using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace GameStudies.Source
{
    public class Mesh
    {
        private float[] _vertices;
        private int[] _textures;
        private string[] _texPaths;
        private int _VAO = 0;
        private int _VBO = 0;
        private int _EBO = 0;
        Shader _shader;

        int[] indices = {
        0, 1, 3, // first triangle
        1, 2, 3  // second triangle
    };



        public Mesh(float[] vertices, string[] texPaths)
        {

            const string vert = "C:/Users/Jeffe/OneDrive/Documents/Projects/GameStudies/shaders/shader.vert";
            const string frag = "C:/Users/Jeffe/OneDrive/Documents/Projects/GameStudies/shaders/shader.frag";

            _shader = new Shader(vert, frag);

            _vertices = vertices;
            _texPaths = texPaths;
        }

        public void Load()
        {

            // Generate VAO and VBO
            _VAO = GL.GenVertexArray();
            _VBO = GL.GenBuffer();
            _EBO = GL.GenBuffer();

            GL.BindVertexArray(_VAO);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            // position attribute
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // color attribute
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // texture coord attribute
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            GL.BindVertexArray(0);
        }

        public void LoadTextures()
        {
            if (_texPaths.Length == 0)
            {
                throw new Exception("please set any texture path");
            }

            _textures = new int[_texPaths.Length];


            for (int i = 0; i < _texPaths.Length; i++)
            {
                _textures[i] = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, _textures[i]);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapNearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                StbImage.stbi_set_flip_vertically_on_load(1);

                using var stream = File.OpenRead(_texPaths[i]);

                var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

                int width = image.Width;
                int height = image.Height;
                ColorComponents nrChannels = image.Comp;
                byte[] pixels = image.Data;

                PixelInternalFormat internalFormat = (int)nrChannels switch
                {
                    1 => PixelInternalFormat.R8,
                    2 => PixelInternalFormat.Rg8,
                    3 => PixelInternalFormat.Rgb8,
                    4 => PixelInternalFormat.Rgba8,
                    _ => throw new NotSupportedException($"Canais não suportados: {nrChannels}")
                };

                PixelFormat pixelFormat = (int)nrChannels switch
                {
                    1 => PixelFormat.Red,
                    2 => PixelFormat.Rg,
                    3 => PixelFormat.Rgb,
                    4 => PixelFormat.Rgba,
                    _ => throw new NotSupportedException($"Canais não suportados: {nrChannels}")
                };


                GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, width, height, 0, pixelFormat, PixelType.UnsignedByte, pixels);

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                _shader.Use();
                string texName = "texture" + i;
                _shader.SetInt(texName, i);
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

            GL.BindVertexArray(_VAO);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            _shader.Delete();

            GL.DeleteVertexArrays(1, ref _VAO);
            GL.DeleteBuffers(1, ref _VBO);
            GL.DeleteBuffers(1, ref _EBO);

        }
    }
}