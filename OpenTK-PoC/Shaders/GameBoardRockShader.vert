#version 460

uniform mat4 mvpMatrix;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;
uniform mat4 normalMatrix;

in vec3 vPosition;
in vec3 vNormal;
in vec4 vColor;

out vec3 worldPosition;
out vec3 fNormal;
out vec4 fColor; 
out vec3 modelPos;

void main()
{
	fColor = vColor;
	fNormal = vNormal;
	modelPos = vPosition;

    vec4 vPos4 = modelMatrix * vec4(vPosition, 1.0);
    worldPosition = vec3(vPos4) / vPos4.w;

	gl_Position = mvpMatrix * vec4(vPosition, 1.0);
}