using OpenTK.Mathematics;
using Assimp = Silk.NET.Assimp;

namespace GameStudies
{
    public static class AssimpConverters
    {
        public static Vector3 ToOpenTK(this System.Numerics.Vector3 vector)
            => new(vector.X, vector.Y, vector.Z);

        public static Quaternion ToOpenTK(this Assimp.AssimpQuaternion q)
            => new(q.X, q.Y, q.Z, q.W);

        public static Matrix4 ToOpenTK(this System.Numerics.Matrix4x4 m)
            => new(
                    m.M11, m.M21, m.M31, m.M41,
                    m.M12, m.M22, m.M32, m.M42,
                    m.M13, m.M23, m.M33, m.M43,
                    m.M14, m.M24, m.M34, m.M44
                );
    }
}