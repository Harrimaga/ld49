#version 430
#extension GL_ARB_bindless_texture : require
layout (location = 0) in vec3 aPos;
out vec3 pos;
flat out int id;
uniform ivec2 screenSize;

struct Sprite {
	layout(rgba8, bindless_image) image2D img;
	int w;
	int h;
	float x;
	float y;
	float scalex;
	float scaley;
	int startx;
	int starty;
	float r, g, b, a, rot;
};

layout(std430, binding=0 ) readonly buffer sprites{
    Sprite Sprites[];
};

void main()
{
	id = gl_InstanceID;
    Sprite s = Sprites[gl_InstanceID];
	if(s.rot != 0) {
		float co = cos(-s.rot);
		float si = sin(-s.rot);
		float x = s.x + s.w/2;
		float y = s.y + s.h/2 ;
		if(aPos.x == 0) {
			x -= 0.5*s.w*co;
			y += 0.5*s.w*si;
		}
		else {
			x += 0.5*s.w*co;
			y -= 0.5*s.w*si;
		}
		if(aPos.y == 0) {
			x -= 0.5*s.h*si;
			y -= 0.5*s.h*co;
		}
		else {
			x += 0.5*s.h*si;
			y += 0.5*s.h*co;
		}
		gl_Position = vec4(x*2/screenSize.x - 1, -y*2/screenSize.y + 1, aPos.z, 1.0);
		pos = vec3((aPos.x*s.w + s.x)*2/screenSize.x - 1, (aPos.y*(s.h) + s.y)*2/screenSize.y - 1, aPos.z);
	}
	else {
		gl_Position = vec4((aPos.x*s.w + s.x)*2/screenSize.x - 1, -(aPos.y*(s.h) + s.y)*2/screenSize.y + 1, aPos.z, 1.0);
		pos = vec3((aPos.x*s.w + s.x)*2/screenSize.x - 1, (aPos.y*(s.h) + s.y)*2/screenSize.y - 1, aPos.z);
	}
}