﻿using OpenTK.Graphics.ES11;
using System;
using System.Collections.Generic;
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
        }

        public void LoadTexture(string path, string name)
        {
            Texture texture = Texture.LoadFromFile(path);
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
