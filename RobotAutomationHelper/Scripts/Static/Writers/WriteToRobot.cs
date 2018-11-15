using System;
using System.Collections.Generic;
using System.IO;
using RobotAutomationHelper.Forms;
using RobotAutomationHelper.Scripts.Objects;

namespace RobotAutomationHelper.Scripts.Static.Writers
{
    internal static class WriteToRobot
    {

        // Fields and Properties ===================================================
        internal static List<Includes> Includes;

        // Methods =================================================================
        internal static void AddTestCaseToRobot(TestCase testCase)
        {
            var fileName = testCase.OutputFilePath;
            var index = RobotFileHandler.GetLineAfterLastTestCase(fileName);
            if (index < 0) index = 0;

            var addTestCase = RobotFileHandler.LocationOfTestCaseOrKeywordInFile(fileName, testCase.Name.Trim(), FormType.Test) == -1;
            if (addTestCase)
            {
                var candidate = new Includes(fileName);
                if (!Includes.Contains(candidate))
                    Includes.Add(candidate);

                //Add test case to robot file
                index = AddName(testCase.Name.Trim(), fileName, index, FormType.Test);

                //adds documentation
                if (testCase.Documentation != null)
                    index = AddTagsDocumentationArguments("[Documentation]", "\t" + testCase.Documentation.Trim(), fileName, index);

                //adds tags
                if (testCase.Tags != null)
                    index = AddTagsDocumentationArguments("[Tags]", "\t" + testCase.Tags.Trim(), fileName, index);
            }

            AddSteps(testCase.Steps, fileName, index, addTestCase);
        }

        internal static void AddKeywordToRobot(Keyword keyword)
        {
            var fileName = keyword.OutputFilePath;
            var index = 0;
            if (fileName != "")
                index = RobotFileHandler.GetLineAfterLastKeyword(fileName);

            if (index < 0) index = 0;

            var addKeywordSteps = RobotFileHandler.LocationOfTestCaseOrKeywordInFile(fileName, keyword.Name.Trim(), FormType.Keyword) == -1;

            if (addKeywordSteps)
                if (keyword.Type == KeywordType.Custom && !StringAndListOperations.StartsWithVariable(keyword.Name))
                {
                    var candidate = new Includes(fileName);
                    if (!Includes.Contains(candidate))
                        Includes.Add(candidate);
                }

            if (addKeywordSteps && (keyword.Type == KeywordType.Custom))
            {
                //Add keyword to robot file
                index = AddName(keyword.Name.Trim(), fileName, index, FormType.Keyword);

                //adds documentation
                if (keyword.Documentation != null)
                    index = AddTagsDocumentationArguments("[Documentation]", "\t" + keyword.Documentation.Trim(), fileName, index);

                //adds arguments
                if (keyword.Arguments != null)
                    index = AddTagsDocumentationArguments("[Arguments]", "\t" + keyword.Arguments.Trim(), fileName, index);
            }

            if (keyword.Type == KeywordType.Custom)
                AddSteps(keyword.Keywords, fileName, index, addKeywordSteps);
        }

