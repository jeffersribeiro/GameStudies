using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;

namespace GameStudies.Controllers
{
    public class ImGuiController : IDisposable
    {
        private IntPtr _context;
        private int _fontTexture;

        public ImGuiController(int width, int height)
        {
            _context = ImGui.CreateContext();
            ImGui.SetCurrentContext(_context);
            var io = ImGui.GetIO();
            io.Fonts.AddFontDefault();

            CreateDeviceResources();
            Resize(width, height);
        }

        private void CreateDeviceResources()
        {
            var io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);

            _fontTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _fontTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            io.Fonts.SetTexID((IntPtr)_fontTexture);
            io.Fonts.ClearTexData();
        }

        public void Update(GameWindow wnd, float deltaSeconds)
        {
            var io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(wnd.Size.X, wnd.Size.Y);
            io.DeltaTime = deltaSeconds;

            // TODO: map keyboard & mouse input from wnd.KeyboardState/MouseState
            ImGui.NewFrame();
        }

        public void Render()
        {
            ImGui.Render();
            ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());
        }

        public void Resize(int width, int height) =>
            ImGui.GetIO().DisplaySize = new System.Numerics.Vector2(width, height);

        public void Dispose()
        {
            GL.DeleteTexture(_fontTexture);
            ImGui.DestroyContext(_context);
        }

        // ⚠ you’ll need a small GL renderer for ImGui draw lists here
        private void ImGui_ImplOpenGL3_RenderDrawData(ImDrawDataPtr drawData)
        {
            // Implementation exists in sample ImGuiController.cs
            // (converts ImGui vertices/indices to GL buffers and issues draw calls)
        }
    }
}