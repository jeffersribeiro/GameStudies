#version 330 core

out vec4 FragColor;

in vec3 vN;
uniform vec3 uColor;

void main() {
    vec3 N = normalize(vN);
    float ndl = max(dot(N, normalize(vec3(0.3, 0.8, 0.4))), 0.15);
    FragColor = vec4(uColor * ndl, 1.0);
}
