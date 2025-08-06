
using OpenTK.Mathematics;

namespace GameStudies.Source
{

    public class Helpers
    {
        private static readonly Random _rnd = new Random();
        public static Vector3 GenRandomPosition()
        {
            var vec = new Vector3();
            for (var i = 0; i < 3; i++)
            {
                var number = (float)(_rnd.NextDouble() * 1.999999999 - 0.9999999);
                vec[i] += number;
            }

            return vec;
        }
    }
}