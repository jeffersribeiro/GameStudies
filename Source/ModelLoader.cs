using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Silk.NET.Assimp;
using Assimp = Silk.NET.Assimp;

namespace GameStudies.Source
{
    public unsafe class Model
    {
        readonly Assimp.Assimp assimp = Assimp.Assimp.GetApi();
        private List<Mesh> _meshes = [];
        private readonly uint _flags = (uint)(Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.FlipUVs);
        private readonly List<Texture> _texturesLoaded = new();
        string _directory;


        public Model(string path)
        {
            LoadModel(path);
        }

        public void Draw(Shader shader)
        {
            for (int i = 0; i < _meshes.Count; i++)
            {
                _meshes[i].Draw(shader);
            }
        }

        private void LoadModel(string path)
        {
            Assimp.Scene* scene = assimp.ImportFile(path, _flags);

            if (scene == null || (scene->MFlags & (uint)Assimp.SceneFlags.Incomplete) != 0 || scene->MRootNode == null)
            {
                throw new Exception("error on loading model");
            }

            _directory = path.Substring(0, path.LastIndexOf('/'));

            ProcessNode(scene->MRootNode, scene);
        }

        private void ProcessNode(Assimp.Node* node, Assimp.Scene* scene)
        {
            for (int i = 0; i < node->MNumMeshes; i++)
            {
                Assimp.Mesh* mesh = scene->MMeshes[node->MMeshes[i]];
                _meshes.Add(ProcessMesh(mesh, scene));
            }

            for (int i = 0; i < node->MNumChildren; i++)
            {
                ProcessNode(node->MChildren[i], scene);
            }

        }

        private Mesh ProcessMesh(Assimp.Mesh* mesh, Assimp.Scene* scene)
        {
            List<Vertex> vertices = [];
            List<uint> indices = [];
            List<Texture> textures = [];

            for (int i = 0; i < mesh->MNumVertices; i++)
            {
                Vertex vertex = new();
                Vector3 vec3;

                vec3.X = mesh->MVertices[i].X;
                vec3.Y = mesh->MVertices[i].Y;
                vec3.Z = mesh->MVertices[i].Z;
                vertex.Position = vec3;

                vec3.X = mesh->MNormals[i].X;
                vec3.Y = mesh->MNormals[i].Y;
                vec3.Z = mesh->MNormals[i].Z;
                vertex.Normal = vec3;

                if (mesh->MTextureCoords[0] != null)
                {
                    Vector2 vec2 = new();
                    vec2.X = mesh->MTextureCoords[0][i].X;
                    vec2.Y = mesh->MTextureCoords[0][i].Y;
                    vertex.vUV = vec2;
                }
                else
                {
                    vertex.vUV = Vector2.Zero;
                }

                vertices.Add(vertex);
            }

            for (int i = 0; i < mesh->MNumFaces; i++)
            {
                Assimp.Face face = mesh->MFaces[i];
                for (int j = 0; j < face.MNumIndices; j++)
                {
                    indices.Add(face.MIndices[j]);
                }
            }

            if (mesh->MMaterialIndex >= 0)
            {
                Assimp.Material* material = scene->MMaterials[mesh->MMaterialIndex];
                List<Texture> diffuseMaps = LoadMaterialTextures(material, Assimp.TextureType.Diffuse, TextureType.Diffuse);
                textures.AddRange(textures.LastOrDefault(), diffuseMaps.First(), diffuseMaps.LastOrDefault());

                List<Texture> specularMaps = LoadMaterialTextures(material, Assimp.TextureType.Specular, TextureType.Specular);
                textures.AddRange(textures.LastOrDefault(), diffuseMaps.FirstOrDefault(), diffuseMaps.LastOrDefault());
            }

            return new Mesh(vertices.ToArray(), indices.ToArray(), textures.ToArray());

        }

        private unsafe List<Texture> LoadMaterialTextures(Assimp.Material* mat, Assimp.TextureType type, TextureType typeName)
        {
            var textures = new List<Texture>();

            uint texCount = assimp.GetMaterialTextureCount(mat, type);

            for (uint i = 0; i < texCount; i++)
            {
                AssimpString str = default;
                TextureMapping mapping = default;
                uint uvIndex = 0;
                float blend = 0;
                TextureOp op = default;
                TextureMapMode mapMode = default;
                uint flags = 0;

                // ✔ Assinatura “completa”
                assimp.GetMaterialTexture(
                    mat, type, i,
                    &str,          // out: caminho
                    &mapping,       // out
                    &uvIndex,       // out
                    &blend,         // out
                    &op,            // out
                    &mapMode,       // out
                    &flags          // out
                );


                bool skip = false;
                foreach (var loaded in _texturesLoaded)
                {
                    if (loaded.Path == str)
                    {
                        textures.Add(loaded);
                        skip = true;
                        break;
                    }
                }

                if (!skip)
                {
                    Texture texture = new()
                    {
                        Id = (uint)TextureFromFile(str, _directory),
                        Type = typeName,
                        Path = str
                    };

                    textures.Add(texture);
                    _texturesLoaded.Add(texture);
                }
            }

            return textures;
        }

        private static int TextureFromFile(string filename, string directory)
        {
            string filepath = Path.Combine(directory, filename);

            int textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, (uint)textureId);

            using var stream = System.IO.File.OpenRead(filepath);
            var image = StbImageSharp.ImageResult.FromStream(stream, StbImageSharp.ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return textureId;
        }
    }
}