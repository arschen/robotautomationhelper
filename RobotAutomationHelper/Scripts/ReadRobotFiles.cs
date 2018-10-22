using System;
using System.Collections.Generic;
using System.IO;

namespace RobotAutomationHelper.Scripts
{
    internal class ReadRobotFiles
    {
        internal static List<TestCase> TestCases = new List<TestCase>();
        private static List<Keyword> currentTestCaseTestSteps = new List<Keyword>();
        private static string currentTestCaseTags = "";
        private static string currentTestCaseDocumentation = "";
        private static string currentTestCase = "";

        // returns the index of the specific tag - keyword / test cases / settings / variables
        internal static void ReadAllTests()
        {
            string[] arrLine;

            foreach (string fileName in FilesAndFolderStructure.GetFullSavedFiles("Tests"))
            {
                if (File.Exists(fileName))
                {
                    arrLine = File.ReadAllLines(fileName);

                    if (arrLine.Length != 0)
                    {
                        bool start = false;
                        for (int i = 0; i < arrLine.Length; i++)
                        {
                            if (start && arrLine[i].StartsWith("***") || i + 1 == arrLine.Length)
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
                                        currentTestCaseTestSteps.Add(new Keyword(arrLine[i],
                                            FilesAndFolderStructure.GetFolder("Tests"), true));
                                        AddKeywordsInKeyword(currentTestCaseTestSteps[currentTestCaseTestSteps.Count - 1],
                                            GetResourcesFromFile(arrLine));
                                    }
                                }
                            }

                            if (arrLine[i].ToLower().Trim().StartsWith("*** test cases ***"))
                                start = true;
                        }
                    }
                }
            }
        }

        private static void AddKeywordsInKeyword(Keyword keyword, List<string> filesList)
        {
            foreach (string fileName in filesList)
            {
                int index = RobotFileHandler.LocationOfTestCaseOrKeywordInFile(fileName, keyword.Name.Trim(), "Keywords");
                if (index != -1)
                {
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
                                keyword.Arguments = arrLine[i];
                            else
                            {
                                if (keyword.Keywords == null)
                                    keyword.Keywords = new List<Keyword>();
                                keyword.Keywords.Add(new Keyword(arrLine[i],
                                    fileName, true));
                                AddKeywordsInKeyword(keyword.Keywords[keyword.Keywords.Count - 1],
                                    GetResourcesFromFile(arrLine));
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
                        Resources.Add(FilesAndFolderStructure.GetFolder("") + arrLine[i].Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[1]);
                    }
                }
            }
            return Resources;
        }

        private static void AddTestCaseAndResetValues(string fileName)
        {
            TestCases.Add(new TestCase(currentTestCase, currentTestCaseDocumentation, currentTestCaseTags, currentTestCaseTestSteps, fileName));
            currentTestCaseTestSteps = new List<Keyword>();
            currentTestCaseDocumentation = "";
            currentTestCase = "";
            currentTestCaseTags = "";
        }
    }
}
