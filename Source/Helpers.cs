
using OpenTK.Mathematics;

namespace GameStudies.Source
{

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