#version 460

uniform mat4 modelMatrix;
uniform vec3 lightPos;
uniform vec3 cameraPos;
uniform sampler2D textureSampler;
uniform sampler2D normalSampler;

in vec3 worldPosition;
in vec2 fUv;
in mat3 TBN;

out vec4 fragColor;

const float lightPower = 20.0;
const vec4 lightColor = vec4(1.0, 1.0, 1.0, 1.0);
const float phongExponent = 1;
const float ka = 0.5;
const float kd = 0.45;
const float ks = 0.05;

void main()
{
    vec4 baseColor = texture(textureSampler, fUv);
    vec4 normalColor = texture(normalSampler, fUv);
    vec3 texNormal = normalColor.rgb * 2.0 - 1.0;
    vec3 n = normalize(TBN * normalize(texNormal));

    vec3 l = lightPos - worldPosition;
    vec3 ln = normalize(l);

    vec4 ambient = baseColor;

    float ldotn = dot(ln, n);
    vec4 diffuse = baseColor * max(ldotn, 0.0);

    vec3 v = normalize(cameraPos - worldPosition);
    vec3 r = 2 * ldotn * n - ln;
    vec4 specular = lightColor * pow(max(dot(r,v),0.0), phongExponent);

    float localLightIntensity = lightPower / dot(l,l);

    fragColor = ka * ambient + localLightIntensity * (kd * diffuse +  ks * specular);
}