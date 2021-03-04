#version 330 core

// The vertex shader is ran once for every vertex

layout(location = 0) in vec2 aPosition;
layout(location = 1) in vec2 aTexCoord;

// output variable that sends the data to the fragment shader
out vec2 texCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec4 spriteColor;

void main(void)
{
	texCoord = aTexCoord;

	gl_Position = vec4(aPosition, 1.0, 1.0) * model * view * projection;
}