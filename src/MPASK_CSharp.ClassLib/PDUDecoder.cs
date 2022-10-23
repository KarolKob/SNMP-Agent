using System;
using System.Collections.Generic;
using System.Linq;

namespace MPASK_CSharp.ClassLib
{
    public class PDUDecoder
    {
        /// <summary>
        /// Decode the type of PDU, take action, get response.
        /// </summary>
        /// <param name="encodedPDU">
        /// Only the PDU, without SNMP message type data.
        /// </param>
        /// <returns></returns>
        public static byte[] DecodeAndExecute(byte[] encodedPDU)
        {
            byte[] response = null;
            int[] allLen = BERDecoder.DecodeLength(encodedPDU);
            byte requestID = encodedPDU[0];

            byte[] trimmedPDU = (byte[])encodedPDU.Skip(allLen[0] + 15).ToArray();

            Dictionary<ValueObject, ValueObject> decodedContent = DecodeContent(trimmedPDU);

            return response;
        }

        public static Dictionary<ValueObject, ValueObject> DecodeContent(byte[] trimmedPDU)
        {
            Dictionary<ValueObject, ValueObject> decodedValues = new Dictionary<ValueObject, ValueObject>();

            int[] allLen = BERDecoder.DecodeLength(trimmedPDU);
            byte[] oid = trimmedPDU.Take(allLen[1]).ToArray();
            BERTree oidVal = BERDecoder.Decode(oid);

            byte[] content = trimmedPDU.Skip(allLen[1] + 2).ToArray();
            BERTree contentVal = BERDecoder.Decode(content);

            return decodedValues;
        }
    }
}