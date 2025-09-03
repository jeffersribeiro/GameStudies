using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GameStudies.Source
{
    public enum TextureType { Diffuse, Specular, Normal, Height }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        // position
        public Vector3 Position;
        // normal
        public Vector3 Normal;
        // color
        public Vector3 Color;
        // texCoords
        public Vector2 vUV;
        // tangent
        public Vector3 Tangent;
        // bitangent
        public Vector3 Bitangent;
        // bone indexes which will influence this vertex
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.MAX_BONE_INFLUENCE)]
        public int[] BoneIDs;
        // weights from each bone
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.MAX_BONE_INFLUENCE)]
        public float[] Weights;
    };

    public struct Texture
    {
        public uint Id;
        public TextureType Type;
        public string Path;
    }

    public class Mesh
    {
        public Vector3 Position = Helpers.GenRandomPosition();
        public float Rotation;
        public Vector3 Scale = Vector3.One;
        public Matrix4 ModelMatrix =>
            Matrix4.CreateTranslation(Position)
          * Matrix4.CreateRotationZ(Rotation)
          * Matrix4.CreateScale(Scale);

        private readonly Vertex[] _vertices;
        private readonly uint[] _indices;
        private readonly Texture[] _textures;
        private int _vao, _vbo, _ebo;

        public Mesh(Vertex[] vertices, uint[] indices, Texture[] textures)
        {
            _vertices = vertices;
            _textures = textures;
            _indices = indices;
        }

        public void Draw(Shader shader)
        {
            int diffuseNr = 1;
            int specularNr = 1;
            int normalNr = 1;
            int heightNr = 1;

            for (int i = 0; i < _textures.Length; i++)
            {

                GL.ActiveTexture(TextureUnit.Texture0 + i);

                string number = "";
                string name = "";
                TextureType type = _textures[i].Type;

                if (type == TextureType.Diffuse)
                {
                    name = "texture_diffuse";
                    number = (diffuseNr++).ToString();
                }
                else if (type == TextureType.Specular)
                {
                    name = "texture_specular";
                    number = (specularNr++).ToString();
                }
                else if (type == TextureType.Normal)
                {
                    name = "texture_normal";
                    number = (normalNr++).ToString();
                }
                else if (type == TextureType.Height)
                {
                    name = "texture_height";
                    number = (heightNr++).ToString();
                }

                shader.Use();
                shader.SetInt(name + number, i);
                GL.BindTexture(TextureTarget.Texture2D, _textures[i].Id);
            }

            shader.Use();
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            GL.ActiveTexture(TextureUnit.Texture0);
        }

        public void SetupMesh()
        {
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();

            int stride = Marshal.SizeOf<Vertex>();
            IntPtr normalOffset = Marshal.OffsetOf<Vertex>(nameof(Vertex.Normal));
            IntPtr colorOffset = Marshal.OffsetOf<Vertex>(nameof(Vertex.Color));
            IntPtr texCoordsOffset = Marshal.OffsetOf<Vertex>(nameof(Vertex.vUV));

            var tangentOffset = Marshal.OffsetOf<Vertex>(nameof(Vertex.Tangent));
            var bitangentOffset = Marshal.OffsetOf<Vertex>(nameof(Vertex.Bitangent));
            var boneIDsOffset = Marshal.OffsetOf<Vertex>(nameof(Vertex.BoneIDs));
            var weightsOffset = Marshal.OffsetOf<Vertex>(nameof(Vertex.Weights));

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * stride, _vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            // vertex Positions
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);

            // vertex normals
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, normalOffset);

            // vertex colors
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, colorOffset);

            // vertex texture coords
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, stride, texCoordsOffset);

            // vertex tangent
            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, stride, tangentOffset);

            // vertex bitangent
            GL.EnableVertexAttribArray(5);
            GL.VertexAttribPointer(5, 3, VertexAttribPointerType.Float, false, stride, bitangentOffset);
            // ids
            GL.EnableVertexAttribArray(6);
            GL.VertexAttribIPointer(6, 4, VertexAttribIntegerType.Int, stride, boneIDsOffset);

            // weights
            GL.EnableVertexAttribArray(7);
            GL.VertexAttribPointer(7, 4, VertexAttribPointerType.Float, false, stride, weightsOffset);

            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            if (_textures.Length > 0)
            {
                var textureIds = _textures.Select(t => (int)t.Id).Where(id => id != 0).ToArray();
                if (textureIds.Length > 0) GL.DeleteTextures(textureIds.Length, textureIds);
            }

            GL.DeleteVertexArrays(1, ref _vao);
            GL.DeleteBuffers(1, ref _vbo);
            GL.DeleteBuffers(1, ref _ebo);
        }
    }
}
