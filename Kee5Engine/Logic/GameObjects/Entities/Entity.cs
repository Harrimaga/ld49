using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kee5Engine
{
    public class Entity
    {
        public Vector2 position, velocity, size;
        public Sprite sprite;

        public Entity(Vector2 position, Vector2 size, Sprite sprite)
        {
            this.position = position;
            this.size = size;
            this.sprite = sprite;

            velocity = new Vector2(0, 0);
        }
    }
}
