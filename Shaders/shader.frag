#version 330 core

out vec4 FragColor;
out vec4 Tex0;
out vec4 Tex1;
in vec3 ourColor;
in vec3 ourPos;
in vec2 TexCoord;

uniform sampler2D texture0;
uniform sampler2D texture1;

uniform float opacity = 1.0;

void main()
{
    Tex0 = texture(texture0, TexCoord);
    Tex1 = texture(texture1, TexCoord);
    
    FragColor = mix(Tex0, Tex1, opacity);
}