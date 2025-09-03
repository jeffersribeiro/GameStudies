using System;
using System.IO;
using GameStudies.Objects;
using GameStudies.Source.Objects;
using OpenTK.Graphics.OpenGL4;
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

            string baseDir = AppContext.BaseDirectory;

            const string vert = "C:/Users/Jeffe/OneDrive/Documents/Projects/Estudos/GameStudies/shaders/shader.vert";
            const string frag = "C:/Users/Jeffe/OneDrive/Documents/Projects/Estudos/GameStudies/shaders/shader.frag";

            string lightVert = "C:/Users/Jeffe/OneDrive/Documents/Projects/Estudos/GameStudies/shaders/light.vert";
            string lightFrag = "C:/Users/Jeffe/OneDrive/Documents/Projects/Estudos/GameStudies/shaders/light.frag";

            var shader = new Shader(vert, frag);
            var lightShader = new Shader(lightVert, lightFrag);

            var camera = new Camera();

            var guitar = new Model("C:/Users/Jeffe/OneDrive/Documents/Projects/Estudos/GameStudies/assets/backpack/backpack.obj");

            CubeObject cubeLight1 = new(lightShader, Helpers.GenRandomPosition());
            CubeObject cubeLight2 = new(lightShader, Helpers.GenRandomPosition());
            CubeObject cubeLight3 = new(lightShader, Helpers.GenRandomPosition());

            cubeLight1.Scale = new(1f);
            cubeLight2.Scale = new(1f);
            cubeLight3.Scale = new(1f);


            Light light1 = new();
            Light light2 = new();
            Light light3 = new();

            light1.Type = LightType.Point;
            light2.Type = LightType.Spot;
            light3.Type = LightType.Directional;

            light1.Diffuse = new(1f, 1f, 1f);
            light2.Diffuse = new(1f, 1f, 1f);
            light3.Diffuse = new(1f, 1f, 1f);


            game.Load += () =>
            {
                game.WindowState = OpenTK.Windowing.Common.WindowState.Maximized;
                GL.Enable(EnableCap.DepthTest);
            };

            game.UpdateFrame += e =>
            {
                var kb = game.KeyboardState;
                camera.ProcessKeyboard(kb, (float)e.Time);
            };

            game.MouseMove += e =>
            {
                var ms = game.MouseState;
                if (ms.IsButtonDown(MouseButton.Left))
                    camera.ProcessMouseMovement(ms.Delta);
            };

            game.RenderFrame += args =>
            {
                GL.ClearColor(0.70f, 0.70f, 0.70f, 1f);
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

                lightShader.SetInt("material.diffuse", 0);
                lightShader.SetInt("material.specular", 1);
                lightShader.SetFloat("material.shininess", 32.0f);

                // desenha o modelo
                guitar.Draw(lightShader);

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
                // libera o modelo e buffers
                guitar.Dispose();
                cubeLight1.Dispose();
                cubeLight2.Dispose();
                cubeLight3.Dispose();

                // shaders devem ser deletados aqui (não dentro do Mesh)
                shader.Delete();
                lightShader.Delete();
            };

            game.Run();
        }
    }
}