        //adds Steps
        private static void AddSteps(IReadOnlyCollection<Keyword> keywordKeywords, string fileName, int index, bool addSteps)
        {
            var container = new Includes(fileName);
            if (keywordKeywords == null) return;
            foreach (var keyword in keywordKeywords)
            {
                if (addSteps)
                {
                    if (keyword.Type == KeywordType.Custom && !StringAndListOperations.StartsWithVariable(keyword.Name))
                        Includes[Includes.IndexOf(container)].AddToList(keyword.OutputFilePath);
                    else
                    if (keyword.Type != KeywordType.Custom)
                        if (!keyword.KeywordString.Equals("BuiltIn") && !keyword.KeywordString.Equals("ForLoop"))
                            Includes[Includes.IndexOf(container)].AddToList(keyword.KeywordString);

                    //adds test steps
                    if (keyword.Type == KeywordType.ForLoopElements || keyword.Type == KeywordType.ForLoopInRange)
                    {
                        //add actual FOR Loop line + keywords inside it
                        index++;
                        if (keyword.Type == KeywordType.ForLoopElements)
                            RobotFileHandler.FileLineAdd("\t" + ":FOR" + "\t" + keyword.Params[0].Value + "\t" + "IN" + "\t" + keyword.Params[1].Value, fileName, index);
                        else
                        if (keyword.Type == KeywordType.ForLoopInRange)
                            RobotFileHandler.FileLineAdd("\t" + ":FOR" + "\t" + keyword.Params[0].Value + "\t" + "IN RANGE" + "\t" + keyword.Params[1].Value + "\t" + keyword.Params[2].Value, fileName, index);

                        foreach (var key in keyword.ForLoopKeywords)
                        {
                            index++;
                            RobotFileHandler.FileLineAdd("\t" + "\\" + "\t" + key.GetName() + key.ParamsToString(), fileName, index);

                            if (key.Type == KeywordType.Custom && !StringAndListOperations.StartsWithVariable(keyword.Name))
                                Includes[Includes.IndexOf(container)].AddToList(key.OutputFilePath);
                            else
                            if (keyword.Type != KeywordType.Custom)
                                if (!keyword.KeywordString.Equals("BuiltIn") && !keyword.KeywordString.Equals("ForLoop"))
                                    Includes[Includes.IndexOf(container)].AddToList(keyword.KeywordString);

                            if (!key.Recursive && !StringAndListOperations.StartsWithVariable(key.Name))
                                AddKeywordToRobot(key);
                        }
                    }
                    else
                    {
                        index++;
                        RobotFileHandler.FileLineAdd("\t" + keyword.GetName() + keyword.ParamsToString(), fileName, index);
                    }    
                }

                if (!keyword.Recursive && !StringAndListOperations.StartsWithVariable(keyword.Name))
                    AddKeywordToRobot(keyword);
            }
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
            var tempTagIndex = RobotFileHandler.HasTag(fileName, tag);
            if (tempTagIndex == -1)
            {
                if (tag.Equals(FormType.Keyword))
                    RobotFileHandler.FileLineAdd("*** Keywords ***", fileName, index);
                else
                {
                    if (tag.Equals(FormType.Test))
                    {
                        var tempKeywordsIndex = RobotFileHandler.HasTag(fileName, FormType.Keyword);
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
            const FormType tag = FormType.Settings;

            foreach (var temp in Includes)
            {
                if (temp.FilesToInclude.Count > 0)
                {
                    var index = 0;
                    var fileName = temp.FileName;
                    var tempTagIndex = RobotFileHandler.HasTag(fileName, tag);
                    if (tempTagIndex == -1)
                    {
                        RobotFileHandler.FileLineAdd("*** Settings ***", fileName, index);
                        index++;
                        RobotFileHandler.FileLineAdd("", fileName, index);
                    }
                    else
                        index = tempTagIndex + 1;

                    temp.FilesToInclude.Sort();
                    foreach (var path in temp.FilesToInclude)
                    {
                        if (!path.Contains("\\"))
                        {
                            if (!RobotFileHandler.OccurenceInSettings(fileName, "Library  " + path).Equals(""))
                                continue;
                            RobotFileHandler.FileLineAdd("Library  " + path, fileName, index);
                            index++;
                        }
                        else
                            if (RobotFileHandler.OccurenceInSettings(fileName, "Resource  " + path.Replace(FilesAndFolderStructure.GetFolder(FolderType.Root),"").Replace('\\', '/')).Equals(""))
                        {
                            RobotFileHandler.FileLineAdd("Resource  " + path.Replace(FilesAndFolderStructure.GetFolder(FolderType.Root), "").Replace('\\', '/'), fileName, index);
                            index++;
                        }
                    }
                }
            }
        }

        //Add variables to test case and keywords files
        internal static void WriteVariablesToRobotFiles()
        {
            const FormType tag = FormType.Variable;
            foreach (var temp in Forms.RobotAutomationHelper.GlobalVariables)
            {
                if (temp.VariableNames != null && temp.VariableNames.Count > 0)
                {
                    var fileName = temp.OutputFilePath;
                    var index = RobotFileHandler.HasTag(fileName, FormType.Test);
                    if (index == -1)
                        index = RobotFileHandler.HasTag(fileName, FormType.Keyword);
                    if (index == -1)
                        index = 0;
                    var tempTagIndex = RobotFileHandler.HasTag(fileName, tag);
                    if (tempTagIndex == -1)
                    {
                        RobotFileHandler.FileLineAdd("*** Variables ***", fileName, index);
                        index++;
                        RobotFileHandler.FileLineAdd("", fileName, index);
                    }
                    else
                        index = tempTagIndex + 1;

                    temp.VariableNames.Sort();
                    foreach (var variable in temp.VariableNames)
                    {
                        RobotFileHandler.FileLineAdd(variable, fileName, index);
                        index++;
                    }
                }
            }
        }

        internal static void WriteSuiteSettingsListToRobot()
        {
            foreach (var suiteSettings in Forms.RobotAutomationHelper.SuiteSettingsList)
                if (suiteSettings.Overwrite)
                {
                    const FolderType type = FolderType.Root;

                    if (suiteSettings.TestTeardown.GetName() != "")
                        ReplaceInSettings("Test Teardown  " + suiteSettings.TestTeardown.GetName() + suiteSettings.TestTeardown.ParamsToString()
                            , "Test Teardown",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));
                    else
                        RemoveFromSettings("Test Teardown",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));

                    if (suiteSettings.TestSetup.GetName() != "")
                        ReplaceInSettings("Test Setup  " + suiteSettings.TestSetup.GetName() + suiteSettings.TestSetup.ParamsToString()
                            , "Test Setup",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));
                    else
                        RemoveFromSettings("Test Setup",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));

                    if (suiteSettings.SuiteTeardown.GetName() != "")
                        ReplaceInSettings("Suite Teardown  " + suiteSettings.SuiteTeardown.GetName() + suiteSettings.SuiteTeardown.ParamsToString()
                            , "Suite Teardown",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));
                    else
                        RemoveFromSettings("Suite Teardown",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));

