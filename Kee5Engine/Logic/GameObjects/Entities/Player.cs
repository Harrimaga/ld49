using Kee5Engine.Audio;
using Kee5Engine.IO;
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
        private Random rng = new Random();

        private readonly string[] sfxs = { "A3", "B3", "C3", "D3", "E3", "F3", "G3" };

        public Player(Vector2 position, Texture tex) : base(position, new Vector2(Globals.tileSize, Globals.tileSize),
            new Sprite(tex, Globals.tileSize, Globals.tileSize, position.X, position.Y, 5, 0, Vector4.One, 6, 0.1f))
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
            onGround = Globals.level.CollidesWithMap(this, out _);
            Tile collission = Globals.level.GetTileBelow(position, size);
            
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

        private void playJumpSFX()
        {
            string sfx = sfxs[rng.Next(sfxs.Length)];
            AudioManager.PlaySFX($"Audio/SFX/{sfx}.wav");
        }

        public void GameOver()
        {
            AudioManager.PlaySFX("Audio/SFX/Death.wav");
            Globals.gameState = GameState.LOST;
            Globals.gameOverMenu = new GameOverMenu();
            Globals.deathCount++;
            Window.WriteSave();
        }

        public void InputHandling()
        {
            if (Window.inputHandler.IsKeyPressed(Keys.R) || Window.inputHandler.IsButtonPressed(ControllerKeys.SELECT))
            {
                Globals.activeButtons.Clear();
                Globals.level = new Level();
                Globals.gameState = GameState.PLAYING;
                Globals.deathCount++;
                Window.WriteSave();
            }

            if (Window.inputHandler.IsKeyPressed(Keys.Escape) || Window.inputHandler.IsButtonPressed(ControllerKeys.START))
            {
                Globals.mainMenu = new MainMenu();
                Globals.gameState = GameState.MENU;
            }

            if (dashTimer > 0)
            {
                dashTimer -= Globals.deltaTime;

                if (dashTimer <= 0)
                {
                    sprite.color = new Vector4(1, 1, 1, 1);
                    velocity.Y = Math.Max(velocity.Y, -Balance.maxSpeed);
                    velocity.X = Math.Clamp(velocity.X, -Balance.maxSpeed, Balance.maxSpeed);
                }

                return;
            }

            if ((Window.inputHandler.IsKeyPressed(Keys.Space) || Window.inputHandler.IsButtonPressed(ControllerKeys.A)) && onGround)
            {
                velocity.Y = -Balance.jumpSpeed;
                playJumpSFX();
            }
            else if ((Window.inputHandler.IsKeyPressed(Keys.Space) || Window.inputHandler.IsButtonPressed(ControllerKeys.A)) && !doubleJumped)
            {
                velocity.Y = -Balance.jumpSpeed;
                doubleJumped = true;
                playJumpSFX();
            }

            if ((Window.inputHandler.IsKeyDown(Keys.Space) || Window.inputHandler.IsButtonPressed(ControllerKeys.A)) && velocity.Y < 0)
            {
                velocity.Y -= Balance.gravity * 0.35f * (float)Globals.deltaTime;
            }

            if (Window.inputHandler.IsKeyDown(Keys.D) || Window.inputHandler.IsKeyDown(Keys.Right) || Window.inputHandler.IsLeftStickAngle(ControllerAngle.RIGHT) || Window.inputHandler.IsButtonDown(ControllerKeys.RIGHT))
            {
                velocity.X = (float)Math.Min(velocity.X + Balance.speed * Globals.deltaTime * (1 + 2 * (Balance.maxSpeed - velocity.X) / Balance.maxSpeed), Balance.maxSpeed);
            }
            else if (Window.inputHandler.IsKeyDown(Keys.A) || Window.inputHandler.IsKeyDown(Keys.Left) || Window.inputHandler.IsLeftStickAngle(ControllerAngle.LEFT) || Window.inputHandler.IsButtonDown(ControllerKeys.LEFT))
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

            if (velocity != Vector2.Zero && (Window.inputHandler.IsKeyPressed(Keys.LeftShift) || Window.inputHandler.IsButtonPressed(ControllerKeys.B)) && canDash)
            {
                velocity = Vector2.NormalizeFast(velocity) * Balance.dashSpeed;
                sprite.color = new Vector4(0, 0.5f, 0.8f, 1);

                dashTimer = Balance.dashTime;
                canDash = false;
            }
        }
    }
}
