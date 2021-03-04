#version 330 core

// The vertex shader is ran once for every vertex

layout(location = 0) in vec4 vertex; // vec2 position vec2 texCoords

// output variable that sends the data to the fragment shader
out vec2 texCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(void)
{
	texCoord = vertex.zw;

	gl_Position = vec4(vertex.xy, 0.0, 1.0) * model * view * projection;
}