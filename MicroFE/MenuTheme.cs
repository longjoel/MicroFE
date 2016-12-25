using System.Drawing;

namespace MicroFE
{
    /// <summary>
    /// 
    /// </summary>
    public class MenuTheme
    {
        public Color TitleColor { get; set; }

        public Color BorderColor { get; set; }
        public Color BackgroundColor { get; set; }

        public Color TextColor { get; set; }

        public Color SelectedTextColor { get; set; }
        public Color SelectedTextBackgroundColor { get; set; }

        public MenuTheme()
        {
            TitleColor = Color.Green;
            BorderColor = Color.Green;
            BackgroundColor = Color.Black;
            TextColor = Color.Green;
            SelectedTextColor = Color.Black;
            SelectedTextBackgroundColor = Color.Green;
        }
    }
}
