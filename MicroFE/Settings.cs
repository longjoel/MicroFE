namespace MicroFE
{
    public class Settings
    {
        public string QuitCombo { get; set; }
        public bool FullScreen { get; set; }

        public Settings()
        {
            QuitCombo = "L3+R3";
            FullScreen = true;
        }

    }
}
