using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MPASK_CSharp.ClassLib
{
    public class MIBTree
    {
        private TreeNode currentNode;

        // A dictionary with leafs of the node
        private Dictionary<int, MIBTree> idDict = new Dictionary<int, MIBTree>();

        // A list of data types used inside object types
        public static List<DataType> dataTypes = new List<DataType>();

        public MIBTree(string name, int id)
        {
            currentNode = new TreeNode(name, id);
        }

        public MIBTree(TreeNode node)
        {
            currentNode = node;
        }

        public bool InsertEntry(string rootName, TreeNode entry)
        {
            if(currentNode.nodeName.Equals(rootName))
            {
                if (idDict.ContainsKey(entry.id))
                {
                    idDict[entry.id] = new MIBTree(entry);
                }
                else
                {
                    idDict.Add(entry.id, new MIBTree(entry));
                }
                return true;
            }
            else if(idDict.Count > 0)
            {
                if (this.IterateNodes(rootName, entry))
                {
                    return true;
                }

                foreach(KeyValuePair<int, MIBTree> iNode in idDict)
                {
                    if(iNode.Value.InsertEntry(rootName, entry))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IterateNodes(string rootName, TreeNode entry)
        {
            foreach(KeyValuePair<int, MIBTree> iNode in idDict)
            {
                if(iNode.Value.currentNode.Equals(entry))
                {
                    return true;
                }
                else if(iNode.Value.currentNode.nodeName.Equals(rootName))
                {
                    if (!iNode.Value.idDict.ContainsKey(entry.id))
                    {
                        iNode.Value.idDict.Add(entry.id, new MIBTree(entry));
                    }
                    else
                    {
                        iNode.Value.idDict[entry.id] = new MIBTree(entry);
                    }
                    
                    return true;
                }
            }
            return false;
        }

        public TreeNode GetEntry(string address)
        {
            string[] splitStr = address.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if(Regex.IsMatch(address, @"[0-9.]+"))
            {
                List<int> ids = new List<int>();
                for(int i = 1; i < splitStr.Length; i++)
                {
                    ids.Add(int.Parse(splitStr[i]));
                }
                return GetEntryIntId(this, ids);
            }
            else
            {
                List<string> ids = new List<string>(splitStr);
                for(int i = 1; i < splitStr.Length; i++)
                {
                    ids.Add(splitStr[i]);
                }
                return GetEntryStrId(this, ids);
            }
        }

        private TreeNode GetEntryIntId(MIBTree tree, List<int> ids)
        {
            if (ids.Count > 1)
            {
                MIBTree next = tree.idDict[ids[0]];
                ids.Remove(ids[0]);
                return GetEntryIntId(next, ids);
            }
            else
            {
                return tree.idDict[ids[0]].currentNode;
            }
        }

        private TreeNode GetEntryStrId(MIBTree tree, List<string> ids)
        {
            if (ids.Count > 1)
            {
                foreach(KeyValuePair<int, MIBTree> it in tree.idDict)
                {
                    if(it.Value.currentNode.Equals(ids[0]))
                    {
                        ids.Remove(ids[0]);
                        return GetEntryStrId(it.Value, ids);
                    }
                }
                return null;
            }
            else
            {
                return tree.idDict[int.Parse(ids[0])].currentNode;
            }
        }
    }
}