using System.Collections.Generic;

namespace MicroFE
{
    public class MicroFEAction
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public List<string> Args { get; set; }
        public string WorkingDirectory { get; set; }
    }

}
