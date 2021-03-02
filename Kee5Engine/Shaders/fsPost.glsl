#version 430
#extension GL_ARB_bindless_texture : require

in vec2 UV;
layout(rgba8, bindless_image) uniform image2D prev;
uniform ivec2 screenSize;

void main() 
{
    float aa = sqrt(UV.x*UV.x + UV.y*UV.y)/1.4142135;
    gl_FragColor = vec4(imageLoad(prev, ivec2((UV.x+1)*screenSize.x/2, (UV.y+1)*screenSize.y/2)).xyz*(1-aa), 1.0);
}