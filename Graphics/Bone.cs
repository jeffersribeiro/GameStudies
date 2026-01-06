using System.Diagnostics;
using Matrix4 = System.Numerics.Matrix4x4;
using Vector3 = System.Numerics.Vector3;
using Quaternion = System.Numerics.Quaternion;
using Assimp = Silk.NET.Assimp;

namespace GameStudies.Graphics
{
    public struct BoneInfo
    {
        public int Id;
        public Matrix4 Offset;
    };

    struct KeyPosition
    {
        public Vector3 Position;
        public float TimeStamp;
    };

    struct KeyRotation
    {
        public Quaternion Orientation;
        public float TimeStamp;
    };

    struct KeyScale
    {
        public Vector3 Scale;
        public float TimeStamp;
    };

    public unsafe class Bone
    {

        private List<KeyPosition> _Positions = new();
        private List<KeyRotation> _Rotations = new();
        private List<KeyScale> _Scales = new();
        private int _NumPositions;
        private int _NumRotations;
        private int _NumScalings;

        private Matrix4 _LocalTransform;
        private string _Name;
        private int _ID;


        public Bone(string name, int ID, Assimp.NodeAnim* channel)
        {
            _Name = name;
            _ID = ID;
            _LocalTransform = Matrix4.Identity;


            _NumPositions = (int)channel->MNumPositionKeys;

            for (int positionIndex = 0; positionIndex < _NumPositions; ++positionIndex)
            {
                var aiPosition = channel->MPositionKeys[positionIndex].MValue;
                float timeStamp = (float)channel->MPositionKeys[positionIndex].MTime;
                KeyPosition data;
                data.Position = aiPosition;
                data.TimeStamp = timeStamp;
                _Positions.Add(data);
            }

            _NumRotations = (int)channel->MNumRotationKeys;
            for (int rotationIndex = 0; rotationIndex < _NumRotations; ++rotationIndex)
            {
                var aiOrientation = channel->MRotationKeys[rotationIndex].MValue;
                float timeStamp = (float)channel->MRotationKeys[rotationIndex].MTime;
                KeyRotation data;
                data.Orientation = aiOrientation;
                data.TimeStamp = timeStamp;
                _Rotations.Add(data);
            }

            _NumScalings = (int)channel->MNumScalingKeys;
            for (int keyIndex = 0; keyIndex < _NumScalings; ++keyIndex)
            {
                var scale = channel->MScalingKeys[keyIndex].MValue;
                float timeStamp = (float)channel->MScalingKeys[keyIndex].MTime;
                KeyScale data;
                data.Scale = scale;
                data.TimeStamp = timeStamp;
                _Scales.Add(data);
            }
        }

        /*interpolates  b/w positions,rotations & scaling keys based on the curren time of 
        the animation and prepares the local transformation matrix by combining all keys 
        tranformations*/
        public void Update(float animationTime)
        {
            Matrix4 translation = InterpolatePosition(animationTime);
            Matrix4 rotation = InterpolateRotation(animationTime);
            Matrix4 scale = InterpolateScaling(animationTime);
            _LocalTransform = translation * rotation * scale;
        }

        public Matrix4 GetLocalTransform() { return _LocalTransform; }

        public string GetBoneName() { return _Name; }
        public int GetBoneID() { return _ID; }


        /* Gets the current index on mKeyPositions to interpolate to based on 
        the current animation time*/
        public int GetPositionIndex(float animationTime)
        {
            for (int index = 0; index < _NumPositions - 1; ++index)
            {
                if (animationTime < _Positions[index + 1].TimeStamp)
                    return index;
            }

            Debug.Print("ERROR: GetPositionIndex");
            return 0;
        }

        /* Gets the current index on mKeyRotations to interpolate to based on the 
        current animation time*/
        public int GetRotationIndex(float animationTime)
        {
            for (int index = 0; index < _NumRotations - 1; ++index)
            {
                if (animationTime < _Rotations[index + 1].TimeStamp)
                    return index;
            }

            Debug.Print("ERROR: GetRotationIndex");
            return 0;
        }

        /* Gets the current index on mKeyScalings to interpolate to based on the 
        current animation time */
        public int GetScaleIndex(float animationTime)
        {
            for (int index = 0; index < _NumScalings - 1; ++index)
            {
                if (animationTime < _Scales[index + 1].TimeStamp)
                    return index;
            }

            Debug.Print("ERROR: GetScaleIndex");
            return 0;
        }



        /* Gets normalized value for Lerp & Slerp*/
        private float GetScaleFactor(float lastTimeStamp, float nextTimeStamp, float animationTime)
        {
            float scaleFactor = 0.0f;
            float midWayLength = animationTime - lastTimeStamp;
            float framesDiff = nextTimeStamp - lastTimeStamp;
            scaleFactor = midWayLength / framesDiff;
            return scaleFactor;
        }

        /*figures out which position keys to interpolate b/w and performs the interpolation 
        and returns the translation matrix*/
        private Matrix4 InterpolatePosition(float animationTime)
        {
            if (_NumPositions == 0) return Matrix4.Identity; // Ou a posição padrão do osso
            if (_NumPositions == 1) return Matrix4.CreateTranslation(_Positions[0].Position);

            int p0Index = GetPositionIndex(animationTime);
            int p1Index = p0Index + 1;

            float scaleFactor = GetScaleFactor(_Positions[p0Index].TimeStamp, _Positions[p1Index].TimeStamp, animationTime);
            var finalPosition = Vector3.Lerp(_Positions[p0Index].Position, _Positions[p1Index].Position, scaleFactor);
            return Matrix4.CreateTranslation(finalPosition);
        }

        /*figures out which rotations keys to interpolate b/w and performs the interpolation 
        and returns the rotation matrix*/
        private Matrix4 InterpolateRotation(float animationTime)
        {
            if (1 == _NumRotations)
            {
                var rotation = _Rotations[0].Orientation;
                return Matrix4.CreateFromQuaternion(rotation);
            }

            int p0Index = GetRotationIndex(animationTime);
            int p1Index = p0Index + 1;

            float scaleFactor = GetScaleFactor(_Rotations[p0Index].TimeStamp, _Rotations[p1Index].TimeStamp, animationTime);

            var finalRotation = Quaternion.Slerp(_Rotations[p0Index].Orientation, _Rotations[p1Index].Orientation, scaleFactor);
            finalRotation = Quaternion.Normalize(finalRotation);
            return Matrix4.CreateFromQuaternion(finalRotation);
        }

        /*figures out which scaling keys to interpolate b/w and performs the interpolation 
        and returns the scale matrix*/
        private Matrix4 InterpolateScaling(float animationTime)
        {
            if (1 == _NumScalings)
                return Matrix4.CreateScale(_Scales[0].Scale);

            int p0Index = GetScaleIndex(animationTime);
            int p1Index = p0Index + 1;
            float scaleFactor = GetScaleFactor(_Scales[p0Index].TimeStamp,
                _Scales[p1Index].TimeStamp, animationTime);
            var finalScale = Vector3.Lerp(_Scales[p0Index].Scale, _Scales[p1Index].Scale, scaleFactor);
            return Matrix4.CreateScale(finalScale);
        }
    }

}