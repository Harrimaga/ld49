using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

using SharpFont;

namespace Kee5Engine
{
    public enum TextAlignment
    {
        LEFT,
        CENTER,
        RIGHT
    }

    public class TextRenderer2D
    {
        private Library _lib;
        private Face _fontFace;
        private float _size;
        internal FontFormatCollection SupportedFormats { get; private set; }

        /// <summary>
        /// Create a new Text Renderer
        /// </summary>
        public TextRenderer2D()
        {
            _lib = new Library();
            _size = 8.25f;
            SupportedFormats = new FontFormatCollection();
            AddFormat("TrueType", "ttf");
            AddFormat("OpenType", "otf");
        }

        /// <summary>
        /// Set the Font used of the Text Renderer
        /// </summary>
        /// <param name="face"></param>
        internal void SetFont(Face face)
        {
            _fontFace = face;
            SetSize(_size);
        }

        /// <summary>
        /// Set the Font used of the Text Renderer
        /// </summary>
        /// <param name="FileName">ttf or otf file path of the font</param>
        internal void SetFont(string FileName)
        {
            _fontFace = new Face(_lib, FileName);
            SetSize(_size);
        }

        /// <summary>
        /// Set the size of the font
        /// </summary>
        /// <param name="size">Size in px</param>
        internal void SetSize(float size)
        {
            _size = size;
            if (_fontFace != null)
            {
                _fontFace.SetCharSize(0, size, 0, 96);
            }
        }

        /// <summary>
        /// Render a text with a transparent background
        /// </summary>
        /// <param name="text">Text to be drawn</param>
        /// <param name="foreColor">Text color</param>
        /// <returns><code>Bitmap</code> containing the render of the text</returns>
        public virtual Bitmap RenderString(string text, Color foreColor)
        {
            return RenderString(_lib, _fontFace, text, foreColor, Color.Transparent);
        }

        /// <summary>
        /// Render a text with a colored background
        /// </summary>
        /// <param name="text">Text to be drawn</param>
        /// <param name="foreColor">Text color</param>
        /// <param name="backColor">Background Color</param>
        /// <returns><code>Bitmap</code> containing the render of the text</returns>
        public virtual Bitmap RenderString(string text, Color foreColor, Color backColor)
        {
            return RenderString(_lib, _fontFace, text, foreColor, backColor);
        }

        /// <summary>
        /// Render the text
        /// </summary>
        /// <param name="library">Library used</param>
        /// <param name="face">Font used</param>
        /// <param name="text">Text to be rendered</param>
        /// <param name="foreColor">Text color</param>
        /// <param name="backColor">Background color</param>
        /// <returns><code>Bitmap</code> containing the render of the text</returns>
        public Bitmap RenderString(Library library, Face face, string text, Color foreColor, Color backColor)
        {
            float penX = 0, penY = 0;
            float stringWidth = 0, stringHeight = 0;
            float overrun = 0, underrun = 0;
            float kern = 0;
            int rightEdge = 0;
            float top = 0, bottom = 0;

            // Read every character in the text
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                // Get the glyph index for the character from the font
                uint glyphIndex = face.GetCharIndex(c);

                // Load the Glyph
                face.LoadGlyph(glyphIndex, LoadFlags.Default, LoadTarget.Normal);

                // Get the metrics for the Glyph
                float gAdvanceX = (float)face.Glyph.Advance.X;
                float gBearingX = (float)face.Glyph.Metrics.HorizontalBearingX;
                float gWidth = face.Glyph.Metrics.Width.ToSingle();

                // Handle Underrun
                underrun += -(gBearingX);
                if (stringWidth == 0)
                {
                    stringWidth += underrun;
                }

                // Handle Overrun
                if (gBearingX + gWidth > 0 || gAdvanceX > 0)
                {
                    overrun -= Math.Max(gBearingX + gWidth, gAdvanceX);
                    if (overrun <= 0) overrun = 0;
                }
                overrun += (float)(gBearingX == 0 && gWidth == 0 ? 0 : gBearingX + gWidth - gAdvanceX);

                // Adjust string width
                if (i == text.Length - 1)
                {
                    stringWidth += overrun;
                }

                // Handle Top and Bottom Bearings
                float glyphTop = (float)face.Glyph.Metrics.HorizontalBearingY;
                float glyphBottom = (float)(face.Glyph.Metrics.Height - face.Glyph.Metrics.HorizontalBearingY);
                if (glyphTop > top)
                {
                    top = glyphTop;
                }
                if (glyphBottom > bottom)
                {
                    bottom = glyphBottom;
                }

                // Adjust string width
                stringWidth += gAdvanceX;

                // If the glyph has kerning, adjust the string width
                if (face.HasKerning && i < text.Length - 1)
                {
                    char cNext = text[i + 1];
                    kern = (float)face.GetKerning(glyphIndex, face.GetCharIndex(cNext), KerningMode.Default).X;

                    if (kern > gAdvanceX * 5 || kern < -(gAdvanceX * 5))
                    {
                        kern = 0;
                    }
                    stringWidth += kern;
                }
            }

