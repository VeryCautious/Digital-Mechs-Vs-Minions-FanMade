#version 460
uniform mat4 mvpMatrix;
uniform vec3 cameraPos;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
uniform mat4 normalMatrix;

in vec3 vPosition;
in vec3 vNormal;

const float thickness = 0.02;

float normalLengthInScreenCoordinates(vec3 worldNormal) {
	vec4 diff4 = projectionMatrix * viewMatrix * vec4(worldNormal, 0.0);
	vec2 diff = diff4.xy / diff4.w;
	return length(diff4);
}

void main()
{
	vec4 vPos4 = modelMatrix * vec4(vPosition, 1.0);
    vec3 worldPosition = vec3(vPos4) / vPos4.w;
	vec3 worldNormal = normalize(vec3(normalMatrix * vec4(vNormal, 0.0)));
	float distanceFactor = length(cameraPos - worldPosition) * 0.01;
	float factor = thickness / normalLengthInScreenCoordinates(worldNormal);
	gl_Position = projectionMatrix * viewMatrix * vec4(worldPosition + factor * worldNormal, 1.0);
}

