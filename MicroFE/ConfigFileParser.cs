using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace MicroFE
{
    /// <summary>
    /// A simple config file parser.
    /// </summary>
    public class ConfigFileParser
    {
        /// Parse a given config file. Parse the emulators first, then the actions.
        public static TreeNode ParseConfigFile(MicroFEConfig config)
        {
            var root = new TreeNode();

            if (config?.Emulators?.Any() ?? false)
            {
                ParsePlaylists(root, config.Emulators);
                ParseEmulators(root, config.Emulators);
            }

            if (config?.Actions?.Any() ?? false)
            {
                ParseActions(root, config.Actions);
            }

            // Quit should always be the last option.
            root["Quit"] = new TreeNode() { OnSelect = new Action(() => { Environment.Exit(0); }) };
            root[" "] = null;

            // Make the documentation available from inside the program
            root[" Project Page"] = new TreeNode()
            {
                OnSelect = new Action(() => { Process.Start("https://github.com/longjoel/MicroFE"); })
            };
            return root;
        }

        private static void ParseActions(TreeNode root, List<MicroFEAction> actions)
        {
            if (actions.Any())
            {
                root["--Actions--"] = null;
            }

            foreach (var action in actions)
            {
                if (File.Exists(action.Path))
                {
                    root[action.Name] = new TreeNode()
                    {
                        OnSelect = new Action(() =>
                        {
                            var startInfo = new ProcessStartInfo()
                            {
                                Arguments = string.Join(" ", action.Args ?? new List<string> { "" }),
                                FileName = "\"" + Path.GetFullPath(action.Path) + "\"",
                                WorkingDirectory = "\"" + Path.GetFullPath(action?.WorkingDirectory ?? Path.GetDirectoryName(action.Path)) + "\""
                            };

                            try
                            {
                                Program.RunningEmulator = Process.Start(startInfo);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(startInfo.FileName + ex.Message);
                            }
                        })
                    };
                }
                else
                {
                    root[action.Name] = new TreeNode();
                }
            }
        }

        private static void ParseEmulators(TreeNode root, List<Emulator> emulators)
        {

            foreach (Emulator emulator in emulators)
            {
                root["--Emulators--"] = null;

                root[emulator.System] = new TreeNode() { };
                if (Directory.Exists(Path.GetFullPath(emulator.RomPath)))
                {
                    AddGames(root, emulator);
                }
            }
        }

        private static void ParsePlaylists(TreeNode root, List<Emulator> emulators)
        {
            foreach (var emulator in emulators)
            {
                if (Directory.Exists(emulator.RomPath))
                {
                    AddPlaylists(root, emulator);
                }
            }
        }

        private static void AddPlaylists(TreeNode root, Emulator emulator)
        {
            var playlists = Directory.GetFiles(emulator.RomPath.ToString(), "*.txt");

            // if there are any playlists found, add the playlist header.
            if (playlists.Any())
            {
                root["--Playlists--"] = null;
            }

            foreach (var pls in playlists)
            {
                var newNode = new TreeNode();
                root[Path.GetFileNameWithoutExtension(pls)] = newNode;

                List<string> playlistFiles = new List<string>();
                foreach (var f in File.ReadAllLines(pls)) { playlistFiles.Add(f); }
                var roms = playlistFiles.Select(x => Path.Combine(emulator.RomPath, x)).ToList();

                foreach (var r in roms)
                {
                    var n = new TreeNode();

                    n.OnSelect = new Action(() =>
                    {
                        var startInfo = new ProcessStartInfo()
                        {
                            Arguments = string.Join(" ", emulator.EmuArgs),
                            FileName = "\"" + Path.GetFullPath(emulator.EmuPath) + "\"",
                            WorkingDirectory = "\"" + Path.GetFullPath(emulator.WorkingDirectory) + "\""
                        };

                        startInfo.Arguments = startInfo.Arguments.Replace("%ROM%", "\"" + Path.GetFullPath(r) + "\"");

                        try
                        {
                            Program.RunningEmulator = Process.Start(startInfo);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(startInfo.FileName + ex.Message);
                        }
                    });
                    newNode[Path.GetFileNameWithoutExtension(r)] = n;
                }
            }
        }

        private static void AddGames(TreeNode root, Emulator emulator)
        {
            var allFiles = Directory.GetFiles(emulator.RomPath);
            var roms = emulator.RomFilter.SelectMany(filter => allFiles.Where(rom => rom.Contains(filter)));

            foreach (var r in roms)
            {
                var n = new TreeNode();
                n.OnSelect = new Action(() =>
                {
                    var startInfo = new ProcessStartInfo()
                    {
                        Arguments = string.Join(" ", emulator.EmuArgs),
                        FileName = "\"" + Path.GetFullPath(emulator.EmuPath) + "\"",
                        WorkingDirectory = "\"" + Path.GetFullPath(emulator.WorkingDirectory) + "\""
                    };

                    startInfo.Arguments = startInfo.Arguments.Replace("%ROM%", "\"" + Path.GetFullPath(r) + "\"");

                    try
                    {
                        Program.RunningEmulator = Process.Start(startInfo);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(startInfo.FileName + ex.Message);
                    }
                });

                root[emulator.System][Path.GetFileNameWithoutExtension(r)] = n;
            }
        }
    }
}
