using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicroFE
{
    /// <summary>
    /// The text buffer class simulates and old school dos buffer.
    /// </summary>
    public class TextBuffer : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public const int TextCols = 80;

        /// <summary>
        /// 
        /// </summary>
        public const int TextRows = 43;

        /// <summary>
        /// 
        /// </summary>
        public int CharWidth { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int CharHeight { get; private set; }

        bool _disposed = false;

        Font _titleFont;
        Brush _titleFontBrush;

        char[] _buffer = new char[TextRows * TextCols];
        Color[] _bufferColors = new Color[TextRows * TextCols];
        Color[] _backColors = new Color[TextRows * TextCols];

        Bitmap _backBuffer;
        Graphics _backBufferContext;

        Dictionary<int, Bitmap> _fontCache;

        // These get cached because 
        Dictionary<int, Brush> _brushCache;

        public TextBuffer()
        {
            _titleFont = new Font(FontFamily.GenericMonospace, 32, FontStyle.Bold, GraphicsUnit.Pixel);
            _titleFontBrush = new SolidBrush(Color.DarkGreen);
            _brushCache = new Dictionary<int, Brush>();

            CharHeight = (int)_titleFont.GetHeight();

            using (var tmpBitmap = new Bitmap(1, 1))
            {
                using (var tmpGfx = Graphics.FromImage(tmpBitmap))
                {
                    var stringSize = tmpGfx.MeasureString("─", _titleFont);
                    CharWidth = (int)stringSize.Width;
                }
            }

            _fontCache = new Dictionary<int, Bitmap>();

        }

        Brush GetBrush(Color c)
        {
            Brush b = null;
            if (!_brushCache.TryGetValue(c.GetHashCode(), out b))
            {
                b = new SolidBrush(c);
                _brushCache[c.GetHashCode()] = b;
            }
            return b;
        }

        Bitmap GetBitmap(Color color, char c)
        {
            Bitmap b = null;
            if (!_fontCache.TryGetValue(new Tuple<Color, char>(color, c).GetHashCode(), out b))
            {
                b = new Bitmap(CharWidth, CharHeight);
                using (var context = Graphics.FromImage(b))
                {
                    context.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
                    context.DrawString(c.ToString(), _titleFont, GetBrush(color), 0, 0);
                }


                _fontCache[new Tuple<Color, char>(color, c).GetHashCode()] = b;
            }
            return b;
        }

        public void PutChar(int x, int y, Color color, char c)
        {
            if (x >= 0 && y >= 0 && x < TextCols && y < TextRows)
            {
                _buffer[y * TextCols + x] = c;
                _bufferColors[y * TextCols + x] = color;
            }
        }

        public void SetBackgroundColor(int x, int y, Color color)
        {
            if (x >= 0 && y >= 0 && x < TextCols && y < TextRows)
            {
                _backColors[y * TextCols + x] = color;
            }
        }

        public void PaintBackground(int x, int y, int w, int h, Color color)
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    SetBackgroundColor(x + i, y + j, color);
                }
            }
        }

        public void PutString(int x, int y, Color color, string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                PutChar(x + i, y, color, text[i]);
            }
        }

        public void RenderTextBuffer(Graphics g, Rectangle r)
        {
            using (_backBuffer = new Bitmap(TextCols * CharWidth, TextRows * CharHeight))
            {
                using (_backBufferContext = Graphics.FromImage(_backBuffer))
                {

                    _backBufferContext.Clear(Color.Black);

                    for (int i = 0; i < TextRows * TextCols; i++)
                    {

                        if (_backColors[i] != default(Color))
                        {

                            _backBufferContext.FillRectangle(GetBrush(_backColors[i]),
                                new Rectangle((int)(i % TextCols) * CharWidth, (int)(i / TextCols) * CharHeight, CharWidth, CharHeight));
                        }

                        if (_bufferColors[i] != default(Color) && _buffer[i] != default(char))
                        {
                            var bmp = GetBitmap(_bufferColors[i], _buffer[i]);
                            _backBufferContext.DrawImage(bmp, (i % TextCols) * CharWidth, (i / TextCols) * CharHeight);
                        }


                    }
                    g.Clear(Color.Black);

                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g.DrawImage(_backBuffer, 0, 0, r.Width, r.Height);
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                foreach (var bc in _brushCache) bc.Value.Dispose();

                _disposed = true;
            }
        }
    }
}
