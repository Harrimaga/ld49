#version 330

out vec4 outputColor;

in vec2 texCoord;
in vec4 vColor;
flat in int vTexID;

// sampler2d is a representation fo a texture in a shader
uniform sampler2D uTextures[32];

void main()
{
	outputColor = vColor * texture(uTextures[vTexID], texCoord);
}