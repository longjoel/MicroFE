using System;
using System.Collections.Generic;

namespace MicroFE
{
    public class TreeNode:Dictionary<string, TreeNode>
    {
        public Action OnSelect { get; set; }
    }
}
