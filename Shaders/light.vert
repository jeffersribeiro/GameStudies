#version 330 core

layout (location = 0) in vec3 pos;
layout (location = 1) in vec3 norm;
layout (location = 2) in vec3 color;
layout (location = 3) in vec2 tex;
layout (location = 4) in vec2 tangent;
layout (location = 5) in vec2 bitrange;
layout (location = 6) in ivec4 boneIds;
layout (location = 7) in vec4 weights;

out vec3 vColor;
out vec2 vUV;
out vec3 Normal;
out vec3 FragPos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

const int MAX_BONES = 100;
const int MAX_BONES_INFLUENCE = 4;
uniform mat4 finalBonesMatrices[MAX_BONES];

void main() {

    vec4 totalPosition = vec4(0.0);
    vec3 totalNormal = vec3(0.0);

    for (int i = 0; i < MAX_BONES_INFLUENCE; i++) {
        int id = boneIds[i];

        if (id < 0 || id >= MAX_BONES)
            continue;

        float w = weights[i];
        if (w <= 0.0)
            continue;

        mat4 boneTransform = finalBonesMatrices[id];

        totalPosition += (boneTransform * vec4(pos, 1.0)) * w;

        totalNormal += (mat3(boneTransform) * norm) * w;
    }

    if (totalPosition == vec4(0.0)) {
        totalPosition = vec4(pos, 1.0);
        totalNormal = norm;
    }

    vec4 worldPos = model * totalPosition;
    FragPos = vec3(worldPos);

    Normal = mat3(transpose(inverse(model))) * totalNormal;
    Normal = normalize(Normal);

    gl_Position = projection * view * worldPos;

    vColor = color;
    vUV = tex;
}