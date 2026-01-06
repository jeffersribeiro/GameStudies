using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

using GameStudies.Graphics;

using Matrix4 = System.Numerics.Matrix4x4;
using Vector3 = System.Numerics.Vector3;
using Vector2 = System.Numerics.Vector2;

namespace GameStudies.Core
{
    public sealed class Game : IDisposable
    {
        private readonly IWindow _window;
        private GL _gl = default!;

        private IInputContext _input = default!;
        private IKeyboard _keyboard = default!;
        private IMouse _mouse = default!;

        private Graphics.Shader _shader = default!;
        private Camera _camera = default!;

        private uint _emptyVao;
        private float _angleY;

        private bool _rightMouseDown;
        private Vector2 _lastMousePos;

        public Game(int width, int height, string title)
        {
            var opts = WindowOptions.Default;
            opts.Title = title;
            opts.Size = new Vector2D<int>(width, height);

            _window = Window.Create(opts);
            _window.Load += OnLoad;
            _window.Update += OnUpdate;
            _window.Render += OnRender;
            _window.Resize += OnResize;
            _window.Closing += OnClosing;
        }

        public void Run() => _window.Run();

        private void OnLoad()
        {
            _gl = GL.GetApi(_window);

            _input = _window.CreateInput();
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

            _gl.Enable(EnableCap.DepthTest);
            _gl.DepthFunc(DepthFunction.Less);

            _gl.Enable(EnableCap.CullFace);
            _gl.CullFace(GLEnum.Back);
            _gl.FrontFace(FrontFaceDirection.Ccw);

            _gl.ClearColor(0.12f, 0.18f, 0.25f, 1f);

            _camera = new Camera();
            _camera.AspectRatio = _window.Size.X / (float)_window.Size.Y;
            _camera.Position = new Vector3(0f, 0f, 3.5f);

            // Shaders below:
            _shader = new(_gl, "cube.vert", "cube.frag");

            // Empty VAO is required in core profile
            _emptyVao = _gl.GenVertexArray();
        }

        private void OnResize(Vector2D<int> size)
        {
            if (_shader?.Prog == 0) return;
            _gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
            _camera.AspectRatio = size.X / (float)size.Y;
        }

        private void OnUpdate(double dt)
        {
            float delta = (float)dt;

            _camera.ProcessKeyboard(_keyboard, delta);

            if (_rightMouseDown)
            {
                var pos = _mouse.Position;
                var d = pos - _lastMousePos;
                _lastMousePos = pos;
                _camera.ProcessMouseMovement(d);
            }

            _angleY += 0.9f * delta;
        }

        private void OnRender(double dt)
        {
            _gl.BindFramebuffer(GLEnum.Framebuffer, 0);
            _gl.Viewport(0, 0, (uint)_window.Size.X, (uint)_window.Size.Y);
            _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();

            var model =
                Matrix4.CreateRotationY(_angleY) *
                Matrix4.CreateRotationX(0.35f);

            _shader.SetMat4("model", model);
            _shader.SetMat4("view", _camera.ViewMatrix);
            _shader.SetMat4("projection", _camera.ProjectionMatrix);

            _shader.SetVec3("uColor", new Vector3(1f, 0.85f, 0.25f));

            _gl.BindVertexArray(_emptyVao);
            _gl.DrawArrays(GLEnum.Triangles, 0, 36); // 12 triangles * 3
            _gl.BindVertexArray(0);
        }

        private void OnClosing() => Dispose();

        public void Dispose()
        {
            if (_emptyVao != 0) _gl.DeleteVertexArray(_emptyVao);
            _shader?.Delete();
        }
    }
}
