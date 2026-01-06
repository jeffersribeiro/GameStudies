#version 330 core

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 vN;

vec3 faceNormal(int vid) {
    // 6 faces, each has 2 triangles = 6 vertices
    int face = vid / 6;

    if (face == 0)
        return vec3(0, 0, -1); // back
    if (face == 1)
        return vec3(0, 0, 1); // front
    if (face == 2)
        return vec3(-1, 0, 0); // left
    if (face == 3)
        return vec3(1, 0, 0); // right
    if (face == 4)
        return vec3(0, -1, 0); // bottom
    return vec3(0, 1, 0);                // top
}

void main() {
    // 36 positions (12 triangles)
    const vec3 P[36] = vec3[36](
        // back (-Z)
    vec3(-0.5, -0.5, -0.5), vec3(0.5, -0.5, -0.5), vec3(0.5, 0.5, -0.5), vec3(-0.5, -0.5, -0.5), vec3(0.5, 0.5, -0.5), vec3(-0.5, 0.5, -0.5),

        // front (+Z)
    vec3(-0.5, -0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(0.5, -0.5, 0.5), vec3(-0.5, -0.5, 0.5), vec3(-0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5),

        // left (-X)
    vec3(-0.5, -0.5, 0.5), vec3(-0.5, -0.5, -0.5), vec3(-0.5, 0.5, -0.5), vec3(-0.5, -0.5, 0.5), vec3(-0.5, 0.5, -0.5), vec3(-0.5, 0.5, 0.5),

        // right (+X)
    vec3(0.5, -0.5, -0.5), vec3(0.5, -0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(0.5, -0.5, -0.5), vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, -0.5),

        // bottom (-Y)
    vec3(-0.5, -0.5, 0.5), vec3(0.5, -0.5, 0.5), vec3(0.5, -0.5, -0.5), vec3(-0.5, -0.5, 0.5), vec3(0.5, -0.5, -0.5), vec3(-0.5, -0.5, -0.5),

        // top (+Y)
    vec3(-0.5, 0.5, -0.5), vec3(0.5, 0.5, -0.5), vec3(0.5, 0.5, 0.5), vec3(-0.5, 0.5, -0.5), vec3(0.5, 0.5, 0.5), vec3(-0.5, 0.5, 0.5));

    int vid = gl_VertexID;
    vec3 pos = P[vid];

    vN = mat3(model) * faceNormal(vid);
    gl_Position = projection * view * model * vec4(pos, 1.0);
}
