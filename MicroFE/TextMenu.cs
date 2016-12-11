using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroFE
{
    public class TextMenu
    {
        const char UpperLeftCorner = '╓';
        const char UpperRightCorner = '╖';
        const char LowerLeftCorner = '╙';
        const char LowerRightCorner = '╜';
        const char LeftBorder = '║';
        const char RightBorder = '║';
        const char TopBorder = '─';
        const char BottomBorder = '─';

        const char LeftDivider = '╟';
        const char RighDivider = '╢';

        public string Title { get; set; }

        public string[] Items { get; set; }
        public int SelectedIndex { get; set; }

        public Color TitleColor { get; set; }

        public Color BorderColor { get; set; }
        public Color BackgroundColor { get; set; }

        public Color TextColor { get; set; }

        public Color SelectedTextColor { get; set; }
        public Color SelectedTextBackgroundColor { get; set; }

        public TextBuffer TextBuffer { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public TextMenu(TextBuffer buffer, int x, int y, int w, int h, string title)
        {
            BackgroundColor = Color.Black;
            BorderColor = Color.DarkGreen;
            SelectedTextBackgroundColor = Color.LightGreen;
            SelectedTextColor = Color.Black;
            TextColor = Color.LightGreen;
            TitleColor = Color.Green;

            X = x;
            Y = y;

            Width = w;
            Height = h;

            Title = title;

            TextBuffer = buffer;
        }

        public void Draw()
        {
            TextBuffer.PaintBackground(X, Y, Width, Height, BackgroundColor);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    char cx;
                    Color bgColor;
                    Color fgColor;

                    // 4 corners
                    if (x == 0 && y == 0) { cx = UpperLeftCorner; fgColor = BorderColor; bgColor = BackgroundColor; }
                    else if (x == Width - 1 && y == 0) { cx = UpperRightCorner; fgColor = BorderColor; bgColor = BackgroundColor; }
                    else if (x == 0 && y == Height - 1) { cx = LowerLeftCorner; fgColor = BorderColor; bgColor = BackgroundColor; }
                    else if (x == Width - 1 && y == Height - 1) { cx = LowerRightCorner; fgColor = BorderColor; bgColor = BackgroundColor; }

                    // title divider
                    else if (x == 0 && y == 2) { cx = LeftDivider; fgColor = BorderColor; bgColor = BackgroundColor; }
                    else if (x == Width - 1 && y == 2) { cx = RighDivider; fgColor = BorderColor; bgColor = BackgroundColor; }
                    else if (y == 2) { cx = BottomBorder; fgColor = BorderColor; bgColor = BackgroundColor; }

                    else if (x == 0) { cx = LeftBorder; fgColor = BorderColor; bgColor = BackgroundColor; }
                    else if (x == Width - 1) { cx = RightBorder; fgColor = BorderColor; bgColor = BackgroundColor; }

                    else if (y == 0) { cx = TopBorder; fgColor = BorderColor; bgColor = BackgroundColor; }
                    else if (y == Height - 1) { cx = BottomBorder; fgColor = BorderColor; bgColor = BackgroundColor; }
                    else { cx = ' '; fgColor = BorderColor; bgColor = BackgroundColor; }

                    TextBuffer.PutChar(X + x, Y + y, BorderColor, cx);
                }
            }

            // Put the title text
            TextBuffer.PutString(X + 1, Y + 1, TitleColor, Title);

            int availableSlots = Math.Min(Items.Count(), this.Height - 4); // top line + border line + title text + bottom line.

            int mx = SelectedIndex - availableSlots / 2;
            if (mx < 0) mx = 0;
            if (mx >= Items.Count()-availableSlots) mx = Items.Count()-availableSlots;


            for (int i = 0; i < availableSlots; i++)
            {
                int cursorX = X + 1;
                int cursorY = Y + 3 + i;


                if (mx == SelectedIndex)
                {
                    TextBuffer.PaintBackground(cursorX, cursorY, Width - 2, 1, SelectedTextBackgroundColor);
                    TextBuffer.PutString(cursorX, cursorY, SelectedTextColor, Items[mx]);
                }
                else
                {
                    TextBuffer.PutString(cursorX, cursorY, TextColor, Items[mx]);
                }
                mx++;

            }



            //int minMenuItem = Math.Max(0, SelectedIndex - availableSlots ); // 
            //int maxMenuItem = Math.Min(Items.Count(), minMenuItem + availableSlots-1);

            //for (int i = minMenuItem; i < maxMenuItem; i++)
            //{
            //    int cursorX = X+1;
            //    int cursorY = Y+3 + (i - minMenuItem);

            //    if (i == SelectedIndex)
            //    {
            //        TextBuffer.PaintBackground(cursorX, cursorY, Width - 2, 1, SelectedTextBackgroundColor);
            //        TextBuffer.PutString(cursorX, cursorY, SelectedTextColor,  Items[i]);
            //    }
            //    else
            //    {
            //        TextBuffer.PutString(cursorX, cursorY, TextColor, Items[i]);
            //    }

            //}

        }

        public void MoveCursorUp() { if (SelectedIndex > 0) SelectedIndex--; }
        public void MoveCursorDown() { if (SelectedIndex < Items.Count() - 1) SelectedIndex++; }


    }
}
