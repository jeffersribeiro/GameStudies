#version 330 core

#define NR_POINT_LIGHTS 3
#define NR_TEXTURES_DIFFUSE 3
#define NR_TEXTURES_SPECULAR 2

struct Material {
    float shininess;
};

struct DirLight {
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct PointLight {
    vec3 position;

    float constant;
    float linear;
    float quadratic;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct SpotLight {
    vec3 position;
    vec3 direction;
    float cutOff;
    float outerCutOff;

    vec3 diffuse;
    vec3 ambient;
    vec3 specular;

    float constant;
    float linear;
    float quadratic;
};

in vec3 vColor;
in vec2 vUV;
in vec3 Normal;
in vec3 FragPos;

out vec4 FragColor;

uniform vec3 viewPos;
uniform DirLight dirLight;
uniform PointLight pointLight;
uniform SpotLight spotLight;

uniform sampler2D texture_diffuse1;
uniform sampler2D texture_diffuse2;
uniform sampler2D texture_diffuse3;

uniform sampler2D texture_specular1;
uniform sampler2D texture_specular2;

uniform int uDiffuseCount;
uniform int uSpecularCount;
uniform Material material;

vec4 GetDiffuse() {
    if (uDiffuseCount > 0)
        return texture(texture_diffuse1, vUV);
    return vec4(1.0);
}
vec4 GetSpecular() {
    if (uSpecularCount > 0)
        return texture(texture_specular1, vUV);
    return vec4(1.0);
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir) {
    vec3 lightDir = normalize(-light.direction);
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

    vec4 albedo = GetDiffuse();
    vec4 ks = GetSpecular();

    vec3 ambient = light.ambient * vec3(albedo);
    vec3 diffuse = light.diffuse * diff * vec3(albedo);
    vec3 specular = light.specular * spec * vec3(ks);
    return ambient + diffuse + specular;
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir) {
    vec3 lightDir = normalize(light.position - fragPos);
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

    float distance = length(light.position - fragPos);

    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));

    vec4 albedo = GetDiffuse();
    vec4 ks = GetSpecular();

    vec3 ambient = light.ambient * vec3(albedo);
    vec3 diffuse = light.diffuse * diff * vec3(albedo);
    vec3 specular = light.specular * spec * vec3(ks);

    return (ambient + diffuse + specular) * attenuation;
}

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir) {
    vec3 lightDir = normalize(light.position - fragPos);

    float theta = dot(lightDir, normalize(-light.direction));
    float epsilon = max(light.cutOff - light.outerCutOff, 1e-4);
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

    float diff = max(dot(normal, lightDir), 0.0);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

    float distance = length(light.position - fragPos);

    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));

    vec4 albedo = GetDiffuse();
    vec4 ks = GetSpecular();

    vec3 ambient = light.ambient * vec3(albedo);
    vec3 diffuse = light.diffuse * diff * vec3(albedo);
    vec3 specular = light.specular * spec * vec3(ks);

    return (ambient + diffuse + specular) * attenuation * intensity;
}

void main() {
    vec4 albedo = GetDiffuse();
    if (albedo.a < 0.1)
        discard;

    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);

    vec3 lit = CalcDirLight(dirLight, norm, viewDir) + CalcPointLight(pointLight, norm, FragPos, viewDir) + CalcSpotLight(spotLight, norm, FragPos, viewDir);

    FragColor = vec4(lit, albedo.a);
}