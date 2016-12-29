namespace MicroFE
{
    public class Settings
    {
        public string QuitCombo { get; set; }
        public bool FullScreen { get; set; }
        public int Rows { get; set; }
        public int Cols { get; set; }

        public Settings()
        {
            QuitCombo = "L3+R3";
            FullScreen = true;
            Rows = 45;
            Cols = 80;
        }

    }
}
