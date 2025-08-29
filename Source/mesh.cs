using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace GameStudies.Source
{
    public enum TextureType { Diffuse, Specular }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Color;
        public Vector2 vUV;
    };
    public struct Texture
    {
        public uint Id;
        public TextureType Type;
        public string Path;
    }

    public class Mesh
    {
        public Vector3 Position;
        public float Rotation;
        public Vector3 Scale = Vector3.One;
        public Matrix4 ModelMatrix =>
        Matrix4.CreateTranslation(Position)
        * Matrix4.CreateRotationZ(Rotation)
        * Matrix4.CreateScale(Scale);

        private readonly Vertex[] _vertices;
        private readonly Texture[] _textures;
        private readonly uint[] _indices;
        private int _vao, _vbo, _ebo;



        public Mesh(Vertex[] vertices, uint[] indices, Texture[] textures)
        {
            _vertices = vertices;
            _textures = textures;
            _indices = indices;
        }

        public void Load()
        {
            // Generate VAO and VBO
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();


            int stride = Marshal.SizeOf<Vertex>();
            IntPtr normalOffset = Marshal.OffsetOf<Vertex>(nameof(Vertex.Normal));
            IntPtr colorOffset = Marshal.OffsetOf<Vertex>(nameof(Vertex.Color));
            IntPtr texCoordsOffset = Marshal.OffsetOf<Vertex>(nameof(Vertex.vUV));

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * stride, _vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);


            // aPos @ location 0 (vec3)
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);

            // aNormal @ location 1 (vec3)
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, normalOffset);

            // aColor @ location 1 (vec3)
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, colorOffset);

            // aTexCoord @ location 2 (vec2)
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, stride, texCoordsOffset);

            GL.BindVertexArray(0);
        }

        public void LoadTextures(Shader shader)
        {
            int diffuseNr = 1;
            int specularNr = 1;

            if (_textures.Length != 0)
            {

                for (int i = 0; i < _textures.Length; i++)
                {
                    _textures[i].Id = (uint)GL.GenTexture();
                    GL.ActiveTexture(TextureUnit.Texture0 + i);
                    GL.BindTexture(TextureTarget.Texture2D, _textures[i].Id);

                    string number = "";
                    string name = "";

                    TextureType type = _textures[i].Type;

                    if (type == TextureType.Diffuse)
                    {
                        name = "texture_diffuse";
                        number = diffuseNr++.ToString();
                    }
                    else if (type == TextureType.Specular)
                    {
                        name = "texture_specular";
                        number = specularNr++.ToString();
                    }

                    if (string.IsNullOrEmpty(number)) throw new Exception("number can not be setted");

                    shader.SetInt($"material.{name}{number}", i);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapNearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                    using var stream = File.OpenRead(_textures[i].Path);
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

                    shader.Use();
                    shader.SetBool("uUseTexture", true);
                }
            }
            else
            {
                shader.Use();
                shader.SetBool("uUseTexture", false);
            }

            shader.SetFloat("material.shininess", 16.0f);
        }

        public void Draw(Shader shader)
        {
            shader.Use();

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void Dispose(Shader shader)
        {
            shader.Delete();

            if (_textures.Length > 0)
            {
                int[] textureIds = _textures.Select(x => (int)x.Id).ToArray();
                GL.DeleteTextures(textureIds.Length, textureIds);
            }

            GL.DeleteVertexArrays(1, ref _vao);
            GL.DeleteBuffers(1, ref _vbo);
            GL.DeleteBuffers(1, ref _ebo);
        }
    }
}