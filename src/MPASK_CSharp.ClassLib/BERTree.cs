using System;
using System.Collections.Generic;

namespace MPASK_CSharp.ClassLib
{
    public enum TagClass { Universal, Application, ContentSpecific, Private }
    public class BERTree
    {
        public string dataType;
        public byte[] value;
        public List<BERTree> subTree;
        public bool isConstructed;
        public TagClass access;

        // TODO: convert bytes to ValueObject class and delete value and dataType fields
        public BERTree(string type, bool constructed, byte[] val = null, List<BERTree> tree = null, TagClass acc = TagClass.Universal)
        {
            dataType = type;
            isConstructed = constructed;
            value = val;
            subTree = tree;
            access = acc;
        }
    }
}