using Silk.NET.OpenGL;
using Vector3 = System.Numerics.Vector3;

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
        public const int MAX_BONE_WEIGHTS = 4;
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
        public static uint Load2D(GL gl, string path)
        {
            uint id = gl.GenTexture();
            gl.BindTexture(GLEnum.Texture2D, id);

            StbImageSharp.StbImage.stbi_set_flip_vertically_on_load(1);
            using var fs = File.OpenRead(path);
            var img = StbImageSharp.ImageResult.FromStream(fs, StbImageSharp.ColorComponents.RedGreenBlueAlpha);

            gl.TexImage2D<byte>(GLEnum.Texture2D, 0, (int)GLEnum.Rgba, (uint)img.Width, (uint)img.Height, 0, GLEnum.Rgba, GLEnum.UnsignedByte, img.Data);
            gl.GenerateMipmap(GLEnum.Texture2D);

            gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)GLEnum.Repeat);
            gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)GLEnum.Repeat);
            gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
            gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
            return id;
        }
    }
}