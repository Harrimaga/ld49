using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

using SharpFont;

namespace Kee5Engine
{
    public class TextRenderer2D
    {
        private Library _lib;
        private Face _fontFace;
        private float _size;
        internal FontFormatCollection SupportedFormats { get; private set; }

        public TextRenderer2D()
        {
            _lib = new Library();
            _size = 8.25f;
            SupportedFormats = new FontFormatCollection();
            AddFormat("TrueType", "ttf");
            AddFormat("OpenType", "otf");
        }

        internal void SetFont(Face face)
        {
            _fontFace = face;
            SetSize(_size);
        }

        internal void SetFont(string FileName)
        {
            _fontFace = new Face(_lib, FileName);
            SetSize(_size);
        }

        internal void SetSize(float size)
        {
            _size = size;
            if (_fontFace != null)
            {
                _fontFace.SetCharSize(0, size, 0, 96);
            }
        }

        public virtual Bitmap RenderString(string text, Color foreColor)
        {
            return RenderString(_lib, _fontFace, text, foreColor, Color.Transparent);
        }

        public virtual Bitmap RenderString(string text, Color foreColor, Color backColor)
        {
            return RenderString(_lib, _fontFace, text, foreColor, backColor);
        }

        public Bitmap RenderString(Library library, Face face, string text, Color foreColor, Color backColor)
        {
            float penX = 0, penY = 0;
            float stringWidth = 0, stringHeight = 0;
            float overrun = 0, underrun = 0;
            float kern = 0;
            int spacingError = 0;
            int rightEdge = 0;
            float top = 0, bottom = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                uint glyphIndex = face.GetCharIndex(c);

                face.LoadGlyph(glyphIndex, LoadFlags.Default, LoadTarget.Normal);

                float gAdvanceX = (float)face.Glyph.Advance.X;
                float gBearingX = (float)face.Glyph.Metrics.HorizontalBearingX;
                float gWidth = face.Glyph.Metrics.Width.ToSingle();

                underrun += -(gBearingX);
                if (stringWidth == 0)
                {
                    stringWidth += underrun;
                }

                if (gBearingX + gWidth > 0 || gAdvanceX > 0)
                {
                    overrun -= Math.Max(gBearingX + gWidth, gAdvanceX);
                    if (overrun <= 0) overrun = 0;
                }
                overrun += (float)(gBearingX == 0 && gWidth == 0 ? 0 : gBearingX + gWidth - gAdvanceX);

                if (i == text.Length - 1)
                {
                    stringWidth += overrun;
                }

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

                stringWidth += gAdvanceX;

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

            stringHeight = top + bottom;

            if (stringWidth == 0 || stringHeight == 0)
            {
                return null;
            }

            Bitmap bmp = new Bitmap((int)Math.Ceiling(stringWidth), (int)Math.Ceiling(stringHeight));
            underrun = 0;
            overrun = 0;
            stringWidth = 0;

            using (var g = Graphics.FromImage(bmp))
            {
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                g.Clear(backColor);

                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];

                    uint glyphIndex = face.GetCharIndex(c);
                    face.LoadGlyph(glyphIndex, LoadFlags.Default, LoadTarget.Normal);
                    face.Glyph.RenderGlyph(RenderMode.Normal);
                    FTBitmap ftbmp = face.Glyph.Bitmap;

                    float gAdvanceX = (float)face.Glyph.Advance.X;
                    float gBearingX = (float)face.Glyph.Metrics.HorizontalBearingX;
                    float gWidth = (float)face.Glyph.Metrics.Width;

                    underrun += -(gBearingX);
                    if (penX == 0)
                    {
                        penX += underrun;
                    }

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

                    if (gBearingX + gWidth > 0 || gAdvanceX > 0)
                    {
                        overrun -= Math.Max(gBearingX + gWidth, gAdvanceX);
                        if (overrun <= 0) overrun = 0;
                    }
                    overrun += (float)(gBearingX == 0 && gWidth == 0 ? 0 : gBearingX + gWidth - gAdvanceX);
                    if (i == text.Length - 1) penX += overrun;

                    penX += (float)face.Glyph.Advance.X;
                    penY += (float)face.Glyph.Advance.Y;


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
            return bmp;
        }

        private void AddFormat(string name, string ext)
        {
            SupportedFormats.Add(name, ext);
        }

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
                /*case PixelMode.VerticalLcd:
				{
					int bmpHeight = b.Rows / 3;
					Bitmap bmp = new Bitmap(b.Width, bmpHeight, PixelFormat.Format24bppRgb);
					var locked = bmp.LockBits(new Rectangle(0, 0, b.Width, bmpHeight), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
					for (int i = 0; i < bmpHeight; i++)
						PInvokeHelper.Copy(Buffer, i * b.Pitch, locked.Scan0, i * locked.Stride, b.Width);
					bmp.UnlockBits(locked);
					return bmp;
				}*/

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
