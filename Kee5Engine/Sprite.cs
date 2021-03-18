﻿using OpenTK.Mathematics;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kee5Engine
{
    public class Sprite
    {
        public Texture texture;
        public float width, height, posX, posY, posZ, rotation, texID;
        public Vector4 color;
        public string name;

        public Sprite(Texture texture, float width, float height, float posX, float posY, float posZ, float rotation, Vector4 color, float texID = 0)
        {
            this.texture = texture;
            this.width = width;
            this.height = height;
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;
            this.rotation = rotation;
            this.color = color;
            this.texID = texID;
        }

        public void Draw()
        {
            
        }

        public virtual void Update(double deltaTime)
        {

        }
    }
}