                    if (suiteSettings.SuiteSetup.GetName() != "")
                        ReplaceInSettings("Suite Setup  " + suiteSettings.SuiteSetup.GetName() + suiteSettings.SuiteSetup.ParamsToString()
                            , "Suite Setup",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));
                    else
                        RemoveFromSettings("Suite Setup",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));

                    if (suiteSettings.Documentation != null && !suiteSettings.Documentation.Equals(""))
                        ReplaceInSettings("Documentation  " + suiteSettings.Documentation, "Documentation",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));
                    else
                        RemoveFromSettings("Documentation",
                            FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));

                    if (suiteSettings.SuiteSetup != null && !suiteSettings.SuiteSetup.GetName().Trim().Equals("")
                        && !StringAndListOperations.StartsWithVariable(suiteSettings.SuiteSetup.Name))
                    {
                        RemoveKeywordChildrenOfKeywordForOverwriting(suiteSettings.SuiteSetup);
                        AddIncludesFromSettingsKeyword(suiteSettings.SuiteSetup, FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));
                        TestCaseKeywordRemove(suiteSettings.SuiteSetup.GetName(), suiteSettings.SuiteSetup.OutputFilePath, true);
                        AddKeywordToRobot(suiteSettings.SuiteSetup);
                    }
                    if (suiteSettings.SuiteTeardown != null && !suiteSettings.SuiteTeardown.GetName().Trim().Equals("")
                         && !StringAndListOperations.StartsWithVariable(suiteSettings.SuiteTeardown.Name))
                    {
                        RemoveKeywordChildrenOfKeywordForOverwriting(suiteSettings.SuiteTeardown);
                        AddIncludesFromSettingsKeyword(suiteSettings.SuiteTeardown, FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));
                        TestCaseKeywordRemove(suiteSettings.SuiteTeardown.GetName(), suiteSettings.SuiteTeardown.OutputFilePath, true);
                        AddKeywordToRobot(suiteSettings.SuiteTeardown);
                    }
                    if (suiteSettings.TestSetup != null && !suiteSettings.TestSetup.GetName().Trim().Equals("")
                        && !StringAndListOperations.StartsWithVariable(suiteSettings.TestSetup.Name))
                    {
                        RemoveKeywordChildrenOfKeywordForOverwriting(suiteSettings.TestSetup);
                        AddIncludesFromSettingsKeyword(suiteSettings.TestSetup, FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));
                        TestCaseKeywordRemove(suiteSettings.TestSetup.GetName(), suiteSettings.TestSetup.OutputFilePath, true);
                        AddKeywordToRobot(suiteSettings.TestSetup);
                    }
                    if (suiteSettings.TestTeardown != null && !suiteSettings.TestTeardown.GetName().Trim().Equals("")
                        && !StringAndListOperations.StartsWithVariable(suiteSettings.TestTeardown.Name))
                    {
                        RemoveKeywordChildrenOfKeywordForOverwriting(suiteSettings.TestTeardown);
                        AddIncludesFromSettingsKeyword(suiteSettings.TestTeardown, FilesAndFolderStructure.ConcatFileNameToFolder(suiteSettings.OutputFilePath, type));
                        TestCaseKeywordRemove(suiteSettings.TestTeardown.GetName(), suiteSettings.TestTeardown.OutputFilePath, true);
                        AddKeywordToRobot(suiteSettings.TestTeardown);
                    }
                }
        }

        // replaces tags and text when writing settings to the files
        private static void ReplaceInSettings(string replacementText, string tag, string outputFileName)
        {
            var location = RobotFileHandler.LocationInSettings(outputFileName, tag);

            //Add settings tag if not present in the file
            if (RobotFileHandler.HasTag(outputFileName , FormType.Settings) == -1)
            {
                RobotFileHandler.FileLineAdd("*** Settings ***", outputFileName, 0);
                RobotFileHandler.FileLineAdd("", outputFileName, 1);
            }

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
            var location = RobotFileHandler.LocationInSettings(outputFileName, tag);
            if (location[0] != -1)
                RobotFileHandler.FileLineRemove(outputFileName
                                , location);
        }

        internal static void RemoveKeywordChildrenOfKeywordForOverwriting(Keyword keyword)
        {
            if (keyword.Keywords == null) return;
            foreach (var step in keyword.Keywords)
            {
                if (step.Recursive) continue;
                if (!step.Type.Equals(KeywordType.Custom)) continue;
                TestCaseKeywordRemove(step.GetName(), step.OutputFilePath, true);
                RemoveKeywordChildrenOfKeywordForOverwriting(step);
            }
        }

        internal static void AddIncludesFromSettingsKeyword(Keyword keyword, string fileName)
        {
            if (!File.Exists(fileName)) return;
            var container = new Includes(fileName);
            if (keyword.Type == KeywordType.Custom && !StringAndListOperations.StartsWithVariable(keyword.Name))
                Includes[Includes.IndexOf(container)].AddToList(keyword.OutputFilePath);
            else
            if (keyword.Type != KeywordType.Custom)
                if (!keyword.KeywordString.Equals("BuiltIn") && !keyword.KeywordString.Equals("ForLoop"))
                    Includes[Includes.IndexOf(container)].AddToList(keyword.KeywordString);
        }

        internal static void TestCaseKeywordRemove(string name, string fileName, bool isKeyword)
        {
            Console.WriteLine(@"Overwrite (remove): " + name + @" " + (isKeyword ? "Keyword" : "Test") + @"\t" + fileName.Replace(FilesAndFolderStructure.GetFolder(FolderType.Root),""));
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

            var index = RobotFileHandler.LocationOfTestCaseOrKeywordInFile(fileName, name, isKeyword ? FormType.Keyword : FormType.Test);

            if (index == -1) return;
            var endOfTestCaseKeyword = false;
            var temp = new List<string>();
            temp.AddRange(arrLine);

            while (!endOfTestCaseKeyword)
            {
                if (index < temp.Count)
                {
                    if (!temp[index].Trim().Equals(""))
                        Console.WriteLine(@"TestCaseKeywordRemove | Remove line: " + temp[index] + @"\t" + fileName.Replace(FilesAndFolderStructure.GetFolder(FolderType.Root),""));
                    temp.RemoveAt(index);
                } 
                else
                    endOfTestCaseKeyword = true;

                if (index >= temp.Count) continue;
                if (temp[index].StartsWith(" ")) continue;
                if (temp[index].StartsWith("\t")) continue;
                if (temp[index].StartsWith("\\")) continue;
                if (temp[index].StartsWith(".")) continue;
                if (!temp[index].Equals(""))
                    endOfTestCaseKeyword = true;
            }

            File.WriteAllLines(fileName, temp);
        }
    }
}
