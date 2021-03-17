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
            LoadTexture("Sprites/Test/Test.png", "Test1");
            LoadTexture("Sprites/Test/Test.png", "Test2");
            LoadTexture("Sprites/Test/Test.png", "Test3");
            LoadTexture("Sprites/Test/Test.png", "Test4");

            LoadTexture("Sprites/Test/Test.png", "Test5");
            LoadTexture("Sprites/Test/Test.png", "Test6");
            LoadTexture("Sprites/Test/Test.png", "Test7");
            LoadTexture("Sprites/Test/Test.png", "Test8");
            LoadTexture("Sprites/Test/Test.png", "Test9");

            LoadTexture("Sprites/Test/Test.png", "Test10");
            LoadTexture("Sprites/Test/Test.png", "Test11");
            LoadTexture("Sprites/Test/Test.png", "Test12");
            LoadTexture("Sprites/Test/Test.png", "Test13");
            LoadTexture("Sprites/Test/Test.png", "Test14");

            LoadTexture("Sprites/Test/Test.png", "Test15");
            LoadTexture("Sprites/Test/Test.png", "Test16");
            LoadTexture("Sprites/Test/Test.png", "Test17");
            LoadTexture("Sprites/Test/Test.png", "Test18");
            LoadTexture("Sprites/Test/Test.png", "Test19");

            LoadTexture("Sprites/Test/Test.png", "Test20");
            LoadTexture("Sprites/Test/Test.png", "Test21");
            LoadTexture("Sprites/Test/Test.png", "Test22");
            LoadTexture("Sprites/Test/Test.png", "Test23");
            LoadTexture("Sprites/Test/Test.png", "Test24");

            LoadTexture("Sprites/Test/Test.png", "Test25");
            LoadTexture("Sprites/Test/Test.png", "Test26");
            LoadTexture("Sprites/Test/Test.png", "Test27");
            LoadTexture("Sprites/Test/Test.png", "Test28");
            LoadTexture("Sprites/Test/Test.png", "Test29");

            LoadTexture("Sprites/Test/Test.png", "Test30");
            LoadTexture("Sprites/Test/Test.png", "Test31");
            LoadTexture("Sprites/Test/Test.png", "Test32");
            LoadTexture("Sprites/Test/Test.png", "Test33");
            LoadTexture("Sprites/Test/Test.png", "Test34");

            LoadTexture("Sprites/Test/Test.png", "Test35");
            LoadTexture("Sprites/Test/Test.png", "Test36");
            LoadTexture("Sprites/Test/Test.png", "Test37");
            LoadTexture("Sprites/Test/Test.png", "Test38");
            LoadTexture("Sprites/Test/Test.png", "Test39");

            LoadTexture("Sprites/Test/Test.png", "Test40");
            LoadTexture("Sprites/Test/Test.png", "Test41");
            LoadTexture("Sprites/Test/Test.png", "Test42");
            LoadTexture("Sprites/Test/Test.png", "Test43");
            LoadTexture("Sprites/Test/Test.png", "Test44");

            LoadTexture("Sprites/Test/Test.png", "Test45");
            LoadTexture("Sprites/Test/Test.png", "Test46");
            LoadTexture("Sprites/Test/Test.png", "Test47");
            LoadTexture("Sprites/Test/Test.png", "Test48");
            LoadTexture("Sprites/Test/Test.png", "Test49");
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
