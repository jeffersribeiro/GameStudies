using GameStudies.Graphics;
using OpenTK.Mathematics;

namespace GameStudies.Core
{
    public unsafe class Animator
    {
        private List<Matrix4> _FinalBoneMatrices = new(100);
        private Animation _CurrentAnimation;
        private float _CurrentTime;
        private float _DeltaTime;

        public Animator(Animation currentAnimation)
        {
            _CurrentTime = 0.0f;
            _CurrentAnimation = currentAnimation;

            for (int i = 0; i < 100; i++)
                _FinalBoneMatrices.Add(Matrix4.Identity);

        }

        public void UpdateAnimation(float dt)
        {
            _DeltaTime = dt;
            if (_CurrentAnimation != null)
            {
                _CurrentTime += _CurrentAnimation.GetTicksPerSecond() * dt;
                float duration = _CurrentAnimation.GetDuration();
                if (duration > 0f)
                {
                    _CurrentTime %= duration;
                    if (_CurrentTime < 0f) _CurrentTime += duration;
                }
                CalculateBoneTransform(_CurrentAnimation.GetRootNode(), Matrix4.Identity);
            }
        }

        public void PlayAnimation(Animation pAnimation)
        {
            _CurrentAnimation = pAnimation;
            _CurrentTime = 0.0f;
        }


        private void CalculateBoneTransform(AssimpNodeData node, Matrix4 parentTransform)
        {
            string nodeName = node.Name;
            Matrix4 nodeTransform = node.Transformation;

            Bone? bone = _CurrentAnimation.FindBone(nodeName);
            if (bone != null)
            {
                bone.Update(_CurrentTime);
                nodeTransform = bone.GetLocalTransform();
            }

            Matrix4 globalTransformation = parentTransform * nodeTransform;

            // Prefer TryGetValue to avoid double lookup
            var boneInfoMap = _CurrentAnimation.GetBoneIDMap();
            if (boneInfoMap.TryGetValue(nodeName, out BoneInfo info))
            {
                int index = info.Id;
                Matrix4 offset = info.Offset;

                _FinalBoneMatrices[index] = _CurrentAnimation._GlobalInverseTransform * globalTransformation * offset;
            }

            // Recurse
            if (node.Children == null) return;

            for (int i = 0; i < node.Children.Count; i++)
                CalculateBoneTransform(node.Children[i], globalTransformation);
        }

        public List<Matrix4> GetFinalBoneMatrices()
        {
            return _FinalBoneMatrices;
        }
    }
}