            // Adjust the string height
            stringHeight = top + bottom;

            // If the string has no width or heigh, it cannot be drawn
            if (stringWidth == 0 || stringHeight == 0)
            {
                return null;
            }

            // Create a new Bitmap for the text render
            Bitmap bmp = new Bitmap((int)Math.Ceiling(stringWidth), (int)Math.Ceiling(stringHeight));

            // Reset Metrics
            underrun = 0;
            overrun = 0;
            stringWidth = 0;

            using (var g = Graphics.FromImage(bmp))
            {
                // Set Bitmap properties
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                // Create the background color
                g.Clear(backColor);

                // For each character in the to be drawn text
                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];

                    // Load the Glyph
                    uint glyphIndex = face.GetCharIndex(c);
                    face.LoadGlyph(glyphIndex, LoadFlags.Default, LoadTarget.Normal);
                    face.Glyph.RenderGlyph(RenderMode.Normal);

                    // Create a Free Type Bitmap for the Glyph
                    FTBitmap ftbmp = face.Glyph.Bitmap;

                    // Get the Metrics
                    float gAdvanceX = (float)face.Glyph.Advance.X;
                    float gBearingX = (float)face.Glyph.Metrics.HorizontalBearingX;
                    float gWidth = (float)face.Glyph.Metrics.Width;

                    // Handle Underrun
                    underrun += -(gBearingX);
                    if (penX == 0)
                    {
                        penX += underrun;
                    }

                    // Draw the Glyph to the bitmap
                    if ((ftbmp.Width > 0 && ftbmp.Rows > 0))
                    {
                        Bitmap cBmp = ToGdipBitmap(ftbmp, foreColor);
                        int x = (int)Math.Round(penX + face.Glyph.BitmapLeft);
                        int y = (int)Math.Round(penY + top - (float)face.Glyph.Metrics.HorizontalBearingY);

                        g.DrawImageUnscaled(cBmp, x, y);
                        rightEdge = Math.Max(rightEdge, x + cBmp.Width);
                    }
                    else
                    {
                        rightEdge = (int)(penX + gAdvanceX);
                    }

                    // Handle overrun
                    if (gBearingX + gWidth > 0 || gAdvanceX > 0)
                    {
                        overrun -= Math.Max(gBearingX + gWidth, gAdvanceX);
                        if (overrun <= 0) overrun = 0;
                    }
                    overrun += (float)(gBearingX == 0 && gWidth == 0 ? 0 : gBearingX + gWidth - gAdvanceX);
                    if (i == text.Length - 1) penX += overrun;

                    // Move the pen so the next glyph can be drawn
                    penX += (float)face.Glyph.Advance.X;
                    penY += (float)face.Glyph.Advance.Y;

