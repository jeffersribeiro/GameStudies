
using OpenTK.Mathematics;

namespace GameStudies
{
    public static class DirPathNames
    {
        public const string ShaderFolderName = "Shaders";
        public const string AssetsFolderName = "Assets";
    }

    public static class Constants
    {
        public const int MAX_BONE_INFLUENCE = 4;
    }

    public class Helpers
    {
        private static readonly Random _rnd = new Random();
        public static Vector3 GenRandomPosition(float range = 2.5f)
        {
            return new Vector3(
                (float)(_rnd.NextDouble() * 2 * range - range),
                (float)(_rnd.NextDouble() * 2 * range - range),
                (float)(_rnd.NextDouble() * 2 * range - range)
            );
        }

        public static Vector3 GenRandomRotation()
        {
            return new Vector3(
                (float)(_rnd.NextDouble() * 360.0),
                (float)(_rnd.NextDouble() * 360.0),
                (float)(_rnd.NextDouble() * 360.0)
            );
        }
    }
}