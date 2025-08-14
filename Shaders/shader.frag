#version 330 core

in vec3 vColor;
in vec2 vUV;

out vec4 FragColor;

uniform bool uUseTexture;
uniform vec4 uColor;
uniform sampler2D texture0;

void main() {
    vec4 base = uUseTexture ? texture(texture0, vUV) : vec4(vColor, 1.0);
    FragColor = base;
}