using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;

namespace GameStudies.Source
{
    public class Program
    {
        public static void Main()
        {
            using var game = new GameWindow(GameWindowSettings.Default, NativeWindowSettings.Default);


            float[] greenVertices = [
                //    positions             colors              texture coords
                //    X      Y     X        R    G     B       X     Y
                0.5f,  0.5f, 0.0f,   1.0f, 0.0f, 0.0f,   2.0f, 2.0f, // top right
                0.5f, -0.5f, 0.0f,   0.0f, 1.0f, 0.0f,   2.0f, 0.0f, // bottom right
                -0.5f, -0.5f, 0.0f,   0.0f, 0.0f, 1.0f,   0.0f, 0.0f, // bottom left
                -0.5f,  0.5f, 0.0f,   1.0f, 1.0f, 0.0f,   0.0f, 2.0f  // top left 
            ];

            string[] texPaths = [
                "C:/Users/Jeffe/OneDrive/Documents/Projects/GameStudies/assets/container_texture.png",
                "C:/Users/Jeffe/OneDrive/Documents/Projects/GameStudies/assets/smile_emoji.png",
            ];

            var triangle = new Mesh(greenVertices, texPaths);

            triangle.LoadTextures();

            game.Load += () =>
            {
                triangle.Load();
            };

            game.RenderFrame += args =>
            {
                GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                triangle.Draw();

                game.SwapBuffers();
            };

            game.Unload += () =>
            {
                triangle.Dispose();
            };

            game.Run();
        }
    }
}