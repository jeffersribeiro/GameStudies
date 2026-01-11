using Silk.NET.Maths;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

using Vector3 = System.Numerics.Vector3;
using Vector2 = System.Numerics.Vector2;
using GameStudies.Objects;
using GameStudies.Graphics;

namespace GameStudies.Core
{
    public sealed class Game : IDisposable
    {
        private static IWindow window;
        private static GL Gl;
        private static Graphics.Shader Shader;
        private static Model Model;
        private static Animator Animator;
        private Camera _camera = default!;
        private float _angle;

        private bool _rightMouseDown;
        private Vector2 _lastMousePos;
        private IInputContext _input = default!;
        private IKeyboard _keyboard = default!;
        private IMouse _mouse = default!;

        public Game(int width, int height, string title)
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(width, height);
            options.Title = title;
            options.API = new GraphicsAPI(
            ContextAPI.OpenGL,
            ContextProfile.Core,
            ContextFlags.Default,
            new APIVersion(3, 3));
            window = Window.Create(options);

            window.Load += OnLoad;
            window.Render += OnRender;
            window.Update += OnUpdate;
            window.FramebufferResize += OnFramebufferResize;
            window.Closing += () => Dispose();
        }

        public void Run() => window.Run();

        private void OnLoad()
        {
            Gl = window.CreateOpenGL();

            Shader = new(Gl, "cube.vert", "cube.frag");

            var modelPath = "Barbarian.glb";

            Model = new(Gl, modelPath);
            var animation = new Animation(modelPath, Model);
            Animator = new(animation);

            _input = window.CreateInput();
            for (int i = 0; i < _input.Keyboards.Count; i++)
            {
                _input.Keyboards[i].KeyDown += KeyDown;
            }

            _keyboard = _input.Keyboards.Count > 0 ? _input.Keyboards[0] : throw new InvalidOperationException("No keyboard found.");
            _mouse = _input.Mice.Count > 0 ? _input.Mice[0] : throw new InvalidOperationException("No mouse found.");

            _mouse.MouseDown += (_, btn) =>
            {
                if (btn == MouseButton.Right)
                {
                    _rightMouseDown = true;
                    _lastMousePos = _mouse.Position;
                }
            };
            _mouse.MouseUp += (_, btn) =>
            {
                if (btn == MouseButton.Right) _rightMouseDown = false;
            };
            _mouse.Scroll += (_, wheel) => _camera.ProcessMouseScroll(wheel);


            Gl.Enable(GLEnum.DepthTest);
            Gl.ClearColor(0.39f, 0.58f, 0.93f, 1.0f);

            _camera = new();
            _camera.AspectRatio = window.Size.X / (float)window.Size.Y;
            _camera.Position = new Vector3(0f, 0f, 3.5f);
        }

        private void OnRender(double dt)
        {
            Gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

            var projection = _camera.ProjectionMatrix;
            var view = _camera.ViewMatrix;

            Shader.Use();
            Shader.SetMat4("projection", projection);
            Shader.SetMat4("view", view);

            var transforms = Animator.GetFinalBoneMatrices();
            for (int i = 0; i < transforms.Count; ++i)
            {
                Shader.SetMat4($"finalBonesMatrices[{i}]", transforms[i]);
            }

            Model.Draw(Shader);
        }

        private void OnUpdate(double dt)
        {
            float delta = (float)dt;

            Animator.UpdateAnimation(delta);

            _camera.ProcessKeyboard(_keyboard, delta);

            if (_rightMouseDown)
            {
                var pos = _mouse.Position;
                var d = pos - _lastMousePos;
                _lastMousePos = pos;
                _camera.ProcessMouseMovement(d);
            }

            _angle += 0.9f * delta;
        }

        private void OnFramebufferResize(Vector2D<int> newSize)
        {
            Gl.Viewport(0, 0, (uint)newSize.X, (uint)newSize.Y);
            _camera.AspectRatio = newSize.X / (float)newSize.Y;
        }

        private static void OnClose()
        {
            //Remember to delete the buffers.
            Model.Dispose();
            Shader.Delete();
        }

        private static void KeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            if (arg2 == Key.Escape)
            {
                window.Close();
            }
        }

        private void OnClosing() => Dispose();
        public void Dispose()
        {

        }
    }
}