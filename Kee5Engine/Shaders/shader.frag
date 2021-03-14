#version 330

out vec4 outputColor;

in vec2 texCoord;
in vec4 vColor;

// sampler2d is a representation fo a texture in a shader
uniform sampler2D texture0;

void main()
{
	outputColor = vColor * texture(texture0, texCoord);
}