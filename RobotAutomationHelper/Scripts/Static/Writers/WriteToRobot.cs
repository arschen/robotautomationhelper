using System;
using System.Collections.Generic;
using System.IO;

namespace RobotAutomationHelper.Scripts
{
    internal static class WriteToRobot
    {

        // Fields and Properties ===================================================
        internal static List<Includes> Includes;

        // Methods =================================================================
        internal static void AddTestCaseToRobot(TestCase testCase)
        {
            string fileName = testCase.OutputFilePath;
            int index = RobotFileHandler.GetLineAfterLastTestCase(fileName);
            if (index < 0) index = 0;

            bool addTestCase = !(RobotFileHandler.LocationOfTestCaseOrKeywordInFile(fileName, testCase.Name.Trim(), FormType.Test) != -1);
            if (addTestCase)
            {
                Includes candidate = new Includes(fileName);
                if (!Includes.Contains(candidate))
                    Includes.Add(candidate);

                //Add test case to robot file
                index = AddName(testCase.Name.Trim(), fileName, index, FormType.Test);

                //adds documentation
                index = AddTagsDocumentationArguments("[Documentation]", "\t" + testCase.Documentation, fileName, index);

                //adds tags
                index = AddTagsDocumentationArguments("[Tags]", "\t" + testCase.Tags, fileName, index);
            }

            index = AddSteps(testCase.Steps, fileName, index, addTestCase, testCase.Overwrite);
        }
        internal static void AddKeywordToRobot(Keyword keyword)
        {
            string fileName = keyword.OutputFilePath;
            int index = 0;
            if (fileName != "")
                index = RobotFileHandler.GetLineAfterLastKeyword(fileName);

            if (index < 0) index = 0;

            bool addKeywordSteps = !(RobotFileHandler.LocationOfTestCaseOrKeywordInFile(fileName, keyword.Name.Trim(), FormType.Keyword) != -1);

            if (addKeywordSteps)
                if (keyword.Type == KeywordType.CUSTOM)
                {
                    Includes candidate = new Includes(fileName);
                    if (!Includes.Contains(candidate))
                        Includes.Add(candidate);
                }

            if (keyword.Saved && addKeywordSteps && (keyword.Type == KeywordType.CUSTOM))
            {
                //Add keyword to robot file
                index = AddName(keyword.Name.Trim(), fileName, index, FormType.Keyword);

                //adds documentation
                index = AddTagsDocumentationArguments("[Documentation]", "\t" + keyword.Documentation, fileName, index);

                //adds arguments
                index = AddTagsDocumentationArguments("[Arguments]", "\t" + keyword.Arguments, fileName, index);
            }

            index = AddSteps(keyword.Keywords, fileName, index, addKeywordSteps, keyword.Overwrite);
        }

