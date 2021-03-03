using System;
using System.Collections.Generic;
using System.Text;

namespace Kee5Engine
{
    public class Sprite
    {
        public Texture texture;
        public float width, height, posX, posY;
        public string name;

        public Sprite(Texture texture, float width, float height, float posX, float posY)
        {
            this.texture = texture;
            this.width = width;
            this.height = height;
            this.posX = posX;
            this.posY = posY;
        }

        public void Draw()
        {
            
        }

        public virtual void Update(double deltaTime)
        {

        }
    }
}
