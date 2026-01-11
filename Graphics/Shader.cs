using Matrix4 = System.Numerics.Matrix4x4;
using Vector4 = System.Numerics.Vector4;
using Vector3 = System.Numerics.Vector3;
using Silk.NET.OpenGL;

namespace GameStudies.Graphics
{
    public class Shader
    {
        private GL _gl;

        public static uint Prog { get; set; }
        public Shader(GL gl, string vertPath, string fragPath)
        {
            _gl = gl;
            string baseDir = AppContext.BaseDirectory;

            string fullVertPath = Path.Combine(baseDir, DirPathNames.ShaderFolderName, vertPath);
            string fullFragPath = Path.Combine(baseDir, DirPathNames.ShaderFolderName, fragPath);

            string vertCode = File.ReadAllText(fullVertPath);
            string fragCode = File.ReadAllText(fullFragPath);

            uint vert = _gl.CreateShader(ShaderType.VertexShader);
            _gl.ShaderSource(vert, vertCode);
            _gl.CompileShader(vert);
            _gl.GetShader(vert, GLEnum.CompileStatus, out int vertStatus);
            if (vertStatus == 0)
            {
                string info = _gl.GetShaderInfoLog(vert);
                throw new Exception($"Vert shader compilation failed:\n{info}");
            }

            uint frag = _gl.CreateShader(ShaderType.FragmentShader);
            _gl.ShaderSource(frag, fragCode);
            _gl.CompileShader(frag);
            _gl.GetShader(frag, GLEnum.CompileStatus, out int fragStatus);
            if (fragStatus == 0)
            {
                string info = _gl.GetShaderInfoLog(frag);
                throw new Exception($"Fragment shader compilation failed:\n{info}");
            }

            Prog = _gl.CreateProgram();

            _gl.AttachShader(Prog, vert);
            _gl.AttachShader(Prog, frag);
            _gl.LinkProgram(Prog);
            _gl.GetProgram(Prog, GLEnum.LinkStatus, out int linkStatus);
            if (linkStatus == 0)
            {
                string info = _gl.GetProgramInfoLog(Prog);
                throw new Exception($"Shader program linking failed:\n{info}");
            }
            _gl.DetachShader(Prog, vert);
            _gl.DetachShader(Prog, frag);
            _gl.DeleteShader(vert);
            _gl.DeleteShader(frag);

        }

        public void Use()
        {
            _gl.UseProgram(Prog);
        }

        public void Delete()
        {
            _gl.DeleteProgram(Prog);
        }

        public void SetInt(string name, int value)
        {
            _gl.Uniform1(_gl.GetUniformLocation(Prog, name), value);
        }

        public void SetBool(string name, bool value)
        {
            _gl.Uniform1(_gl.GetUniformLocation(Prog, name), value ? 1 : 0);
        }

        public void SetFloat(string name, float value)
        {
            _gl.Uniform1(_gl.GetUniformLocation(Prog, name), value);
        }

        public void SetVec3(string name, Vector3 v)
        {
            int transformLoc = _gl.GetUniformLocation(Prog, name);
            _gl.Uniform3(transformLoc, v);
        }

        public void SetVec4(string name, Vector4 v)
        {
            int transformLoc = _gl.GetUniformLocation(Prog, name);
            _gl.Uniform4(transformLoc, v);
        }

        public unsafe void SetMat4(string name, Matrix4 m)
        {
            int loc = _gl.GetUniformLocation(Prog, name);
            if (loc < 0) return;

            _gl.UniformMatrix4(loc, 1, false, (float*)&m);
        }

    }
}