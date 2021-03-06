﻿using Newtonsoft.Json;
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
                try
                {
                    var settingsConfig = JsonConvert.DeserializeObject<MicroFEConfig>(System.IO.File.ReadAllText("config.json"));

                    Settings = settingsConfig.Settings;
                    root = ConfigFileParser.ParseConfigFile(settingsConfig);
                    theme = settingsConfig.Theme ?? new MenuTheme();
                }
                catch
                {
                    Settings = new Settings();
                    root = new TreeNode()
                    {


                        ["config.json is not a valid json document. github.com/longjoel/MicroFE"] = new TreeNode()
                        {
                            OnSelect = new Action(() => { Process.Start("https://github.com/longjoel/MicroFE"); })
                        },

                        ["(Quit)"] = new TreeNode()
                        {
                            OnSelect = new Action(() => { Environment.Exit(0); })
                        },
                    };
                }
            }

            else
            {
                Settings = new Settings();
                root = new TreeNode()
                {


                    ["config.json not found. github.com/longjoel/MicroFE"] = new TreeNode()
                    {
                        OnSelect = new Action(() => { Process.Start("https://github.com/longjoel/MicroFE"); })
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
                WindowBorder = Settings.FullScreen ? OpenTK.WindowBorder.Hidden : OpenTK.WindowBorder.Resizable,
                Bounds = Screen.PrimaryScreen.Bounds
            })
            {
                viewWindow.Run(15, 60);
            }
        }
    }
}
