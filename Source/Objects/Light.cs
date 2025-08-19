using OpenTK.Mathematics;

namespace GameStudies.Source.Objects
{
    public enum LightType { Directionl, Point, Spot }

    public class Light
    {
        public LightType Type { get; set; } = LightType.Point;

        public Vector3 Position { get; set; } = new(0, 0, 1);
        public Vector3 Direction { get; set; } = new(0, -1, 0);

        public Vector3 Ambient { get; set; } = new(0.1f, 0.1f, 0.1f);
        public Vector3 Diffuse { get; set; } = new(1f, 1f, 1f);
        public Vector3 Specular { get; set; } = new(1f, 1f, 1f);

        // attenuation (for Point/Spot)
        public float Constant { get; set; } = 1.0f;
        public float Linear { get; set; } = 0.09f;
        public float Quadratic { get; set; } = 0.032f;

        // spot params
        static float Radians(float deg) => deg * (MathF.PI / 180f);
        public float CutoffCos { get; set; } = MathF.Cos(Radians(12.5f));
        public float OuterCutoffCos { get; set; } = MathF.Cos(Radians(17.5f));

        public void Apply(Shader shader, int i)
        {
            shader.Use();
            shader.SetInt($"lights[{i}].type", (int)Type);
            shader.SetVec3($"lights[{i}].position", Position);
            shader.SetVec3($"lights[{i}].direction", Direction);
            shader.SetVec3($"lights[{i}].ambient", Ambient);
            shader.SetVec3($"lights[{i}].diffuse", Diffuse);
            shader.SetVec3($"lights[{i}].specular", Specular);
            shader.SetFloat($"lights[{i}].constant", Constant);
            shader.SetFloat($"lights[{i}].linear", Linear);
            shader.SetFloat($"lights[{i}].quadratic", Quadratic);
            shader.SetFloat($"lights[{i}].cutoff", CutoffCos);
            shader.SetFloat($"lights[{i}].outerCutoff", OuterCutoffCos);
        }

    }
}