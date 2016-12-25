namespace MicroFE
{

    public class Emulator
    {
        public string System { get; set; }
        public string WorkingDirectory { get; set; }
        public string EmuPath { get; set; }
        public string[] EmuArgs { get; set; }
        public string RomPath { get; set; }
        public string[] RomFilter { get; set; }
    }

}
