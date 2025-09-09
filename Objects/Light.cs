using GameStudies.Graphics;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameStudies.Objects
{
    public enum LightType { Directional, Point, Spot }

    public class Light
    {
        public LightType Type { get; set; } = LightType.Point;

        public Vector3 Position = Vector3.Zero;
        public float Speed { get; set; } = 2.5f;

        public Vector3 Direction { get; set; } = new(1f, 1f, 1f);

        public Vector3 Ambient { get; set; } = new(1.0f, 1.0f, 1.0f);
        public Vector3 Diffuse { get; set; } = new(1.0f, 1.0f, 1.0f);
        public Vector3 Specular { get; set; } = new(1.0f, 1.0f, 1.0f);

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
                case LightType.Directional:
                    shader.SetVec3($"dirLight.direction", Direction);
                    shader.SetVec3($"dirLight.ambient", Ambient);
                    shader.SetVec3($"dirLight.diffuse", Diffuse);
                    shader.SetVec3($"dirLight.specular", Specular);
                    break;
                case LightType.Point:
                    shader.SetVec3($"pointLight.position", Position);
                    shader.SetFloat($"pointLight.constant", Constant);
                    shader.SetFloat($"pointLight.linear", Linear);
                    shader.SetFloat($"pointLight.quadratic", Quadratic);
                    shader.SetVec3($"pointLight.ambient", Ambient);
                    shader.SetVec3($"pointLight.diffuse", Diffuse);
                    shader.SetVec3($"pointLight.specular", Specular);
                    break;
                case LightType.Spot:
                    shader.SetVec3($"spotLight.position", Position);
                    shader.SetVec3($"spotLight.direction", Direction);
                    shader.SetFloat($"spotLight.cutOff", CutoffCos);
                    shader.SetFloat($"spotLight.outerCutOff", OuterCutoffCos);
                    shader.SetFloat($"spotLight.constant", Constant);
                    shader.SetFloat($"spotLight.linear", Linear);
                    shader.SetFloat($"spotLight.quadratic", Quadratic);
                    shader.SetVec3($"spotLight.specular", Specular);
                    shader.SetVec3($"spotLight.ambient", Ambient);
                    shader.SetVec3($"spotLight.diffuse", Diffuse);
                    break;
            }
        }

        public void ProcessKeyboard(KeyboardState kb, float deltaTime)
        {
            float velocity = Speed * deltaTime;

            if (kb.IsKeyDown(Keys.Right)) Position.X += velocity;
            if (kb.IsKeyDown(Keys.Left)) Position.X -= velocity;
            if (kb.IsKeyDown(Keys.Up)) Position.Z -= velocity;
            if (kb.IsKeyDown(Keys.Down)) Position.Z += velocity;
            if (kb.IsKeyDown(Keys.KeyPad1)) Position.Y += velocity;
            if (kb.IsKeyDown(Keys.KeyPad0)) Position.Y -= velocity;
        }

    }
}