#version 460

in vec4 fColor;
uniform vec4 particleColor;
out vec4 fragColor;

void main()
{
    fragColor = particleColor;
}