using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicroFE
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            TreeNode root = null;
            try
            {
                root = ConfigFileParser.ParseConfigFile("config.json");

            }
            catch { }


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

            var theme = ConfigFileParser.ParseMenuTheme("config.json");

            using (var viewWindow = new VideoWindow(root, theme)
            {
                Title = "MicroFE",
                WindowBorder = OpenTK.WindowBorder.Hidden,
                Bounds = Screen.PrimaryScreen.Bounds

            })
            {
                viewWindow.Run();
            }
        }
    }
}
