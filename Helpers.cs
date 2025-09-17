
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace GameStudies
{
    public static class DirPathNames
    {
        public const string ShaderFolderName = "Shaders";
        public const string AssetsFolderName = "Assets";
    }

    public static class Constants
    {
        public const int MAX_BONE_INFLUENCE = 4;
    }

    public class Helpers
    {
        private static readonly Random _rnd = new Random();
        public static Vector3 GenRandomPosition(float range = 2.5f)
        {
            return new Vector3(
                (float)(_rnd.NextDouble() * 2 * range - range),
                (float)(_rnd.NextDouble() * 2 * range - range),
                (float)(_rnd.NextDouble() * 2 * range - range)
            );
        }

        public static Vector3 GenRandomRotation()
        {
            return new Vector3(
                (float)(_rnd.NextDouble() * 360.0),
                (float)(_rnd.NextDouble() * 360.0),
                (float)(_rnd.NextDouble() * 360.0)
            );
        }
    }

    public static class TextureLoader
    {
        public static int Load2D(string path)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            StbImageSharp.StbImage.stbi_set_flip_vertically_on_load(1);
            using var fs = File.OpenRead(path);
            var img = StbImageSharp.ImageResult.FromStream(fs, StbImageSharp.ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, img.Width, img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, img.Data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            return id;
        }
    }
}