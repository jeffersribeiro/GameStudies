using OpenTK.Graphics.OpenGL4;

namespace GameStudies.Source
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

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public void Delete()
        {
            GL.DeleteProgram(Handle);
        }

        public void SetInt(string name, int value)
        {
            GL.Uniform1(GL.GetUniformLocation(Handle, name), value);
        }

        public void setBool(string name, bool value)
        {
            GL.Uniform1(GL.GetUniformLocation(Handle, name), value ? 1 : 0);
        }

        public void setInt(string name, int value)
        {
            GL.Uniform1(GL.GetUniformLocation(Handle, name), value);
        }

        public void setFloat(string name, float value)
        {
            GL.Uniform1(GL.GetUniformLocation(Handle, name), value);
        }

    }
}