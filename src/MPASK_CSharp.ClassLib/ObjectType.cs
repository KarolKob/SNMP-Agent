using System;

namespace MPASK_CSharp.ClassLib
{
    enum Access {ReadOnly, ReadWrite, WriteOnly, NotAccessible}
    enum Status {Mandatory, Optional, Obsolete}
    public class ObjectType : TreeNode
    {
        private string syntax, syntax_vals, description, index;
        private Access access;
        private Status status;
        private DataType dataType = null;
        public ObjectType(string name, int id, string syntax, string syntax_vals, 
                            string access, string status, string description, string index, DataType dt) : base(name, id)
        {
            this.syntax = syntax;
            this.syntax_vals = syntax_vals;
            this.description = description;
            this.index = index;
            this.dataType = dt;
            switch (access)
            {
                case "read-only":
                    this.access = Access.ReadOnly;
                    break;

                case "read-write":
                    this.access = Access.ReadWrite;
                    break;
                
                case "write-only":
                    this.access = Access.WriteOnly;
                    break;

                case "not-accessible":
                    this.access = Access.NotAccessible;
                    break;
                
                default:
                    break;
            }

            switch (status)
            {
                case "mandatory":
                    this.status = Status.Mandatory;
                    break;

                case "optional":
                    this.status = Status.Optional;
                    break;
                
                case "obsolete":
                    this.status = Status.Obsolete;
                    break;
                
                default:
                    break;
            }
        }

        public override string ToString()
        {
            if (dataType == null)
            {
                return base.ToString() + " " + syntax + " " + syntax_vals + " " + access.ToString() + " " +
             status.ToString() + " " + description + " " + index;
            }
            else
            {
                return base.ToString() + " " + syntax + " " + syntax_vals + " " + access.ToString() + " " +
             status.ToString() + " " + description + " " + index + " " + dataType.name + " " + dataType.type;
            }
        }
    }
}