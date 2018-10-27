using System.Collections.Generic;
using System.IO;
using System;

namespace RobotAutomationHelper.Scripts
{
    internal static class RobotFileHandler
    {
        internal static int GetLineAfterLastKeyword(string fileName)
        {
            return GetWriteLocationOfType(fileName, FormType.Keyword);
        }
        internal static int GetLineAfterLastTestCase(string fileName)
        {
            return GetWriteLocationOfType(fileName, FormType.Test);
        }

        // get the index line where to add specific type ( keyword / test cases / settings )
        private static int GetWriteLocationOfType(string fileName, FormType type)
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
                        if (arrLine[i].ToLower().Contains(type.ToString().ToLower()))
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
        internal static int HasTag(string fileName, FormType formType)
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
                    if (arrLine[i].ToLower().Contains(formType.ToString().ToLower()))
                        index = i;

            return index;
        }

        // returns index with the line if the file contains a keyword / test case with the same name
        internal static int LocationOfTestCaseOrKeywordInFile(string fileName, string name, FormType formType)
        {
            if (File.Exists(fileName))
            {
                int index = HasTag(fileName, formType);
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

        // returns non empty string of the line where the specific include is found
        internal static string OccuranceInSettings(string fileName, string name)
        {
            if (File.Exists(fileName))
            {
                int index = HasTag(fileName, FormType.Settings);
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

        // returns list of indexes with locations of specifig string
        internal static List<int> LocationInSettings(string fileName, string name)
        {
            List<int> result = new List<int>();
            if (File.Exists(fileName))
            {
                int index = HasTag(fileName, FormType.Settings);
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
                                    {
                                        result.Add(ind);
                                        return result;
                                    }
                                }
                                else
                                {
                                    if (arrLine[ind].ToLower().Trim().StartsWith(name.ToLower()))
                                    {
                                        result.Add(ind);
                                        for (int j = ind + 1; j < arrLine.Length; j++)
                                            if (arrLine[j].StartsWith("..."))
                                                result.Add(j);
                                            else
                                                break;
                                        return result;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            result.Add(-1);
            return result;
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

        // removes linesToReplace from fileName
        internal static void FileLineRemove(string fileName, List<int> linesToRemove)
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
            if (linesToRemove.Count != 0)
                for (int i = 0; i < linesToRemove.Count; i++)
                    temp.RemoveAt(linesToRemove[i] - i);
            File.WriteAllLines(fileName, temp);
        }

        // replace linesToReplace with single line newText
        internal static void FileLineReplace(string newText, string fileName, List<int> linesToReplace)
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
            temp[linesToReplace[0]] = newText;
            if (linesToReplace.Count != 1)
                for (int i = 1; i < linesToReplace.Count; i++)
                    temp.RemoveAt(linesToReplace[i] - i + 1);
            File.WriteAllLines(fileName, temp);
        }

        // add newText on new line to file fileName after specified line
        internal static void TrimFile(string fileName)
        {
            if (File.Exists(fileName))
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

                // clears all tags ( settings / keywords / test cases ) that have no implementation inside them
                arrLine = File.ReadAllLines(fileName);

                if (!(arrLine == null) && !(arrLine.Length == 0))
                {
                    List<string> temp = new List<string>();
                    temp.AddRange(arrLine);
                    List<int> indexesToRemove = new List<int>();
                    foreach (string line in temp)
                    {
                        if (line.StartsWith("***"))
                            if (temp.IndexOf(line) + 1 < temp.Count)
                            {
                                for (int j = temp.IndexOf(line) + 1; j < temp.Count; j++)
                                {
                                    if (!temp[j].StartsWith("***") && !temp[j].Trim().Equals(""))
                                        break;
                                    else
                                    {
                                        if (temp[j].StartsWith("***"))
                                        {
                                            for (int k = temp.IndexOf(line); k < j; k++)
                                            {
                                                indexesToRemove.Add(k);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                indexesToRemove.Add(temp.IndexOf(line));
                            }
                    }
                    if (indexesToRemove != null && indexesToRemove.Count != 0)
                    {
                        for (int i = indexesToRemove.Count - 1; i >= 0; i--)
                            temp.RemoveAt(i);
                    }
                    File.WriteAllLines(fileName, temp);
                }
            }
        }
    }
}