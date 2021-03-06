using OpenTK.Graphics.ES11;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Kee5Engine
{
    public class TextureList
    {
        private Dictionary<string, Texture> _textures;

        /// <summary>
        /// Create a List to hold all loaded Textures
        /// </summary>
        public TextureList()
        {
            _textures = new Dictionary<string, Texture>();
            init();
        }

        /// <summary>
        /// Load all the Textures
        /// </summary>
        public void init()
        {
            // Test Sprites
            LoadTexture("Sprites/Test/Test.png", "Test");
            LoadTexture("Sprites/Test/Pixel.png", "Pixel");
            LoadTexture("Sprites/Test/PlayerIdle.png", "PlayerIdle");

            // Others
            LoadTexture("Sprites/Tiles/End.png", "EndTile");
            LoadTexture("Sprites/Tiles/Key.png", "KeyTile");

            // Tiles
            LoadTexture("Sprites/Tiles/IceTiles.png", "IceTile");
            LoadTexture("Sprites/Tiles/SafeTile.png", "SafeTile");
            LoadTexture("Sprites/Tiles/Spike.png", "SpikeTile");
            LoadTexture("Sprites/Tiles/UnstableTile.png", "UnstableTile");
            LoadTexture("Sprites/Tiles/ArrowR.png", "ArrowRight");
            LoadTexture("Sprites/Tiles/ArrowL.png", "ArrowLeft");
            LoadTexture("Sprites/Tiles/ArrowD.png", "ArrowDown");
            LoadTexture("Sprites/Tiles/ArrowU.png", "ArrowUp");

            // Backgrounds
            LoadTexture("Sprites/Background/Background1.png", "Background1");
            LoadTexture("Sprites/Background/Background2.png", "Background2");
            LoadTexture("Sprites/Background/Background3.png", "Background3");
            LoadTexture("Sprites/Background/Background4.png", "Background4");
            LoadTexture("Sprites/Background/Background5.png", "Background5");

            // Player
            LoadTexture("Sprites/Player.png", "Player");
        }

        /// <summary>
        /// Loads a texture from a filepath
        /// </summary>
        /// <param name="path">Path to the texture</param>
        /// <param name="name">Name of the texture</param>
        public void LoadTexture(string path, string name)
        {
            Texture texture = Texture.LoadFromFile(path, name);

            // If the texture already exists, unload it first
            if (_textures.ContainsKey(name))
            {
                GL.DeleteTexture(_textures[name].Handle);
            }
            _textures[name] = texture;
        }

        /// <summary>
        /// Loads a texture from a Bitmap
        /// </summary>
        /// <param name="image">Bitmap</param>
        /// <param name="name">Name of the texture</param>
        public Texture LoadTexture(Bitmap image, string name)
        {
            Texture texture = Texture.LoadFromBmp(image, name, false);
            
            // If the texture already exists, unload it first
            if (_textures.ContainsKey(name))
            {
                GL.DeleteTexture(_textures[name].Handle);
            }
            _textures[name] = texture;
            return texture;
        }

        /// <summary>
        /// Get a loaded texture from a name
        /// </summary>
        /// <param name="name">Name of the texture</param>
        /// <returns><code>Texture</code></returns>
        public Texture GetTexture(string name)
        {
            return _textures[name];
        }

        /// <summary>
        /// Unload all textures from memory
        /// </summary>
        public void UnLoad()
        {
            foreach (Texture texture in _textures.Values)
            {
                Console.WriteLine($"Unloaded {texture.name}");
                GL.DeleteTexture(texture.Handle);
                Globals.unloaded++;
            }
        }

        /// <summary>
        /// Check if a texture exists in the list
        /// </summary>
        /// <param name="name"><code>string</code> name of the texture</param>
        /// <returns><code>true</code> if the texture exists</returns>
        /// <returns><code>false</code> if the texture doesn't exist</returns>
        public bool CheckIfTextureExists(string name)
        {
            return _textures.ContainsKey(name);
        }
    }
}
