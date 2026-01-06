using System.Diagnostics;
using Matrix4 = System.Numerics.Matrix4x4;
using Vector3 = System.Numerics.Vector3;
using Vector2 = System.Numerics.Vector2;
using Quaternion = System.Numerics.Quaternion;
using Assimp = Silk.NET.Assimp;


namespace GameStudies
{
    public static class AssimpConverters
    {
        public static Vector3 ToOpenTK(this System.Numerics.Vector3 vector)
            => new(vector.X, vector.Y, vector.Z);

        public static Quaternion ToOpenTK(this Assimp.AssimpQuaternion q)
            => new(q.X, q.Y, q.Z, q.W);

        public static Matrix4 ToOpenTK(this System.Numerics.Matrix4x4 m) => new(
            m.M11, m.M12, m.M13, m.M14,
            m.M21, m.M22, m.M23, m.M24,
            m.M31, m.M32, m.M33, m.M34,
            m.M41, m.M42, m.M43, m.M44
       );

        static bool HasInvalid(System.Numerics.Matrix4x4 m)
        {
            return float.IsNaN(m.M11) || float.IsInfinity(m.M11) ||
                   float.IsNaN(m.M12) || float.IsInfinity(m.M12) ||
                   float.IsNaN(m.M13) || float.IsInfinity(m.M13) ||
                   float.IsNaN(m.M14) || float.IsInfinity(m.M14) ||
                   float.IsNaN(m.M21) || float.IsInfinity(m.M21) ||
                   float.IsNaN(m.M22) || float.IsInfinity(m.M22) ||
                   float.IsNaN(m.M23) || float.IsInfinity(m.M23) ||
                   float.IsNaN(m.M24) || float.IsInfinity(m.M24) ||
                   float.IsNaN(m.M31) || float.IsInfinity(m.M31) ||
                   float.IsNaN(m.M32) || float.IsInfinity(m.M32) ||
                   float.IsNaN(m.M33) || float.IsInfinity(m.M33) ||
                   float.IsNaN(m.M34) || float.IsInfinity(m.M34) ||
                   float.IsNaN(m.M41) || float.IsInfinity(m.M41) ||
                   float.IsNaN(m.M42) || float.IsInfinity(m.M42) ||
                   float.IsNaN(m.M43) || float.IsInfinity(m.M43) ||
                   float.IsNaN(m.M44) || float.IsInfinity(m.M44);
        }
    }
}