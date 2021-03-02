#version 430
#extension GL_ARB_bindless_texture : require

//out vec4 outputColor;
uniform ivec2 screenSize;
in vec3 pos;
flat in int id;

struct Sprite {
	uvec2 img;
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
    Sprite s = Sprites[id];
	layout(rgba8) image2D myImage = layout(rgba8) image2D(s.img);
    vec2 poss = vec2((pos.x+1)*screenSize.x/2, (1+pos.y)*screenSize.y/2);
    ivec2 posi = ivec2(int((poss.x-s.x)/s.scalex+s.startx), int((s.h-(poss.y-s.y))/s.scaley+s.starty));
	if(posi.x < 0) {
		posi.x = 0;
	}
	if(posi.y < 0) {
		posi.y = 0;
	}
	if(posi.x > int(s.startx + 0.9995*s.w/s.scalex)) {
		posi.x = int(s.startx + 0.9995*s.w/s.scalex);
	}
	if(posi.y > int(s.starty + 0.9995*s.h/s.scalex)) {
		posi.y = int(s.starty + 0.9995*s.h/s.scalex);
	}
    vec4 sd = imageLoad(myImage, posi) * vec4(s.r, s.g, s.b, s.a);
    gl_FragColor = sd;
	return;
}