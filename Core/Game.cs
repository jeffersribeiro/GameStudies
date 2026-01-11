using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using Silk.NET.Maths;
using GameStudies.Core;

using Matrix4 = System.Numerics.Matrix4x4;
using Vector3 = System.Numerics.Vector3;
using Vector2 = System.Numerics.Vector2;
using GameStudies.Objects;

namespace GameStudies.Core
{
    public sealed class Game : IDisposable
    {
        private static IWindow window;
        private static GL Gl;
        private static Graphics.Shader Shader;
        private static CubeObject Model;
        private Camera _camera = default!;
        private float _angle;

        private bool _rightMouseDown;
        private Vector2 _lastMousePos;
        private IInputContext _input = default!;
        private IKeyboard _keyboard = default!;
        private IMouse _mouse = default!;

        // 24 vertices
        // layout: pos.xyz | normal.xyz | color.rgb
        private static readonly float[] _vertices =
        {
            // +Z (front) - RED
            -0.5f,-0.5f, 0.5f,   0,0,1,   1,0,0,
            0.5f,-0.5f, 0.5f,   0,0,1,   1,0,0,
            0.5f, 0.5f, 0.5f,   0,0,1,   1,0,0,
            -0.5f, 0.5f, 0.5f,   0,0,1,   1,0,0,

            // -Z (back) - GREEN
            0.5f,-0.5f,-0.5f,   0,0,-1,  0,1,0,
            -0.5f,-0.5f,-0.5f,   0,0,-1,  0,1,0,
            -0.5f, 0.5f,-0.5f,   0,0,-1,  0,1,0,
            0.5f, 0.5f,-0.5f,   0,0,-1,  0,1,0,

            // -X (left) - BLUE
            -0.5f,-0.5f,-0.5f,  -1,0,0,   0,0,1,
            -0.5f,-0.5f, 0.5f,  -1,0,0,   0,0,1,
            -0.5f, 0.5f, 0.5f,  -1,0,0,   0,0,1,
            -0.5f, 0.5f,-0.5f,  -1,0,0,   0,0,1,

            // +X (right) - YELLOW
            0.5f,-0.5f, 0.5f,   1,0,0,   1,1,0,
            0.5f,-0.5f,-0.5f,   1,0,0,   1,1,0,
            0.5f, 0.5f,-0.5f,   1,0,0,   1,1,0,
            0.5f, 0.5f, 0.5f,   1,0,0,   1,1,0,

            // +Y (top) - CYAN
            -0.5f, 0.5f, 0.5f,   0,1,0,   0,1,1,
            0.5f, 0.5f, 0.5f,   0,1,0,   0,1,1,
            0.5f, 0.5f,-0.5f,   0,1,0,   0,1,1,
            -0.5f, 0.5f,-0.5f,   0,1,0,   0,1,1,

            // -Y (bottom) - MAGENTA
            -0.5f,-0.5f,-0.5f,   0,-1,0,  1,0,1,
            0.5f,-0.5f,-0.5f,   0,-1,0,  1,0,1,
            0.5f,-0.5f, 0.5f,   0,-1,0,  1,0,1,
            -0.5f,-0.5f, 0.5f,   0,-1,0,  1,0,1,
        };

        public static readonly uint[] _indices =
        {
            00,01,02,  02,03,00,   // front
            04,05,06,  06,07,04,   // back
            08,09,10,  10,11,08,   // left
            12,13,14,  14,15,12,   // right
            16,17,18,  18,19,16,   // top
            20,21,22,  22,23,20    // bottom
        };

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

        private unsafe void OnLoad()
        {
            Gl = window.CreateOpenGL();

            Shader = new(Gl, "cube.vert", "cube.frag");

            Model = new(Gl);

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

        private unsafe void OnRender(double dt) //Method needs to be unsafe due to draw elements.
        {
            Gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

            Shader.Use();

            var projection = _camera.ProjectionMatrix;
            var view = _camera.ViewMatrix;
            var color = new Vector3(1f, 0.85f, 0.25f);

            Shader.SetMat4("projection", projection);
            Shader.SetMat4("view", view);
            Shader.SetVec3("uColor", color);

            Model.Rotation += new Vector3(_angle * 0.6f);

            Model.Draw(Shader);

            //Draw the geometry.
            Gl.DrawElements(PrimitiveType.Triangles, (uint)_indices.Length, DrawElementsType.UnsignedInt, null);
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