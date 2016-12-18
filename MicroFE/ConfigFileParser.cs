﻿using System;
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
            if(File.Exists(configPath))
            {
                dynamic jsonRoot = JObject.Parse(File.ReadAllText(configPath));

                var theme = new MenuTheme()
                {
                    BackgroundColor = Color.FromName(Convert.ToString(jsonRoot?.Theme?.BackgroundColor) ?? "Black") ,
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

                ParseEmulators(root, jsonRoot);
                ParseActions(root, jsonRoot);

                //Quit should always be the last option.
                root["Quit"] = new TreeNode() { OnSelect = new Action(() => { Environment.Exit(0); }) };

                return root;
            }
            return null;
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
                    if (File.Exists((string)action.Path))
                    {
                        actionNode[(string)action.Name] = new TreeNode()
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
                        actionNode[(string)action.Name] = new TreeNode();
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

                                Program.RunningEmulator = Process.Start(startInfo);
                            });


                            root[(string)emulator.System][Path.GetFileNameWithoutExtension(r)] = n;
                        }
                    }


                }
            }
        }
    }
}
