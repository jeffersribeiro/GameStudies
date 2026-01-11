#version 330 core

in vec3 vColor;
in vec2 vUV;

out vec4 FragColor;

uniform sampler2D texture_diffuse1;

void main() {
    vec4 tex = texture(texture_diffuse1, vUV);
    FragColor = vec4(tex.rgb, tex.a);
}
