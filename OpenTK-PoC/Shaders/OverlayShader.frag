#version 460
in vec2 fUv;
uniform sampler2D texture0;
out vec4 fragColor;

void main()
{
    fragColor = texture(texture0, fUv);
}