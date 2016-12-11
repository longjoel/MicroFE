using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using System.IO;

namespace MicroFE
{
    public class ConfigFileParser
    {
        public static TreeNode ParseConfigFile(string configPath)
        {
            var root = new TreeNode() { };

            dynamic jsonRoot = Newtonsoft.Json.JsonConvert.DeserializeObject(configPath);

            // parse out utilitieFolder
            if(jsonRoot.ScriptsDirectory != null)
            {
                foreach(var s in System.IO.Directory.GetFiles(jsonRoot.ScriptsDirectory))
                {
                    root[Path.GetFileNameWithoutExtension(s)] = new TreeNode() { OnSelect = new Action(() => { System.Diagnostics.Process.Start(s); }) };
                }
            }



            return root;
           
        }
    }
}
