#version 460
in vec3 vPosition;

out vec2 fUv;

void main()
{
    gl_Position = vec4(vPosition, 1.0);
	fUv = vPosition.xy;
}