using RobotAutomationHelper.Forms;
using RobotAutomationHelper.Scripts.Static;
using RobotAutomationHelper.Scripts.Static.Readers;
using System;
using System.Collections.Generic;
using System.IO;

namespace RobotAutomationHelper.Scripts.Objects
{
    public class RobotFile
    {
        public List<TestCase> TestCasesList = new List<TestCase>();
        public List<SuiteSettings> SuiteSettingsList = new List<SuiteSettings>();
        public List<Variables> VariablesList = new List<Variables>();
        public List<Keyword> KeywordsList = new List<Keyword>();
        public string FileName;

        public RobotFile(string FileName)
        {
            this.FileName = FileName;
        }

        // returns the list of variables in specific file
        public List<Variables> ReadVariablesFromFile(string[] FileLines)
        {
            var listOfVariables = new List<Variables>();

            var index = HasTag(FileLines, FormType.Variable);
            if (index == -1) return listOfVariables;

            var names = new List<string>();
            for (int i = index + 1; i < FileLines.Length; i++)
            {
                if (FileLines[i].StartsWith("*")) break;
                if (FileLines[i].Trim().Length == 0) continue;
                if (StringAndListOperations.StartsWithVariable(FileLines[i].Trim())) names.Add(FileLines[i].Trim());
            }

            if (names.Count == 0) return listOfVariables;

            var currentVariables = new Variables(names, FileName);
            listOfVariables.Add(currentVariables);

            return listOfVariables;
        }

        // returns the list of keywords in specific file
        public List<Keyword> ReadKeywordsFromFile(string[] FileLines)
        {
            var listOfKeywords = new List<Keyword>();

            var index = HasTag(FileLines, FormType.Keyword);
            if (index == -1) return listOfKeywords;

            var currentKeyword = new Keyword(null);

            for (int i = index + 1; i < FileLines.Length; i++)
            {
                if (FileLines[i].StartsWith("*"))
                {
                    if (currentKeyword.Name != null)
                        listOfKeywords.Add(currentKeyword);
                    break;
                }

                if (FileLines[i].Trim().Length == 0) continue;

                if (!FileLines[i].StartsWith(" ") && !FileLines[i].StartsWith("\t"))
                {
                    if (currentKeyword.Name != null)
                    {
                        listOfKeywords.Add(currentKeyword);
                        currentKeyword = new Keyword(null);
                    }
                    currentKeyword.Name = FileLines[i].Trim();
                    continue;
                }

                if (FileLines[i].Trim().StartsWith("[Documentation]"))
                {
                    currentKeyword.Documentation = FileLines[i];
                    for (int j = i + 1; j < FileLines.Length; j++)
                    {
                        if (!FileLines[j].Trim().StartsWith(".")) break;
                        currentKeyword.Documentation += FileLines[j].Trim().Replace("...","");
                    }
                    continue;
                }

                if (FileLines[i].Trim().StartsWith("[Arguments]"))
                {
                    var j = i;
                    var multiLine = "";
                    if (i + 1 < FileLines.Length)
                    {
                        while (FileLines[i + 1].Trim().StartsWith("..."))
                        {
                            multiLine += "  " + FileLines[i + 1].Replace("...", "");
                            i++;
                            if (i + 1 >= FileLines.Length) break;
                        }
                        multiLine.TrimEnd();
                    }

                    var splitKeyword = (FileLines[j] + multiLine).Replace("[Arguments]", "").Trim().Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                    for (var counter = 0; counter < splitKeyword.Length; counter++)
                    {
                        if (!splitKeyword[counter].Contains("="))
                        {
                            if (currentKeyword.Params != null)
                            {
                                if (currentKeyword.Params.Count <= counter)
                                    currentKeyword.Params.Add(new Param(splitKeyword[counter], ""));
                                else
                                    currentKeyword.Params[counter].Name = splitKeyword[counter];
                            }
                            else
                            {
                                currentKeyword.Params = new List<Param>(){
                                            new Param(splitKeyword[counter], "")
                                        };
                            }
                        }
                        else
                        {
                            // check if after splitting the first string matches any param name
                            var temp = splitKeyword[counter].Split('=');
                            var toAdd = true;
                            foreach (var par in currentKeyword.Params)
                            {
                                if (!par.Name.Equals(temp[0])) continue;
                                toAdd = false;
                                break;
                            }
                            if (toAdd)
                                currentKeyword.Params.Add(new Param(temp[0], temp[1]));
                        }
                    }
                    currentKeyword.Arguments = FileLines[j];
                    continue;
                }

                if (!FileLines[i].Trim().StartsWith("#"))
                {
                    if (currentKeyword.Keywords == null) currentKeyword.Keywords = new List<Keyword>();
                    currentKeyword.Keywords.Add(new Keyword(FileLines[i].Trim(), FileName, GetResourcesFromFile(FileName), null));
                    continue;
                }
            }

            if (currentKeyword.Name != null)
                if (!listOfKeywords.Contains(currentKeyword))
                    listOfKeywords.Add(currentKeyword);

            return listOfKeywords;
        }

