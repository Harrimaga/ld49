using OpenTK.Mathematics;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kee5Engine
{
    public class Sprite
    {
        public Texture texture;
        public float width, height, posX, posY, posZ, rotation, texID, texX;
        public Vector4 color;
        public string name;
        public int frames, currentFrame;
        private double _animationTime, _timePassed;

        /// <summary>
        /// Create a new Sprite
        /// </summary>
        /// <param name="texture">Texture</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="posX">X position (center-origin)</param>
        /// <param name="posY">Y position (center-origin)</param>
        /// <param name="posZ">Drawing layer</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="color">Sprite Color</param>
        /// <param name="frames">Total frames in the spritesheet</param>
        /// <param name="animationTime">Time in seconds per frame</param>
        /// <param name="texID">ID of the texture in the batch</param>
        public Sprite(Texture texture, float width, float height, float posX, float posY, float posZ, float rotation, Vector4 color, int frames = 1, double animationTime = 1, float texID = 0)
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
            this.frames = frames;
            _animationTime = animationTime;

            texX = 1.0f / frames;
            currentFrame = 0;
        }

        /// <summary>
        /// Draw the sprite
        /// </summary>
        public void Draw()
        {
            Window.spriteRenderer.DrawSprite(this);
        }

        /// <summary>
        /// Update the sprite
        /// </summary>
        /// <param name="deltaTime"></param>
        public virtual void Update(double deltaTime)
        {
            // If the time passed since last frame > time per frame, go to the next frame
            _timePassed += deltaTime;
            if (_timePassed > _animationTime)
            {
                currentFrame = (currentFrame + 1) % frames;
                _timePassed = 0;
            }
        }
    }
}
