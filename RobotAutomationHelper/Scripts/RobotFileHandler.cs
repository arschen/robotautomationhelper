using System.Collections.Generic;
using System.IO;
using System;

namespace RobotAutomationHelper.Scripts
{
    internal static class RobotFileHandler
    {
        internal static int GetLineAfterLastKeyword(string fileName)
        {
            return GetLine(fileName, "keywords");
        }

        internal static int GetLineAfterLastTestCase(string fileName)
        {
            return GetLine(fileName, "test cases");
        }

        // get the index line where to add specific type ( keyword / test cases / settings )
        private static int GetLine(string fileName, string type)
        {
            int index = -1;
            string[] arrLine;
            if (File.Exists(fileName))
                arrLine = File.ReadAllLines(fileName);
            else
            {
                string directory = fileName.Replace(fileName.Split('\\')[fileName.Split('\\').Length - 1], "");
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                var myFile = File.Create(fileName);
                myFile.Close();
                /*
                List<string> temp = new List<string>
                {
                    "*** Settings ***",
                    "*** Variables ***",
                    "*** Test Cases ***",
                    "*** Keywords ***"
                };
                File.WriteAllLines(fileName, temp);
                temp.Clear();
                arrLine = File.ReadAllLines(fileName);
                */
                arrLine = new string[0];
            }

            if (arrLine.Length != 0)
            {
                bool start = false;
                for (int i = 0; i < arrLine.Length; i++)
                {
                    if (arrLine[i].StartsWith("***"))
                    {
                        if (start)
                        {
                            index = i;
                            break;
                        }
                        if (arrLine[i].ToLower().Contains(type))
                            start = true;
                    }
                }

                if (!start)
                    if (arrLine.Length - 1 >= 0)
                        index = arrLine.Length;
                    else
                        index = 0;
                else
                    if (index == -1)
                {
                    if (arrLine.Length - 1 >= 0)
                        index = arrLine.Length;
                    else
                        index = 0;
                }
            }

            return index;
        }

        // returns the index of the specific tag - keyword / test cases / settings / variables
        internal static int HasTag(string fileName, string type)
        {
            int index = -1;

            string[] arrLine;

            if (File.Exists(fileName))
                arrLine = File.ReadAllLines(fileName);
            else
            {
                var myFile = File.Create(fileName);
                myFile.Close();
                arrLine = File.ReadAllLines(fileName);
            }

            for (int i = 0; i < arrLine.Length; i++)
                if (arrLine[i].StartsWith("***"))
                    if (arrLine[i].ToLower().Contains(type.ToLower()))
                        index = i;

            return index;
        }

        // returns index with the line if the file contains a keyword / test case with the same name
        internal static int ContainsTestCaseOrKeyword(string fileName, string name, string type)
        {
            if (File.Exists(fileName))
            {
                int index = HasTag(fileName, type);
                if (index != -1)
                {
                    string[] arrLine;
                    arrLine = File.ReadAllLines(fileName);
                    for (int ind = index; ind<arrLine.Length; ind++)
                    {
                        if (!arrLine[ind].StartsWith("***"))
                        {
                            if ((!arrLine[ind].StartsWith(" ")) && (!arrLine[ind].StartsWith("\\")) && (!arrLine[ind].StartsWith(".")))
                            {

                                string[] temp = arrLine[ind].ToLower().Split(new string[] { "  " }, System.StringSplitOptions.RemoveEmptyEntries);
                                foreach (string s in temp)
                                    if (s.Equals(name.ToLower()))
                                    {
                                        if (RobotAutomationHelper.Log) Console.WriteLine(arrLine[ind].StartsWith("	") + " | " + arrLine[ind]);
                                        return ind;
                                    }
                            }
                        }
                    }
                }
            }
            return -1;
        }

