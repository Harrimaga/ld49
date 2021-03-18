using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Kee5Engine
{
    public class Button
    {
        public delegate void EventAction();
        private string _text;
        private Texture _textRender;
        private Sprite _background;
        private EventAction OnClickAction { get; set; }
        private EventAction OnRightClickAction { get; set; }

        public float posX, posY, width, height, layer;
        private Vector4 _spriteColor;
        private Vector3 _textColor;

        public Button(float posX, float posY, float width, float height, float layer, string text, Vector3 textColor, EventAction onClickAction, EventAction onRightClickAction = null)
        {
            this.posX = posX;
            this.posY = posY;
            this.width = width;
            this.height = height;
            this.layer = layer;
            _text = text;
            _textColor = textColor;
            _spriteColor = new Vector4(0, 0, 0, 0);

            OnClickAction = onClickAction;
            OnRightClickAction = onRightClickAction == null ? () => { } : onRightClickAction;

            Load("Pixel");
        }

        public Button(float posX, float posY, float width, float height, float layer, string sprite, Vector4 spriteColor, EventAction onClickAction, EventAction onRightClickAction = null)
        {
            this.posX = posX;
            this.posY = posY;
            this.width = width;
            this.height = height;
            this.layer = layer;
            _text = "";
            _textColor = Vector3.Zero;
            _spriteColor = spriteColor;

            OnClickAction = onClickAction;
            OnRightClickAction = onRightClickAction == null ? () => { } : onRightClickAction;

            Load(sprite);
        }

        public Button(float posX, float posY, float width, float height, float layer, string sprite, string text, Vector4 spriteColor, Vector3 textColor, EventAction onClickAction, EventAction onRightClickAction = null)
        {
            this.posX = posX;
            this.posY = posY;
            this.width = width;
            this.height = height;
            this.layer = layer;
            _text = text;
            _textColor = textColor;
            _spriteColor = spriteColor;

            OnClickAction = onClickAction;
            OnRightClickAction = onRightClickAction == null ? () => { } : onRightClickAction;

            Load(sprite);
        }

        private void Load(string sprite)
        {
            _background = new Sprite(Window.textures.GetTexture(sprite), width, height, posX, posY, layer, 0f, _spriteColor);
            Window.textRenderer.SetSize(width / 4);
            Window.textRenderer.SetFont("Fonts/arial.ttf");
            _textRender = Texture.LoadFromBmp(Window.textRenderer.RenderString(_text, Color.FromArgb((int)(_textColor.X * 255), (int)(_textColor.Y * 255), (int)(_textColor.Z * 255)), Color.Transparent), "Button");
        }

        public void OnClick()
        {
            OnClickAction();
        }

        public void OnRightClick()
        {
            OnRightClickAction();
        }

        public void Draw()
        {
            Window.spriteRenderer.DrawSprite(_background);
            Window.spriteRenderer.DrawSprite(_textRender, new Vector2(posX, posY), _textRender.Size, layer + 0.01f, 0f, Vector4.One);
        }

        public bool IsInButton(float x, float y)
        {
            return x >= posX - width / 2 && x <= posX + width / 2 && y >= posY - height / 2 && y <= posY + height / 2;
        }
    }
}
