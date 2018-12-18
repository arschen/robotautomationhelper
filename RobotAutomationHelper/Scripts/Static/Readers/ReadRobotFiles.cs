using System;
using System.Collections.Generic;
using System.IO;
using RobotAutomationHelper.Forms;
using RobotAutomationHelper.Scripts.Objects;
using RobotAutomationHelper.Scripts.Static.Writers;

namespace RobotAutomationHelper.Scripts.Static.Readers
{
    internal static class ReadRobotFiles
    {
        private static List<TestCase> _testCases;
        private static List<Keyword> _currentTestCaseTestSteps;
        private static string _currentTestCaseTags;
        private static string _currentTestCaseDocumentation;
        private static string _currentTestCase;

        // returns the index of the specific tag - keyword / test cases / settings / variables
        internal static List<TestCase> ReadAllTests()
        {
            _testCases = new List<TestCase>();
            _currentTestCaseTestSteps = new List<Keyword>();
            _currentTestCase = "";

            foreach (var fileName in FilesAndFolderStructure.GetFullSavedFiles(FolderType.Tests))
            {
                if (!File.Exists(fileName)) continue;
                var arrLine = File.ReadAllLines(fileName);

                if (arrLine.Length == 0) continue;
                var start = false;
                for (var i = 0; i < arrLine.Length; i++)
                {
                    if (start && arrLine[i].StartsWith("***"))
                    {
                        if (!_currentTestCase.Equals(""))
                        {
                            //Setup test creation for previous Test case
                            AddTestCaseAndResetValues(fileName);
                            _currentTestCase = !_currentTestCase.Equals(arrLine[i]) ? arrLine[i] : "";
                        }
                        else
                        {
                            _currentTestCase = arrLine[i];
                        }
                        break;
                    }

                    if (start && !arrLine[i].StartsWith(" ") && !arrLine[i].StartsWith("\t") && !arrLine[i].Trim().Equals(""))
                    {
                        if (_currentTestCase != "")
                            AddTestCaseAndResetValues(fileName);
                        _currentTestCase = arrLine[i];
                    }
                    else
                    {
                        if (start && !arrLine[i].Trim().Equals(""))
                        {
                            if (arrLine[i].Trim().StartsWith("[Documentation]"))
                                _currentTestCaseDocumentation = arrLine[i];
                            else
                            if (arrLine[i].Trim().StartsWith("[Tags]"))
                                _currentTestCaseTags = arrLine[i];
                            else
                            {
                                if (!arrLine[i].Trim().ToLower().StartsWith(":for")
                                    && !arrLine[i].Trim().ToLower().StartsWith("\\"))
                                {
                                    var j = i;
                                    var multiLine = "";
                                    if (i + 1 < arrLine.Length)
                                    {
                                        while (arrLine[i + 1].Trim().StartsWith("..."))
                                        {
                                            multiLine += "  " + arrLine[i + 1].Replace("...", "");
                                            i++;
                                            if (i + 1 >= arrLine.Length) break;
                                        }
                                        multiLine.TrimEnd();
                                    }

                                    _currentTestCaseTestSteps.Add(new Keyword(arrLine[j] + multiLine,
                                        FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot", GetLibs(fileName), null));
                                    AddKeywordsFromKeyword(_currentTestCaseTestSteps[_currentTestCaseTestSteps.Count - 1],
                                        GetResourcesFromFile(fileName));
                                }
                                else
                                {
                                    if (arrLine[i].Trim().ToLower().StartsWith(":for"))
                                    {
                                        if (arrLine[i].Trim().ToLower().Contains("range"))
                                        {
                                            var temp = new Keyword(null);
                                            temp.CopyKeyword(SuggestionsClass.GetForLoop(KeywordType.ForLoopInRange));
                                            var splitKeyword = arrLine[i].Split(new [] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                                            if (splitKeyword.Length == 1)
                                                splitKeyword = arrLine[i].Split(new [] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                                            if (splitKeyword.Length >= 5)
                                            {
                                                temp.Params[0].Value = splitKeyword[1];
                                                temp.Params[1].Value = splitKeyword[3];
                                                temp.Params[2].Value = splitKeyword[4];
                                            }
                                            _currentTestCaseTestSteps.Add(temp);
                                            _currentTestCaseTestSteps[_currentTestCaseTestSteps.Count - 1].ForLoopKeywords = new List<Keyword>();
                                        }
                                        else
                                        {
                                            var temp = new Keyword(null);
                                            temp.CopyKeyword(SuggestionsClass.GetForLoop(KeywordType.ForLoopElements));
                                            var splitKeyword = arrLine[i].Split(new [] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                                            if (splitKeyword.Length == 1)
                                                splitKeyword = arrLine[i].Split(new [] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                                            if (splitKeyword.Length >= 4)
                                            {
                                                temp.Params[0].Value = splitKeyword[1];
                                                temp.Params[1].Value = splitKeyword[3];
                                            }
                                            _currentTestCaseTestSteps.Add(temp);
                                            _currentTestCaseTestSteps[_currentTestCaseTestSteps.Count - 1].ForLoopKeywords = new List<Keyword>();
                                        }
                                    }
                                    else
                                    {
                                        _currentTestCaseTestSteps[_currentTestCaseTestSteps.Count - 1].ForLoopKeywords.Add(
                                            new Keyword(arrLine[i].Trim().Remove(0,1).Trim(),
                                                FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot", GetLibs(fileName), null));
                                        AddKeywordsFromKeyword(_currentTestCaseTestSteps[_currentTestCaseTestSteps.Count - 1].ForLoopKeywords[_currentTestCaseTestSteps[_currentTestCaseTestSteps.Count - 1].ForLoopKeywords.Count - 1],
                                            GetResourcesFromFile(fileName));
                                    }
                                }
                            }
                        }
                    }

                    if (i + 1 == arrLine.Length)
                    {
                        if (!_currentTestCase.Equals(""))
                        {
                            //Setup test creation for previous Test case
                            AddTestCaseAndResetValues(fileName);
                            _currentTestCase = "";
                        }
                    }

                    if (arrLine[i].ToLower().Trim().StartsWith("*** test cases ***"))
                        start = true;
                }
            }
            return _testCases;
        }

        private static void AddKeywordsFromKeyword(Keyword keyword, List<string> filesList)
        {
            foreach (var fileName in filesList)
            {
                var continueLoop = !(keyword.IncludeImportFile && !fileName.Contains(keyword.ImportFileName + ".robot"));
                if (!continueLoop) continue;
                var index = RobotFileHandler.LocationOfTestCaseOrKeywordInFile(fileName, keyword.Name == null? "":keyword.Name.Trim(), FormType.Keyword);
                if (index == -1) continue;
                keyword.OutputFilePath = fileName;
                var arrLine = File.ReadAllLines(fileName);
                for (var i = index; i < arrLine.Length; i++)
                {
                    if (!arrLine[i].StartsWith(" ") && !arrLine[i].StartsWith("\t") && !arrLine[i].Trim().Equals(""))
                    {
                        if (i != index)
                            break;
                        keyword.Name = arrLine[i];
                    }
                    else
                    if (!arrLine[i].Trim().Equals(""))
                    {
                        if (arrLine[i].Trim().StartsWith("[Documentation]"))
                            keyword.Documentation = arrLine[i];
                        else
                        if (arrLine[i].Trim().StartsWith("[Arguments]"))
                        {
                            var j = i;
                            var multiLine = "";
                            if (i + 1 < arrLine.Length)
                            {
                                while (arrLine[i + 1].Trim().StartsWith("..."))
                                {
                                    multiLine += "  " + arrLine[i + 1].Replace("...", "");
                                    i++;
                                    if (i + 1 >= arrLine.Length) break;
                                }
                                multiLine.TrimEnd();
                            }

                            var splitKeyword = (arrLine[j] + multiLine).Replace("[Arguments]", "").Trim().Split(new [] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                            for (var counter = 0; counter < splitKeyword.Length; counter++)
                            {
                                if (!splitKeyword[counter].Contains("="))
                                {
                                    if (keyword.Params != null)
                                        keyword.Params[counter].Name = splitKeyword[counter];
                                    else
                                    {
                                        keyword.Params = new List<Param>(){
                                            new Param(splitKeyword[counter], "")
                                        };
                                    }
                                }
                                else
                                {
                                    // check if after splitting the first string matches any param name
                                    var temp = splitKeyword[counter].Split('=');
                                    var toAdd = true;
                                    foreach (var par in keyword.Params)
                                    {
                                        if (!par.Name.Equals(temp[0])) continue;
                                        toAdd = false;
                                        break;
                                    }
                                    if (toAdd)
                                        keyword.Params.Add(new Param(temp[0], temp[1]));
                                }
                            }
                            keyword.Arguments = arrLine[j];
                        }
                        else
                        {
                            var j = i;
                            var multiLine = "";
                            if (i + 1 < arrLine.Length)
                            {
                                while (arrLine[i + 1].Trim().StartsWith("..."))
                                {
                                    multiLine += "  " + arrLine[i + 1].Replace("...", "");
                                    i++;
                                    if (i + 1 >= arrLine.Length) break;
                                }
                                multiLine.TrimEnd();
                            }

                            if (keyword.Keywords == null)
                                keyword.Keywords = new List<Keyword>();
                            if (!arrLine[j].Trim().ToLower().StartsWith(":for")
                                && !arrLine[j].Trim().ToLower().StartsWith("\\"))
                            {
                                keyword.Keywords.Add(new Keyword(arrLine[j] + multiLine,
                                    FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot", GetLibs(fileName), keyword));
                                if (keyword.Keywords[keyword.Keywords.Count - 1].IsRecursive(keyword.Keywords[keyword.Keywords.Count - 1]))
                                {
                                    keyword.Keywords[keyword.Keywords.Count - 1].Recursive = true;
                                }
                                else
                                    AddKeywordsFromKeyword(keyword.Keywords[keyword.Keywords.Count - 1],
                                        GetResourcesFromFile(fileName));
                            }
                            else
                            {
                                if (arrLine[j].Trim().ToLower().StartsWith(":for"))
                                {
                                    if (arrLine[j].Trim().ToLower().Contains("range"))
                                    {
                                        var temp = new Keyword(keyword);
                                        temp.CopyKeyword(SuggestionsClass.GetForLoop(KeywordType.ForLoopInRange));
                                        var splitKeyword = arrLine[j].Split(new [] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                                        if (splitKeyword.Length == 1)
                                            splitKeyword = arrLine[j].Split(new [] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                                        temp.Params[0].Value = splitKeyword[1];
                                        temp.Params[1].Value = splitKeyword[3];
                                        temp.Params[2].Value = splitKeyword[4];
                                        keyword.Keywords.Add(temp);
                                        keyword.Keywords[keyword.Keywords.Count - 1].ForLoopKeywords = new List<Keyword>();
                                    }
                                    else
                                    {
                                        var temp = new Keyword(keyword);
                                        temp.CopyKeyword(SuggestionsClass.GetForLoop(KeywordType.ForLoopElements));
                                        var splitKeyword = arrLine[j].Split(new [] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                                        if (splitKeyword.Length == 1)
                                            splitKeyword = arrLine[j].Split(new [] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                                        temp.Params[0].Value = splitKeyword[1];
                                        temp.Params[1].Value = splitKeyword[3];
                                        keyword.Keywords.Add(temp);
                                        keyword.Keywords[keyword.Keywords.Count - 1].ForLoopKeywords = new List<Keyword>();
                                    }
                                }
                                else
                                {
                                    keyword.Keywords[keyword.Keywords.Count - 1].ForLoopKeywords.Add(
                                        new Keyword(arrLine[j].Trim().Remove(0, 1).Trim(),
                                            FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot", GetLibs(fileName), keyword));
                                    if (keyword.Keywords[keyword.Keywords.Count - 1].IsRecursive(keyword.Keywords[keyword.Keywords.Count - 1]))
                                    {
                                        keyword.Keywords[keyword.Keywords.Count - 1].Recursive = true;
                                    }
                                    else
                                        AddKeywordsFromKeyword(keyword.Keywords[keyword.Keywords.Count - 1].ForLoopKeywords[keyword.Keywords[keyword.Keywords.Count - 1].ForLoopKeywords.Count - 1],
                                            GetResourcesFromFile(fileName));
                                }
                            }
                        }
                    }
                }
                break;
            }
        }

        // returns the index of the specific tag - keyword / test cases / settings / variables
        internal static List<SuiteSettings> ReadAllSettings()
        {
            var suiteSettings = new List<SuiteSettings>();
            foreach (var fileName in FilesAndFolderStructure.GetFullSavedFiles(FolderType.Tests))
            {
                var currentSuiteSettings = new SuiteSettings(fileName.Replace(FilesAndFolderStructure.GetFolder(FolderType.Root), ""))
                {
                    Documentation = RobotFileHandler.OccurenceInSettings(fileName, "Documentation").Replace("Documentation", "").Trim(),
                    TestSetup = new Keyword(RobotFileHandler.OccurenceInSettings(fileName, "Test Setup").Replace("Test Setup", "").Trim(), fileName, GetLibs(fileName), null)
                };
                AddKeywordsFromKeyword(currentSuiteSettings.TestSetup, GetResourcesFromFile(fileName));
                currentSuiteSettings.TestTeardown = new Keyword(RobotFileHandler.OccurenceInSettings(fileName, "Test Teardown").Replace("Test Teardown", "").Trim(), fileName, GetLibs(fileName), null);
                AddKeywordsFromKeyword(currentSuiteSettings.TestTeardown, GetResourcesFromFile(fileName));
                currentSuiteSettings.SuiteSetup = new Keyword(RobotFileHandler.OccurenceInSettings(fileName, "Suite Setup").Replace("Suite Setup", "").Trim(), fileName, GetLibs(fileName), null);
                AddKeywordsFromKeyword(currentSuiteSettings.SuiteSetup, GetResourcesFromFile(fileName));
                currentSuiteSettings.SuiteTeardown = new Keyword(RobotFileHandler.OccurenceInSettings(fileName, "Suite Teardown").Replace("Suite Teardown", "").Trim(), fileName, GetLibs(fileName), null);
                AddKeywordsFromKeyword(currentSuiteSettings.SuiteTeardown, GetResourcesFromFile(fileName));
                currentSuiteSettings.Overwrite = true;
                suiteSettings.Add(currentSuiteSettings);
            }
            return suiteSettings;
        }

        // returns the index of the specific tag - keyword / test cases / settings / variables
        internal static List<Variables> ReadAllVariables()
        {
            var listOfVariables = new List<Variables>();
            foreach (var fileName in FilesAndFolderStructure.GetFullSavedFiles(FolderType.Root))
            {
                if (!File.Exists(fileName)) continue;
                var arrLine = File.ReadAllLines(fileName);

                var index = RobotFileHandler.HasTag(fileName, FormType.Variable);
                if (index == -1) continue;

                var names = new List<string>();
                for (int i = index + 1; i < arrLine.Length; i++)
                {
                    if (arrLine[i].StartsWith("***")) break;
                    if (arrLine[i].Trim().Length == 0) continue;
                    if (StringAndListOperations.StartsWithVariable(arrLine[i].Trim())) names.Add(arrLine[i].Trim());
                }

                if (names.Count == 0) continue;
                var currentSuiteSettings = new Variables(names, fileName);
                listOfVariables.Add(currentSuiteSettings);
            }
            return listOfVariables;
        }

        private static List<string> GetResourcesFromFile(string fileName)
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

        internal static List<string> GetLibs(string fileName)
        {
            // find all libraries
            var libraries = new List<string>
            {
                "BuiltIn"
            };

            if (File.Exists(fileName))
            {
                var arrLine = File.ReadAllLines(fileName);
                if (arrLine.Length != 0)
                {
                    var start = false;
                    foreach (var line in arrLine)
                    {
                        if (start && line.StartsWith("***"))
                            break;
                        if (line.StartsWith("*** Settings ***"))
                            start = true;

                        if (start && line.StartsWith("Library"))
                        {
                            libraries.Add(line.Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[1]);
                        }
                    }
                }

                foreach (var resourceFileName in GetResourcesFromFile(fileName))
                {
                    arrLine = File.ReadAllLines(resourceFileName);
                    if (arrLine.Length == 0) continue;
                    var start = false;
                    foreach (var line in arrLine)
                    {
                        if (start && line.StartsWith("***"))
                            break;
                        if (line.StartsWith("*** Settings ***"))
                            start = true;

                        if (start && line.StartsWith("Library"))
                        {
                            libraries.Add(line.Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[1]);
                        }
                    }
                }
            }

            return libraries;
        }

        private static void AddTestCaseAndResetValues(string fileName)
        {
            _testCases.Add(new TestCase(_currentTestCase, _currentTestCaseDocumentation, _currentTestCaseTags, _currentTestCaseTestSteps, fileName, true));
            _currentTestCaseTestSteps = new List<Keyword>();
            _currentTestCaseDocumentation = null;
            _currentTestCase = "";
            _currentTestCaseTags = null;
        }
    }
}
