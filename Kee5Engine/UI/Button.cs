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

        private TextAlignment _alignment;

        public float posX, posY, width, height, layer, startPosX, startPosY;
        private Vector4 _spriteColor;
        private Vector3 _textColor;
        private bool _isStatic;

        /// <summary>
        /// Create a new text-only Button
        /// </summary>
        /// <param name="posX">X position</param>
        /// <param name="posY">Y position</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="layer">Drawing Layer</param>
        /// <param name="text">Button Text</param>
        /// <param name="textColor">Button Text Color</param>
        /// <param name="isStatic">True if not effected by camera movement</param>
        /// <param name="onClickAction">Is called when the button is left clicked</param>
        /// <param name="onRightClickAction">Is called when the button is right clicked</param>
        public Button(float posX, float posY, float width, float height, float layer, string text, Vector3 textColor, TextAlignment alignment, bool isStatic, EventAction onClickAction, EventAction onRightClickAction = null)
        {
            this.posX = posX;
            this.posY = posY;
            this.width = width;
            this.height = height;
            this.layer = layer;
            _isStatic = isStatic;
            _text = text;
            _textColor = textColor;
            _spriteColor = new Vector4(0, 0, 0, 0);
            _alignment = alignment;

            OnClickAction = onClickAction;
            OnRightClickAction = onRightClickAction == null ? () => { } : onRightClickAction;

            Load("Pixel");
        }

        /// <summary>
        /// Create a new sprite-only Button
        /// </summary>
        /// <param name="posX">X position</param>
        /// <param name="posY">Y position</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="layer">Drawing Layer</param>
        /// <param name="sprite">Button Background Sprite</param>
        /// <param name="spriteColor">Background Sprite Color</param>
        /// <param name="isStatic">True if not effected by camera movement</param>
        /// <param name="onClickAction">Is called when the button is left clicked</param>
        /// <param name="onRightClickAction">Is called when the button is right clicked</param>
        public Button(float posX, float posY, float width, float height, float layer, string sprite, Vector4 spriteColor, bool isStatic, EventAction onClickAction, EventAction onRightClickAction = null)
        {
            this.posX = posX;
            this.posY = posY;
            this.width = width;
            this.height = height;
            this.layer = layer;
            _isStatic = isStatic;
            _text = "";
            _textColor = Vector3.Zero;
            _spriteColor = spriteColor;

            OnClickAction = onClickAction;
            OnRightClickAction = onRightClickAction == null ? () => { } : onRightClickAction;

            Load(sprite);
        }

        /// <summary>
        /// Create a new Text + Sprite button
        /// </summary>
        /// <param name="posX">X position</param>
        /// <param name="posY">Y position</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="layer">Drawing Layer</param>
        /// <param name="sprite">Button Background Sprite</param>
        /// <param name="text">Button Text</param>
        /// <param name="spriteColor">Background Sprite Color</param>
        /// <param name="textColor">Button Text Color</param>
        /// <param name="isStatic">True if not effected by camera movement</param>
        /// <param name="onClickAction">Is called when the button is left clicked</param>
        /// <param name="onRightClickAction">Is called when the button is right clicked</param>
        public Button(float posX, float posY, float width, float height, float layer, string sprite, string text, Vector4 spriteColor, Vector3 textColor, TextAlignment alignment, bool isStatic, EventAction onClickAction, EventAction onRightClickAction = null)
        {
            this.posX = posX;
            this.posY = posY;
            this.width = width;
            this.height = height;
            this.layer = layer;
            _isStatic = isStatic;
            _text = text;
            _textColor = textColor;
            _spriteColor = spriteColor;
            _alignment = alignment;

            OnClickAction = onClickAction;
            OnRightClickAction = onRightClickAction == null ? () => { } : onRightClickAction;

            Load(sprite);
        }

        /// <summary>
        /// Load the texture for the button
        /// </summary>
        /// <param name="sprite">Background Sprite</param>
        private void Load(string sprite)
        {
            startPosX = posX;
            startPosY = posY;
            _background = new Sprite(Window.textures.GetTexture(sprite), width, height, posX, posY, layer, 0f, _spriteColor);

            LoadText();
        }

        private void LoadText()
        {
            Window.textRenderer.SetSize(Math.Min(width / 4, height / 2));
            if (_text.Length > 0)
            {
                _textRender = Texture.LoadFromBmp(Window.textRenderer.RenderString(_text, Color.FromArgb((int)(_textColor.X * 255), (int)(_textColor.Y * 255), (int)(_textColor.Z * 255)), Color.Transparent), "Button", false);
            }
        }

        /// <summary>
        /// Is called when the button is clicked
        /// </summary>
        public void OnClick()
        {
            OnClickAction();
        }

        /// <summary>
        /// Is called when the button is right clicked
        /// </summary>
        public void OnRightClick()
        {
            OnRightClickAction();
        }

        /// <summary>
        /// Draw the Button
        /// </summary>
        public void Draw()
        {
            Window.spriteRenderer.DrawSprite(_background);
            if (_text.Length > 0)
            {
                switch (_alignment)
                {
                    case TextAlignment.CENTER:
                        Window.spriteRenderer.DrawSprite(_textRender, new Vector2(posX, posY), _textRender.Size, layer + 0.01f, 0f, Vector4.One);
                        break;
                    case TextAlignment.LEFT:
                        Window.spriteRenderer.DrawSprite(_textRender, new Vector2(posX - width / 2 + _textRender.Size.X / 2 + 5, posY), _textRender.Size, layer + 0.01f, 0f, Vector4.One);
                        break;
                    case TextAlignment.RIGHT:
                        Window.spriteRenderer.DrawSprite(_textRender, new Vector2(posX + width / 2 - _textRender.Size.X / 2 - 5, posY), _textRender.Size, layer + 0.01f, 0f, Vector4.One);
                        break;
                }
            }
        }

        /// <summary>
        /// Update the button
        /// </summary>
        public void Update()
        {
            if (_isStatic)
            {
                posX = startPosX + Window.camera.Position.X;
                posY = startPosY + Window.camera.Position.Y;
                _background.posX = posX;
                _background.posY = posY;
            }
        }

        /// <summary>
        /// Check if a click was within the bounds of the button
        /// </summary>
        /// <param name="x">Click x position</param>
        /// <param name="y">Click y position</param>
        /// <returns><code>True</code> if the click was within the bounds</returns>
        public bool IsInButton(float x, float y)
        {
            return x + Window.camera.Position.X >= posX - width / 2 && x + Window.camera.Position.X <= posX + width / 2 && y + Window.camera.Position.Y >= posY - height / 2 && y + Window.camera.Position.Y <= posY + height / 2;
        }
    }
}
