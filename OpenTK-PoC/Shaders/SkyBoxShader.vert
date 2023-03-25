#version 460
in vec3 vPosition;

out vec3 TexCoords;

uniform mat4 mvpMatrix;

void main()
{
    TexCoords = vPosition;
    gl_Position = mvpMatrix * vec4(vPosition, 1.0);
} 