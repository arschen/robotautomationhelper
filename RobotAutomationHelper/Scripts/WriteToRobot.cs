using System;
using System.Collections.Generic;

namespace RobotAutomationHelper.Scripts
{

    internal static class WriteToRobot
    {

        internal static List<Includes> includes;

        internal static void AddTestCaseToRobot(TestCase testCase)
        {
            string fileName = testCase.OutputFilePath;
            int index = RobotFileHandler.GetLineAfterLastTestCase(fileName);
            if (index < 0) index = 0;

            bool addTestCase = !(RobotFileHandler.LocationOfTestCaseOrKeywordInFile(fileName, testCase.Name.Trim(), "test cases") != -1);
            if (addTestCase)
            {
                Includes candidate = new Includes(fileName);
                if (!includes.Contains(candidate))
                    includes.Add(candidate);

                //Add test case to robot file
                index = AddName(testCase.Name.Trim(), fileName, index, "test cases");

                //adds documentation
                index = AddTagsDocumentationArguments("[Documentation]", "\t" + testCase.Documentation, fileName, index);

                //adds tags
                index = AddTagsDocumentationArguments("[Tags]", "\t" + testCase.Tags, fileName, index);
            }

            index = AddKeyword(testCase.Steps, fileName, index, addTestCase, testCase.Overwrite);
        }

        internal static void AddKeywordToRobot(Keyword keyword)
        {
            string fileName = keyword.OutputFilePath;
            int index = 0;
            if (fileName != "")
                index = RobotFileHandler.GetLineAfterLastKeyword(fileName);

            if (index < 0) index = 0;

            bool addKeywordSteps = !(RobotFileHandler.LocationOfTestCaseOrKeywordInFile(fileName, keyword.Name.Trim(), "keyword") != -1);

            if (addKeywordSteps)
                if (keyword.Type == KeywordType.CUSTOM)
                {
                    Includes candidate = new Includes(fileName);
                    if (!includes.Contains(candidate))
                        includes.Add(candidate);
                }

            if (keyword.Saved && addKeywordSteps && (keyword.Type == KeywordType.CUSTOM))
            {
                //Add keyword to robot file
                index = AddName(keyword.Name.Trim(), fileName, index, "keywords");

                //adds documentation
                index = AddTagsDocumentationArguments("[Documentation]", "\t" + keyword.Documentation, fileName, index);

                //adds arguments
                index = AddTagsDocumentationArguments("[Arguments]", "\t" + keyword.Arguments, fileName, index);
            }

            index = AddKeyword(keyword.Keywords, fileName, index, addKeywordSteps, keyword.Overwrite);
        }

        //adds Keywords
        private static int AddKeyword(List<Keyword> keywordKeywords, string fileName, int index, bool addSteps, bool overwrite)
        {
            Includes container = new Includes(fileName);
            if (keywordKeywords != null)
                foreach (Keyword keywordKeyword in keywordKeywords)
                {
                    if (addSteps)
                    {
                        if (keywordKeyword.Saved && keywordKeyword.Type == KeywordType.CUSTOM)
                            includes[includes.IndexOf(container)].AddToList(keywordKeyword.OutputFilePath);
                        else
                            if (keywordKeyword.Type == KeywordType.SELENIUM)
                            includes[includes.IndexOf(container)].AddToList("SeleniumLibrary");

                        //adds test steps
                        index++;
                        RobotFileHandler.FileLineAdd("\t" + keywordKeyword.Name + keywordKeyword.ParamsToString(), fileName, index);
                    }

                    AddKeywordToRobot(keywordKeyword);
                }
            return index;
        }



        //adds Tags / Documentation / Arguments
        private static int AddTagsDocumentationArguments(string type, string addString, string fileName, int index)
        {
            if (addString == null)
                addString = "";
            if (!addString.Replace(type, "").Trim().Equals(""))
            {
                index++;
                RobotFileHandler.FileLineAdd(addString, fileName, index);
            }
            return index;
        }

        //Add test case / keyword name to robot file
        private static int AddName(string name, string fileName, int index, string tag)
        {
            int tempTagIndex = RobotFileHandler.HasTag(fileName, tag);
            if (tempTagIndex == -1)
            {
                if (tag.Equals("keywords"))
                    RobotFileHandler.FileLineAdd("*** Keywords ***", fileName, index);
                else
                {
                    if (tag.Equals("test cases"))
                    {
                        int tempKeywordsIndex = RobotFileHandler.HasTag(fileName, "keywords");
                        if (tempKeywordsIndex != -1)
                        {
                            RobotFileHandler.FileLineAdd("*** Test Cases ***", fileName, tempKeywordsIndex);
                            index = tempKeywordsIndex;
                        }
                        else
                            RobotFileHandler.FileLineAdd("*** Test Cases ***", fileName, index);
                    }
                }
                index++;
            }
            else
                if (tempTagIndex + 1 != index)
            {
                RobotFileHandler.FileLineAdd("", fileName, index);
                index++;
            }
            RobotFileHandler.FileLineAdd(name, fileName, index);
            return index;
        }

