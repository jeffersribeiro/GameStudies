#version 330 core

in vec3 vColor;
in vec2 vUV;
in vec3 Normal;

out vec4 FragColor;

uniform bool uUseTexture;
uniform vec4 uColor;
uniform sampler2D texture0;
uniform vec3 lightColor;
uniform vec3 lightPos;

void main()
{
    float ambientStrength = 0.5;
    vec3 ambient = ambientStrength * lightColor;
    vec3 result = ambient * vColor;

    vec4 base = uUseTexture ? texture(texture0, vUV) : vec4(result, 1.0);;
    FragColor = base;
}