using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ImGuiNET;

// your own namespaces (adjust as per your project)
using GameStudies.Graphics;     // Shader, Model, etc.
using GameStudies.Controllers;  // ImGuiController, Camera
using GameStudies.Objects;
using GameStudies.Factories;      // CubeObject, Light, Helpers

namespace GameStudies.Core
{
    public class Game : GameWindow
    {
        // --- Rendering / Scene ---
        private Shader _shader = default!;
        private Camera _camera = default!;

        // Model & light gizmos
        private Model _guitar = default!;
        private Light _light1 = new();
        private Light _light2 = new();
        private Light _light3 = new();
        private CubeObject cube;
        private SquareObject square;

        // --- Input state ---
        private bool _rightMouseDown;
        private Vector2 _lastMousePos;

        // --- ImGui ---
        private ImGuiController _imgui = default!;

        // UI-controlled properties for one cube (example: _cubeLight2)
        private bool _autoRotate = true;
        private System.Numerics.Vector3 _cubePos = System.Numerics.Vector3.Zero;
        private float _cubeScale = 1.0f;
        private float _cubeSpeed = 1.5f;

        public Game(int width, int height, string title)
            : base(
                GameWindowSettings.Default,
                new NativeWindowSettings
                {
                    Size = new Vector2i(width, height),
                    Title = title,
                })
        {
            UpdateFrame += OnUpdateFrame;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // GL state
            GL.ClearColor(0.3f, 0.3f, 0.3f, 1f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

            // Camera
            _camera = new Camera();

            // Shaders
            // TODO: point to your actual shader paths or content pipeline
            var vertPath = "light.vert";
            var fragPath = "light.frag";
            _shader = new Shader(vertPath, fragPath);

            // Model (example asset)
            // TODO: replace with your asset path
            _guitar = new Model("ps1psx-hoplite-chan/source/HOPLITE CHAN ANIMATED.glb");
            cube = new(Helpers.GenRandomPosition());
            square = new(Helpers.GenRandomPosition());

            // Lights configuration
            _light1.Type = LightType.Point;
            _light2.Type = LightType.Spot;
            _light3.Type = LightType.Directional;

            _light2.Specular = new(1.0f, 1.0f, 1.0f);
            _light2.Ambient = new(1.0f, 1.0f, 1.0f);
            _light2.Direction = new(1.0f, 1.0f, 1.0f);

            // ImGui
            _imgui = new ImGuiController(ClientSize.X, ClientSize.Y);

        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            _imgui?.Resize(e.Width, e.Height);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Right && e.IsPressed)
            {
                _rightMouseDown = true;
                _lastMousePos = MouseState.Position;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Right && !e.IsPressed)
            {
                _rightMouseDown = false;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            _camera.ProcessMouseScroll(e.Offset);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // Keyboard movement (WASD, etc.) for camera
            var kb = KeyboardState;
            _camera.ProcessKeyboard(kb, (float)e.Time);
            square.ProcessKeyboard(kb, (float)e.Time);

            // RMB look
            if (_rightMouseDown)
            {
                var pos = MouseState.Position;
                var delta = pos - _lastMousePos; // pixels this frame
                _lastMousePos = pos;
                _camera.ProcessMouseMovement(delta);
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            // Start ImGui frame (this also gathers input)
            _imgui.Update(this, (float)args.Time);

            // === Inspector UI ===
            ImGui.Begin("Cube Inspector");
            ImGui.Checkbox("Auto Rotate", ref _autoRotate);
            ImGui.SliderFloat3("Position", ref _cubePos, -10f, 10f);
            ImGui.SliderFloat("Scale", ref _cubeScale, 0.1f, 5f);
            ImGui.SliderFloat("Speed", ref _cubeSpeed, 0f, 10f);
            ImGui.End();

            // --- Render 3D scene ---
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var view = _camera.ViewMatrix;
            var proj = _camera.ProjectionMatrix;

            _shader.Use();
            _shader.SetMat4("view", view);
            _shader.SetMat4("projection", proj);
            _shader.SetVec3("viewPos", _camera.Position);

            // Material (if your shader uses these)
            _shader.SetFloat("material.shininess", 32.0f);

            _light1.Diffuse = new(1.0f, 0, 0);

            // Draw model
            _guitar.Draw(_shader);

            cube.Draw(_shader);
            square.Draw(_shader);

            _light2.Position = cube.Position;


            // Push light uniforms
            _light1.Apply(_shader, 0);
            _light2.Apply(_shader, 1);
            //_light3.Apply(_shader, 2);

            // --- Render UI on top ---
            _imgui.Render();

            SwapBuffers();
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _guitar?.Dispose();
            cube.Dispose();
            square.Dispose();

            _shader?.Delete();
            _imgui?.Dispose();
        }
    }
}
