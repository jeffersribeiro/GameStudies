using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GameStudies.Graphics
{
    public enum TextureType { Diffuse, Specular, Normal, Height }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Vertex
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
        public Vector4i BoneIDs;
        // weights from each bone
        public Vector4 Weights;
    };

    public struct Texture
    {
        public uint Id;
        public TextureType Type;
        public string Path;
    }

    public class Mesh
    {
        private readonly Vertex[] _vertices;
        private readonly uint[] _indices;
        private readonly Texture[] _textures;
        private readonly Matrix4 _nodeTransform;

        private int _vao, _vbo, _ebo;

        public Mesh(Vertex[] vertices, uint[] indices, Texture[] textures, in Matrix4 nodeTransform)
        {
            _nodeTransform = nodeTransform;
            _vertices = vertices;
            _textures = textures;
            _indices = indices;

            SetupMesh();
        }

        public void Draw(Shader shader, in Matrix4 modelFromEntity)
        {
            int diffuseNr = 0, specularNr = 0, normalNr = 0, heightNr = 0;

            Matrix4 model = modelFromEntity * _nodeTransform;
            shader.Use();
            shader.SetMat4("model", model);

            for (int i = 0; i < _textures.Length; i++)
            {

                GL.ActiveTexture(TextureUnit.Texture0 + i);

                string? uname = null;
                switch (_textures[i].Type)
                {
                    case TextureType.Diffuse:
                        uname = $"texture_diffuse{diffuseNr + 1}";
                        diffuseNr++;
                        break;
                    case TextureType.Specular:
                        uname = $"texture_specular{specularNr + 1}";
                        specularNr++;
                        break;
                }

                if (uname != null) shader.SetInt(uname, i);
                GL.BindTexture(TextureTarget.Texture2D, _textures[i].Id);
            }

            shader.SetInt("uDiffuseCount", diffuseNr);
            shader.SetInt("uSpecularCount", specularNr);

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            GL.ActiveTexture(TextureUnit.Texture0);
        }

        public unsafe void SetupMesh()
        {
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();

            int stride = sizeof(Vertex);

            nint off(string field) => Marshal.OffsetOf<Vertex>(field);

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * stride, _vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            // vertex Positions
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, off(nameof(Vertex.Position)));

            // vertex normals
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, off(nameof(Vertex.Normal)));

            // vertex colors
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, off(nameof(Vertex.Color)));

            // vertex texture coords
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, stride, off(nameof(Vertex.vUV)));

            // vertex tangent
            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, stride, off(nameof(Vertex.Tangent)));

            // vertex bitangent
            GL.EnableVertexAttribArray(5);
            GL.VertexAttribPointer(5, 3, VertexAttribPointerType.Float, false, stride, off(nameof(Vertex.Bitangent)));
            // ids
            GL.EnableVertexAttribArray(6);
            GL.VertexAttribIPointer(6, 4, VertexAttribIntegerType.Int, stride, off(nameof(Vertex.BoneIDs)));

            // weights
            GL.EnableVertexAttribArray(7);
            GL.VertexAttribPointer(7, 4, VertexAttribPointerType.Float, false, stride, off(nameof(Vertex.Weights)));

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
