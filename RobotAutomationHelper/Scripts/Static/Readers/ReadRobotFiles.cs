using System;
using System.Collections.Generic;
using System.IO;

namespace RobotAutomationHelper.Scripts
{
    internal static class ReadRobotFiles
    {
        private static List<TestCase> TestCases;
        private static List<Keyword> currentTestCaseTestSteps;
        private static string currentTestCaseTags;
        private static string currentTestCaseDocumentation;
        private static string currentTestCase;

        // returns the index of the specific tag - keyword / test cases / settings / variables
        internal static List<TestCase> ReadAllTests()
        {
            TestCases = new List<TestCase>();
            currentTestCaseTestSteps = new List<Keyword>();
            currentTestCaseTags = "";
            currentTestCaseDocumentation = "";
            currentTestCase = "";
            string[] arrLine;

            foreach (string fileName in FilesAndFolderStructure.GetFullSavedFiles(FolderType.Tests))
            {
                if (File.Exists(fileName))
                {
                    arrLine = File.ReadAllLines(fileName);

                    if (arrLine.Length != 0)
                    {
                        bool start = false;
                        for (int i = 0; i < arrLine.Length; i++)
                        {
                            if (start && arrLine[i].StartsWith("***"))
                            {
                                if (!currentTestCase.Equals(""))
                                {
                                    //Setup test creation for previous Test case
                                    AddTestCaseAndResetValues(fileName);
                                    if (!currentTestCase.Equals(arrLine[i]))
                                        currentTestCase = arrLine[i];
                                    else
                                        currentTestCase = "";
                                }
                                else
                                {
                                    currentTestCase = arrLine[i];
                                }
                                break;
                            }

                            if (start && !arrLine[i].StartsWith(" ") && !arrLine[i].StartsWith("\t"))
                            {
                                currentTestCase = arrLine[i];
                            }
                            else
                            {
                                if (start)
                                {
                                    if (arrLine[i].Trim().StartsWith("[Documentation]"))
                                        currentTestCaseDocumentation = arrLine[i];
                                    else
                                    if (arrLine[i].Trim().StartsWith("[Tags]"))
                                        currentTestCaseTags = arrLine[i];
                                    else
                                    {
                                        if (!arrLine[i].Trim().ToLower().StartsWith(":for")
                                            && !arrLine[i].Trim().ToLower().StartsWith("\\"))
                                        {
                                            currentTestCaseTestSteps.Add(new Keyword(arrLine[i],
                                                FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot", true));
                                            AddKeywordsFromKeyword(currentTestCaseTestSteps[currentTestCaseTestSteps.Count - 1],
                                                GetResourcesFromFile(arrLine));
                                        }
                                        else
                                        {
                                            if (arrLine[i].Trim().ToLower().StartsWith(":for"))
                                            {
                                                if (arrLine[i].Trim().ToLower().Contains("range"))
                                                {
                                                    Keyword temp = new Keyword();
                                                    temp.CopyKeyword(SuggestionsClass.GetForLoop(KeywordType.FOR_LOOP_IN_RANGE));
                                                    string[] splitKeyword = arrLine[i].Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                                                    if (splitKeyword.Length == 1)
                                                        splitKeyword = arrLine[i].Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                                                    temp.Params[0].Value = splitKeyword[1];
                                                    temp.Params[1].Value = splitKeyword[3];
                                                    temp.Params[2].Value = splitKeyword[4];
                                                    currentTestCaseTestSteps.Add(temp);
                                                    currentTestCaseTestSteps[currentTestCaseTestSteps.Count - 1].ForLoopKeywords = new List<Keyword>();
                                                }
                                                else
                                                {
                                                    Keyword temp = new Keyword();
                                                    temp.CopyKeyword(SuggestionsClass.GetForLoop(KeywordType.FOR_LOOP_ELEMENTS));
                                                    string[] splitKeyword = arrLine[i].Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                                                    if (splitKeyword.Length == 1)
                                                        splitKeyword = arrLine[i].Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                                                    temp.Params[0].Value = splitKeyword[1];
                                                    temp.Params[1].Value = splitKeyword[3];
                                                    currentTestCaseTestSteps.Add(temp);
                                                    currentTestCaseTestSteps[currentTestCaseTestSteps.Count - 1].ForLoopKeywords = new List<Keyword>();
                                                }
                                            }
                                            else
                                            {
                                                currentTestCaseTestSteps[currentTestCaseTestSteps.Count - 1].ForLoopKeywords.Add(
                                                    new Keyword(arrLine[i].Trim().Remove(0,1).Trim(),
                                                    FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot", true));
                                                AddKeywordsFromKeyword(currentTestCaseTestSteps[currentTestCaseTestSteps.Count - 1].ForLoopKeywords[currentTestCaseTestSteps[currentTestCaseTestSteps.Count - 1].ForLoopKeywords.Count - 1],
                                                    GetResourcesFromFile(arrLine));
                                            }
                                        }
                                    }
                                }
                            }

                            if (i + 1 == arrLine.Length)
                            {
                                if (!currentTestCase.Equals(""))
                                {
                                    //Setup test creation for previous Test case
                                    AddTestCaseAndResetValues(fileName);
                                    if (!currentTestCase.Equals(arrLine[i]))
                                        currentTestCase = arrLine[i];
                                    else
                                        currentTestCase = "";
                                }
                                else
                                {
                                    currentTestCase = arrLine[i];
                                }
                            }

                            if (arrLine[i].ToLower().Trim().StartsWith("*** test cases ***"))
                                start = true;
                        }
                    }
                }
            }
            return TestCases;
        }

        private static void AddKeywordsFromKeyword(Keyword keyword, List<string> filesList)
        {
            foreach (string fileName in filesList)
            {
                int index = RobotFileHandler.LocationOfTestCaseOrKeywordInFile(fileName, keyword.Name.Trim(), FormType.Keyword);
                if (index != -1)
                {
                    keyword.OutputFilePath = fileName;
                    string[] arrLine = File.ReadAllLines(fileName);
                    for (int i = index; i < arrLine.Length; i++)
                    {
                        if (!arrLine[i].StartsWith(" ") && !arrLine[i].StartsWith("\t"))
                        {
                            if (i != index)
                                break;
                            else
                                keyword.Name = arrLine[i];
                        }
                        else
                        {
                            if (arrLine[i].Trim().StartsWith("[Documentation]"))
                                keyword.Documentation = arrLine[i];
                            else
                            if (arrLine[i].Trim().StartsWith("[Arguments]"))
                            {
                                string[] splitKeyword = arrLine[i].Replace("[Arguments]", "").Trim().Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                                for (int counter = 0; counter < splitKeyword.Length; counter++)
                                {
                                    if (!splitKeyword[counter].Contains("="))
                                        keyword.Params[counter].Name = splitKeyword[counter];
                                    else
                                    {
                                        // check if after spliting the first string matches any param name
                                        string[] temp = splitKeyword[counter].Split('=');
                                        bool toAdd = true;
                                        foreach (Param par in keyword.Params)
                                        {
                                            if (par.Name.Equals(temp[0]))
                                            {
                                                toAdd = false;
                                                break;
                                            }
                                        }
                                        if (toAdd)
                                            keyword.Params.Add(new Param(temp[0], temp[1]));
                                    }
                                }
                                keyword.Arguments = arrLine[i];
                            }
                            else
                            {
                                if (keyword.Keywords == null)
                                    keyword.Keywords = new List<Keyword>();
                                if (!arrLine[i].Trim().ToLower().StartsWith(":for")
                                            && !arrLine[i].Trim().ToLower().StartsWith("\\"))
                                {
                                    keyword.Keywords.Add(new Keyword(arrLine[i],
                                        FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot", true));
                                    AddKeywordsFromKeyword(keyword.Keywords[keyword.Keywords.Count - 1],
                                        GetResourcesFromFile(arrLine));
                                }
                                else
                                {
                                    if (arrLine[i].Trim().ToLower().StartsWith(":for"))
                                    {
                                        if (arrLine[i].Trim().ToLower().Contains("range"))
                                        {
                                            Keyword temp = new Keyword();
                                            temp.CopyKeyword(SuggestionsClass.GetForLoop(KeywordType.FOR_LOOP_IN_RANGE));
                                            string[] splitKeyword = arrLine[i].Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                                            if (splitKeyword.Length == 1)
                                                splitKeyword = arrLine[i].Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                                            temp.Params[0].Value = splitKeyword[1];
                                            temp.Params[1].Value = splitKeyword[3];
                                            temp.Params[2].Value = splitKeyword[4];
                                            keyword.Keywords.Add(temp);
                                            keyword.Keywords[keyword.Keywords.Count - 1].ForLoopKeywords = new List<Keyword>();
                                        }
                                        else
                                        {
                                            Keyword temp = new Keyword();
                                            temp.CopyKeyword(SuggestionsClass.GetForLoop(KeywordType.FOR_LOOP_ELEMENTS));
                                            string[] splitKeyword = arrLine[i].Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                                            if (splitKeyword.Length == 1)
                                                splitKeyword = arrLine[i].Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                                            temp.Params[0].Value = splitKeyword[1];
                                            temp.Params[1].Value = splitKeyword[3];
                                            keyword.Keywords.Add(temp);
                                            keyword.Keywords[keyword.Keywords.Count - 1].ForLoopKeywords = new List<Keyword>();
                                        }
                                    }
                                    else
                                    {
                                        keyword.Keywords[keyword.Keywords.Count - 1].ForLoopKeywords.Add(
                                            new Keyword(arrLine[i].Trim().Remove(0, 1).Trim(),
                                            FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot", true));
                                        AddKeywordsFromKeyword(keyword.Keywords[keyword.Keywords.Count - 1].ForLoopKeywords[keyword.Keywords[keyword.Keywords.Count - 1].ForLoopKeywords.Count - 1],
                                            GetResourcesFromFile(arrLine));
                                    }
                                }
                            }
                        }
                    }
                    break;
                }
            }
        }

        private static List<string> GetResourcesFromFile(string[] arrLine)
        {
            // find all resources
            List<string> Resources = new List<string>();
            if (arrLine.Length != 0)
            {
                bool start = false;
                for (int i = 0; i < arrLine.Length; i++)
                {
                    if (start && arrLine[i].StartsWith("***"))
                        break;
                    if (arrLine[i].StartsWith("*** Settings ***"))
                        start = true;

                    if (start && arrLine[i].StartsWith("Resource"))
                    {
                        Resources.Add(FilesAndFolderStructure.GetFolder(FolderType.Root) + arrLine[i].Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("/","\\"));
                    }
                }
            }
            return Resources;
        }

        private static void AddTestCaseAndResetValues(string fileName)
        {
            TestCases.Add(new TestCase(currentTestCase, currentTestCaseDocumentation, currentTestCaseTags, currentTestCaseTestSteps, fileName, true));
            currentTestCaseTestSteps = new List<Keyword>();
            currentTestCaseDocumentation = "";
            currentTestCase = "";
            currentTestCaseTags = "";
        }
    }
}