                    // Handle Kerning
                    if (face.HasKerning && i < text.Length - 1)
                    {
                        char cNext = text[i + 1];
                        kern = (float)face.GetKerning(glyphIndex, face.GetCharIndex(cNext), KerningMode.Default).X;
                        if (kern > gAdvanceX * 5 || kern < -(gAdvanceX * 5))
                        {
                            kern = 0;
                        }
                        penX += (float)kern;
                    }
                }
            }
            // Return the rendered text as a Bitmap
            return bmp;
        }

        /// <summary>
        /// Adds supported font format
        /// </summary>
        /// <param name="name">Format name</param>
        /// <param name="ext">Format extention</param>
        private void AddFormat(string name, string ext)
        {
            SupportedFormats.Add(name, ext);
        }

        /// <summary>
        /// Create Bitmap from FreeType Bitmap
        /// </summary>
        /// <param name="b">Bitmap to be converted</param>
        /// <param name="color">Bitmap Color</param>
        /// <returns><code>System.Drawing.Bitmap</code></returns>
        public static Bitmap ToGdipBitmap(FTBitmap b, Color color)
        {
            if (b.IsDisposed)
                throw new ObjectDisposedException("FTBitmap", "Cannot access a disposed object.");

            if (b.Width == 0 || b.Rows == 0)
                throw new InvalidOperationException("Invalid image size - one or both dimensions are 0.");


            switch (b.PixelMode)
            {
                case PixelMode.Mono:
                    {
                        Bitmap bmp = new Bitmap(b.Width, b.Rows, PixelFormat.Format1bppIndexed);
                        var locked = bmp.LockBits(new Rectangle(0, 0, b.Width, b.Rows), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);

                        for (int i = 0; i < b.Rows; i++)
                            Copy(b.Buffer, i * b.Pitch, locked.Scan0, i * locked.Stride, locked.Stride);

                        bmp.UnlockBits(locked);

                        ColorPalette palette = bmp.Palette;
                        palette.Entries[0] = Color.FromArgb(0, color);
                        palette.Entries[1] = Color.FromArgb(255, color);

                        bmp.Palette = palette;
                        return bmp;
                    }

                case PixelMode.Gray4:
                    {
                        Bitmap bmp = new Bitmap(b.Width, b.Rows, PixelFormat.Format4bppIndexed);
                        var locked = bmp.LockBits(new Rectangle(0, 0, b.Width, b.Rows), ImageLockMode.ReadWrite, PixelFormat.Format4bppIndexed);

                        for (int i = 0; i < b.Rows; i++)
                            Copy(b.Buffer, i * b.Pitch, locked.Scan0, i * locked.Stride, locked.Stride);

                        bmp.UnlockBits(locked);

                        ColorPalette palette = bmp.Palette;
                        for (int i = 0; i < palette.Entries.Length; i++)
                        {
                            float a = (i * 17) / 255f;
                            palette.Entries[i] = Color.FromArgb(i * 17, (int)(color.R * a), (int)(color.G * a), (int)(color.B * a));
                        }

                        bmp.Palette = palette;
                        return bmp;
                    }

                case PixelMode.Gray:
                    {
                        Bitmap bmp = new Bitmap(b.Width, b.Rows, PixelFormat.Format8bppIndexed);
                        var locked = bmp.LockBits(new Rectangle(0, 0, b.Width, b.Rows), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

                        for (int i = 0; i < b.Rows; i++)
                            Copy(b.Buffer, i * b.Pitch, locked.Scan0, i * locked.Stride, locked.Stride);

                        bmp.UnlockBits(locked);

                        ColorPalette palette = bmp.Palette;
                        for (int i = 0; i < palette.Entries.Length; i++)
                        {
                            float a = i / 255f;
                            palette.Entries[i] = Color.FromArgb(i, (int)(color.R * a), (int)(color.G * a), (int)(color.B * a));
                        }

                        bmp.Palette = palette;
                        return bmp;
                    }

                case PixelMode.Lcd:
                    {
                        //TODO apply color
                        int bmpWidth = b.Width / 3;
                        Bitmap bmp = new Bitmap(bmpWidth, b.Rows, PixelFormat.Format24bppRgb);
                        var locked = bmp.LockBits(new Rectangle(0, 0, bmpWidth, b.Rows), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                        for (int i = 0; i < b.Rows; i++)
                            Copy(b.Buffer, i * b.Pitch, locked.Scan0, i * locked.Stride, locked.Stride);

                        bmp.UnlockBits(locked);

                        return bmp;
                    }
                default:
                    throw new InvalidOperationException("System.Drawing.Bitmap does not support this pixel mode.");
            }
        }

        static unsafe void Copy(IntPtr source, int sourceOffset, IntPtr destination, int destinationOffset, int count)
        {
            byte* src = (byte*)source + sourceOffset;
            byte* dst = (byte*)destination + destinationOffset;
            byte* end = dst + count;

            while (dst != end)
                *dst++ = *src++;
        }
    }

    internal class FontFormatCollection : Dictionary<string, FontFormat>
    {
        public void Add(string name, string ext)
        {
            if (!ext.StartsWith(".")) ext = "." + ext;
            Add(ext, new FontFormat(name, ext));
        }

        public bool ContainsExt(string ext)
        {
            return ContainsKey(ext);
        }
    }

    internal class FontFormat
    {
        public string Name { get; private set; }
        public string FileExtension { get; private set; }
        
        public FontFormat(string name, string ext)
        {
            if (!ext.StartsWith(".")) ext = "." + ext;
            Name = name;
            FileExtension = ext;
        }
    }
}
