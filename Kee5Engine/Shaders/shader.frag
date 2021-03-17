#version 330

out vec4 outputColor;

in vec2 texCoord;
in vec4 vColor;
flat in float vTexID;

// sampler2d is a representation fo a texture in a shader
uniform sampler2D uTextures[32];

void main()
{
	int index = int(vTexID);
	outputColor = vColor * texture(uTextures[index], texCoord);
}