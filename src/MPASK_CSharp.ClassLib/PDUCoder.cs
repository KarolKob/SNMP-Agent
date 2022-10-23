using System;
using System.Collections.Generic;
using System.Linq;

namespace MPASK_CSharp.ClassLib
{
    public enum RequestID { GetRequest, GetNextRequest, GetResponse, SetRequest }
    public class PDUCoder
    {
        public static byte[] Encode(RequestID requestID, Dictionary<ValueObject, ValueObject> varBindList, int errorStatus = 0, int errorIndex = 0)
        {
            byte[] encodedPDU = null;

            // Add the proper SNMP PDU fields before content
            byte[] reqIDBytes = BEREncoder.Encode("INTEGER", (int)requestID + 1);
            byte[] errorStatusBytes = BEREncoder.Encode("INTEGER", errorStatus);
            byte[] errorIndexBytes = BEREncoder.Encode("INTEGER", errorIndex);
            encodedPDU = CombineByteArrays(reqIDBytes, errorStatusBytes, errorIndexBytes);

            // Count the initial array length to add it after adding content bytes
            int initLen = reqIDBytes.Length + errorStatusBytes.Length + errorIndexBytes.Length;

            List<byte> tempList = new List<byte>();

            // Encode iterating through the list
            foreach(KeyValuePair<ValueObject, ValueObject> vals in varBindList)
            {
                byte[] oidBytes = BEREncoder.EncodeObjectIdentifer(vals.Key.valOid);
                byte[] valBytes = null;

                if (vals.Value.valType != "SEQUENCE")
                {
                    valBytes = CombineByteArrays(oidBytes, BEREncoder.Encode(vals.Value.valType, vals.Value.valInt, vals.Value.valBool, vals.Value.valStr));
                }
                else
                {
                    valBytes = CombineByteArrays(oidBytes, BEREncoder.EncodeSequence(vals.Value.valSeq, vals.Value.valSeqName));
                }

                // Count the size and put the sequence fields before encoded data
                byte[] len = BEREncoder.CountLength(valBytes);
                tempList.Add(0x30);
                tempList.AddRange(len);
                tempList.AddRange(valBytes);
            }

            // Put the elements on their place
            byte[] contentBytes = tempList.ToArray();
            byte[] partialLen = BEREncoder.CountLength(contentBytes);
            contentBytes = CombineByteArrays(encodedPDU, new byte[]{ 0x30 }, partialLen, contentBytes);

            byte[] totalLen = BEREncoder.CountLength(contentBytes);
            encodedPDU = CombineByteArrays(new byte[]{ GetTag(requestID) }, totalLen, contentBytes);

            return encodedPDU;
        }

        private static byte GetTag(RequestID requestID)
        {
            switch ((int)requestID)
            {
                case 0:
                    return 0xA0;
                case 1:
                    return 0xA1;
                case 2:
                    return 0xA2;
                case 3:
                    return 0xA3;
                default:
                    return 0x00; // Should not happen
            }
        }

        private static byte[] CombineByteArrays(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays) {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }
    }
}