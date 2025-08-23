
using GameStudies.Objects;
using GameStudies.Source.Objects;
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
            var lightShader = new Shader(lightVert, lightFrag);

            var camera = new Camera();

            int quantityCubes = 20;
            List<CubeObject> cubes = [];

            for (int i = 0; i < quantityCubes; i++)
            {
                cubes.Add(new CubeObject(lightShader, Helpers.GenRandomPosition()));
                cubes[i].Rotation = Helpers.GenRandomRotation();
            }

            CubeObject cubeLight1 = new(lightShader, Helpers.GenRandomPosition());
            CubeObject cubeLight2 = new(lightShader, Helpers.GenRandomPosition());
            CubeObject cubeLight3 = new(lightShader, Helpers.GenRandomPosition());

            cubeLight1.Scale = new(0.3f);
            cubeLight2.Scale = new(0.3f);
            cubeLight3.Scale = new(0.2f);


            Light light1 = new();
            Light light2 = new();
            Light light3 = new();

            light1.Type = LightType.Point;
            light2.Type = LightType.Spot;
            light3.Type = LightType.Directionl;

            light1.Diffuse = new(1.0f, 0.0f, 0.0f);
            light2.Diffuse = new(0.0f, 1.0f, 0.0f);
            light3.Diffuse = new(0.0f, 0.0f, 1.0f);


            // light3.Ambient = new Vector3(0.2f, 0.2f, 0.2f);
            // light3.Diffuse = new Vector3(0.5f, 0.5f, 0.5f);
            // light3.Specular = new Vector3(1f, 1f, 1f);


            game.Load += () =>
            {
                game.WindowState = OpenTK.Windowing.Common.WindowState.Maximized;

                GL.Enable(EnableCap.DepthTest);
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

            game.RenderFrame += args =>
            {
                var dt = (float)args.Time;

                GL.ClearColor(0.07f, 0.07f, 0.07f, 0f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                var view = camera.ViewMatrix;
                var proj = camera.ProjectionMatrix;

                shader.Use();

                shader.SetMat4("view", view);
                shader.SetMat4("projection", proj);

                lightShader.Use();
                lightShader.SetMat4("view", view);
                lightShader.SetMat4("projection", proj);

                lightShader.SetVec3("viewPos", camera.Position);

                for (int i = 0; i < quantityCubes; i++)
                {
                    cubes[i].Draw();
                }


                light1.Position = new(cubeLight1.Position);
                light2.Position = new(cubeLight2.Position);
                light3.Position = new(cubeLight3.Position);

                light1.Apply(lightShader, 1);
                light2.Apply(lightShader, 2);
                light3.Apply(lightShader, 3);

                cubeLight1.Draw();
                cubeLight2.Draw();
                cubeLight3.Draw();

                game.SwapBuffers();
            };

            game.Unload += () =>
            {
                for (int i = 0; i < quantityCubes; i++)
                {
                    cubes[i].Dispose();
                }

                cubeLight1.Dispose();
                cubeLight2.Dispose();
                cubeLight3.Dispose();
            };

            game.Run();
        }
    }
}
