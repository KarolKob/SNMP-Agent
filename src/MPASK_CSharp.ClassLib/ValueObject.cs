using System;
using System.Collections.Generic;

namespace MPASK_CSharp.ClassLib
{
    public class ValueObject
    {
        public string valType { get; set; }
        public uint[] valOid { get; set; }
        public Int64 valInt { get; set; } 
        public bool valBool { get; set; }
        public string valStr { get; set; }
        public Dictionary<string, string> valSeq { get; set; }
        public string valSeqName { get; set; }

        public ValueObject(string valType, uint[] valOid = null, Int64 valInt = 0, bool valB = false, 
        string valStr = null, Dictionary<string, string> valSeq = null, string valSeqName = null)
        {
            this.valType = valType;
            this.valOid = valOid;
            this.valInt = valInt;
            this.valBool = valB;
            this.valStr = valStr;
            this.valSeq = valSeq;
            this.valSeqName = valSeqName;
        }
    }
}