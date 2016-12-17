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
        public const int TextCols = 100;

        /// <summary>
        /// 
        /// </summary>
        public const int TextRows = 50;

        /// <summary>
        /// 
        /// </summary>
        public int CharWidth { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int CharHeight { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        bool _disposed = false;

        /// <summary>
        /// 
        /// </summary>
        Font _textFont;

        /// <summary>
        /// 
        /// </summary>
        Brush _textBrish;

        char[] _buffer = new char[TextRows * TextCols];
        Color[] _bufferColors = new Color[TextRows * TextCols];
        Color[] _backColors = new Color[TextRows * TextCols];

        /// <summary>
        /// Context Independent back buffer.
        /// </summary>
        Bitmap _backBuffer;

        /// <summary>
        /// Graphics context for back buffer.
        /// </summary>
        Graphics _backBufferContext;

        /// <summary>
        /// Also, creating and destroying bitmaps over and over again is also bad. GDI+ can blow up on this.
        /// To speed things up, since drawing text is slow, cache the text drawn to screen as bitmaps.
        /// </summary>
        Dictionary<int, Bitmap> _fontCache;

        /// <summary>
        /// Creating and destroying brushes over and over is bad. Cache the ones used instead.
        /// </summary>
        Dictionary<int, Brush> _brushCache;

        /// <summary>
        /// Figure out the width and height of the text. 
        /// </summary>
        public TextBuffer()
        {
            _textFont = new Font(FontFamily.GenericMonospace, 48, FontStyle.Bold, GraphicsUnit.Pixel);
            _brushCache = new Dictionary<int, Brush>();

            CharHeight = (int)_textFont.GetHeight();

            using (var tmpBitmap = new Bitmap(1, 1))
            {
                using (var tmpGfx = Graphics.FromImage(tmpBitmap))
                {
                    var stringSize = tmpGfx.MeasureString("─", _textFont);
                    CharWidth = (int)stringSize.Width;
                }
            }

            _fontCache = new Dictionary<int, Bitmap>();

        }

        /// <summary>
        /// Get a brush for a given color then fetch it from the cache. If it's not there, add it.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Fetch a bitmap for a given color and character. If it doesn't exist, create it.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Bitmap GetCharBitmap(Color color, char c)
        {
            Bitmap b = null;

            if (!_fontCache.TryGetValue(new Tuple<Color, char>(color, c).GetHashCode(), out b))
            {
                b = new Bitmap(CharWidth, CharHeight);
                using (var bx = new Bitmap(CharWidth, CharHeight))
                {
                    using (var context = Graphics.FromImage(bx))
                    {
                        context.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                      
                        context.DrawString(c.ToString(), _textFont, GetBrush(color), 0, 0, new StringFormat() {
                            Alignment = StringAlignment.Near,
                            FormatFlags = StringFormatFlags.NoClip|StringFormatFlags.NoWrap,
                            LineAlignment = StringAlignment.Near,
                            Trimming = StringTrimming.Character
                        });
                       
                    }

                    using(var context = Graphics.FromImage(b))
                    {
                        context.DrawImage(bx,  
                            new Rectangle(0,0,CharWidth, CharHeight), 
                            new Rectangle(8, 3, CharWidth-16 , CharHeight-6 ), 
                            GraphicsUnit.Pixel);
                    }
                }


                _fontCache[new Tuple<Color, char>(color, c).GetHashCode()] = b;
            }
            return b;
        }

        /// <summary>
        /// Put a character into the buffer at a given point with a given color.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        /// <param name="c"></param>
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
                            var bmp = GetCharBitmap(_bufferColors[i], _buffer[i]);
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
