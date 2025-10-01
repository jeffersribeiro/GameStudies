using OpenTK.Graphics.OpenGL;

namespace GameStudies.Graphics
{
    public class Framebuffer : IDisposable
    {
        public int Fbo { get; private set; }
        public int ColorTex { get; private set; }
        public int DepthRbo { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Framebuffer(int width, int height)
        {
            Create(width, height);
        }

        public void Resize(int width, int height)
        {
            if (width == Width && height == Height) return;
            DisposeGL();
            Create(width, height);
        }

        void Create(int w, int h)
        {
            Width = w; Height = h;

            Fbo = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Fbo);

            ColorTex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ColorTex);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f,
                          w, h, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                                    TextureTarget.Texture2D, ColorTex, 0);

            DepthRbo = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthRbo);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, w, h);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                                       RenderbufferTarget.Renderbuffer, DepthRbo);

            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
                throw new Exception($"FBO incomplete: {status}");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Bind() => GL.BindFramebuffer(FramebufferTarget.Framebuffer, Fbo);
        public static void BindDefault() => GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        void DisposeGL()
        {
            if (DepthRbo != 0) GL.DeleteRenderbuffer(DepthRbo);
            if (ColorTex != 0) GL.DeleteTexture(ColorTex);
            if (Fbo != 0) GL.DeleteFramebuffer(Fbo);
            Fbo = ColorTex = DepthRbo = 0;
        }

        public void Dispose() => DisposeGL();
    }
}
