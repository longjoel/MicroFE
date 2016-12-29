using System.Collections.Generic;

namespace MicroFE
{

    public class MicroFEConfig
    {
        public Settings Settings { get; set; }
        public MenuTheme Theme { get; set; }
        public List<MicroFEAction> Actions { get; set; }
        public List<Emulator> Emulators { get; set; }

        public MicroFEConfig()
        {
            Settings = new Settings();
            Theme = new MenuTheme();
            Actions = new List<MicroFEAction>();
            Emulators = new List<Emulator>();
        }
    }
}
