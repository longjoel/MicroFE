using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace MicroFE
{
    public class ConfigFileParser
    {
        public static TreeNode ParseConfigFile(string configPath)
        {
            var root = new TreeNode() { };



            dynamic jsonRoot = JObject.Parse(File.ReadAllText(configPath));

            ParseEmulators(root, jsonRoot);

            ParseActions(root, jsonRoot);

            root["Quit"] = new TreeNode() { OnSelect = new Action(() => { Application.Exit(); }) };

            return root;

        }

        private static void ParseActions(TreeNode root, dynamic jsonRoot)
        {
            root["Actions"] = new TreeNode();

            // parse out Actions
            if (jsonRoot.Actions != null)
            {
                foreach (var action in jsonRoot.Actions)
                {
                    var actionNode = root["Actions"];
                    actionNode[(string)action.Name] = new TreeNode()
                    {
                        OnSelect = new Action(() =>
                        {
                            var startInfo = new ProcessStartInfo()
                            {
                                Arguments = string.Join(" ", action.Args?? ""),
                                FileName = action.Path,
                                WorkingDirectory = action.WorkingDirectory
                            };
                            Process.Start(startInfo);
                        })
                    };
                }
            }
        }

        private static void ParseEmulators(TreeNode root, dynamic jsonRoot)
        {
            if (jsonRoot.Emulators != null)
            {

                foreach (var emulator in jsonRoot.Emulators)
                {
                    root[(string)emulator.System] = new TreeNode() { };

                    var roms = Directory.GetFiles((string)emulator.RomPath);

                    foreach (var r in roms)
                    {
                        var n = new TreeNode();

                        n.OnSelect = new Action(() =>
                        {
                            var startInfo = new ProcessStartInfo()
                            {
                                Arguments = string.Join(" ", emulator.EmuArgs),
                                FileName = "\"" + emulator.EmuPath + "\"",
                                WorkingDirectory = "\"" + emulator.WorkingDirectory + "\""
                            };

                            startInfo.Arguments = startInfo.Arguments.Replace("%ROM%", "\"" + r + "\"");

                            Process.Start(startInfo);
                        });


                        root[(string)emulator.System][Path.GetFileNameWithoutExtension(r)] = n;
                    }


                }
            }
        }
    }
}
