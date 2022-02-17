using System;
using MPASK_CSharp.ClassLib;
using System.Collections.Generic;

namespace MPASK_CSharp.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser mibParser = new Parser();
            MIBTree tree = new MIBTree("iso", 1);
            string text = System.IO.File.ReadAllText(@"RFC1213-MIB.txt");

            byte[] bytes = BitConverter.GetBytes(-129);
            Array.Reverse(bytes);
            Console.WriteLine("byte array: " + BitConverter.ToString(bytes));

            mibParser.ParseFile(text, ref tree);
            // Console.WriteLine("Output for: 1.3.6.1.2.1.3.1");
            // Console.WriteLine(tree.GetEntry("1.3.6.1.2.1.3.1").ToString());
            // Console.WriteLine("Output for: 1.3.6.1.2.1.1.1");
            // Console.WriteLine(tree.GetEntry("1.3.6.1.2.1.1.1").ToString());
            // Console.WriteLine("Output for: 1.3.6.1.2.1.1.3");
            // Console.WriteLine(tree.GetEntry("1.3.6.1.2.1.1.3").ToString());
            // Console.WriteLine("Output for: 1.3.6.1.2.1.4.21.1.13");
            // Console.WriteLine(tree.GetEntry("1.3.6.1.2.1.4.21.1.13").ToString());

            // AtEntry ::= SEQUENCE {
            //     atIfIndex
            //         INTEGER,
            //     atPhysAddress
            //         OCTET STRING,
            //     atNetAddress
            //         IpAddress
            // }
            byte[] addr = {123, 34, 45, 22};

            Dictionary<string, string> seq = new Dictionary<string, string>();
            seq.Add("atIfIndex", "39587549");
            seq.Add("atPhysAddress", "AddressThatIsPhys");
            seq.Add("atNetAddress", System.Text.Encoding.UTF8.GetString(addr));

            Console.WriteLine(BitConverter.ToString(BEREncoder.Encode("BOOLEAN", valB: true)));
            Console.WriteLine(BitConverter.ToString(BEREncoder.Encode("BOOLEAN", valB: false)));
            Console.WriteLine(BitConverter.ToString(BEREncoder.Encode("STRING", valStr: "note that simple SEQUENCEs are not directly")));
            Console.WriteLine(BitConverter.ToString(BEREncoder.Encode("INTEGER", (Int64)39587549)));
            Console.WriteLine(BitConverter.ToString(BEREncoder.Encode("NULL")));
            Console.WriteLine(BitConverter.ToString(BEREncoder.EncodeObjectIdentifer(1, 3, 6, 1, 4, 1)));
            Console.WriteLine(BitConverter.ToString(BEREncoder.EncodeObjectIdentifer(1, 2, 123123, 1)));

            Console.WriteLine(BitConverter.ToString(BEREncoder.EncodeSequence(seq, "AtEntry")));
        }
    }
}
