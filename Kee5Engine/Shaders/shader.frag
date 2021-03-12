#version 330

out vec4 outputColor;

in vec4 vColor;
in vec2 texCoord;

// sampler2d is a representation fo a texture in a shader
uniform sampler2D texture0;
uniform vec4 spriteColor;

void main()
{
	outputColor = spriteColor * texture(texture0, texCoord);
}