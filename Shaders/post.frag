#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D sceneTex;

void main() {
    vec3 color = vec3(texture(sceneTex, TexCoords));
    FragColor = vec4(color, 1.0);
}