#version 430
#extension GL_ARB_bindless_texture : require

layout(location = 0) in vec3 position;

out vec2 UV;

void main()
{
	gl_Position = vec4(position.x*2 - 1, position.y*2 - 1, 0, 1.0);
	UV = vec2(position.x*2 - 1, position.y*2 - 1);
}