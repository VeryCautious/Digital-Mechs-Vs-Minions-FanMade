#version 460
uniform mat4 modelProjectionMatrix;
in vec3 vPosition;
in vec2 vUv;
out vec2 fUv;

void main()
{
    gl_Position = modelProjectionMatrix * vec4(vPosition, 1.0);
	fUv = vUv;
}