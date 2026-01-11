using Matrix4 = System.Numerics.Matrix4x4;
using Vector4 = System.Numerics.Vector4;
using Vector4i = System.Numerics.Vector4;
using Vector3 = System.Numerics.Vector3;
using Vector2 = System.Numerics.Vector2;
using Silk.NET.OpenGL;
using System.Runtime.InteropServices;

namespace GameStudies.Graphics
{
    public enum TextureType { Diffuse, Specular, Normal, Height }

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
        private readonly GL _gl;
        private readonly Vertex[] _vertices;
        private readonly uint[] _indices;
        private readonly Texture[] _textures;

        private uint _vao, _vbo, _ebo;

        public Mesh(GL gl, Vertex[] vertices, uint[] indices, Texture[] textures)
        {
            _gl = gl;
            _vertices = vertices;
            _textures = textures;
            _indices = indices;

            SetupMesh();
        }

        public unsafe void Draw(Shader shader, in Matrix4 modelFromEntity)
        {
            int diffuseNr = 0, specularNr = 0, normalNr = 0, heightNr = 0;
            shader.Use();

            Matrix4 model = modelFromEntity; // sem _nodeTransform
            shader.SetMat4("model", model);

            for (int i = 0; i < _textures.Length; i++)
            {

                _gl.ActiveTexture(TextureUnit.Texture0 + i);

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
                _gl.BindTexture(TextureTarget.Texture2D, _textures[i].Id);
            }

            shader.SetInt("uDiffuseCount", diffuseNr);
            shader.SetInt("uSpecularCount", specularNr);

            _gl.BindVertexArray(_vao);
            _gl.DrawElements(PrimitiveType.Triangles, (uint)_indices.Length, DrawElementsType.UnsignedInt, null);

            _gl.BindVertexArray(0);

            _gl.ActiveTexture(GLEnum.Texture0);
        }

        public unsafe void SetupMesh()
        {
            _vao = _gl.GenVertexArray();
            _vbo = _gl.GenBuffer();
            _ebo = _gl.GenBuffer();

            uint stride = (uint)sizeof(Vertex);

            static nint off(string field) => Marshal.OffsetOf<Vertex>(field);

            _gl.BindVertexArray(_vao);

            _gl.BindBuffer(GLEnum.ArrayBuffer, _vbo);
            _gl.BufferData<Vertex>(GLEnum.ArrayBuffer, _vertices.AsSpan(), GLEnum.StaticDraw);

            _gl.BindBuffer(GLEnum.ElementArrayBuffer, _ebo);
            _gl.BufferData<uint>(GLEnum.ElementArrayBuffer, (uint)(_indices.Length * sizeof(uint)), _indices, GLEnum.StaticDraw);

            // vertex Positions
            _gl.EnableVertexAttribArray(0);
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, off(nameof(Vertex.Position)));

            // vertex normals
            _gl.EnableVertexAttribArray(1);
            _gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, off(nameof(Vertex.Normal)));
            // vertex colors
            _gl.EnableVertexAttribArray(2);
            _gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, off(nameof(Vertex.Color)));

            // vertex texture coords
            _gl.EnableVertexAttribArray(3);
            _gl.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, stride, off(nameof(Vertex.vUV)));

            // vertex tangent
            _gl.EnableVertexAttribArray(4);
            _gl.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, stride, off(nameof(Vertex.Tangent)));

            // vertex bitangent
            _gl.EnableVertexAttribArray(5);
            _gl.VertexAttribPointer(5, 3, VertexAttribPointerType.Float, false, stride, off(nameof(Vertex.Bitangent)));
            // ids
            _gl.EnableVertexAttribArray(6);
            _gl.VertexAttribIPointer(6, 4, VertexAttribIType.Int, stride, off(nameof(Vertex.BoneIDs)));

            // weights
            _gl.EnableVertexAttribArray(7);
            _gl.VertexAttribPointer(7, 4, VertexAttribPointerType.Float, false, stride, off(nameof(Vertex.Weights)));

            _gl.BindVertexArray(0);
        }

        public void Dispose()
        {
            if (_textures is { Length: > 0 })
            {
                uint[] ids = _textures
                    .Select(t => t.Id)
                    .Where(id => id != 0)
                    .ToArray();

                if (ids.Length > 0)
                    _gl.DeleteTextures(ids);
            }

            _gl.DeleteVertexArrays(1, ref _vao);
            _gl.DeleteBuffers(1, ref _vbo);
            _gl.DeleteBuffers(1, ref _ebo);
        }
    }
}
