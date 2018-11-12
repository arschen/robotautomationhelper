using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RobotAutomationHelper.Forms;

namespace RobotAutomationHelper.Scripts.Static.Writers
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
            var index = -1;
            string[] arrLine;
            if (File.Exists(fileName))
                arrLine = File.ReadAllLines(fileName);
            else
            {
                var directory = fileName.Replace(fileName.Split('\\')[fileName.Split('\\').Length - 1], "");
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

            if (arrLine.Length == 0) return index;
            var start = false;
            for (var i = 0; i < arrLine.Length; i++)
            {
                if (!arrLine[i].StartsWith("***")) continue;
                if (start)
                {
                    index = i;
                    break;
                }
                if (arrLine[i].ToLower().Contains(type.ToString().ToLower()))
                    start = true;
            }

            if (!start)
                index = arrLine.Length - 1 >= 0 ? arrLine.Length : 0;
            else
            if (index == -1)
            {
                index = arrLine.Length - 1 >= 0 ? arrLine.Length : 0;
            }

            return index;
        }

        // returns the index of the specific tag - keyword / test cases / settings / variables
        internal static int HasTag(string fileName, FormType formType)
        {
            var index = -1;

            string[] arrLine;

            if (File.Exists(fileName))
                arrLine = File.ReadAllLines(fileName);
            else
            {
                var myFile = File.Create(fileName);
                myFile.Close();
                arrLine = File.ReadAllLines(fileName);
            }

            for (var i = 0; i < arrLine.Length; i++)
                if (arrLine[i].StartsWith("***"))
                    if (arrLine[i].ToLower().Contains(formType.ToString().ToLower()))
                        index = i;

            return index;
        }

        // returns index with the line if the file contains a keyword / test case with the same name
        internal static int LocationOfTestCaseOrKeywordInFile(string fileName, string name, FormType formType)
        {
            if (!File.Exists(fileName)) return -1;
            var index = HasTag(fileName, formType);
            if (index == -1) return -1;
            var arrLine = File.ReadAllLines(fileName);
            for (var ind = index; ind<arrLine.Length; ind++)
            {
                if (arrLine[ind].StartsWith("***")) continue;
                if ((arrLine[ind].StartsWith(" ")) || (arrLine[ind].StartsWith("\\")) ||
                    (arrLine[ind].StartsWith("."))) continue;
                var temp = arrLine[ind].ToLower().Split(new [] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                if (!temp.Any(s => s.Equals(name.ToLower()))) continue;
                if (Forms.RobotAutomationHelper.Log) Console.WriteLine(arrLine[ind].StartsWith("	") + @" | " + arrLine[ind]);
                return ind;
            }
            return -1;
        }

        // returns non empty string of the line where the specific include is found
        internal static string OccurenceInSettings(string fileName, string name)
        {
            if (!File.Exists(fileName)) return "";
            var index = HasTag(fileName, FormType.Settings);
            if (index == -1) return "";
            var arrLine = File.ReadAllLines(fileName);
            for (var ind = index; ind < arrLine.Length; ind++)
            {
                if (arrLine[ind].StartsWith("***")) continue;
                if ((arrLine[ind].StartsWith(" ")) || (arrLine[ind].StartsWith("\t")) ||
                    (arrLine[ind].StartsWith("\\")) || (arrLine[ind].StartsWith("."))) continue;
                if (name.StartsWith("Library") || name.StartsWith("Resource"))
                {
                    if (arrLine[ind].ToLower().Trim().Equals(name.ToLower()))
                        return arrLine[ind];
                }
                else
                {
                    if (!arrLine[ind].ToLower().Trim().StartsWith(name.ToLower())) continue;
                    var temp = arrLine[ind];
                    for (var j = ind + 1; j < arrLine.Length; j++)
                        if (arrLine[j].StartsWith("..."))
                            if (name.Equals("Documentation"))
                                temp += " " + arrLine[j].Replace("...", "").Trim();
                            else
                                temp += "  " + arrLine[j].Replace("...", "").Trim();
                    return temp;
                }
            }
            return "";
        }

        // returns list of indexes with locations of specific string
        internal static List<int> LocationInSettings(string fileName, string name)
        {
            var result = new List<int>();
            if (File.Exists(fileName))
            {
                var index = HasTag(fileName, FormType.Settings);
                if (index != -1)
                {
                    var arrLine = File.ReadAllLines(fileName);
                    for (var ind = index; ind < arrLine.Length; ind++)
                    {
                        if (arrLine[ind].StartsWith("***")) continue;
                        if ((arrLine[ind].StartsWith(" ")) || (arrLine[ind].StartsWith("\t")) ||
                            (arrLine[ind].StartsWith("\\")) || (arrLine[ind].StartsWith("."))) continue;
                        if (name.StartsWith("Library") || name.StartsWith("Resource"))
                        {
                            if (!arrLine[ind].ToLower().Trim().Equals(name.ToLower())) continue;
                            result.Add(ind);
                            return result;
                        }

                        if (!arrLine[ind].ToLower().Trim().StartsWith(name.ToLower())) continue;
                        result.Add(ind);
                        for (var j = ind + 1; j < arrLine.Length; j++)
                            if (arrLine[j].StartsWith("..."))
                                result.Add(j);
                            else
                                break;
                        return result;
                    }
                }
            }
            result.Add(-1);
            return result;
        }

        // add newText on new line to file fileName after specified line
        internal static void FileLineAdd(string newText, string fileName, int lineToAddAfter)
        {
            string[] arrLine;
            if (File.Exists(fileName))
                arrLine = File.ReadAllLines(fileName);
            else
            {
                var directory = fileName.Replace(fileName.Split('\\')[fileName.Split('\\').Length - 1], "");
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                var myFile = File.Create(fileName);
                myFile.Close();
                arrLine = File.ReadAllLines(fileName);
            }

            var temp = new List<string>();
            temp.AddRange(arrLine);
            if (!(lineToAddAfter > arrLine.Length))
                temp.Insert(lineToAddAfter, newText);
            else
                temp.Add(newText);
            File.WriteAllLines(fileName, temp);
            if (!newText.Equals(""))
                Console.WriteLine(@"Line added: " + newText + @"	" + fileName.Replace(FilesAndFolderStructure.GetFolder(FolderType.Root),""));
        }

        // removes linesToReplace from fileName
        internal static void FileLineRemove(string fileName, List<int> linesToRemove)
        {
            string[] arrLine;
            if (File.Exists(fileName))
                arrLine = File.ReadAllLines(fileName);
            else
            {
                var directory = fileName.Replace(fileName.Split('\\')[fileName.Split('\\').Length - 1], "");
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                var myFile = File.Create(fileName);
                myFile.Close();
                arrLine = File.ReadAllLines(fileName);
            }

            var temp = new List<string>();
            temp.AddRange(arrLine);
            if (linesToRemove.Count != 0)
                for (var i = 0; i < linesToRemove.Count; i++)
                {
                    if (!temp[linesToRemove[i] - i].Trim().Equals(""))
                        Console.WriteLine(@"Line removed: " + temp[linesToRemove[i] - i] + @"	" + fileName.Replace(FilesAndFolderStructure.GetFolder(FolderType.Root),""));
                    temp.RemoveAt(linesToRemove[i] - i);
                }
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
                var directory = fileName.Replace(fileName.Split('\\')[fileName.Split('\\').Length - 1], "");
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                var myFile = File.Create(fileName);
                myFile.Close();
                arrLine = File.ReadAllLines(fileName);
            }

            var temp = new List<string>();
            temp.AddRange(arrLine);
            temp[linesToReplace[0]] = newText;
            if (linesToReplace.Count != 1)
                for (var i = 1; i < linesToReplace.Count; i++)
                {
                    Console.WriteLine(@"Line replaced: " + temp[linesToReplace[i] - i + 1] + @"\t" + fileName.Replace(FilesAndFolderStructure.GetFolder(FolderType.Root),""));
                    temp.RemoveAt(linesToReplace[i] - i + 1);
                }
            File.WriteAllLines(fileName, temp);
        }

        // add newText on new line to file fileName after specified line
        internal static void TrimFile(string fileName)
        {
            if (!File.Exists(fileName)) return;
            var arrLine = File.ReadAllLines(fileName);
            var temp = new List<string>();

            if (arrLine.Length != 0)
            {
                for (var i = 1; i < arrLine.Length; i++)
                {
                    if (!(arrLine[i - 1].Trim().Equals("") && arrLine[i].Trim().Equals("")))
                        temp.Add(arrLine[i - 1]);
                }
                if (!arrLine[arrLine.Length - 1].Trim().Equals(""))
                    temp.Add(arrLine[arrLine.Length - 1]);
                File.WriteAllLines(fileName, temp);
            }

            // clears all tags ( settings / keywords / test cases ) that have no implementation inside them
            if (temp.Count == 0) return;
            {
                var indexesToRemove = new List<int>();
                foreach (var line in temp)
                {
                    if (!line.StartsWith("***")) continue;
                    if (temp.IndexOf(line) + 1 < temp.Count)
                    {
                        var initialIndex = temp.IndexOf(line);
                        var endIndex = initialIndex;
                        var addForRemoval = false;
                        for (var j = temp.IndexOf(line) + 1; j < temp.Count; j++)
                        {
                            if (!temp[j].StartsWith("***") && !temp[j].Trim().Equals(""))
                            {
                                break;
                            }

                            if (temp[j].StartsWith("***"))
                            {
                                addForRemoval = true;
                                endIndex = j - 1;
                                break;
                            }

                            if (j != temp.Count - 1) continue;
                            addForRemoval = true;
                            endIndex = j;
                            break;
                        }

                        if (!addForRemoval) continue;
                        for (var indexes = initialIndex; indexes <= endIndex; indexes++)
                            indexesToRemove.Add(indexes);
                    }
                    else
                    {
                        indexesToRemove.Add(temp.IndexOf(line));
                    }
                }
                if (indexesToRemove.Count != 0)
                {
                    for (var i = indexesToRemove.Count - 1; i >= 0; i--)
                    {
                        Console.WriteLine(@"Trim: " + temp[indexesToRemove[i]] + @"\t" + fileName);
                        temp.RemoveAt(indexesToRemove[i]);
                    }
                }
                File.WriteAllLines(fileName, temp);
            }
        }
    }
}