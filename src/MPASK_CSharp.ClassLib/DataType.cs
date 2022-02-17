using System;
using System.Collections;

namespace MPASK_CSharp.ClassLib
{
    public class DataType
    {
        public string name, type;
        //public Int64 value;

        // For strings etc. -1 for not specified
        private int size = -1;

        // Value range
        private Int64[] range = {0, 0};

        public DataType(string name, string data_type)
        {
            this.name = name;

            string[] splitStr = data_type.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if(splitStr[0] == "INTEGER")
            {
                this.type = "INTEGER";

                if(splitStr.Length > 1)
                {
                    string trimmed = splitStr[1].Trim('(', ')', '\r');
                    string[] mm = trimmed.Split(new String[] { ".." }, StringSplitOptions.RemoveEmptyEntries);
                    if(!mm[0].Contains("{"))
                    {
                        range[0] = int.Parse(mm[0]);
                        range[1] = Int64.Parse(mm[1]);
                    }
                }
            }
            else if(splitStr[0] == "OCTET")
            {
                this.type = "STRING";

                if(splitStr.Length > 2)
                {
                    string trimmed = splitStr[3].Trim('(', ')');
                    this.size = int.Parse(trimmed);
                }
            }
            else if(splitStr[0] == "NULL")
            {
                this.type = "NULL";
            }
            else if(splitStr[0] == "OBJECT IDENTIFIER")
            {
                this.type = "OBJECT IDENTIFIER";
            }
        }

        public bool Equals(string name)
        {
            if (this.name.Equals(name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}