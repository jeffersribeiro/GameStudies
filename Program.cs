using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;

namespace SimpleTriangle
{
    public class Program
    {
        public static void Main()
        {
            using var game = new GameWindow(GameWindowSettings.Default, NativeWindowSettings.Default);
            int shaderProgram = 0;
            int VAO = 0;
            int VBO = 0;

            float[] vertices = {
                -0.5f, -0.5f, 0.0f, // V0: canto inferior esquerdo
                0.5f, -0.5f, 0.0f,  // V1: canto inferior direito
                0.0f, 0.5f, 0.0f    // V2: topo central
            };

            game.Load += () =>
            {
                var shader = new Shader("C:\\Users\\Jeffe\\OneDrive\\Documents\\Projects\\GameStudies\\shader.vert", "C:\\Users\\Jeffe\\OneDrive\\Documents\\Projects\\GameStudies\\shader.frag");

                shaderProgram = shader.Handle;

                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length, vertices, BufferUsageHint.StaticDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindVertexArray(0);

            };

            game.RenderFrame += args =>
          {
              GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f); // cinza escuro
              GL.Clear(ClearBufferMask.ColorBufferBit);

              GL.UseProgram(shaderProgram);
              GL.BindVertexArray(VAO);
              GL.DrawElements(PrimitiveType.Triangles, 0, DrawElementsType.UnsignedInt, 3);

              game.SwapBuffers();
          };


            GL.DeleteVertexArray(VAO);
            GL.DeleteBuffer(VBO);
            GL.DeleteProgram(shaderProgram);

            game.Run();
        }
    }
}