#version 420 core

in vec2 texCoord;

out vec4 FragColor;

uniform sampler2D texture0;
uniform sampler2D texture1;

void main()
{
    vec4 outputColor = mix(texture(texture0, texCoord), texture(texture1, texCoord), 0.2);
    if(outputColor.a < 0.1)
        discard;

    FragColor = outputColor;
}