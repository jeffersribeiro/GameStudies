
using GameStudies.Obects;
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

            float deltaTime = 0.0f;
            float lastFrame = 0.0f;

            var shader = new Shader(vert, frag);

            var camera = new Camera();

            var vec = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            var trans = Matrix4.Identity;
            float angle = 0f;
            Vector2 position = Vector2.Zero;
            float speed = 0.5f;
            float scale = 0.5f;
            float elapsedTime = 0f;   // acumula o tempo total

            float rotZ = 0f;
            const float rotSpeed = 60;


            CubeObject cube1 = new(shader, Helpers.GenRandomPosition());
            CubeObject cube2 = new(shader, Helpers.GenRandomPosition());

            game.Load += () =>
            {
                GL.Enable(EnableCap.DepthTest);
            };

            game.UpdateFrame += e =>
            {
                deltaTime = 0.0f;
                lastFrame = 0.0f;

                var currentFrame = (float)e.Time;
                deltaTime = currentFrame - lastFrame;
                lastFrame = currentFrame;

                var kb = game.KeyboardState;
                camera.ProcessKeyboard(kb, (float)e.Time);
                cube1.ProcessKeyboard(kb, (float)e.Time);
                // cube2.ProcessKeyboard(kb, (float)e.Time);

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

                float dt = (float)args.Time;
                elapsedTime += dt;

                if (elapsedTime >= 3f)
                {
                    elapsedTime = 0;
                    scale = -scale;
                }

                scale += 0.000005f;
                angle += dt;
                rotZ += rotSpeed * dt;

                GL.ClearColor(0.2f, 0.3f, 0.3f, 1f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                shader.Use();

                var view = camera.ViewMatrix;
                var proj = camera.ProjectionMatrix;

                shader.SetMatrix4("view", view);
                shader.SetMatrix4("projection", proj);

                cube1.Draw();
                cube2.Draw();


                game.SwapBuffers();
            };

            game.Unload += () =>
            {
                cube1.Dispose();
                cube2.Dispose();

            };

            game.Run();
        }
    }
}
