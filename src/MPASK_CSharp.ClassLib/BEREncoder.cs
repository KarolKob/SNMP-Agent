using System;
using System.Collections.Generic;
using System.Text;

namespace MPASK_CSharp.ClassLib
{
    public class BEREncoder
    {
        public static byte[] Encode(string type, Int64 valInt = 0, bool valB = false, string valStr = null)
        {
            byte[] encodedBytes;
            byte id;
            byte length;

            switch (type)
            {
                case "BOOLEAN":
                    id = 0x01;
                    length = 0x01;
                    encodedBytes = new byte[3];
                    encodedBytes[0] = id;
                    encodedBytes[1] = length;

                    if (valB)
                    {
                        encodedBytes[2] = 0xFF;
                    }
                    else
                    {
                        encodedBytes[2] = 0x00;
                    }
                    return encodedBytes;

                case "STRING":
                    id = 0x04;
                    byte[] strBytes = Encoding.ASCII.GetBytes(valStr);
                    byte[] strLen = CountLength(strBytes);

                    encodedBytes = new byte[1 + strBytes.Length + strLen.Length];
                    encodedBytes[0] = id;

                    for (int i = 0; i < strLen.Length; i++)
                    {
                        encodedBytes[i + 1] = strLen[i];
                    }
                    for (int i = 0; i < strBytes.Length; i++)
                    {
                        encodedBytes[i + 1 + strLen.Length] = strBytes[i];
                    }
                    return encodedBytes;

                case "INTEGER":

                    id = 0x02;
                    byte[] bytes = BitConverter.GetBytes(valInt);
                    byte[] trimmedBytes;
                    int numBytes = 0;

                    if (valInt == 0)
                    {
                        numBytes = 1;
                    }
                    else
                    {
                        foreach (byte b in bytes)
                        {
                            if (!(b == 0xFF || b == 0x00))
                            {
                                numBytes++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    trimmedBytes = new byte[numBytes];
                    length = (byte)numBytes;

                    for (int i = 0; i < numBytes; i++)
                    {
                        trimmedBytes[i] = bytes[i];
                    }
                    encodedBytes = new byte[2 + numBytes];
                    encodedBytes[0] = id;
                    encodedBytes[1] = length;

                    Array.Reverse(trimmedBytes);

                    for (int i = 0; i < numBytes; i++)
                    {
                        encodedBytes[i + 2] = trimmedBytes[i];
                    }
                    return encodedBytes;

                case "NULL":
                    id = 0x05;
                    length = 0x00;
                    encodedBytes = new byte[2];
                    encodedBytes[0] = id;
                    encodedBytes[1] = length;
                    return encodedBytes;

                default:
                break;
            }

            return null;
        }

        public static byte[] EncodeSequence(Dictionary<string, string> sequenceDict, string sequenceName = null)
        {
            byte[] encodedBytes;
            List<byte> tempList = new List<byte>();

            if (sequenceName != null && Parser.sequenceDict.ContainsKey(sequenceName))
            {
                foreach(KeyValuePair<string, string> type in Parser.sequenceDict[sequenceName])
                {
                    byte[] typeEn = SequencePickType(type.Value, sequenceDict[type.Key]);

                    if (typeEn == null)
                    {
                        SequenceFindNested(ref tempList, type);
                    }
                    else
                    {
                        tempList.AddRange(typeEn);
                    }
                }
            }
            else
            {
                foreach(KeyValuePair<string, string> entry in sequenceDict)
                {
                    SequenceFindNested(ref tempList, entry);
                }
            }

            byte[] len = CountLength(tempList.ToArray());

            encodedBytes = new byte[tempList.Count + 1 + len.Length];
            encodedBytes[0] = 0x10;
            
            for (int i = 0; i < len.Length; i++)
            {
                encodedBytes[i + 1] = len[i];
            }

            for (int i = 0; i < tempList.Count; i++)
            {
                encodedBytes[len.Length + 1 + i] = tempList[i];
            }

            return encodedBytes;
        }

        private static void SequenceFindNested(ref List<byte> byteList, KeyValuePair<string, string> entry)
        {
            DataType dataType = null;
            foreach(DataType type in MIBTree.dataTypes)
            {
                if (type.name.Equals(entry.Key))
                {
                    dataType = type;
                    break;
                }
            }
            if (!(dataType == null))
            {
                byte[] tempBytes = SequencePickType(dataType.type, entry.Value);
                
                if (!(tempBytes == null))
                {
                    byteList.AddRange(tempBytes);
                }
                else
                {
                    Console.WriteLine("SequencePickType returned NULL");
                }
            }
            else
            {
                foreach(KeyValuePair<string, Dictionary<string, string>> nestedDict in Parser.sequenceDict)
                {
                    if (nestedDict.Key == entry.Value)
                    {
                        // Nested type handling
                        byte[] tempBytes = EncodeSequence(nestedDict.Value);

                        if (!(tempBytes == null))
                        {
                            byteList.AddRange(tempBytes);
                        }
                        else
                        {
                            Console.WriteLine("SequencePickType returned NULL");
                        }

                        break;
                    }
                }
            }
        }

        private static byte[] SequencePickType(string type, string value)
        {
            switch (type)
            {
                case "BOOLEAN":
                return Encode(type, valB: bool.Parse(value));

                case "STRING":
                return Encode(type, valStr: value);

                case "OCTET STRING":
                return Encode("STRING", valStr: value);

                case "INTEGER":
                return Encode(type, Int64.Parse(value));

                case "NULL":
                return Encode(type);

                default:
                foreach(DataType dType in MIBTree.dataTypes)
                {
                    if (dType.name.Equals(type))
                    {
                        return SequencePickType(dType.type, value);
                    }
                }
                break;
            }
            return null;
        }

        public static byte[] EncodeObjectIdentifer(params uint[] ids)
        {
            byte[] encodedBytes;
            List<byte> tempList = new List<byte>();

            if (ids.Length > 1)
            {
                uint firstId = 40 * ids[0] + ids[1];
                byte[] bytes = BitConverter.GetBytes(firstId);

                if (firstId > 128)
                {
                    byte additionalByte = 0x00;

                    ProcessIDs(ref bytes, ref additionalByte);
                    
                    if (additionalByte != 0x00)
                    {
                        tempList.Add(additionalByte);
                    }                        
                    Array.Reverse(bytes);

                    foreach(byte b in bytes)
                    {
                        if(b != 0x00 && b != 0x80)
                            tempList.Add(b);
                    }
                }
                else
                {
                    SetBit(ref bytes[0], 7, false);
                    tempList.Add(bytes[0]);
                }

                for (int i = 2; i < ids.Length; i++)
                {
                    byte[] idBytes = BitConverter.GetBytes(ids[i]);

                    if (ids[i] > 128)
                    {
                        byte additionalByte = 0x00;

                        ProcessIDs(ref idBytes, ref additionalByte);

                        if (additionalByte != 0x00)
                        {
                            tempList.Add(additionalByte);
                        }                        
                        Array.Reverse(idBytes);

                        foreach(byte b in idBytes)
                        {
                            if(b != 0x00 && b != 0x80)
                                tempList.Add(b);
                        }
                    }
                    else
                    {
                        SetBit(ref idBytes[0], 7, false);
                        tempList.Add(idBytes[0]);
                    }
                }
            }
            else
            {
                byte[] idBytes = BitConverter.GetBytes(ids[0]);

                if (ids[0] > 128)
                {
                    byte additionalByte = 0x00;

                    ProcessIDs(ref idBytes, ref additionalByte);

                    if (additionalByte != 0x00)
                    {
                        tempList.Add(additionalByte);
                    }                        
                    Array.Reverse(idBytes);

                    foreach(byte b in idBytes)
                    {
                        if(b != 0x00 && b != 0x80)
                            tempList.Add(b);
                    }
                }
                else
                {
                    SetBit(ref idBytes[0], 7, false);
                    tempList.Add(idBytes[0]);
                }
            }

            encodedBytes = new byte[tempList.Count + 2];
            encodedBytes[0] = 0x06;
            encodedBytes[1] = (byte)tempList.Count;

            for (int i = 0; i < tempList.Count; i++)
            {
                encodedBytes[i + 2] = tempList[i];
            }

            return encodedBytes;
        }

        private static void ProcessIDs(ref byte[] bytes, ref byte additionalByte)
        {
            bool[] mem = null, mem1;

            for (int i = 0; i < bytes.Length; i++)
            {
                if (i != bytes.Length - 1)
                {
                    if (i == 0)
                    {
                        mem = new bool[i + 1];
                        mem[i] = GetBit(bytes[i], 7);    
                        SetBit(ref bytes[i], 7, false);
                    }
                    else
                    {
                        mem1 = new bool[i + 1];
                        for (int j = 0; j < i + 1; j++)
                        {
                            mem1[j] = GetBit(bytes[i], 7 - j);
                        }
                        bytes[i] = (byte)((bytes[i] << i) & 0xFF);

                        SetBit(ref bytes[i], 7, true);

                        for (int j = 0; j < i; j++)
                        {
                            SetBit(ref bytes[i], j, mem[j]);
                        }
                        mem = new bool[mem1.Length];
                        mem = mem1;
                    }
                }
                else
                {
                    if (BoolArrayToInt(mem) + bytes[i] > 128)
                    {
                        mem1 = new bool[i + 1];
                        for (int j = 0; j < i + 1; j++)
                        {
                            mem1[j] = GetBit(bytes[i], 7 - j);
                        }
                        bytes[i] = (byte)((bytes[i] << i) & 0xFF);

                        SetBit(ref bytes[i], 7, true);

                        for (int j = 0; j < i; j++)
                        {
                            SetBit(ref bytes[i], j, mem[j]);
                        }

                        SetBit(ref additionalByte, 7, true);

                        for (int j = 0; j < i + 1; j++)
                        {
                            SetBit(ref additionalByte, j, mem1[j]);
                        }
                    }
                    else
                    {
                        bytes[i] = (byte)((bytes[i] << i) & 0xFF);

                        SetBit(ref bytes[i], 7, true);

                        for (int j = 0; j < i; j++)
                        {
                            SetBit(ref bytes[i], j, mem[j]);
                        }
                    }
                }
            }
        }

        private static uint BoolArrayToInt(bool[] bits)
        {
            if(bits.Length > 32) throw new ArgumentException("Can only fit 32 bits in a uint");
            
            uint r = 0;
            for(int i = 0; i < bits.Length; i++) if(bits[i]) r |= (uint)(1 << (bits.Length - i));
            return r;
        }

        public static byte[] CountLength(byte[] data)
        {
            byte[] length;

            if (data.Length > 128)
            {
                if (data.Length > Math.Pow(2, 1008))
                {
                    return null;
                }
                else
                {
                    length = new byte[1 + DivRoundUp(data.Length, 8)];
                    length[0] = (byte)DivRoundUp(data.Length, 8);
                    SetBit(ref length[0], 7, true);

                    byte[] lenBytes = BitConverter.GetBytes(data.Length);
                    Array.Reverse(lenBytes);

                    for (int i = 0; i < lenBytes.Length; i++)
                    {
                        length[i + 1] = lenBytes[i];
                    }
                    return length;
                }
            }
            else
            {
                length = new byte[1];
                length[0] = (byte)data.Length;
                return length;
            }
        }

        private static int DivRoundUp(int dividend, int divisor)
{
            if (divisor == 0 ) 
                throw new DivideByZeroException();
            if (divisor == -1 && dividend == Int32.MinValue) 
                throw new OverflowException();
            int roundedTowardsZeroQuotient = dividend / divisor;
            bool dividedEvenly = (dividend % divisor) == 0;
            if (dividedEvenly) 
                return roundedTowardsZeroQuotient;
            bool wasRoundedDown = ((divisor > 0) == (dividend > 0));
            if (wasRoundedDown) 
                return roundedTowardsZeroQuotient + 1;
            else
                return roundedTowardsZeroQuotient;
        }

        public static void SetBit(ref byte aByte, int pos, bool value)
        {
            if (value)
            {
            // left-shift 1, then bitwise OR
            aByte = (byte)(aByte | (1 << pos));
            } else
            {
            // left-shift 1, then take complement, then bitwise AND
            aByte = (byte)(aByte & ~(1 << pos));
            }
        }
        
        public static bool GetBit(byte aByte, int pos)
        {
            // left-shift 1, then bitwise AND, then check for non-zero
            return ((aByte & (1 << pos)) != 0);
        }
    }
}