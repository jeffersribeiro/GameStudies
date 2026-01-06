#version 330 core

out vec4 FragColor;

in vec2 vTex;
in vec3 vNormal;

uniform sampler2D texture_diffuse1;
uniform int uDiffuseCount;

void main() {
    vec3 base = vec3(0.85, 0.85, 0.9);

    if (uDiffuseCount > 0)
        base = texture(texture_diffuse1, vTex).rgb;

    // tiny fake lighting just so you see shape
    vec3 N = normalize(vNormal);
    float ndl = max(dot(N, normalize(vec3(0.3, 0.8, 0.4))), 0.15);

    FragColor = vec4(base * ndl, 1.0);
}
