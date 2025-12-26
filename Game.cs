using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using GameStudies.Graphics;

namespace GameStudies.Core
{
    public class Game : GameWindow
    {
        private Shader _shader = default!;
        private Camera _camera = default!;

        private Model _model = default!;
        private Animator _animator = default!;
        private bool _rightMouseDown;
        private Vector2 _lastMousePos;

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

            _camera = new Camera();
            _camera.AspectRatio = (float)ClientSize.X / ClientSize.Y;


            var vertPath = "shader.vert";
            var fragPath = "shader.frag";
            _shader = new Shader(vertPath, fragPath);

            _model = new Model("TinySword/Characters/gltf/Knight.glb");
            Animation danceAnimation = new("TinySword/Characters/gltf/Knight.glb", _model);
            _animator = new(danceAnimation);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
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

            _animator.UpdateAnimation((float)e.Time);

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

            GL.ClearColor(0.05f, 0.05f, 0.05f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var view = _camera.ViewMatrix;
            var proj = _camera.ProjectionMatrix;

            _shader.Use();
            _shader.SetMat4("view", view);
            _shader.SetMat4("projection", proj);
            _shader.SetVec3("viewPos", _camera.Position);

            var transforms = _animator.GetFinalBoneMatrices();
            for (int i = 0; i < transforms.Count; ++i)
            {
                _shader.SetMat4($"finalBonesMatrices[{i}]", transforms[i]);
            }

            var Rotation = new Vector3();

            var model =
            Matrix4.CreateScale(Vector3.One)
            * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X))
            * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y))
            * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z))
            * Matrix4.CreateTranslation(Vector3.Zero);

            _shader.SetMat4("model", model);
            _model.Draw(_shader);

            SwapBuffers();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            _model?.Dispose();
            _shader?.Delete();
        }
    }
}
