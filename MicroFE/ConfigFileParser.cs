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
using System.Drawing;
namespace MicroFE
{
    /// <summary>
    /// A simple config file parser.
    /// </summary>
    public class ConfigFileParser
    {
        public static Settings ParseSettings(string configPath)
        {
            var settings = new Settings();

            dynamic jsonRoot = JObject.Parse(File.ReadAllText(configPath));

            settings.QuitCombo = Convert.ToString(jsonRoot?.Settings?.QuitCombo) ?? "L3+R3";


            return settings;
        }

        /// <summary>
        /// Extract the theme overrides from a given config file.
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static MenuTheme ParseMenuTheme(string configPath)
        {
            if (File.Exists(configPath))
            {
                dynamic jsonRoot = JObject.Parse(File.ReadAllText(configPath));

                var theme = new MenuTheme()
                {
                    BackgroundColor = Color.FromName(Convert.ToString(jsonRoot?.Theme?.BackgroundColor) ?? "Black"),
                    SelectedTextBackgroundColor = Color.FromName(Convert.ToString(jsonRoot?.Theme?.SelectedTextBackgroundColor) ?? "Green"),
                    BorderColor = Color.FromName(Convert.ToString(jsonRoot?.Theme?.BorderColor) ?? "Green"),
                    SelectedTextColor = Color.FromName(Convert.ToString(jsonRoot?.Theme?.SelectedTextColor) ?? "Black"),
                    TextColor = Color.FromName(Convert.ToString(jsonRoot?.Theme?.TextColor) ?? "Green"),
                    TitleColor = Color.FromName(Convert.ToString(jsonRoot?.Theme?.TitleColor) ?? "Green"),
                };

                return theme;
            }

            return null;
        }

        // Parse a given config file. Parse the emulators first, then the actions.
        public static TreeNode ParseConfigFile(string configPath)
        {
            if (File.Exists(configPath))
            {

                var root = new TreeNode();
                dynamic jsonRoot = JObject.Parse(File.ReadAllText(configPath));

                root["--Playlists--"] = null;
                ParsePlaylists(root, jsonRoot);

                root["--Emulators--"] = null;
                ParseEmulators(root, jsonRoot);

                root["--Actions--"] = null;
                ParseActions(root, jsonRoot);

                //Quit should always be the last option.
                root["Quit"] = new TreeNode() { OnSelect = new Action(() => { Environment.Exit(0); }) };

                return root;
            }
            return null;
        }

        private static void ParseActions(TreeNode root, dynamic jsonRoot)
        {
            
            // parse out Actions
            if (jsonRoot.Actions != null)
            {
                foreach (var action in jsonRoot.Actions)
                {
                    if (File.Exists((string)action.Path))
                    {
                        root[(string)action.Name] = new TreeNode()
                        {
                            OnSelect = new Action(() =>
                            {
                                var startInfo = new ProcessStartInfo()
                                {
                                    Arguments = string.Join(" ", action.Args ?? ""),
                                    FileName = action.Path,
                                    WorkingDirectory = action.WorkingDirectory
                                };
                                Process.Start(startInfo);
                            })
                        };
                    }
                    else
                    {
                        root[(string)action.Name] = new TreeNode();
                    }
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
                    if (Directory.Exists((string)emulator.RomPath))
                    {
                        AddGames(root, emulator);

                    }

                }
            }
        }

        private static void ParsePlaylists(TreeNode root, dynamic jsonRoot)
        {
            if (jsonRoot.Emulators != null)
            {

                foreach (var emulator in jsonRoot.Emulators)
                {
                    
                    if (Directory.Exists((string)emulator.RomPath))
                    {
                        
                        AddPlaylists(root, emulator);
                    }

                }
            }
        }

        private static void AddPlaylists(TreeNode root, dynamic emulator)
        {
            var playlists = Directory.GetFiles(emulator.RomPath.ToString(), "*.txt");

            foreach (var pls in playlists)
            {
                var newNode = new TreeNode();
                root[Path.GetFileNameWithoutExtension(pls)] = newNode;

                List<string> playlistFiles = new List<string>();
                foreach (var f in File.ReadAllLines(pls)) { playlistFiles.Add(f); }


                var roms = playlistFiles.Select(x => Path.Combine((string)(emulator.RomPath), x)).ToList();

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

                        Program.RunningEmulator = Process.Start(startInfo);
                    });

                    newNode[Path.GetFileNameWithoutExtension(r)] = n;

                }

            }
        }

        private static void AddGames(TreeNode root, dynamic emulator)
        {
            var filters = new List<string>();

            foreach (var fx in emulator.RomFilter) filters.Add(fx.ToString());

            var allFiles = Directory.GetFiles((string)emulator.RomPath);

            var roms = filters.SelectMany(f => allFiles.Where(s => s.Contains(f)));

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

                    Program.RunningEmulator = Process.Start(startInfo);
                });


                root[(string)emulator.System][Path.GetFileNameWithoutExtension(r)] = n;
            }
        }
    }
}
