
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


            var pos = Helpers.GenRandomPosition();
            CubeObject cube = new(shader, pos);

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
                if (kb.IsKeyDown(Keys.W)) camera.ProcessKeyboard(Keys.W, (float)e.Time);
                if (kb.IsKeyDown(Keys.S)) camera.ProcessKeyboard(Keys.S, (float)e.Time);
                if (kb.IsKeyDown(Keys.A)) camera.ProcessKeyboard(Keys.A, (float)e.Time);
                if (kb.IsKeyDown(Keys.D)) camera.ProcessKeyboard(Keys.D, (float)e.Time);
                if (kb.IsKeyDown(Keys.Space)) camera.ProcessKeyboard(Keys.Space, (float)e.Time);
                if (kb.IsKeyDown(Keys.LeftControl)) camera.ProcessKeyboard(Keys.LeftControl, (float)e.Time);
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

                cube.Draw();


                game.SwapBuffers();
            };

            game.Unload += () =>
            {
                cube.Dispose();

            };

            game.Run();
        }
    }
}
