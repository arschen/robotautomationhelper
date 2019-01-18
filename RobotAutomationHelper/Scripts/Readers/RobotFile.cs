using RobotAutomationHelper.Forms;
using RobotAutomationHelper.Scripts.Static;
using RobotAutomationHelper.Scripts.Static.Readers;
using RobotAutomationHelper.Scripts.Static.Writers;
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
        public string fileName;
        public string[] fileLines;
        public List<string> resources;
        public List<string> libraries;

        public RobotFile(string FileName)
        {
            this.fileName = FileName;
            fileLines = ReadAllLines(FileName);
            resources = GetResourcesFromFile();
            libraries = GetLibs();
            VariablesList = ReadVariablesFromFile();
            SuiteSettingsList = ReadSettingsFromFile();
            KeywordsList = ReadKeywordsFromFile();
            TestCasesList = ReadTestCasesFromFile();
        }

        // returns the list of variables in specific file
        public List<Variables> ReadVariablesFromFile()
        {
            var listOfVariables = new List<Variables>();

            var index = HasTag(FormType.Variable);
            if (index == -1) return listOfVariables;

            var names = new List<string>();
            for (int i = index + 1; i < fileLines.Length; i++)
            {
                if (fileLines[i].StartsWith("*")) break;
                if (fileLines[i].Trim().Length == 0) continue;
                if (StringAndListOperations.StartsWithVariable(fileLines[i].Trim())) names.Add(fileLines[i].Trim());
            }

            if (names.Count == 0) return listOfVariables;

            var currentVariables = new Variables(names, fileName);
            listOfVariables.Add(currentVariables);

            return listOfVariables;
        }

        // returns the list of test cases in specific file
        public List<TestCase> ReadTestCasesFromFile()
        {
            var listOfTestCases = new List<TestCase>();

            var index = HasTag(FormType.Test);
            if (index == -1) return listOfTestCases;

            var currentTestCase = new TestCase("", fileName);

            for (int i = index + 1; i < fileLines.Length; i++)
            {
                if (fileLines[i].StartsWith("*"))
                {
                    if (currentTestCase.Name != "")
                        listOfTestCases.Add(currentTestCase);
                    break;
                }

                if (fileLines[i].Trim().Length == 0) continue;

                if (!fileLines[i].StartsWith(" ") && !fileLines[i].StartsWith("\t") && !fileLines[i].Trim().StartsWith("#"))
                {
                    if (currentTestCase.Name != "")
                    {
                        listOfTestCases.Add(currentTestCase);
                        currentTestCase = new TestCase("", fileName);
                    }
                    currentTestCase.Name = fileLines[i].Trim();
                    continue;
                }

                if (fileLines[i].Trim().StartsWith("[Documentation]"))
                {
                    currentTestCase.Documentation = fileLines[i];
                    for (int j = i + 1; j < fileLines.Length; j++)
                    {
                        if (!fileLines[j].Trim().StartsWith(".")) break;
                        currentTestCase.Documentation += fileLines[j].Trim().Replace("...", "");
                    }
                    continue;
                }

                if (fileLines[i].Trim().StartsWith("[Tags]"))
                {
                    var j = i;
                    var multiLine = "";
                    if (i + 1 < fileLines.Length)
                    {
                        while (fileLines[i + 1].Trim().StartsWith("..."))
                        {
                            multiLine += "  " + fileLines[i + 1].Replace("...", "");
                            i++;
                            if (i + 1 >= fileLines.Length) break;
                        }
                        multiLine.TrimEnd();
                    }
                    currentTestCase.Tags = fileLines[j] + multiLine;
                    continue;
                }

                // for loops
                if (!fileLines[i].Trim().ToLower().StartsWith(":for")
                                    && !fileLines[i].Trim().ToLower().StartsWith("\\"))
                {
                    var j = i;
                    var multiLine = "";
                    if (i + 1 < fileLines.Length)
                    {
                        while (fileLines[i + 1].Trim().StartsWith("..."))
                        {
                            multiLine += "  " + fileLines[i + 1].Replace("...", "");
                            i++;
                            if (i + 1 >= fileLines.Length) break;
                        }
                        multiLine.TrimEnd();
                    }

                    if (fileLines[j].Trim().StartsWith("#")) continue;
                    if (currentTestCase.Steps == null) currentTestCase.Steps = new List<Keyword>();
                    currentTestCase.Steps.Add(new Keyword(fileLines[j] + multiLine,
                        FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot", libraries, null));
                    continue;
                }
                else
                {
                    if (fileLines[i].Trim().ToLower().StartsWith(":for"))
                    {
                        if (fileLines[i].Trim().ToLower().Contains("range"))
                        {
                            var temp = new Keyword(null);
                            temp.CopyKeyword(SuggestionsClass.GetForLoop(KeywordType.ForLoopInRange));
                            var splitKeyword = fileLines[i].Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                            if (splitKeyword.Length == 1)
                                splitKeyword = fileLines[i].Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                            if (splitKeyword.Length >= 5)
                            {
                                temp.Params[0].Value = splitKeyword[1];
                                temp.Params[1].Value = splitKeyword[3];
                                temp.Params[2].Value = splitKeyword[4];
                            }
                            currentTestCase.Steps.Add(temp);
                            currentTestCase.Steps[currentTestCase.Steps.Count - 1].ForLoopKeywords = new List<Keyword>();
                        }
                        else
                        {
                            var temp = new Keyword(null);
                            temp.CopyKeyword(SuggestionsClass.GetForLoop(KeywordType.ForLoopElements));
                            var splitKeyword = fileLines[i].Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                            if (splitKeyword.Length == 1)
                                splitKeyword = fileLines[i].Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                            if (splitKeyword.Length >= 4)
                            {
                                temp.Params[0].Value = splitKeyword[1];
                                temp.Params[1].Value = splitKeyword[3];
                            }
                            currentTestCase.Steps.Add(temp);
                            currentTestCase.Steps[currentTestCase.Steps.Count - 1].ForLoopKeywords = new List<Keyword>();
                        }
                    }
                    else
                    {
                        if (currentTestCase.Steps == null) currentTestCase.Steps = new List<Keyword>();
                        currentTestCase.Steps[currentTestCase.Steps.Count - 1].ForLoopKeywords.Add(
                            new Keyword(fileLines[i].Trim().Remove(0, 1).Trim(),
                                FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot", libraries, null));
                    }
                    continue;
                }
            }

            if (currentTestCase.Name != null)
                if (!listOfTestCases.Contains(currentTestCase))
                    listOfTestCases.Add(currentTestCase);

            return listOfTestCases;
        }

        // returns the list of keywords in specific file
        public List<Keyword> ReadKeywordsFromFile()
        {
            var listOfKeywords = new List<Keyword>();

            var index = HasTag(FormType.Keyword);
            if (index == -1) return listOfKeywords;

            var currentKeyword = new Keyword(null)
            {
                OutputFilePath = fileName
            };

            for (int i = index + 1; i < fileLines.Length; i++)
            {
                if (fileLines[i].StartsWith("*"))
                {
                    if (currentKeyword.Name != null)
                        listOfKeywords.Add(currentKeyword);
                    break;
                }

                if (fileLines[i].Trim().Length == 0) continue;

                if (!fileLines[i].StartsWith(" ") && !fileLines[i].StartsWith("\t"))
                {
                    if (currentKeyword.Name != null)
                    {
                        listOfKeywords.Add(currentKeyword);
                        currentKeyword = new Keyword(null)
                        {
                            OutputFilePath = fileName
                        };
                    }
                    currentKeyword.Name = fileLines[i].Trim();
                    continue;
                }

                if (fileLines[i].Trim().StartsWith("[Documentation]"))
                {
                    currentKeyword.Documentation = fileLines[i];
                    for (int p = i + 1; p < fileLines.Length; p++)
                    {
                        if (!fileLines[p].Trim().StartsWith(".")) break;
                        currentKeyword.Documentation += fileLines[p].Trim().Replace("...","");
                    }
                    continue;
                }

                var j = i;
                var multiLine = "";
                if (fileLines[i].Trim().StartsWith("[Arguments]"))
                {
                    if (i + 1 < fileLines.Length)
                    {
                        while (fileLines[i + 1].Trim().StartsWith("..."))
                        {
                            multiLine += "  " + fileLines[i + 1].Replace("...", "");
                            i++;
                            if (i + 1 >= fileLines.Length) break;
                        }
                        multiLine.TrimEnd();
                    }

                    var splitKeyword = (fileLines[j] + multiLine).Replace("[Arguments]", "").Trim().Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
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
                    currentKeyword.Arguments = fileLines[j];
                    continue;
                }

                // for loops
                j = i;
                multiLine = "";
                if (i + 1 < fileLines.Length)
                {
                    while (fileLines[i + 1].Trim().StartsWith("..."))
                    {
                        multiLine += "  " + fileLines[i + 1].Replace("...", "");
                        i++;
                        if (i + 1 >= fileLines.Length) break;
                    }
                    multiLine.TrimEnd();
                }
                
                if (!fileLines[j].Trim().ToLower().StartsWith(":for")
                                && !fileLines[j].Trim().ToLower().StartsWith("\\"))
                {
                    if (fileLines[j].Trim().StartsWith("#")) continue;
                    if (currentKeyword.Keywords == null) currentKeyword.Keywords = new List<Keyword>();
                    currentKeyword.Keywords.Add(new Keyword(fileLines[j] + multiLine,
                        FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot", libraries, currentKeyword));
                    if (currentKeyword.Keywords[currentKeyword.Keywords.Count - 1].IsRecursive(currentKeyword.Keywords[currentKeyword.Keywords.Count - 1]))
                    {
                        currentKeyword.Keywords[currentKeyword.Keywords.Count - 1].Recursive = true;
                    }
                    continue;
                }
                else
                {
                    if (fileLines[j].Trim().ToLower().StartsWith(":for"))
                    {
                        if (fileLines[j].Trim().ToLower().Contains("range"))
                        {
                            var temp = new Keyword(currentKeyword);
                            temp.CopyKeyword(SuggestionsClass.GetForLoop(KeywordType.ForLoopInRange));
                            var splitKeyword = fileLines[j].Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                            if (splitKeyword.Length == 1)
                                splitKeyword = fileLines[j].Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                            temp.Params[0].Value = splitKeyword[1];
                            temp.Params[1].Value = splitKeyword[3];
                            temp.Params[2].Value = splitKeyword[4];
                            currentKeyword.Keywords.Add(temp);
                            currentKeyword.Keywords[currentKeyword.Keywords.Count - 1].ForLoopKeywords = new List<Keyword>();
                        }
                        else
                        {
                            var temp = new Keyword(currentKeyword);
                            temp.CopyKeyword(SuggestionsClass.GetForLoop(KeywordType.ForLoopElements));
                            var splitKeyword = fileLines[j].Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                            if (splitKeyword.Length == 1)
                                splitKeyword = fileLines[j].Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                            temp.Params[0].Value = splitKeyword[1];
                            temp.Params[1].Value = splitKeyword[3];
                            currentKeyword.Keywords.Add(temp);
                            currentKeyword.Keywords[currentKeyword.Keywords.Count - 1].ForLoopKeywords = new List<Keyword>();
                        }
                    }
                    else
                    {
                        if (currentKeyword.Keywords == null) currentKeyword.Keywords = new List<Keyword>();
                        currentKeyword.Keywords[currentKeyword.Keywords.Count - 1].ForLoopKeywords.Add(
                            new Keyword(fileLines[j].Trim().Remove(0, 1).Trim(),
                                FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot", libraries, currentKeyword));
                        if (currentKeyword.Keywords[currentKeyword.Keywords.Count - 1].IsRecursive(currentKeyword.Keywords[currentKeyword.Keywords.Count - 1]))
                        {
                            currentKeyword.Keywords[currentKeyword.Keywords.Count - 1].Recursive = true;
                        }
                    }
                    continue;
                }
            }

            if (currentKeyword.Name != null)
                if (!listOfKeywords.Contains(currentKeyword))
                    listOfKeywords.Add(currentKeyword);

            return listOfKeywords;
        }

        // returns the index of the specific tag - keyword / test cases / settings / variables
        public List<SuiteSettings> ReadSettingsFromFile()
        {
            var suiteSettings = new List<SuiteSettings>();
            foreach (var fileName in FilesAndFolderStructure.GetFullSavedFiles(FolderType.Tests))
            {
                var currentSuiteSettings = new SuiteSettings(fileName.Replace(FilesAndFolderStructure.GetFolder(FolderType.Root), ""))
                {
                    Documentation = RobotFileHandler.OccurenceInSettings(fileName, "Documentation").Replace("Documentation", "").Trim(),
                    TestSetup = new Keyword(RobotFileHandler.OccurenceInSettings(fileName, "Test Setup").Replace("Test Setup", "").Trim(), fileName, libraries, null)
                };
                currentSuiteSettings.TestTeardown = new Keyword(RobotFileHandler.OccurenceInSettings(fileName, "Test Teardown").Replace("Test Teardown", "").Trim(), fileName, libraries, null);
                currentSuiteSettings.SuiteSetup = new Keyword(RobotFileHandler.OccurenceInSettings(fileName, "Suite Setup").Replace("Suite Setup", "").Trim(), fileName, libraries, null);
                currentSuiteSettings.SuiteTeardown = new Keyword(RobotFileHandler.OccurenceInSettings(fileName, "Suite Teardown").Replace("Suite Teardown", "").Trim(), fileName, libraries, null);
                currentSuiteSettings.Overwrite = true;
                suiteSettings.Add(currentSuiteSettings);
            }
            return suiteSettings;
        }

        // returns the index of the specific tag - keyword / test cases / settings / variables
        public int HasTag(FormType formType)
        {
            var index = -1;

            for (var i = 0; i < fileLines.Length; i++)
                if (fileLines[i].StartsWith("*"))
                    if (fileLines[i].ToLower().Contains(formType.ToString().ToLower()))
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

        // gets the libs in the file
        public List<string> GetLibs()
        {
            // find all libraries
            var libraries = new List<string>
            {
                "BuiltIn"
            };

            if (fileLines.Length != 0)
            {
                var start = false;
                foreach (var line in fileLines)
                {
                    if (start && line.StartsWith("***"))
                        break;
                    if (line.StartsWith("*** Settings ***"))
                        start = true;

                    if (start && line.StartsWith("Library"))
                    {
                        if (!libraries.Contains(line.Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[1]))
                            libraries.Add(line.Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[1].Trim());
                    }
                }
            }

            foreach (var resourceFileName in resources)
            {
                var includedFileLines = File.ReadAllLines(resourceFileName);
                if (includedFileLines.Length == 0) continue;
                var start = false;
                foreach (var line in includedFileLines)
                {
                    if (start && line.StartsWith("***"))
                        break;
                    if (line.StartsWith("*** Settings ***"))
                        start = true;

                    if (start && line.StartsWith("Library"))
                    {
                        if (!libraries.Contains(line.Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[1]))
                            libraries.Add(line.Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[1].Trim());
                    }
                }
            }

            return libraries;
        }

        // gets the resources used in the file
        public List<string> GetResourcesFromFile()
        {
            // find all resources
            var resources = new List<string>
            {
                fileName
            };

            if (fileLines.Length == 0) return resources;
            var start = false;
            foreach (var line in fileLines)
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
                            resources.Add(FilesAndFolderStructure.GetFolder(FolderType.Root) + "\\" + line.Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("/", "\\"));
                    }
                    else
                    {
                        resources.Add(fileName.Remove(fileName.LastIndexOf('\\') + 1, fileName.Length - fileName.LastIndexOf('\\') - 1) + line.Replace("./", "").Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("/", "\\"));
                    }
                }
            }
            return resources;
        }

        // to string returns filename
        public override string ToString()
        {
            return fileName;
        }
    }
}
