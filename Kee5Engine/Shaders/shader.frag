#version 330

out vec4 outputColor;

in vec2 texCoord;
in vec4 vColor;
flat in int vTexID;

// sampler2d is a representation for a texture in a shader
uniform sampler2D uTextures[32];

void main()
{
	switch(vTexID) {
		case 0:
			outputColor = vColor * texture(uTextures[0], texCoord);
			break;
		case 1:
			outputColor = vColor * texture(uTextures[1], texCoord);
			break;
		case 2:
			outputColor = vColor * texture(uTextures[2], texCoord);
			break;
		case 3:
			outputColor = vColor * texture(uTextures[3], texCoord);
			break;
		case 4:
			outputColor = vColor * texture(uTextures[4], texCoord);
			break;
		case 5:
			outputColor = vColor * texture(uTextures[5], texCoord);
			break;
		case 6:
			outputColor = vColor * texture(uTextures[6], texCoord);
			break;
		case 7:
			outputColor = vColor * texture(uTextures[7], texCoord);
			break;
		case 8:
			outputColor = vColor * texture(uTextures[8], texCoord);
			break;
		case 9:
			outputColor = vColor * texture(uTextures[9], texCoord);
			break;
		case 10:
			outputColor = vColor * texture(uTextures[10], texCoord);
			break;
		case 11:
			outputColor = vColor * texture(uTextures[11], texCoord);
			break;
		case 12:
			outputColor = vColor * texture(uTextures[12], texCoord);
			break;
		case 13:
			outputColor = vColor * texture(uTextures[13], texCoord);
			break;
		case 14:
			outputColor = vColor * texture(uTextures[14], texCoord);
			break;
		case 15:
			outputColor = vColor * texture(uTextures[15], texCoord);
			break;
		case 16:
			outputColor = vColor * texture(uTextures[16], texCoord);
			break;
		case 17:
			outputColor = vColor * texture(uTextures[17], texCoord);
			break;
		case 18:
			outputColor = vColor * texture(uTextures[18], texCoord);
			break;
		case 19:
			outputColor = vColor * texture(uTextures[19], texCoord);
			break;
		case 20:
			outputColor = vColor * texture(uTextures[20], texCoord);
			break;
		case 21:
			outputColor = vColor * texture(uTextures[21], texCoord);
			break;
		case 22:
			outputColor = vColor * texture(uTextures[22], texCoord);
			break;
		case 23:
			outputColor = vColor * texture(uTextures[23], texCoord);
			break;
		case 24:
			outputColor = vColor * texture(uTextures[24], texCoord);
			break;
		case 25:
			outputColor = vColor * texture(uTextures[25], texCoord);
			break;
		case 26:
			outputColor = vColor * texture(uTextures[26], texCoord);
			break;
		case 27:
			outputColor = vColor * texture(uTextures[27], texCoord);
			break;
		case 28:
			outputColor = vColor * texture(uTextures[28], texCoord);
			break;
		case 29:
			outputColor = vColor * texture(uTextures[29], texCoord);
			break;
		case 30:
			outputColor = vColor * texture(uTextures[30], texCoord);
			break;
		case 31:
			outputColor = vColor * texture(uTextures[31], texCoord);
			break;
	}
	
}