﻿using System;
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
                                                FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot", true, GetLibs(fileName), null));
                                            AddKeywordsFromKeyword(currentTestCaseTestSteps[currentTestCaseTestSteps.Count - 1],
                                                GetResourcesFromFile(fileName));
                                        }
                                        else
                                        {
                                            if (arrLine[i].Trim().ToLower().StartsWith(":for"))
                                            {
                                                if (arrLine[i].Trim().ToLower().Contains("range"))
                                                {
                                                    Keyword temp = new Keyword(null);
                                                    temp.CopyKeyword(SuggestionsClass.GetForLoop(KeywordType.FOR_LOOP_IN_RANGE));
                                                    string[] splitKeyword = arrLine[i].Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                                                    if (splitKeyword.Length == 1)
                                                        splitKeyword = arrLine[i].Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                                                    if (splitKeyword != null && splitKeyword.Length >= 5)
                                                    {
                                                        temp.Params[0].Value = splitKeyword[1];
                                                        temp.Params[1].Value = splitKeyword[3];
                                                        temp.Params[2].Value = splitKeyword[4];
                                                    }
                                                    currentTestCaseTestSteps.Add(temp);
                                                    currentTestCaseTestSteps[currentTestCaseTestSteps.Count - 1].ForLoopKeywords = new List<Keyword>();
                                                }
                                                else
                                                {
                                                    Keyword temp = new Keyword(null);
                                                    temp.CopyKeyword(SuggestionsClass.GetForLoop(KeywordType.FOR_LOOP_ELEMENTS));
                                                    string[] splitKeyword = arrLine[i].Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                                                    if (splitKeyword.Length == 1)
                                                        splitKeyword = arrLine[i].Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                                                    if (splitKeyword != null && splitKeyword.Length >= 4)
                                                    {
                                                        temp.Params[0].Value = splitKeyword[1];
                                                        temp.Params[1].Value = splitKeyword[3];
                                                    }
                                                    currentTestCaseTestSteps.Add(temp);
                                                    currentTestCaseTestSteps[currentTestCaseTestSteps.Count - 1].ForLoopKeywords = new List<Keyword>();
                                                }
                                            }
                                            else
                                            {
                                                currentTestCaseTestSteps[currentTestCaseTestSteps.Count - 1].ForLoopKeywords.Add(
                                                    new Keyword(arrLine[i].Trim().Remove(0,1).Trim(),
                                                    FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot", true, GetLibs(fileName), null));
                                                AddKeywordsFromKeyword(currentTestCaseTestSteps[currentTestCaseTestSteps.Count - 1].ForLoopKeywords[currentTestCaseTestSteps[currentTestCaseTestSteps.Count - 1].ForLoopKeywords.Count - 1],
                                                    GetResourcesFromFile(fileName));
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
                                        FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot", true, GetLibs(fileName), keyword));
                                    AddKeywordsFromKeyword(keyword.Keywords[keyword.Keywords.Count - 1],
                                        GetResourcesFromFile(fileName));
                                }
                                else
                                {
                                    if (arrLine[i].Trim().ToLower().StartsWith(":for"))
                                    {
                                        if (arrLine[i].Trim().ToLower().Contains("range"))
                                        {
                                            Keyword temp = new Keyword(keyword);
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
                                            Keyword temp = new Keyword(keyword);
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
                                            FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot", true, GetLibs(fileName), keyword));
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
        }

        // returns the index of the specific tag - keyword / test cases / settings / variables
        internal static List<Keyword> ReadAllSettings()
        {
            List<Keyword> SettingsKeywords = new List<Keyword>();
            foreach (string fileName in FilesAndFolderStructure.GetFullSavedFiles(FolderType.Tests))
            {
                List<Keyword> SetupsAndTeardowns = new List<Keyword>
                {
                    new Keyword(RobotFileHandler.OccuranceInSettings(fileName, "Test Setup").Replace("Test Setup", "").Trim(),
                    fileName, true, GetLibs(fileName), null),
                    new Keyword(RobotFileHandler.OccuranceInSettings(fileName, "Test Teardown").Replace("Test Teardown", "").Trim(),
                    fileName, true, GetLibs(fileName), null),
                    new Keyword(RobotFileHandler.OccuranceInSettings(fileName, "Suite Setup").Replace("Suite Setup", "").Trim(),
                    fileName, true, GetLibs(fileName), null),
                    new Keyword(RobotFileHandler.OccuranceInSettings(fileName, "Suite Teardown").Replace("Suite Teardown", "").Trim(),
                    fileName, true, GetLibs(fileName), null)
                };

                foreach (Keyword settingsKeyword in SetupsAndTeardowns)
                {
                    if (!settingsKeyword.Name.Equals(""))
                    {
                        AddKeywordsFromKeyword(settingsKeyword, GetResourcesFromFile(fileName));
                        Keyword temp = new Keyword(null);
                        temp.CopyKeyword(settingsKeyword);
                        SettingsKeywords.Add(temp);
                    }
                }
            }
            return SettingsKeywords;
        }

        private static List<string> GetResourcesFromFile(string fileName)
        {
            string[] arrLine = File.ReadAllLines(fileName);
            // find all resources
            List<string> Resources = new List<string>()
            {
                fileName
            };
            
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

        internal static List<string> GetLibs(string fileName)
        {
            string[] arrLine = File.ReadAllLines(fileName);
            // find all resources
            List<string> Libraries = new List<string>();

            if (arrLine.Length != 0)
            {
                bool start = false;
                for (int i = 0; i < arrLine.Length; i++)
                {
                    if (start && arrLine[i].StartsWith("***"))
                        break;
                    if (arrLine[i].StartsWith("*** Settings ***"))
                        start = true;

                    if (start && arrLine[i].StartsWith("Library"))
                    {
                        Libraries.Add(arrLine[i].Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[1]);
                    }
                }
            }
            return Libraries;
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
