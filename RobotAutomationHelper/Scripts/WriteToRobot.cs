﻿using System;
using System.Collections.Generic;
using System.IO;

namespace RobotAutomationHelper.Scripts
{

    internal static class WriteToRobot
    {

        internal static List<Includes> includes;

        internal static void AddTestCaseToRobot(TestCase testCase)
        {
            string fileName = testCase.GetOutputFilePath();
            int index = RobotFileHandler.GetLineAfterLastTestCase(fileName);

            bool addTestCase = !(RobotFileHandler.ContainsTestCaseOrKeyword(fileName, testCase.GetTestName().Trim(), "test cases") != -1);
            if (addTestCase)
            {
                Includes candidate = new Includes(fileName);
                if (!includes.Contains(candidate))
                    includes.Add(candidate);

                //Add test case to robot file
                index = AddName(testCase.GetTestName().Trim(), fileName, index, "test cases");

                //adds documentation
                index = AddTagsDocumentationArguments("[Documentation]", testCase.GetTestDocumentation(), fileName, index);

                //adds tags
                index = AddTagsDocumentationArguments("[Tags]", testCase.GetTestCaseTags(), fileName, index);
            }

            index = AddKeyword(testCase.GetTestSteps(), fileName, index, addTestCase);
        }

        internal static void AddKeywordToRobot(Keyword keyword)
        {
            string fileName = keyword.GetOutputFilePath();
            int index = 0;
            if (fileName != "")
                index = RobotFileHandler.GetLineAfterLastKeyword(fileName);

            if (keyword.Type == KeywordType.CUSTOM)
            {
                Includes candidate = new Includes(fileName);
                if (!includes.Contains(candidate))
                    includes.Add(candidate);
            }

            bool addKeywordSteps = !(RobotFileHandler.ContainsTestCaseOrKeyword(fileName, keyword.GetKeywordName().Trim(), "keyword") != -1);
            if (keyword.IsSaved() && addKeywordSteps && (keyword.Type == KeywordType.CUSTOM))
            {
                //Add keyword to robot file
                index = AddName(keyword.GetKeywordName().Trim(), fileName, index, "keywords");

                //adds documentation
                index = AddTagsDocumentationArguments("[Documentation]", keyword.GetKeywordDocumentation(), fileName, index);

                //adds arguments
                index = AddTagsDocumentationArguments("[Arguments]", keyword.GetKeywordArguments(), fileName, index);
            }

            index = AddKeyword(keyword.GetKeywordKeywords(), fileName, index, addKeywordSteps);
        }

        //adds Keywords
        private static int AddKeyword(List<Keyword> keywordKeywords, string fileName, int index, bool addSteps)
        {
            Includes container = new Includes(fileName);
            if (keywordKeywords != null)
                foreach (Keyword keywordKeyword in keywordKeywords)
                {
                    if (keywordKeyword.IsSaved() && keywordKeyword.Type == KeywordType.CUSTOM)
                        includes[includes.IndexOf(container)].AddToList(keywordKeyword.GetOutputFilePath());
                    else
                    if (keywordKeyword.Type == KeywordType.SELENIUM)
                        includes[includes.IndexOf(container)].AddToList("SeleniumLibrary");

                    if (addSteps)
                    {
                        //adds test steps
                        index++;
                        FileLineAdd(keywordKeyword.GetKeywordName() + keywordKeyword.ParamsToString(), fileName, index);
                    }
                    
                    AddKeywordToRobot(keywordKeyword);
                }
            return index;
        }

        // add newText on new line to file fileName after specified line
        internal static void FileLineAdd(string newText, string fileName, int line_to_add_after)
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

            List<string> temp = new List<string>();
            temp.AddRange(arrLine);
            temp.Insert(line_to_add_after, newText);
            File.WriteAllLines(fileName, temp);
        }

        // add newText on new line to file fileName after specified line
        internal static void FileLineReplace(string newText, string fileName, int line_to_add_after)
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

            List<string> temp = new List<string>();
            temp.AddRange(arrLine);
            temp[line_to_add_after] = newText;
            File.WriteAllLines(fileName, temp);
        }

        //adds Tags / Documentation / Arguments
        private static int AddTagsDocumentationArguments(string type, string addString, string fileName, int index)
        {
            if (addString == null)
                addString = "";
            if (!addString.Replace(type, "").Trim().Equals(""))
            {
                index++;
                FileLineAdd(addString, fileName, index);
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
                    FileLineAdd("*** Keywords ***", fileName, index);
                else
                {
                    if (tag.Equals("test cases"))
                    {
                        int tempKeywordsIndex = RobotFileHandler.HasTag(fileName, "keywords");
                        if (tempKeywordsIndex != -1)
                        {
                            FileLineAdd("*** Test Cases ***", fileName, tempKeywordsIndex);
                            index = tempKeywordsIndex;
                        }
                        else
                            FileLineAdd("*** Test Cases ***", fileName, index);
                    }
                }
                index++;
            }else
                if (tempTagIndex + 1 != index)
                {
                    FileLineAdd("", fileName, index);
                    index++;
                }
            FileLineAdd(name, fileName, index);
            return index;
        }

        //Add includes to test case and keywords files
        internal static void AddIncludes()
        {
            int index;
            string tag = "settings";
            string fileName;

            foreach (Includes temp in includes)
            {
                index = 0;
                fileName = temp.GetFileName();
                int tempTagIndex = RobotFileHandler.HasTag(fileName, tag);
                if (tempTagIndex == -1)
                {
                    FileLineAdd("*** Settings ***", fileName, index);
                    index++;
                }
                else
                    index = tempTagIndex + 1;

                foreach (string path in temp.GetFilesToInclude())
                {
                    if (path.Equals("SeleniumLibrary"))
                        FileLineAdd("Library  " + path, fileName, index);
                    else
                        FileLineAdd("Resource  \\" + path.Replace(FilesAndFolderStructure.GetFolder(), ""), fileName, index);
                    index++;
                }
            }
        }
    }
}
