using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using GameStudies.Graphics;
using GameStudies.Objects;

namespace GameStudies.Core
{
    public class Game : GameWindow
    {
        private Shader _shader = default!;
        private Camera _camera = default!;

        private Framebuffer _fbo = default!;
        private ScreenQuad _quad = default!;
        private Shader _postShader = default!;

        private Skybox _skybox;

        private Model _guitar = default!;
        private Light _light1 = new();
        private Light _light2 = new();
        private Light _light3 = new();
        private CubeObject cube;
        private SquareObject square1;
        private SquareObject square2;

        private bool _rightMouseDown;
        private Vector2 _lastMousePos;
        private bool _autoRotate = true;
        private Vector3 _cubePos = Vector3.Zero;
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
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _camera = new Camera();
            _camera.AspectRatio = (float)ClientSize.X / ClientSize.Y;


            string[] faces = [
                "skybox/right.jpg",
                "skybox/left.jpg",
                "skybox/top.jpg",
                "skybox/bottom.jpg",
                "skybox/front.jpg",
                "skybox/back.jpg"
            ];

            _skybox = new(faces);

            _fbo = new(ClientSize.X, ClientSize.Y);
            _quad = new();
            _postShader = new("post.vert", "post.frag");

            var vertPath = "light.vert";
            var fragPath = "light.frag";
            _shader = new Shader(vertPath, fragPath);

            _guitar = new Model("TinySword/Characters/gltf/Barbarian.glb");

            cube = new(Helpers.GenRandomPosition());
            square1 = new(Helpers.GenRandomPosition());
            square2 = new(Helpers.GenRandomPosition());

            square2.Rotation = new(1.0f, 0, 0);

            _light1.Type = LightType.Point;
            _light2.Type = LightType.Spot;
            _light3.Type = LightType.Directional;

            _light2.Specular = new(1.0f, 1.0f, 1.0f);
            _light2.Ambient = new(1.0f, 1.0f, 1.0f);
            _light2.Direction = new(1.0f, 1.0f, 1.0f);

        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            _fbo.Resize(e.Width, e.Height);
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
            var kb = KeyboardState;
            _camera.ProcessKeyboard(kb, (float)e.Time);
            square1.ProcessKeyboard(kb, (float)e.Time);

            if (_rightMouseDown)
            {
                var pos = MouseState.Position;
                var delta = pos - _lastMousePos;
                _lastMousePos = pos;
                _camera.ProcessMouseMovement(delta);
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            _fbo.Bind();
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.DepthMask(true);
            GL.Viewport(0, 0, _fbo.Width, _fbo.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var view = _camera.ViewMatrix;
            var proj = _camera.ProjectionMatrix;

            _shader.Use();
            _shader.SetMat4("view", view);
            _shader.SetMat4("projection", proj);
            _shader.SetVec3("viewPos", _camera.Position);

            _shader.SetFloat("material.shininess", 32.0f);

            _light1.Diffuse = new(1.0f, 0, 0);

            _guitar.Draw(_shader);
            cube.Draw(_shader);

            var transparentObjects = new List<SquareObject> { square1, square2 };

            transparentObjects = transparentObjects
            .OrderByDescending(sq => (_camera.Position - sq.Position).Length)
            .ToList();

            foreach (var sq in transparentObjects)
            {
                sq.Draw(_shader);
            }

            _light2.Position = cube.Position;

            _light1.Apply(_shader, 0);
            _light2.Apply(_shader, 1);
            //_light3.Apply(_shader, 2);

            GL.DepthFunc(DepthFunction.Lequal);
            GL.Disable(EnableCap.CullFace);

            _skybox.Draw(_camera.ViewMatrix, _camera.ProjectionMatrix);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.DepthFunc(DepthFunction.Less);

            Framebuffer.BindDefault();
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.Disable(EnableCap.DepthTest);
            _postShader.Use();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _fbo.ColorTex);
            _postShader.SetInt("sceneTex", 0);

            _quad.Draw();
            GL.Enable(EnableCap.DepthTest);

            SwapBuffers();
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _guitar?.Dispose();
            cube.Dispose();
            square1.Dispose();
            square2.Dispose();

            _postShader?.Delete();
            _quad?.Dispose();
            _fbo?.Dispose();

            _shader?.Delete();
        }
    }
}
