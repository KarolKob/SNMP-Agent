using System;
using System.Collections.Generic;

namespace MPASK_CSharp.ClassLib
{
    public class PDUDecoder
    {
        public static Dictionary<ValueObject, ValueObject> DecodeContent(byte[] encodedPDU)
        {
            Dictionary<ValueObject, ValueObject> decodedValues = new Dictionary<ValueObject, ValueObject>();

            int[] allLen = BERDecoder.DecodeLength(encodedPDU);

            

            return decodedValues;
        }
    }
}