        //adds Steps
        private static int AddSteps(List<Keyword> keywordKeywords, string fileName, int index, bool addSteps, bool overwrite)
        {
            Includes container = new Includes(fileName);
            if (keywordKeywords != null)
                foreach (Keyword keywordKeyword in keywordKeywords)
                {
                    if (addSteps)
                    {
                        if (keywordKeyword.Saved && keywordKeyword.Type == KeywordType.CUSTOM)
                            Includes[Includes.IndexOf(container)].AddToList(keywordKeyword.OutputFilePath);
                        else
                            if (!keywordKeyword.KeywordString.Equals("BuiltIn") && !keywordKeyword.KeywordString.Equals("ForLoop"))
                                Includes[Includes.IndexOf(container)].AddToList(keywordKeyword.KeywordString);

                        //adds test steps
                        if (keywordKeyword.Type == KeywordType.FOR_LOOP_ELEMENTS || keywordKeyword.Type == KeywordType.FOR_LOOP_IN_RANGE)
                        {
                            //add actual FOR Loop line + keywords inside it
                            index++;
                            if (keywordKeyword.Type == KeywordType.FOR_LOOP_ELEMENTS)
                                RobotFileHandler.FileLineAdd("\t" + ":FOR" + "\t" + keywordKeyword.Params[0].Value + "\t" + "IN" + "\t" + keywordKeyword.Params[1].Value, fileName, index);
                            else
                                if (keywordKeyword.Type == KeywordType.FOR_LOOP_IN_RANGE)
                                RobotFileHandler.FileLineAdd("\t" + ":FOR" + "\t" + keywordKeyword.Params[0].Value + "\t" + "IN RANGE" + "\t" + keywordKeyword.Params[1].Value + "\t" + keywordKeyword.Params[2].Value, fileName, index);

                            foreach (Keyword key in keywordKeyword.ForLoopKeywords)
                            {
                                index++;
                                RobotFileHandler.FileLineAdd("\t" + "\\" + "\t" + key.Name + key.ParamsToString(), fileName, index);

                                if (key.Saved && key.Type == KeywordType.CUSTOM)
                                    Includes[Includes.IndexOf(container)].AddToList(key.OutputFilePath);
                                else
                                    if (!keywordKeyword.KeywordString.Equals("BuiltIn") && !keywordKeyword.KeywordString.Equals("ForLoop"))
                                            Includes[Includes.IndexOf(container)].AddToList(keywordKeyword.KeywordString);

                                if (!key.Recursive)
                                    AddKeywordToRobot(key);
                            }
                        }
                        else
                        {
                            index++;
                            RobotFileHandler.FileLineAdd("\t" + keywordKeyword.Name + keywordKeyword.ParamsToString(), fileName, index);
                        }    
                    }

                    if (!keywordKeyword.Recursive)
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
        private static int AddName(string name, string fileName, int index, FormType tag)
        {
            int tempTagIndex = RobotFileHandler.HasTag(fileName, tag);
            if (tempTagIndex == -1)
            {
                if (tag.Equals(FormType.Keyword))
                    RobotFileHandler.FileLineAdd("*** Keywords ***", fileName, index);
                else
                {
                    if (tag.Equals(FormType.Test))
                    {
                        int tempKeywordsIndex = RobotFileHandler.HasTag(fileName, FormType.Keyword);
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
            FormType tag = FormType.Settings;
            string fileName;

            foreach (Includes temp in Includes)
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
                        if (!path.Contains("\\"))
                        {
                            if (RobotFileHandler.OccuranceInSettings(fileName, "Library  " + path).Equals(""))
                            {
                                RobotFileHandler.FileLineAdd("Library  " + path, fileName, index);
                                index++;
                            }
                        }
                        else
                            if (RobotFileHandler.OccuranceInSettings(fileName, "Resource  " + path.Replace(FilesAndFolderStructure.GetFolder(FolderType.Root),"").Replace('\\', '/')).Equals(""))
                        {
                            RobotFileHandler.FileLineAdd("Resource  " + path.Replace(FilesAndFolderStructure.GetFolder(FolderType.Root), "").Replace('\\', '/'), fileName, index);
                            index++;
                        }
                    }
                }
            }
        }
        internal static void WriteSuiteSettingsListToRobot()
        {
            foreach (SuiteSettings suiteSettings in RobotAutomationHelper.SuiteSettingsList)
                if (suiteSettings.Overwrite)
                {
                    FolderType type = FolderType.Root;
                    if (suiteSettings.Documentation != "")
                        ReplaceInSettings("Documentation  " + suiteSettings.Documentation, "Documentation",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));
                    else
                        RemoveFromSettings("Documentation",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));

                    if (suiteSettings.TestSetup.Name != "")
                        ReplaceInSettings("Test Setup  " + suiteSettings.TestSetup.Name + suiteSettings.TestSetup.ParamsToString()
                            , "Test Setup",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));
                    else
                        RemoveFromSettings("Test Setup",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));

