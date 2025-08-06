using System;
using GameStudies.Obects;
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
                Size = new Vector2i(800, 600),
                Title = "GameStudies"
            };
            using var game = new GameWindow(GameWindowSettings.Default, nativeSettings);

            const string vert = "C:/Users/Jeffe/OneDrive/Documents/Projects/GameStudies/shaders/shader.vert";
            const string frag = "C:/Users/Jeffe/OneDrive/Documents/Projects/GameStudies/shaders/shader.frag";



            var shader = new Shader(vert, frag);

            var camera = new Camera();

            var view = camera.ViewMatrix;
            var proj = camera.ProjectionMatrix;


            var vec = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            var trans = Matrix4.Identity;
            float angle = 0f;
            Vector2 position = Vector2.Zero;
            float speed = 0.5f;
            float scale = 0.5f;
            float elapsedTime = 0f;   // acumula o tempo total

            float rotZ = 0f;
            const float rotSpeed = 60;


            CubeObject[] cubes = new CubeObject[10];

            for (var i = 0; i < cubes.Length; i++)
            {
                var pos = Helpers.GenRandomPosition();
                cubes[i] = new(shader, pos);
            }

            game.Load += () =>
                {
                    GL.Enable(EnableCap.DepthTest);
                };

            game.RenderFrame += args =>
                {

                    float dt = (float)args.Time;
                    elapsedTime += dt;

                    if (elapsedTime >= 3f)
                    {
                        speed = -speed;    // inverte direção
                        elapsedTime = 0;
                        scale = -scale;
                    }

                    scale += 0.000005f;
                    angle += dt;
                    rotZ += rotSpeed * dt;

                    position.X += speed * (float)args.Time;


                    GL.ClearColor(0.2f, 0.3f, 0.3f, 1f);
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    shader.Use();

                    foreach (var cube in cubes)
                    {
                        cube.Draw(view, proj);
                        cube.SetRotationZ(rotZ);
                        cube.SetRotationX(angle);
                        // cube.SetPositionX(position.X);
                        cube.SetScale(scale);
                    }

                    game.SwapBuffers();
                };

            game.Unload += () =>
            {
                foreach (var cube in cubes)
                {
                    cube.Dispose();
                }
            };

            game.Run();
        }
    }
}
