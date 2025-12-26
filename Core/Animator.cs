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
                CalculateBoneTransform(ref _CurrentAnimation.GetRootNode(), Matrix4.Identity);
            }
        }

        public void PlayAnimation(Animation pAnimation)
        {
            _CurrentAnimation = pAnimation;
            _CurrentTime = 0.0f;
        }

        public void CalculateBoneTransform(ref AssimpNodeData node, Matrix4 parentTransform)
        {
            string nodeName = node.Name;
            Matrix4 nodeTransform = node.Transformation;

            Bone bone = _CurrentAnimation.FindBone(nodeName);

            if (bone != null)
            {
                bone.Update(_CurrentTime);
                nodeTransform = bone.GetLocalTransform();
            }

            Matrix4 globalTransformation = nodeTransform * parentTransform;

            var boneInfoMap = _CurrentAnimation.GetBoneIDMap();
            if (boneInfoMap.TryGetValue(nodeName, out var boneInfo))
            {
                _FinalBoneMatrices[boneInfo.Id] = boneInfo.Offset * globalTransformation;
            }

            for (int i = 0; i < node.Children.Count; i++)
            {
                var children = node.Children[i];
                CalculateBoneTransform(ref children, globalTransformation);
                node.Children[i] = children;
            }
        }

        public List<Matrix4> GetFinalBoneMatrices()
        {
            return _FinalBoneMatrices;
        }
    }
}