
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
                Size = new Vector2i(1920, 1080),
                Title = "GameStudies",
            };
            using var game = new GameWindow(GameWindowSettings.Default, nativeSettings);

            const string vert = "C:/Users/Jeffe/OneDrive/Documents/Projects/GameStudies/shaders/shader.vert";
            const string frag = "C:/Users/Jeffe/OneDrive/Documents/Projects/GameStudies/shaders/shader.frag";

            const string lightVert = "C:/Users/Jeffe/OneDrive/Documents/Projects/GameStudies/shaders/light.vert";
            const string lightFrag = "C:/Users/Jeffe/OneDrive/Documents/Projects/GameStudies/shaders/light.frag";

            var shader = new Shader(vert, frag);
            var lightCubeShader = new Shader(lightVert, lightFrag);

            var camera = new Camera();

            int quantityCubes = 10;
            List<CubeObject> cubes = [];

            for (int i = 0; i < quantityCubes; i++)
            {
                cubes.Add(new CubeObject(lightCubeShader, Helpers.GenRandomPosition()));
                cubes[i].Rotation = Helpers.GenRandomRotation();
            }

            CubeLight cubeLight1 = new(lightCubeShader);

            game.Load += () =>
            {
                game.WindowState = OpenTK.Windowing.Common.WindowState.Maximized;

                GL.Enable(EnableCap.DepthTest);
                cubeLight1.Load();
            };

            game.UpdateFrame += e =>
            {
                float dt = (float)e.Time;

                var kb = game.KeyboardState;
                camera.ProcessKeyboard(kb, (float)e.Time);
                cubeLight1.ProcessKeyboard(kb, (float)e.Time);

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
                cubeLight1.ProcessMouseScroll(e.Offset);
            };

            game.RenderFrame += args =>
            {
                var dt = (float)args.Time;

                GL.ClearColor(0.09f, 0.09f, 0.09f, 0f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                var view = camera.ViewMatrix;
                var proj = camera.ProjectionMatrix;

                shader.Use();

                shader.SetMat4("view", view);
                shader.SetMat4("projection", proj);


                lightCubeShader.Use();
                lightCubeShader.SetMat4("view", view);
                lightCubeShader.SetMat4("projection", proj);

                lightCubeShader.SetVec3("viewPos", camera.Position);

                cubeLight1.Draw(dt);

                for (int i = 0; i < quantityCubes; i++)
                {
                    cubes[i].Draw();
                }


                game.SwapBuffers();
            };

            game.Unload += () =>
            {
                cubeLight1.Dispose();

                for (int i = 0; i < quantityCubes; i++)
                {
                    cubes[i].Dispose();
                }
            };

            game.Run();
        }
    }
}
