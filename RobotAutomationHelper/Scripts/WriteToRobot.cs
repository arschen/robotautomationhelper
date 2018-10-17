using System.Collections.Generic;

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
                index = AddTagsDocumentationArguments("[Documentation]", "\t" + testCase.GetTestDocumentation(), fileName, index);

                //adds tags
                index = AddTagsDocumentationArguments("[Tags]", "\t" + testCase.GetTestCaseTags(), fileName, index);
            }

            index = AddKeyword(testCase.GetTestSteps(), fileName, index, addTestCase, testCase.Overwrite);
        }

        internal static void AddKeywordToRobot(Keyword keyword)
        {
            string fileName = keyword.GetOutputFilePath();
            int index = 0;
            if (fileName != "")
                index = RobotFileHandler.GetLineAfterLastKeyword(fileName);

            bool addKeywordSteps = !(RobotFileHandler.ContainsTestCaseOrKeyword(fileName, keyword.GetKeywordName().Trim(), "keyword") != -1);

            if (addKeywordSteps)
                if (keyword.Type == KeywordType.CUSTOM)
                {
                    Includes candidate = new Includes(fileName);
                    if (!includes.Contains(candidate))
                        includes.Add(candidate);
                }

            if (keyword.IsSaved() && addKeywordSteps && (keyword.Type == KeywordType.CUSTOM))
            {
                //Add keyword to robot file
                index = AddName(keyword.GetKeywordName().Trim(), fileName, index, "keywords");

                //adds documentation
                index = AddTagsDocumentationArguments("[Documentation]", "\t" + keyword.GetKeywordDocumentation(), fileName, index);

                //adds arguments
                index = AddTagsDocumentationArguments("[Arguments]", "\t" + keyword.GetKeywordArguments(), fileName, index);
            }

            index = AddKeyword(keyword.GetKeywordKeywords(), fileName, index, addKeywordSteps, keyword.Overwrite);
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
                        if (keywordKeyword.IsSaved() && keywordKeyword.Type == KeywordType.CUSTOM)
                            includes[includes.IndexOf(container)].AddToList(keywordKeyword.GetOutputFilePath());
                        else
                            if (keywordKeyword.Type == KeywordType.SELENIUM)
                            includes[includes.IndexOf(container)].AddToList("SeleniumLibrary");

                        //adds test steps
                        index++;
                        RobotFileHandler.FileLineAdd("\t" + keywordKeyword.GetKeywordName() + keywordKeyword.ParamsToString(), fileName, index);
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
            }else
                if (tempTagIndex + 1 != index)
                {
                RobotFileHandler.FileLineAdd("", fileName, index);
                    index++;
                }
            RobotFileHandler.FileLineAdd(name, fileName, index);
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
                    RobotFileHandler.FileLineAdd("*** Settings ***", fileName, index);
                    index++;
                }
                else
                    index = tempTagIndex + 1;

                foreach (string path in temp.GetFilesToInclude())
                {
                    if (path.Equals("SeleniumLibrary"))
                    {
                        if (!RobotFileHandler.ContainsSettings(fileName, "Library  " + path))
                            RobotFileHandler.FileLineAdd("Library  " + path, fileName, index);
                    }
                    else
                        if (!RobotFileHandler.ContainsSettings(fileName, "Resource  ./" + path.Replace(FilesAndFolderStructure.GetFolder().Replace('\\', '/'), "")))
                            RobotFileHandler.FileLineAdd("Resource  ./" + path.Replace(FilesAndFolderStructure.GetFolder(), "").Replace('\\', '/'), fileName.Replace('\\','/'), index);
                    index++;
                }
            }
        }
    }
}
