using Matrix4 = System.Numerics.Matrix4x4;
using Assimp = Silk.NET.Assimp;

namespace GameStudies.Graphics
{
    public struct AssimpNodeData
    {
        public Matrix4 Transformation;
        public string Name;
        public int ChildrenCount;
        public List<AssimpNodeData> Children;
    };

    public unsafe class Animation
    {
        private float _Duration;
        private int _TicksPerSecond;
        private List<Bone> _Bones = new();
        private AssimpNodeData _RootNode;
        private Dictionary<string, BoneInfo> _BoneInfoMap;
        readonly Assimp.Assimp assimp = Assimp.Assimp.GetApi();

        public Animation() { }

        public Animation(string animationPath, Model model)
        {
            string baseDir = AppContext.BaseDirectory;
            string fullpath = Path.Combine(baseDir, DirPathNames.AssetsFolderName, animationPath);

            Assimp.Scene* scene = assimp.ImportFile(fullpath, (uint)Assimp.PostProcessSteps.Triangulate);
            if (scene == null || scene->MRootNode == null)
                throw new Exception("error on loading scene and rootNode");

            var animation = scene->MAnimations[0];
            _Duration = (float)animation->MDuration;
            _TicksPerSecond = (int)animation->MTicksPerSecond;
            if (_TicksPerSecond <= 0) _TicksPerSecond = 25;


            ReadHeirarchyData(ref _RootNode, scene->MRootNode);
            ReadMissingBones(animation, ref model);

            assimp.ReleaseImport(scene);
        }

        public Bone FindBone(string name)
        {
            return _Bones.FirstOrDefault(bone => bone.GetBoneName() == name);
        }

        public float GetTicksPerSecond() { return _TicksPerSecond; }

        public float GetDuration() { return _Duration; }

        public ref AssimpNodeData GetRootNode() { return ref _RootNode; }

        public ref Dictionary<string, BoneInfo> GetBoneIDMap()
        {
            return ref _BoneInfoMap;
        }


        private void ReadMissingBones(Assimp.Animation* animation, ref Model model)
        {
            int channelCount = (int)animation->MNumChannels;

            // IMPORTANT: capture the ref returns *as refs*
            ref var boneInfoMap = ref model.GetBoneInfoMap();  // Dictionary<string, BoneInfo>
            ref int boneCount = ref model.GetBoneCount();    // int

            for (int i = 0; i < channelCount; i++)
            {
                var channel = animation->MChannels[i];

                // In Silk.NET.Assimp, MNodeName is AssimpString; make sure to get a C# string
                string boneName = channel->MNodeName.AsString;

                // If this channel's bone is not in the model's map yet, add it
                if (!boneInfoMap.TryGetValue(boneName, out var info))
                {
                    info = new BoneInfo
                    {
                        Id = boneCount,
                        // Animation-only bones (not present in mesh->bones) have no offset matrix from Assimp.
                        // Use Identity so they don't distort the mesh.
                        Offset = Matrix4.Identity
                    };

                    boneInfoMap[boneName] = info;
                    boneCount++;
                }

                // Use the ID that exists in the map (either preexisting or just added)
                _Bones.Add(new Bone(boneName, boneInfoMap[boneName].Id, channel));
            }

            // If you keep a local animator-side copy, sync it
            _BoneInfoMap = boneInfoMap;
        }
        private void ReadHeirarchyData(ref AssimpNodeData dest, Assimp.Node* src)
        {

            dest.Name = src->MName.AsString;

            var matrix = src->MTransformation;
            dest.Transformation = matrix;
            dest.ChildrenCount = (int)src->MNumChildren;

            if (dest.Children == null)
            {
                dest.Children = new();
            }

            for (int i = 0; i < src->MNumChildren; i++)
            {
                AssimpNodeData newData = new();
                ReadHeirarchyData(ref newData, src->MChildren[i]);
                dest.Children.Add(newData);
            }
        }
    }
}