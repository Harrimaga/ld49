#version 330 core

// The vertex shader is ran once for every vertex

layout(location = 0) in vec2 aPosition;
layout(location = 1) in vec4 aColor;
layout(location = 2) in vec2 aTexCoord;

// output variable that sends the data to the fragment shader
out vec4 vColor;
out vec2 texCoord;

uniform mat4 view;
uniform mat4 projection;

void main(void)
{
	texCoord = aTexCoord;
	vColor = aColor;

	gl_Position = vec4(aPosition.xy, 1.0, 1.0) * view * projection;
}