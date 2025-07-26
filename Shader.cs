using OpenTK.Graphics.OpenGL4;

namespace SimpleTriangle
{
    public class Shader
    {
        public int Handle { get; }
        public Shader(string vertexPath, string framentPath)
        {
            string vertexCode = File.ReadAllText(vertexPath);
            string framentCode = File.ReadAllText(framentPath);

            int vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, vertexCode);
            GL.CompileShader(vertex);

            int frament = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(frament, framentCode);
            GL.CompileShader(frament);

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, vertex);
            GL.AttachShader(Handle, frament);

            GL.LinkProgram(Handle);

            GL.DeleteShader(vertex);
            GL.DeleteShader(frament);
        }

    }
}