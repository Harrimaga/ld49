using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kee5Engine
{
    public class Player : Entity
    {
        private bool doubleJumped = false, onGround = false, canDash = true;
        private double dashTimer = 0;

        public Player(Vector2 position, Texture tex) : base(position, new Vector2(Globals.tileSize, Globals.tileSize),
            new Sprite(tex, Globals.tileSize, Globals.tileSize, position.X, position.Y, 5, 0, Vector4.One, 4, 0.2f))
        {

        }

        public void Draw()
        {
            sprite.posX = position.X;
            sprite.posY = position.Y;
            sprite.Draw();
        }

        public void Update()
        {
            position.Y += 1;
            onGround = Globals.level.CollidesWithMap(this, out Tile collission);
            
            if (collission is SpikeTile)
            {
                // Dead
                GameOver();
                return;
            }

            position.Y -= 1;
            if (onGround)
            {
                doubleJumped = false;
                canDash = true;
            }
            InputHandling();

            if (dashTimer <= 0)
            {
                velocity.Y += (float)(Balance.gravity * Globals.deltaTime);
            }
            position.X += velocity.X * (float)Globals.deltaTime;

            if (Globals.level.CollidesWithMap(this, out _))
            {
                if (velocity.X > 0)
                {
                    position.X = Globals.tileSize * (float)Math.Floor(position.X / Globals.tileSize);
                }
                else
                {
                    position.X = Globals.tileSize * (float)Math.Floor(position.X / Globals.tileSize) + Globals.tileSize;
                }
                velocity.X = 0;
            }
            position.Y += velocity.Y * (float)Globals.deltaTime;
            if (Globals.level.CollidesWithMap(this, out _))
            {
                if (velocity.Y > 0)
                {
                    position.Y = Globals.tileSize * (float)Math.Floor(position.Y / Globals.tileSize);
                }
                else
                {
                    position.Y = Globals.tileSize * (float)Math.Floor(position.Y / Globals.tileSize) + Globals.tileSize;
                }
                velocity.Y = 0;
            }

            if (position.Y > Globals.level.entityDeathPlane)
            {
                GameOver();
            }

            sprite.Update(Globals.deltaTime);
        }

        public void GameOver()
        {
            Globals.gameState = GameState.LOST;
            Globals.gameOverMenu = new GameOverMenu();
        }

        public void InputHandling()
        {
            if (dashTimer > 0)
            {
                dashTimer -= Globals.deltaTime;

                if (dashTimer <= 0)
                {
                    velocity.Y = Math.Max(velocity.Y, -Balance.maxSpeed);
                }

                return;
            }

            if (Window.inputHandler.IsKeyPressed(Keys.Space) && onGround)
            {
                velocity.Y = -Balance.jumpSpeed;
            }
            else if (Window.inputHandler.IsKeyPressed(Keys.Space) && !doubleJumped)
            {
                velocity.Y = -Balance.jumpSpeed;
                doubleJumped = true;
            }

            if (Window.inputHandler.IsKeyDown(Keys.Space) && velocity.Y < 0)
            {
                velocity.Y -= Balance.gravity * 0.35f * (float)Globals.deltaTime;
            }

            if (velocity != Vector2.Zero && Window.inputHandler.IsKeyPressed(Keys.LeftShift) && canDash)
            {
                velocity = Vector2.NormalizeFast(velocity) * Balance.dashSpeed;

                dashTimer = Balance.dashTime;
                canDash = false;
            }

            if (Window.inputHandler.IsKeyDown(Keys.D) || Window.inputHandler.IsKeyDown(Keys.Right))
            {
                velocity.X = (float)Math.Min(velocity.X + Balance.speed * Globals.deltaTime * (1 + 2 * (Balance.maxSpeed - velocity.X) / Balance.maxSpeed), Balance.maxSpeed);
            }
            else if (Window.inputHandler.IsKeyDown(Keys.A) || Window.inputHandler.IsKeyDown(Keys.Left))
            {
                velocity.X = (float)Math.Max(velocity.X - Balance.speed * Globals.deltaTime * (1 + 2 * (Balance.maxSpeed + velocity.X) / Balance.maxSpeed), -Balance.maxSpeed);

            }
            else
            {
                position.Y += 1;
                Globals.level.CollidesWithMap(this, out Tile groundTile);
                position.Y -= 1;

                velocity.X -= (float)(velocity.X * Globals.deltaTime * Balance.baseResistance * (groundTile != null ? groundTile.resistanceFactor : 1));
                if (velocity.X > -50 && velocity.X < 50)
                {
                    velocity.X = 0;
                }
            }
        }
    }
}
