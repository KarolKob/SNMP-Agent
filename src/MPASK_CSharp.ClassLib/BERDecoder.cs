using System;
using System.Linq;
using System.Collections.Generic;

namespace MPASK_CSharp.ClassLib
{
    public class BERDecoder
    {
        public static BERTree Decode(byte[] encodedBytes)
        {
            BERTree decodedTree = null;
            byte tag = encodedBytes[0];

            switch (tag)
            {
                // BOOLEAN
                case 0x01:
                    decodedTree = new BERTree("BOOLEAN", false, new byte[] { encodedBytes[2] });
                    break;

                // INTEGER
                case 0x02:
                    if (BEREncoder.GetBit(encodedBytes[1], 7) == false)
                    {
                        decodedTree = new BERTree("INTEGER", false, encodedBytes.Skip(2).Take(encodedBytes[1]).ToArray());
                    }
                    else
                    {
                        int[] length = DecodeLength(encodedBytes);
                        decodedTree = new BERTree("INTEGER", false, encodedBytes.Skip(length[0] + 2).Take(length[1]).ToArray());
                    }
                    break;

                // OCTET STRING
                case 0x04:
                    if (BEREncoder.GetBit(encodedBytes[1], 7) == false)
                    {
                        decodedTree = new BERTree("STRING", false, encodedBytes.Skip(2).Take(encodedBytes[1]).ToArray());
                    }
                    else
                    {
                        int[] length = DecodeLength(encodedBytes);
                        decodedTree = new BERTree("STRING", false, encodedBytes.Skip(length[0] + 2).Take(length[1]).ToArray());
                    }
                    break;

                // NULL
                case 0x05:
                    decodedTree = new BERTree("NULL", false);
                    break;

                // OBJECT IDENTIFIER
                case 0x06:
                    if (BEREncoder.GetBit(encodedBytes[1], 7) == false)
                    {
                        decodedTree = new BERTree("OBJECT IDENTIFIER", false, encodedBytes.Skip(2).Take(encodedBytes[1]).ToArray());
                    }
                    else
                    {
                        int[] length = DecodeLength(encodedBytes);
                        decodedTree = new BERTree("OBJECT IDENTIFIER", false, encodedBytes.Skip(length[0] + 2).Take(length[1]).ToArray());
                    }
                    break;

                // SEQUENCE
                case 0x10:
                    if (BEREncoder.GetBit(encodedBytes[1], 7) == false)
                    {
                        decodedTree = DecodeSequence(encodedBytes, encodedBytes[1], 1);
                    }
                    else
                    {
                        int[] length = DecodeLength(encodedBytes);
                        decodedTree = DecodeSequence(encodedBytes, length[1], length[0]);
                    }
                    break;

                default:
                    break;
            }

            return decodedTree;
        }

        // Array - 0 for number of bytes encoding length, 1 for length of the content
        public static int[] DecodeLength(byte[] encoded)
        {
            byte lenLen = encoded[1];

            if (BEREncoder.GetBit(lenLen, 7))
            {
                BEREncoder.SetBit(ref lenLen, 7, false);

                byte[] intBytes = new byte[lenLen];

                for (int i = 0; i < lenLen; i++)
                {
                    intBytes[lenLen - 1 - i] = encoded[i + 2];
                }
                return new int[] { lenLen, BitConverter.ToInt32(intBytes, 0) };
            }
            else
            {
                return new int[] { 0, lenLen };
            }
        }

        private static BERTree DecodeSequence(byte[] encodedBytes, int length, int lenByteCount)
        {
            BERTree decoded;
            List<BERTree> treeList = new List<BERTree>();
            int offsetInd = 2;

            if (lenByteCount > 1)
            {
                offsetInd += lenByteCount;
            }

            while (offsetInd < length)
            {
                if (BEREncoder.GetBit(encodedBytes[offsetInd + 1], 7) == false)
                {
                    treeList.Add(Decode(encodedBytes.Skip(offsetInd).Take(encodedBytes[offsetInd + 1] + 2).ToArray()));
                    offsetInd = offsetInd + encodedBytes[offsetInd + 1] + 2;
                }
                else
                {
                    int[] len = DecodeLength(encodedBytes.Skip(offsetInd).ToArray());

                    // Take all bytes (incuding length)
                    treeList.Add(Decode(encodedBytes.Skip(offsetInd).Take(len[1] + len[0] + 2).ToArray()));
                    offsetInd = offsetInd + len[1] + len[0] + 2;
                }
            }

            decoded = new BERTree("SEQUENCE", true, tree: treeList);

            return decoded;
        }
    }
}