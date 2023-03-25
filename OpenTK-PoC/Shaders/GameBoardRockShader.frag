#version 460

uniform mat4 modelMatrix;
uniform vec3 lightPos;
uniform vec3 cameraPos;
uniform sampler2D textureSampler;

in vec3 worldPosition;
in vec3 fNormal;
in vec4 fColor;
in vec3 modelPos;

out vec4 fragColor;

const float lightPower = 20.0;
const vec4 lightColor = vec4(0.8, 0.8, 0.8, 1.0);
const float phongExponent = 1;
const float ka = 0.5;
const float kd = 0.45;
const float ks = 0.05;

void main()
{
    vec4 texColor = texture(textureSampler, 0.33 * worldPosition.xz);
    vec4 baseColor = fColor;
    if (worldPosition.y < 0.45) {
        float factor = baseColor.r + 0.5;
        baseColor = factor * baseColor + (1-factor) * texColor;
    }

    vec3 n = normalize(vec3(modelMatrix * vec4(fNormal, 0.0)));

    vec3 l = lightPos - worldPosition;
    vec3 ln = normalize(l);

    vec4 ambient = baseColor;

    float ldotn = dot(ln, n);
    vec4 diffuse = baseColor * max(ldotn, 0.0);

    vec3 v = normalize(cameraPos - worldPosition);
    vec3 r = 2 * ldotn * n - ln;
    vec4 specular = lightColor * pow(max(dot(r,v),0.0), phongExponent);

    float localLightIntensity = lightPower / dot(l,l);

    vec4 color = ka * ambient + localLightIntensity * (kd * diffuse +  ks * specular);
    if (baseColor.a < 0.99){
        color.a = 0.0;
    }
    fragColor = color;
}