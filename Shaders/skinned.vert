#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 3) in vec2 aTex;
layout (location = 6) in ivec4 aBoneIDs;
layout (location = 7) in vec4 aWeights;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

const int MAX_BONES = 100;
uniform mat4 finalBonesMatrices[MAX_BONES];

out vec2 vTex;
out vec3 vNormal;

mat4 skinMatrix() {
    mat4 skin = mat4(0.0);

    // If aBoneIDs contains -1, ignore that slot
    for (int i = 0; i < 4; i++) {
        int id = aBoneIDs[i];
        float w = aWeights[i];

        if (id >= 0 && w > 0.0)
            skin += finalBonesMatrices[id] * w;
    }

    // If no weights, fallback to identity
    if (skin[0][0] == 0.0 && skin[1][1] == 0.0 && skin[2][2] == 0.0 && skin[3][3] == 0.0)
        skin = mat4(1.0);

    return skin;
}

void main() {
    mat4 skin = skinMatrix();

    vec4 localPos = skin * vec4(aPos, 1.0);
    gl_Position = projection * view * model * localPos;

    // normal transform (good enough for debugging)
    vNormal = mat3(model) * mat3(skin) * aNormal;
    vTex = aTex;
}
