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
                List<string> temp = new List<string>();
                temp.Add("*** Settings ***");
                temp.Add("*** Variables ***");
                temp.Add("*** Test Cases ***");
                temp.Add("*** Keywords ***");
                File.WriteAllLines(fileName, temp);
                temp.Clear();
                arrLine = File.ReadAllLines(fileName);
            }

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
                                        //Console.WriteLine(arrLine[ind].StartsWith("	") + " | " + arrLine[ind]);
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
        internal static bool ContainsSettings(string fileName, string name)
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
                            if ((!arrLine[ind].StartsWith(" ")) && (!arrLine[ind].StartsWith("\\t")) && (!arrLine[ind].StartsWith("\\")) && (!arrLine[ind].StartsWith(".")))
                            {
                                if (arrLine[ind].ToLower().Trim().Equals(name.ToLower()))
                                    return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}