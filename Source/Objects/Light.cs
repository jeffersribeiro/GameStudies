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

            switch (Type)
            {
                case LightType.Directionl:
                    shader.SetVec3($"dirLight[{i}].directional", Direction);
                    shader.SetVec3($"dirLight[{i}].ambient", Ambient);
                    shader.SetVec3($"dirLight[{i}].diffuse", Diffuse);
                    shader.SetVec3($"dirLight[{i}].specular", Specular);
                    break;
                case LightType.Point:
                    shader.SetVec3($"pointLights[{i}].position", Position);
                    shader.SetFloat($"pointLights[{i}].constant", Constant);
                    shader.SetFloat($"pointLights[{i}].linear", Linear);
                    shader.SetFloat($"pointLights[{i}].quadratic", Quadratic);
                    shader.SetVec3($"pointLights[{i}].ambient", Ambient);
                    shader.SetVec3($"pointLights[{i}].diffuse", Diffuse);
                    shader.SetVec3($"pointLights[{i}].specular", Specular);
                    break;
                case LightType.Spot:
                    shader.SetVec3($"spotLight[{i}].position", Position);
                    shader.SetVec3($"spotLight[{i}].direction", Direction);
                    shader.SetVec3($"spotLight[{i}].diffuse", Diffuse);
                    shader.SetVec3($"spotLight[{i}].specular", Specular);
                    shader.SetFloat($"spotLight[{i}].cutOff", CutoffCos);
                    shader.SetFloat($"spotLight[{i}].outerCutOff", OuterCutoffCos);
                    break;
            }
        }

    }
}