#version 460

uniform mat4 mvpMatrix;
uniform mat4 modelMatrix;
uniform mat4 normalMatrix;

in vec3 vPosition;
in vec2 vUv;
in vec3 vNormal;
in vec3 vTangent;
in vec3 vBitangent;

out vec2 fUv;
out mat3 TBN;
out vec3 worldPosition;

void main()
{
    // https://learnopengl.com/Advanced-Lighting/Normal-Mapping
    vec3 T = normalize(vec3(normalMatrix * vec4(vTangent, 0.0)));
    vec3 N = normalize(vec3(normalMatrix * vec4(vNormal, 0.0)));
    T = normalize(T - dot(T, N) * N);
    vec3 B = cross(N, T);

    TBN = mat3(T, B, N);
    fUv = vUv;

    vec4 vPos4 = modelMatrix * vec4(vPosition, 1.0);
    worldPosition = vec3(vPos4) / vPos4.w;

	gl_Position = mvpMatrix * vec4(vPosition, 1.0);
}