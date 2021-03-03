#version 330

out vec4 outputColor;

in vec2 texCoord;

// sampler2d is a representation fo a texture in a shader
uniform sampler2D texture0;

void main()
{
	outputColor = texture(texture0, texCoord);
}