using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MPASK_CSharp.ClassLib
{
    public class Parser
    {
        private RegexOptions optionsObjType;

        // Remember parsed files so that they aren't parsed again
        private static List<string> fileList = new List<string>();

        public static Dictionary<string, Dictionary<string, string>> sequenceDict = 
                                new Dictionary<string, Dictionary<string, string>>();

        public Parser()
        {
            optionsObjType = RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.Compiled;
        }

        public void ParseFile(in string input, ref MIBTree tree)
        {
            Console.WriteLine("Prarsing Imports...");

            string importPatt = @"IMPORTS\s*(?<imports>[\w\s,\-]*)";
            string imported, importFile;

            foreach (Match match in Regex.Matches(input, importPatt, RegexOptions.Compiled | RegexOptions.Multiline))
            {
                imported = match.Groups["imports"].Value;

                foreach (Match impMatch in Regex.Matches(input, @"FROM\s(?<file>[\w-]*)", RegexOptions.Compiled | RegexOptions.Multiline))
                {
                    if (fileList.Contains(@"../../../" + impMatch.Groups["file"].Value + ".txt"))
                    {
                        //Console.WriteLine("File {0} already parsed.", impMatch.Groups["file"].Value + ".txt");
                    }
                    else
                    {
                        fileList.Add(@"../../../" + impMatch.Groups["file"].Value + ".txt");
                        importFile = System.IO.File.ReadAllText(@"../../../" + impMatch.Groups["file"].Value + ".txt");
                        //Console.WriteLine("Parsing currently {0}", impMatch.Groups["file"].Value + ".txt");
                        this.ParseFile(importFile, ref tree);
                    }
                }
            }

            Console.WriteLine("Prarsing Data types...");

            string typeDeclarationPatt = @"(?<name>\w*)\s::=\s*\[APPLICATION\s(?<app_num>\w*)\][\s\w\-.,]*$\s*IMPLICIT\s(?<data_type>[\w\s(.]*[)\w])";
            string name, app_num, data_type;

             foreach (Match match in Regex.Matches(input, typeDeclarationPatt, optionsObjType))
            {
                name = match.Groups["name"].Value;
                app_num = match.Groups["app_num"].Value;
                data_type = match.Groups["data_type"].Value;

                //Console.WriteLine(name + data_type);

                MIBTree.dataTypes.Add(new DataType(name, data_type));
            }

            Console.WriteLine("Prarsing Object identifiers...");
            string objIndPatt = @"(?<name>^\w[\w\-]*)\s*OBJECT\sIDENTIFIER\s*::=\s*\{\s?(?<root>.*?)\s?\}";
            string root;
            string[] rootSplit;

            foreach (Match match in Regex.Matches(input, objIndPatt, RegexOptions.Compiled | RegexOptions.Multiline))
            {
                name = match.Groups["name"].Value;
                root = match.Groups["root"].Value;
                rootSplit = root.Split(' ');

                //Console.WriteLine(name + root);

                if(rootSplit.Length > 2)
                {
                    string[] entSplit;
                    string prevRoot = rootSplit[0];
                    for(int i = 1; i < rootSplit.Length - 1; i++)
                    {
                        entSplit = rootSplit[i].Split('(', ')');
                        tree.InsertEntry(prevRoot, new TreeNode(entSplit[0], int.Parse(entSplit[1])));
                        prevRoot = entSplit[0];
                    }
                    tree.InsertEntry(prevRoot, new TreeNode(name, int.Parse(rootSplit[rootSplit.Length - 1])));
                }
                else
                {
                    tree.InsertEntry(rootSplit[0], new TreeNode(name, int.Parse(rootSplit[1])));
                }
            }

            string sequencePatt = @"(?<name>\w+)\s::=\s+SEQUENCE\s\{(?<inside>[\w\s,]+)\}";

            string inside;

            foreach (Match match in Regex.Matches(input, sequencePatt, optionsObjType))
            {
                name = match.Groups["name"].Value;
                inside = match.Groups["inside"].Value;
                string[] splitCom = inside.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                Dictionary<string, string> insideDict = new Dictionary<string, string>();

                foreach(string entry in splitCom)
                {
                    string[] splitEnt = entry.Split(new char[] { ' ', '\n', '\t', '\r', '\v' }, StringSplitOptions.RemoveEmptyEntries);

                    if (splitEnt.Length > 2)
                    {
                        string mergeStr = splitEnt[1];

                        for (int i = 2; i < splitEnt.Length; i++)
                        {
                            mergeStr += " " + splitEnt[i];
                        }

                        insideDict.Add(splitEnt[0], mergeStr);
                    }
                    else
                    {
                        insideDict.Add(splitEnt[0], splitEnt[1]);
                    }
                }

                if (!sequenceDict.ContainsKey(name))
                {
                    sequenceDict.Add(name, insideDict);
                }
                else
                {
                    sequenceDict[name] = insideDict;
                }
                
            }
            
            Console.WriteLine("Prarsing Object types...");

            string objTypePatt = @"(?<name>\w*)\s*OBJECT-TYPE\s*SYNTAX\s*(?<syntax>[\w\s().{},]*\{?$)(?<syntax_vals>[\/\s\w\-(),.\[\]]*\})?.*?ACCESS\s*(?<access>.*?\w*?$).*?STATUS\s*(?<status>.*?\w*?$)\s(?<rest>.+?)::=\s*\{\s?(?<root>.*?)\s?\}";

            string syntax, access, status, description, syntax_vals, rest, index;
            int j = 547;

            // Match the appropriate DataType and put it into the tree (list)
            foreach (Match match in Regex.Matches(input, objTypePatt, optionsObjType))
            {
                j++;
                name = match.Groups["name"].Value;
                syntax = match.Groups["syntax"].Value;
                syntax_vals = match.Groups["syntax_vals"].Value;
                access = match.Groups["access"].Value;
                status = match.Groups["status"].Value;
                root = match.Groups["root"].Value;
                rest = match.Groups["rest"].Value;

                DataType dataType = null;
                bool inList = false;

                foreach(DataType dt in MIBTree.dataTypes)
                {
                    if(dt.Equals(syntax))
                    {
                        inList = false;
                        break;
                    }
                    inList = true;
                }

                if (inList)
                {
                    dataType = new DataType(name + j.ToString(), syntax);
                    inList = false;
                }

                description = index = "";

                string matchDescInd = @"\s*DESCRIPTION\s*""(?<description>.*?)""\s*(INDEX\s*\{\s(?<ind>\w*)\s\})?";
                if(Regex.IsMatch(rest, matchDescInd, optionsObjType))
                {
                    Match match1 = Regex.Match(input, matchDescInd, optionsObjType);
                    description = match1.Groups["description"].Value;
                    index = match1.Groups["index"].Value;                         // TODO: INDEX not working
                }

                rootSplit = root.Split(' ');

                //Console.WriteLine(name + root + index);

                tree.InsertEntry(rootSplit[0], new ObjectType(name, int.Parse(rootSplit[1]), syntax, syntax_vals, 
                                                                    access, status, description, index, dataType));
            }
        }
    }
}
