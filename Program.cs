using GameStudies.Core;

namespace GameStudies
{
    class Program
    {
        static void Main(string[] args)
        {
            using Game game = new(1920, 1080, "GltfMesh + OpenGL");
            game.Run();
        }
    }

}