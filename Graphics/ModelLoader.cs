using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Silk.NET.Assimp;
using Assimp = Silk.NET.Assimp;

namespace GameStudies.Graphics
{
    public unsafe class Model : IDisposable
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Rotation;
        public Vector3 Scale = Vector3.One;

        readonly Assimp.Assimp assimp = Assimp.Assimp.GetApi();
        private readonly List<Mesh> _meshes = new();
        private readonly uint _flags = (uint)(
        PostProcessSteps.Triangulate |
        PostProcessSteps.FlipUVs);

        private Dictionary<string, BoneInfo> _boneInfoMap = new();
        private int _boneCounter = 0;

        public ref Dictionary<string, BoneInfo> GetBoneInfoMap() { return ref _boneInfoMap; }
        public ref int GetBoneCount() { return ref _boneCounter; }

        private const int MAX_BONE_WEIGHTS = Constants.MAX_BONE_WEIGHTS;

        private readonly List<Texture> _texturesLoaded = new();
        private string _directory = string.Empty;

        public Model(string path)
        {
            LoadModel(path);
        }

        public void Draw(Shader shader)
        {
            var model =
            Matrix4.CreateScale(Scale)
            * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X))
            * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y))
            * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z))
            * Matrix4.CreateTranslation(Position);

            for (int i = 0; i < _meshes.Count; i++)
                _meshes[i].Draw(shader, in model);
        }

        public void Dispose()
        {
            foreach (var m in _meshes)
                m.Dispose();
        }

        private void LoadModel(string path)
        {

            string baseDir = AppContext.BaseDirectory;
            string fullpath = Path.Combine(baseDir, DirPathNames.AssetsFolderName, path);

            Assimp.Scene* scene = assimp.ImportFile(fullpath, _flags);

            if (scene == null || (scene->MFlags & (uint)Assimp.SceneFlags.Incomplete) != 0 || scene->MRootNode == null)
                throw new Exception("error on loading model");

            try
            {
                _directory = fullpath.Substring(0, fullpath.LastIndexOfAny(new[] { '/', '\\' }));
                ProcessNode(scene->MRootNode, Matrix4.Identity, scene);
            }
            finally
            {
                // prevent native memory leak
                assimp.ReleaseImport(scene);
            }
        }

        private void ProcessNode(Assimp.Node* node, Matrix4 parentGlobal, Assimp.Scene* scene)
        {
            Matrix4 local = node->MTransformation.ToOpenTK();
            Matrix4 global = parentGlobal * local;

            for (int i = 0; i < node->MNumMeshes; i++)
            {
                Assimp.Mesh* mesh = scene->MMeshes[node->MMeshes[i]];

                // Skinned vs rigid:
                Matrix4 nodeTransformForMesh;

                if (mesh->MNumBones > 0)
                {
                    // Skinned mesh: DO NOT bake node transform, bones will handle it
                    nodeTransformForMesh = Matrix4.Identity;
                }
                else
                {
                    // Rigid mesh: we still need the node global transform
                    nodeTransformForMesh = global;
                }

                _meshes.Add(ProcessMesh(mesh, nodeTransformForMesh, scene));
            }

            for (int i = 0; i < node->MNumChildren; i++)
                ProcessNode(node->MChildren[i], global, scene);
        }

        private static void SetVertexBoneDataToDefault(ref Vertex vertex)
        {
            for (int i = 0; i < MAX_BONE_WEIGHTS; i++)
            {
                vertex.BoneIDs[i] = -1;
                vertex.Weights[i] = 0.0f;
            }
        }

        private Mesh ProcessMesh(Assimp.Mesh* mesh, Matrix4 global, Assimp.Scene* scene)
        {
            List<Vertex> vertices = new();
            List<uint> indices = new();
            List<Texture> textures = new();

            for (int i = 0; i < mesh->MNumVertices; i++)
            {
                Vertex vertex = new();
                Vector3 vec3;

                SetVertexBoneDataToDefault(ref vertex);


                // positions
                vec3.X = mesh->MVertices[i].X;
                vec3.Y = mesh->MVertices[i].Y;
                vec3.Z = mesh->MVertices[i].Z;
                vertex.Position = vec3;

                // normals (guard against null)
                if (mesh->MNormals != null)
                {
                    vec3.X = mesh->MNormals[i].X;
                    vec3.Y = mesh->MNormals[i].Y;
                    vec3.Z = mesh->MNormals[i].Z;
                    vertex.Normal = vec3;
                }
                else
                {
                    vertex.Normal = Vector3.UnitZ; // fallback; or compute later
                }

                // texcoords
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
                    indices.Add(face.MIndices[j]);
            }

            if (mesh->MMaterialIndex >= 0)
            {
                Assimp.Material* material = scene->MMaterials[mesh->MMaterialIndex];

                var diffuseMaps = LoadMaterialTextures(scene, material, Assimp.TextureType.Diffuse, TextureType.Diffuse);
                textures.AddRange(diffuseMaps);

                var specularMaps = LoadMaterialTextures(scene, material, Assimp.TextureType.Specular, TextureType.Specular);
                textures.AddRange(specularMaps);

                var normalMaps = LoadMaterialTextures(scene, material, Assimp.TextureType.Height, TextureType.Normal);
                textures.AddRange(normalMaps);

                var heightMaps = LoadMaterialTextures(scene, material, Assimp.TextureType.Ambient, TextureType.Height);
                textures.AddRange(heightMaps);

            }

            ExtractBoneWeightForVertices(ref vertices, mesh, scene);

            return new Mesh(vertices.ToArray(), indices.ToArray(), textures.ToArray(), in global);
        }

        private static void SetVertexBoneData(ref Vertex vertex, int boneID, float weight)
        {
            for (int i = 0; i < MAX_BONE_WEIGHTS; ++i)
            {
                if (vertex.BoneIDs[i] < 0)
                {
                    vertex.Weights[i] = weight;
                    vertex.BoneIDs[i] = boneID;
                    break;
                }
            }
        }

        private void ExtractBoneWeightForVertices(ref List<Vertex> vertices, Assimp.Mesh* mesh, Assimp.Scene* scene)
        {

            for (int boneIndex = 0; boneIndex < mesh->MNumBones; ++boneIndex)
            {
                int boneID = -1;
                string boneName = mesh->MBones[boneIndex]->MName;
                if (!_boneInfoMap.TryGetValue(boneName, out var newBoneInfo))
                {
                    newBoneInfo.Id = _boneCounter;
                    var offsetMatrix = mesh->MBones[boneIndex]->MOffsetMatrix.ToOpenTK();
                    newBoneInfo.Offset = offsetMatrix;
                    _boneInfoMap[boneName] = newBoneInfo;
                    boneID = _boneCounter;
                    _boneCounter++;
                }
                else
                {
                    boneID = _boneInfoMap[boneName].Id;
                }
                var weights = mesh->MBones[boneIndex]->MWeights;
                int numWeights = (int)mesh->MBones[boneIndex]->MNumWeights;

                for (int weightIndex = 0; weightIndex < numWeights; ++weightIndex)
                {
                    int vertexId = (int)weights[weightIndex].MVertexId;
                    float weight = weights[weightIndex].MWeight;
                    var v = vertices[vertexId];
                    SetVertexBoneData(ref v, boneID, weight);
                    vertices[vertexId] = v;
                }
            }

            for (int i = 0; i < vertices.Count; i++)
            {
                var v = vertices[i];
                NormalizeBoneWeights(ref v);
                vertices[i] = v;
            }
        }

        public static void NormalizeBoneWeights(ref Vertex v)
        {
            float total = v.Weights.X + v.Weights.Y + v.Weights.Z + v.Weights.W;

            if (total > 0f)
            {
                v.Weights.X /= total;
                v.Weights.Y /= total;
                v.Weights.Z /= total;
                v.Weights.W /= total;
            }
            else
            {
                // fallback: no weights assigned → give full weight to slot0
                v.Weights = new Vector4(1, 0, 0, 0);
            }
        }

        private List<Texture> LoadMaterialTextures(Assimp.Scene* scene, Assimp.Material* mat, Assimp.TextureType type, TextureType typeName)
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

                assimp.GetMaterialTexture(
                    mat, type, i,
                    &str, &mapping, &uvIndex, &blend, &op, &mapMode, &flags
                );

                // convert AssimpString -> string
                string pathStr = str.AsString; // or str.ToString() depending on Silk.NET version

                bool skip = false;
                foreach (var loaded in _texturesLoaded)
                {
                    if (loaded.Path == pathStr)
                    {
                        textures.Add(loaded);
                        skip = true;
                        break;
                    }
                }

                if (!skip)
                {
                    Texture texture;


                    if (!string.IsNullOrEmpty(pathStr) && pathStr[0] == '*')
                    {
                        // TEXTURA EMBUTIDA: "*0" -> scene->MTextures[0]
                        if (!int.TryParse(pathStr.Substring(1), out int idx))
                            throw new Exception($"Embedded texture index parse failed: {pathStr}");

                        if (idx < 0 || (uint)idx >= scene->MNumTextures)
                            throw new Exception($"Embedded texture index out of range: {pathStr} (num={scene->MNumTextures})");

                        Assimp.Texture* emb = scene->MTextures[idx];
                        if (emb == null)
                            throw new Exception($"Embedded texture pointer null: {pathStr}");

                        texture = new Texture
                        {
                            Id = (uint)TextureFromEmbedded(emb),
                            Type = typeName,
                            Path = pathStr
                        };
                    }
                    else
                    {
                        // TEXTURA EXTERNA NO DISCO
                        texture = new Texture
                        {
                            Id = (uint)TextureFromFile(pathStr, _directory),
                            Type = typeName,
                            Path = pathStr
                        };
                    }

                    textures.Add(texture);
                    _texturesLoaded.Add(texture);
                }
            }

            return textures;
        }

        private static int TextureFromFile(string filename, string directory)
        {
            // Build a portable path: directory of the model + texture filename
            string filepath = Path.Combine(directory, filename);

            int textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            // StbImageSharp.StbImage.stbi_set_flip_vertically_on_load(1);

            using var stream = System.IO.File.OpenRead(filepath);
            var image = StbImageSharp.ImageResult.FromStream(stream, StbImageSharp.ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // reasonable defaults
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return textureId;
        }

        private static int TextureFromEmbedded(Assimp.Texture* emb)
        {
            int textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            if (emb->MHeight != 0)
            {
                int width = (int)emb->MWidth;
                int height = (int)emb->MHeight;

                // Cada texel tem 4 bytes (R,G,B,A). Em Silk, o tipo costuma ser 'Texel' com bytes R,G,B,A.
                int size = width * height * 4;
                byte[] pixelData = new byte[size];

                // Copia memória dos texels
                // emb->PCData é Texel*, copiamos como stream de bytes
                Marshal.Copy((IntPtr)emb->PcData, pixelData, 0, size);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                    width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixelData);
            }
            else
            {
                int byteCount = (int)emb->MWidth;
                byte[] compressed = new byte[byteCount];
                Marshal.Copy((IntPtr)emb->PcData, compressed, 0, byteCount);

                // StbImageSharp.StbImage.stbi_set_flip_vertically_on_load(1);

                using var ms = new MemoryStream(compressed);
                var image = StbImageSharp.ImageResult.FromStream(ms, StbImageSharp.ColorComponents.RedGreenBlueAlpha);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                    image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return textureId;

        }
    }
}
