#version 460

uniform samplerCube textureSampler;

in vec3 TexCoords;

out vec4 fragColor;

void main()
{    
    fragColor = texture(textureSampler, TexCoords);
}