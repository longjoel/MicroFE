using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MicroFE
{
    static class Program
    {
        public static Process RunningEmulator { get; set; }

        public static Settings Settings { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            TreeNode root = null;
            MenuTheme theme = null;

            if (System.IO.File.Exists("config.json"))
            {

                var settingsConfig = JsonConvert.DeserializeObject<MicroFEConfig>(System.IO.File.ReadAllText("config.json"));

                Settings = new Settings() { QuitCombo = "L3+R3" };
                root = ConfigFileParser.ParseConfigFile(settingsConfig);
                theme = settingsConfig.Theme ?? new MenuTheme();
            }

            else
            {
                root = new TreeNode()
                {
                    ["config.json not found. See setup.html for help."] = new TreeNode()
                    {
                        OnSelect = new Action(() => { System.Diagnostics.Process.Start("Setup.html"); })
                    },

                    ["or go to https://github.com/longjoel/MicroFE"] = new TreeNode()
                    {
                        OnSelect = new Action(() => { System.Diagnostics.Process.Start("https://github.com/longjoel/MicroFE"); })
                    },

                    ["(Quit)"] = new TreeNode()
                    {
                        OnSelect = new Action(() => { Environment.Exit(0); })
                    },
                };
            }


            if (root == null)
            {
                root = new TreeNode()
                {
                    ["Unable to parse config.json or no actions available. Press 'right' to exit."] = new TreeNode()
                    {
                        OnSelect = new Action(() => { Environment.Exit(0); })
                    }
                };
            }



            using (var viewWindow = new VideoWindow(root, theme)
            {
                Title = "MicroFE",
                //WindowBorder = OpenTK.WindowBorder.Hidden,
                Bounds = Screen.PrimaryScreen.Bounds
            })
            {
                viewWindow.Run(15, 60);
            }
        }
    }
}