                    if (suiteSettings.TestTeardown.Name != "")
                        ReplaceInSettings("Test Teardown  " + suiteSettings.TestTeardown.Name + suiteSettings.TestTeardown.ParamsToString()
                            , "Test Teardown",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));
                    else
                        RemoveFromSettings("Test Teardown",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));

                    if (suiteSettings.SuiteSetup.Name != "")
                        ReplaceInSettings("Suite Setup  " + suiteSettings.SuiteSetup.Name + suiteSettings.SuiteSetup.ParamsToString()
                            , "Suite Setup",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));
                    else
                        RemoveFromSettings("Suite Setup",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));

                    if (suiteSettings.SuiteTeardown.Name != "")
                        ReplaceInSettings("Suite Teardown  " + suiteSettings.SuiteTeardown.Name + suiteSettings.SuiteTeardown.ParamsToString()
                            , "Suite Teardown",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));
                    else
                        RemoveFromSettings("Suite Teardown",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));

                    if (suiteSettings.SuiteSetup != null && !suiteSettings.SuiteSetup.Name.Trim().Equals(""))
                    {
                        RemoveKeywordForOverwriting(suiteSettings.SuiteSetup);
                        AddIncludesFromSettingsKeyword(suiteSettings.SuiteSetup, FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));
                        if (suiteSettings.SuiteSetup.Overwrite)
                            TestCaseKeywordRemove(suiteSettings.SuiteSetup.Name, suiteSettings.SuiteSetup.OutputFilePath, true);
                        AddKeywordToRobot(suiteSettings.SuiteSetup);
                    }
                    if (suiteSettings.SuiteTeardown != null && !suiteSettings.SuiteTeardown.Name.Trim().Equals(""))
                    {
                        RemoveKeywordForOverwriting(suiteSettings.SuiteTeardown);
                        AddIncludesFromSettingsKeyword(suiteSettings.SuiteTeardown, FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));
                        if (suiteSettings.SuiteTeardown.Overwrite)
                            TestCaseKeywordRemove(suiteSettings.SuiteTeardown.Name, suiteSettings.SuiteTeardown.OutputFilePath, true);
                        AddKeywordToRobot(suiteSettings.SuiteTeardown);
                    }
                    if (suiteSettings.TestSetup != null && !suiteSettings.TestSetup.Name.Trim().Equals(""))
                    {
                        RemoveKeywordForOverwriting(suiteSettings.TestSetup);
                        AddIncludesFromSettingsKeyword(suiteSettings.TestSetup, FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));
                        if (suiteSettings.TestSetup.Overwrite)
                            TestCaseKeywordRemove(suiteSettings.TestSetup.Name, suiteSettings.TestSetup.OutputFilePath, true);
                        AddKeywordToRobot(suiteSettings.TestSetup);
                    }
                    if (suiteSettings.TestTeardown != null && !suiteSettings.TestTeardown.Name.Trim().Equals(""))
                    {
                        RemoveKeywordForOverwriting(suiteSettings.TestTeardown);
                        AddIncludesFromSettingsKeyword(suiteSettings.TestTeardown, FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));
                        if (suiteSettings.TestTeardown.Overwrite)
                            TestCaseKeywordRemove(suiteSettings.TestTeardown.Name, suiteSettings.TestTeardown.OutputFilePath, true);
                        AddKeywordToRobot(suiteSettings.TestTeardown);
                    }
                }
        }

        // replaces tags and text when writing settings to the files
        private static void ReplaceInSettings(string replacementText, string tag, string outputFileName)
        {
            List<int> location = RobotFileHandler.LocationInSettings(outputFileName, tag);

            //Add settings tag if not present in the file
            if (RobotFileHandler.HasTag(outputFileName , FormType.Settings) == -1)
                RobotFileHandler.FileLineAdd("*** Settings ***"
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

        internal static void RemoveKeywordForOverwriting(Keyword keyword)
        {
            if (keyword.Keywords != null)
                foreach (Keyword step in keyword.Keywords)
                {
                    if (!step.Recursive)
                    {
                        RemoveKeywordForOverwriting(step);
                        if (step.Overwrite)
                            TestCaseKeywordRemove(step.Name, step.OutputFilePath, true);
                    }
                }
        }

        internal static void AddIncludesFromSettingsKeyword(Keyword keyword, string fileName)
        {
            Includes container = new Includes(fileName);
            if (keyword.Type == KeywordType.CUSTOM)
                Includes[Includes.IndexOf(container)].AddToList(keyword.OutputFilePath);
            else
                if (!keyword.KeywordString.Equals("BuiltIn") && !keyword.KeywordString.Equals("ForLoop"))
                    Includes[Includes.IndexOf(container)].AddToList(keyword.KeywordString);
        }

        internal static void TestCaseKeywordRemove(string name, string fileName, bool isKeyword)
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

            int index = 0;
            if (isKeyword)
                index = RobotFileHandler.LocationOfTestCaseOrKeywordInFile(fileName, name, FormType.Keyword);
            else
                index = RobotFileHandler.LocationOfTestCaseOrKeywordInFile(fileName, name, FormType.Test);

            if (index != -1)
            {
                bool endOfTestCaseKeyword = false;
                List<string> temp = new List<string>();
                temp.AddRange(arrLine);

                while (!endOfTestCaseKeyword)
                {
                    if (index < temp.Count)
                        temp.RemoveAt(index);
                    else
                        endOfTestCaseKeyword = true;

                    if (index < temp.Count)
                        if (!temp[index].StartsWith(" "))
                            if (!temp[index].StartsWith("\t"))
                                if (!temp[index].StartsWith("\\"))
                                    if (!temp[index].StartsWith("."))
                                        if (!temp[index].Equals(""))
                                            endOfTestCaseKeyword = true;
                }

                File.WriteAllLines(fileName, temp);
            }
        }
    }
}
