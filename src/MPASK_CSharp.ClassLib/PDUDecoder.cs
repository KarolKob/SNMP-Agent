using System;
using System.Collections.Generic;

namespace MPASK_CSharp.ClassLib
{
    public class PDUDecoder
    {
        public static Dictionary<ValueObject, ValueObject> Decode(byte[] encodedPDU)
        {
            Dictionary<ValueObject, ValueObject> decodedValues = new Dictionary<ValueObject, ValueObject>();

            int[] allLen = BERDecoder.DecodeLength(encodedPDU);

            return decodedValues;
        }

        public static RequestID GetRequestID(byte[] encodedPDU)
        {
            switch (encodedPDU[0])
            {
                case 0xA0:
                    return RequestID.GetRequest;
                case 0xA1:
                    return RequestID.GetNextRequest;
                case 0xA2:
                    return RequestID.GetResponse;
                case 0xA3:
                    return RequestID.SetRequest;
                default:
                    return RequestID.GetRequest;
            }
        }
    }
}