        // returns bool of the line where the specific include is found
        internal static string ContainsSettings(string fileName, string name)
        {
            if (File.Exists(fileName))
            {
                int index = HasTag(fileName, "settings");
                if (index != -1)
                {
                    string[] arrLine;
                    arrLine = File.ReadAllLines(fileName);
                    for (int ind = index; ind < arrLine.Length; ind++)
                    {
                        if (!arrLine[ind].StartsWith("***"))
                        {
                            if ((!arrLine[ind].StartsWith(" ")) && (!arrLine[ind].StartsWith("\t")) && (!arrLine[ind].StartsWith("\\")) && (!arrLine[ind].StartsWith(".")))
                            {
                                if (name.StartsWith("Library") || name.StartsWith("Resource"))
                                {
                                    if (arrLine[ind].ToLower().Trim().Equals(name.ToLower()))
                                        return arrLine[ind];
                                }
                                else
                                {
                                    if (arrLine[ind].ToLower().Trim().StartsWith(name.ToLower()))
                                    {
                                        string temp = arrLine[ind];
                                        for (int j = ind + 1; j < arrLine.Length; j++)
                                            if (arrLine[j].StartsWith("..."))
                                                if (name.Equals("Documentation"))
                                                    temp += " " + arrLine[j].Replace("...", "").Trim();
                                                else
                                                    temp += "  " + arrLine[j].Replace("...", "").Trim();
                                        return temp;
                                    }
                                }          
                            }
                        }
                    }
                }
            }
            return "";
        }

        // add newText on new line to file fileName after specified line
        internal static void FileLineAdd(string newText, string fileName, int line_to_add_after)
        {
            string[] arrLine;
            if (File.Exists(fileName))
                arrLine = File.ReadAllLines(fileName);
            else
            {
                string directory = fileName.Replace(fileName.Split('\\')[fileName.Split('\\').Length - 1], "");
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                var myFile = File.Create(fileName);
                myFile.Close();
                arrLine = File.ReadAllLines(fileName);
            }

            List<string> temp = new List<string>();
            temp.AddRange(arrLine);
            temp.Insert(line_to_add_after, newText);
            File.WriteAllLines(fileName, temp);
        }

        // add newText on new line to file fileName after specified line
        internal static void TrimFile(string fileName)
        {
            string[] arrLine = File.ReadAllLines(fileName);

            if (!(arrLine == null) && !(arrLine.Length == 0))
            {
                List<string> temp = new List<string>();
                for (int i = 1; i < arrLine.Length; i++)
                {
                    if (!(arrLine[i - 1].Trim().Equals("") && arrLine[i].Trim().Equals("")))
                        temp.Add(arrLine[i - 1]);
                    else
                        Console.WriteLine(i);
                }
                if (!arrLine[arrLine.Length - 1].Trim().Equals(""))
                    temp.Add(arrLine[arrLine.Length - 1]);
                File.WriteAllLines(fileName, temp);
            }
        }

        // add newText on new line to file fileName after specified line
        internal static void TestCaseKeywordRemove(string name, string fileName, bool isKeyword)
        {
            string[] arrLine;
            if (File.Exists(fileName))
                arrLine = File.ReadAllLines(fileName);
            else
            {
                string directory = fileName.Replace(fileName.Split('\\')[fileName.Split('\\').Length - 1], "");
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                var myFile = File.Create(fileName);
                myFile.Close();
                arrLine = File.ReadAllLines(fileName);
            }

            int index = 0;
            if (isKeyword)
                index = ContainsTestCaseOrKeyword(fileName, name, "keywords");
            else
                index = ContainsTestCaseOrKeyword(fileName, name, "test cases");

            if (index != -1)
            {
                bool endOfTestCaseKeyword = false;
                List<string> temp = new List<string>();
                temp.AddRange(arrLine);

                while (!endOfTestCaseKeyword)
                {
                    if (index < temp.Count)
                        temp.RemoveAt(index);
                    else
                        endOfTestCaseKeyword = true;

                    if (index < temp.Count)
                        if (!temp[index].StartsWith(" "))
                            if (!temp[index].StartsWith("\t"))
                                if (!temp[index].StartsWith("\\"))
                                    if(!temp[index].StartsWith("."))
                                            if (!temp[index].Equals(""))
                                                endOfTestCaseKeyword = true;
                }

                File.WriteAllLines(fileName, temp);
            }
        }
    }
}