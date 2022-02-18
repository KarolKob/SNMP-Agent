using System;
using System.Collections.Generic;

namespace MPASK_CSharp.ClassLib
{
    public class ValueObject
    {
        public string valType;
        public uint[] valOid;
        public Int64 valInt; 
        public bool valBool; 
        public string valStr;
        public Dictionary<string, string> valSeq;
        public string valSeqName;

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