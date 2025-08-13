using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GameStudies.Source
{
    public class Shader
    {
        public int Prog { get; }
        public Shader(string vertPath, string fragPath)
        {
            string vertCode = File.ReadAllText(vertPath);
            string fragCode = File.ReadAllText(fragPath);

            int vert = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vert, vertCode);
            GL.CompileShader(vert);
            GL.GetShader(vert, ShaderParameter.CompileStatus, out int vertStatus);
            if (vertStatus == 0)
            {
                string info = GL.GetShaderInfoLog(vert);
                throw new Exception($"Vert shader compilation failed:\n{info}");
            }

            int frag = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(frag, fragCode);
            GL.CompileShader(frag);
            GL.GetShader(frag, ShaderParameter.CompileStatus, out int fragStatus);
            if (fragStatus == 0)
            {
                string info = GL.GetShaderInfoLog(frag);
                throw new Exception($"Fragment shader compilation failed:\n{info}");
            }

            Prog = GL.CreateProgram();

            GL.AttachShader(Prog, vert);
            GL.AttachShader(Prog, frag);
            GL.LinkProgram(Prog);
            GL.GetProgram(Prog, GetProgramParameterName.LinkStatus, out int linkStatus);
            if (linkStatus == 0)
            {
                string info = GL.GetProgramInfoLog(Prog);
                throw new Exception($"Shader program linking failed:\n{info}");
            }
            GL.DetachShader(Prog, vert);
            GL.DetachShader(Prog, frag);
            GL.DeleteShader(vert);
            GL.DeleteShader(frag);

        }

        public void Use()
        {
            GL.UseProgram(Prog);
        }

        public void Delete()
        {
            GL.DeleteProgram(Prog);
        }

        public void SetInt(string name, int value)
        {
            GL.Uniform1(GL.GetUniformLocation(Prog, name), value);
        }

        public void SetBool(string name, bool value)
        {
            GL.Uniform1(GL.GetUniformLocation(Prog, name), value ? 1 : 0);
        }

        public void SetFloat(string name, float value)
        {
            GL.Uniform1(GL.GetUniformLocation(Prog, name), value);
        }

        public void SetVec3(string name, Vector3 v)
        {
            int transformLoc = GL.GetUniformLocation(Prog, name);
            GL.Uniform3(transformLoc, v);
        }

        public void SetVec4(string name, Vector4 v)
        {
            int transformLoc = GL.GetUniformLocation(Prog, name);
            GL.Uniform4(transformLoc, v);
        }

        public void SetMat4(string name, Matrix4 m)
        {
            int transformLoc = GL.GetUniformLocation(Prog, name);
            GL.UniformMatrix4(transformLoc, false, ref m);
        }

    }
}