        // returns the index of the specific tag - keyword / test cases / settings / variables
        public int HasTag(string[] FileLines, FormType formType)
        {
            var index = -1;

            for (var i = 0; i < FileLines.Length; i++)
                if (FileLines[i].StartsWith("*"))
                    if (FileLines[i].ToLower().Contains(formType.ToString().ToLower()))
                        index = i;

            return index;
        }

        // returns all lines in specific file
        public string[] ReadAllLines(string FileName)
        {
            string[] arrLine;
            if (!File.Exists(FileName))
            {
                var myFile = File.Create(FileName);
                myFile.Close();
            }

            arrLine = File.ReadAllLines(FileName);

            return arrLine;
        }

        // gets the resources used in the file
        public List<string> GetResourcesFromFile(string fileName)
        {
            var arrLine = File.ReadAllLines(fileName);
            // find all resources
            var resources = new List<string>
            {
                fileName
            };

            if (arrLine.Length == 0) return resources;
            var start = false;
            foreach (var line in arrLine)
            {
                if (start && line.StartsWith("***"))
                    break;
                if (line.StartsWith("*** Settings ***"))
                    start = true;

                if (!start || !line.StartsWith("Resource")) continue;
                if (line.Contains("../"))
                {
                    string temp = line;
                    string tempFileName = fileName;
                    tempFileName = tempFileName.Replace("\\", "/");
                    tempFileName = tempFileName.Remove(tempFileName.LastIndexOf('/'));
                    while (temp.Contains("../"))
                    {
                        tempFileName = tempFileName.Remove(tempFileName.LastIndexOf('/'));
                        temp = temp.Remove(temp.LastIndexOf("../"), 3);
                        //Console.WriteLine(temp + " || " + tempFileName);
                    }
                    resources.Add(tempFileName.Replace("/", "\\") + "\\" + temp.Replace("../", "").Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("/", "\\"));
                }
                else
                {
                    if (!line.Contains("./"))
                    {
                        if (!line.Contains("/"))
                        {
                            string tempFileName = fileName;
                            tempFileName = tempFileName.Replace("\\", "/");
                            tempFileName = tempFileName.Remove(tempFileName.LastIndexOf('/'));
                            resources.Add(tempFileName.Replace("/", "\\") + "\\" + line.Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("/", "\\"));
                        }
                        else
                            resources.Add(FilesAndFolderStructure.GetFolder(FolderType.Root) + line.Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("/", "\\"));
                    }
                    else
                    {
                        resources.Add(fileName.Remove(fileName.LastIndexOf('\\') + 1, fileName.Length - fileName.LastIndexOf('\\') - 1) + line.Replace("./", "").Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("/", "\\"));
                    }
                }
            }
            return resources;
        }
    }
}