        //Add includes to test case and keywords files
        internal static void WriteIncludesToRobotFiles()
        {
            int index;
            string tag = "settings";
            string fileName;

            foreach (Includes temp in includes)
            {
                if (temp.FilesToInclude.Count > 0)
                {
                    index = 0;
                    fileName = temp.FileName;
                    int tempTagIndex = RobotFileHandler.HasTag(fileName, tag);
                    if (tempTagIndex == -1)
                    {
                        RobotFileHandler.FileLineAdd("*** Settings ***", fileName, index);
                        index++;
                    }
                    else
                        index = tempTagIndex + 1;

                    foreach (string path in temp.FilesToInclude)
                    {
                        if (path.Equals("SeleniumLibrary"))
                        {
                            if (RobotFileHandler.OccuranceInSettings(fileName, "Library  " + path).Equals(""))
                                RobotFileHandler.FileLineAdd("Library  " + path, fileName, index);
                        }
                        else
                            if (RobotFileHandler.OccuranceInSettings(fileName, "Resource  ./" + path.Replace(FilesAndFolderStructure.GetFolder().Replace('\\', '/'), "")).Equals(""))
                            RobotFileHandler.FileLineAdd("Resource  ./" + path.Replace(FilesAndFolderStructure.GetFolder(), "").Replace('\\', '/'), fileName.Replace('\\', '/'), index);
                        index++;
                    }
                }
            }
        }

        internal static void WriteSuiteSettingsListToRobot()
        {
            foreach (SuiteSettings suiteSettings in RobotAutomationHelper.SuiteSettingsList)
                if (suiteSettings.Overwrite)
                {
                    if (suiteSettings.Documentation != "")
                        ReplaceInSettings("Documentation  " + suiteSettings.Documentation, "Documentation",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath));
                    else
                        RemoveFromSettings("Documentation",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath));

                    if (suiteSettings.TestSetup.Name != "")
                        ReplaceInSettings("Test Setup  " + suiteSettings.TestSetup.Name + suiteSettings.TestSetup.ParamsToString()
                            , "Test Setup",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath));
                    else
                        RemoveFromSettings("Test Setup",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath));

                    if (suiteSettings.TestTeardown.Name != "")
                        ReplaceInSettings("Test Teardown  " + suiteSettings.TestTeardown.Name + suiteSettings.TestTeardown.ParamsToString()
                            , "Test Teardown",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath));
                    else
                        RemoveFromSettings("Test Teardown",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath));

                    if (suiteSettings.TestSetup.Name != "")
                        ReplaceInSettings("Suite Setup  " + suiteSettings.SuiteSetup.Name + suiteSettings.SuiteSetup.ParamsToString()
                            , "Suite Setup",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath));
                    else
                        RemoveFromSettings("Suite Setup",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath));

                    if (suiteSettings.SuiteTeardown.Name != "")
                        ReplaceInSettings("Suite Teardown  " + suiteSettings.SuiteTeardown.Name + suiteSettings.SuiteTeardown.ParamsToString()
                            , "Suite Teardown",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath));
                    else
                        RemoveFromSettings("Suite Teardown",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath));
                }
        }

        internal static void RemoveKeyword(Keyword keyword)
        {
            if (keyword.Keywords != null)
                foreach (Keyword step in keyword.Keywords)
                {
                    RemoveKeyword(step);
                    if (step.Overwrite)
                        RobotFileHandler.TestCaseKeywordRemove(step.Name, step.OutputFilePath, true);
                }
        }

        // replaces tags and text when writing settings to the files
        private static void ReplaceInSettings(string replacementText, string tag, string outputFileName)
        {
            List<int> location = RobotFileHandler.LocationInSettings(outputFileName, tag);

            //Add settings tag if not present in the file
            if (RobotFileHandler.HasTag(outputFileName , "Settings") == -1)
                RobotFileHandler.FileLineAdd(replacementText
                    , outputFileName
                    , 0);

            if (location[0] == -1)
                RobotFileHandler.FileLineAdd(replacementText
                    , outputFileName
                    , 1);
            else
                RobotFileHandler.FileLineReplace(replacementText
                    , outputFileName
                    , location);
        }

        // replaces tags and text when writing settings to the files
        private static void RemoveFromSettings(string tag, string outputFileName)
        {
            List<int> location = RobotFileHandler.LocationInSettings(outputFileName, tag);
            if (location[0] != -1)
                RobotFileHandler.FileLineRemove(outputFileName
                                , location);
        }
    }
}
