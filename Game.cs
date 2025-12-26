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

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

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

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.DepthMask(true);
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

            _shader.SetFloat("material.shininess", 32.0f);

            _model.Draw(_shader);

            GL.DepthFunc(DepthFunction.Lequal);
            GL.Disable(EnableCap.CullFace);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.DepthFunc(DepthFunction.Less);

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.Enable(EnableCap.DepthTest);

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
