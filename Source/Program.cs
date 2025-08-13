
using GameStudies.Objects;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameStudies.Source
{
    public class Program
    {
        public static void Main()
        {
            var nativeSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "GameStudies"
            };
            using var game = new GameWindow(GameWindowSettings.Default, nativeSettings);

            const string vert = "C:/Users/Jeffe/OneDrive/Documents/Projects/GameStudies/shaders/shader.vert";
            const string frag = "C:/Users/Jeffe/OneDrive/Documents/Projects/GameStudies/shaders/shader.frag";

            const string lightVert = "C:/Users/Jeffe/OneDrive/Documents/Projects/GameStudies/shaders/light.vert";
            const string lightFrag = "C:/Users/Jeffe/OneDrive/Documents/Projects/GameStudies/shaders/light.frag";

            var shader = new Shader(vert, frag);
            var lightCubeShader = new Shader(lightVert, lightFrag);

            var camera = new Camera();

            // CubeObject cube1 = new(shader, Helpers.GenRandomPosition());
            CubeObject cube2 = new(lightCubeShader, Helpers.GenRandomPosition());

            game.Load += () =>
            {
                GL.Enable(EnableCap.DepthTest);

            };

            game.UpdateFrame += e =>
            {
                float dt = (float)e.Time;

                var kb = game.KeyboardState;
                camera.ProcessKeyboard(kb, (float)e.Time);
                cube2.ProcessKeyboard(kb, (float)e.Time);

            };

            game.MouseMove += e =>
            {
                var ms = game.MouseState;
                if (ms.IsButtonDown(MouseButton.Left))
                {
                    camera.ProcessMouseMovement(ms.Delta);
                }
            };

            game.MouseWheel += e =>
            {
                camera.ProcessMouseScroll(e.Offset);
            };

            game.RenderFrame += args =>
            {
                GL.ClearColor(0.09f, 0.09f, 0.09f, 0f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                var view = camera.ViewMatrix;
                var proj = camera.ProjectionMatrix;

                shader.Use();

                shader.SetMat4("view", view);
                shader.SetMat4("projection", proj);

                // cube1.Draw();

                lightCubeShader.Use();
                lightCubeShader.SetMat4("view", view);
                lightCubeShader.SetMat4("projection", proj);

                cube2.Draw();


                game.SwapBuffers();
            };

            game.Unload += () =>
            {
                // cube1.Dispose();
                cube2.Dispose();

            };

            game.Run();
        }
    }
}
