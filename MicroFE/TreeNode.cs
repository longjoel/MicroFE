using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroFE
{
    public class TreeNode:Dictionary<string, TreeNode>
    {
        public Action OnSelect { get; set; }
    }
}
