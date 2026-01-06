
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace GameStudies.Graphics
{
    public class Framebuffer : IDisposable
    {
        private readonly GL _gl;
        public uint Fbo { get; private set; }
        public uint ColorTex { get; private set; }
        public uint DepthRbo { get; private set; }
        public uint Width { get; private set; }
        public uint Height { get; private set; }

        public Framebuffer(GL gl, uint width, uint height)
        {
            _gl = gl;
            Create(width, height);
        }

        public void Resize(uint width, uint height)
        {
            if (width == Width && height == Height) return;
            DisposeGL();
            Create(width, height);
        }

        unsafe void Create(uint w, uint h)
        {
            Width = w; Height = h;

            Fbo = _gl.GenFramebuffer();
            _gl.BindFramebuffer(FramebufferTarget.Framebuffer, Fbo);

            ColorTex = _gl.GenTexture();
            _gl.BindTexture(TextureTarget.Texture2D, ColorTex);

            _gl.TexImage2D(GLEnum.Texture2D, 0, (int)InternalFormat.Rgba16f, (uint)w, (uint)h, 0, GLEnum.Rgba, GLEnum.Float, null);

            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            _gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                                    TextureTarget.Texture2D, ColorTex, 0);

            DepthRbo = _gl.GenRenderbuffer();
            _gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthRbo);
            _gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, GLEnum.DepthComponent24, w, h);
            _gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                                       RenderbufferTarget.Renderbuffer, DepthRbo);

            _gl.DrawBuffer(DrawBufferMode.ColorAttachment0);

            var status = _gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != GLEnum.FramebufferComplete)
                throw new Exception($"FBO incomplete: {status}");

            _gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Bind() => _gl.BindFramebuffer(FramebufferTarget.Framebuffer, Fbo);
        public void BindDefault() => _gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        void DisposeGL()
        {
            if (DepthRbo != 0) _gl.DeleteRenderbuffer(DepthRbo);
            if (ColorTex != 0) _gl.DeleteTexture(ColorTex);
            if (Fbo != 0) _gl.DeleteFramebuffer(Fbo);
            Fbo = ColorTex = DepthRbo = 0;
        }

        public void Dispose() => DisposeGL();
    }
}
