#version 460

uniform mat4 mvpMatrix;
uniform mat4 modelMatrix;
uniform mat4 normalMatrix;

in vec3 vPosition;
in vec4 vColor;

out vec4 fColor; 

void main()
{
    fColor = vColor;
	gl_Position = mvpMatrix * vec4(vPosition, 1.0);
}