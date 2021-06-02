#version 330 core

// The vertex shader is ran once for every vertex

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;
layout(location = 2) in vec4 aColor;
layout(location = 3) in int aTexID;

// output variable that sends the data to the fragment shader
out vec2 texCoord;
out vec4 vColor;
flat out int vTexID;

uniform mat4 view;
uniform mat4 projection;

void main(void)
{
	texCoord = aTexCoord;
	vColor = aColor;
	vTexID = aTexID;

	gl_Position = vec4(aPosition, 1.0) * view * projection;
}