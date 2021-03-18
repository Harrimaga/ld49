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

        public TextureList()
        {
            _textures = new Dictionary<string, Texture>();
            init();
        }

        public void init()
        {
            LoadTexture("Sprites/Test/Test.png", "Test");
            LoadTexture("Sprites/Test/Pixel.png", "Pixel");
        }

        public void LoadTexture(string path, string name)
        {
            Texture texture = Texture.LoadFromFile(path, name);
            _textures[name] = texture;
        }

        public void LoadTexture(Bitmap image, string name)
        {
            Texture texture = Texture.LoadFromBmp(image, name);
            _textures[name] = texture;
        }

        public Texture GetTexture(string name)
        {
            return _textures[name];
        }

        public void UnLoad()
        {
            int unloaded = 0;
            foreach (Texture texture in _textures.Values)
            {
                GL.DeleteTexture(texture.Handle);
                unloaded++;
            }
            Console.WriteLine($"{unloaded} textures unloaded");
        }
    }
}
