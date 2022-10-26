using System;

namespace MPASK_CSharp.ClassLib
{
    public class TreeNode
    {
        public string nodeName { get; set; }
        public int id { get; set; }
        
        public TreeNode(string name, int id)
        {
            nodeName = name;
            this.id = id;
        }

        override public string ToString()
        {
            return nodeName + "(" + id.ToString() + ")";
        }
    }
}