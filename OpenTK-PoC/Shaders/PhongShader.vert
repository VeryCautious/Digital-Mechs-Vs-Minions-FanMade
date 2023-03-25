#version 460

uniform mat4 lightProjectionMatrix;
uniform mat4 lightViewMatrix;

uniform mat4 mvpMatrix;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;
uniform mat4 normalMatrix;

in vec3 vPosition;
in vec2 vUv;
in vec3 vNormal;

out vec4 lightSpacePos;
out vec3 worldPosition;
out vec3 fNormal;
out vec2 fUv; 

void main()
{
	fUv = vUv;
	fNormal = vNormal;

    vec4 vPos4 = modelMatrix * vec4(vPosition, 1.0);
    worldPosition = vPos4.xyz / vPos4.w;

	lightSpacePos = lightProjectionMatrix * lightViewMatrix * modelMatrix * vec4(vPosition, 1.0);

	gl_Position = mvpMatrix * vec4(vPosition, 1.0);
}