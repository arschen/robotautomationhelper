using System.Collections.Generic;
using System.IO;

namespace RobotAutomationHelper.Scripts
{
    public static class WriteToRobot
    {
        public static void AddTestCasesToRobot(TestCase testCase)
        {
            string fileName = testCase.GetOutputFilePath();
            string testCaseName = testCase.GetTestName();
            List<Keyword> testCaseTestSteps = testCase.GetTestSteps();
            string testCaseDocumentation = testCase.GetTestDocumentation();
            string testCaseTags = testCase.GetTestCaseTags();
            int index = RobotFileHandler.GetLineAfterLastTestCase(fileName);

            //Add test case to robot file
            FileLineAdd("", fileName, index);
            index++;
            FileLineAdd(testCaseName.Trim(), fileName, index);

            //adds documentation
            if (!testCaseDocumentation.Replace("[Documentation]","").Trim().Equals(""))
            {
                index++;
                FileLineAdd(testCaseDocumentation, fileName, index);
            }

            if (!testCaseTags.Replace("[Tags]", "").Trim().Equals(""))
            {
                index++;
                FileLineAdd(testCaseTags, fileName, index);
            }
            
            if (testCaseTestSteps != null)
                foreach (Keyword testStep in testCaseTestSteps)
                {
                    //adds test steps
                    index++;
                    FileLineAdd(testStep.GetKeywordName(), fileName, index);
                    AddKeywordToRobot(testStep);
                }
        }

        public static void AddKeywordToRobot(Keyword keyword)
        {
            string fileName = keyword.GetOutputFilePath();
            string keywordName = keyword.GetKeywordName();
            List<Keyword> keywordKeywords = keyword.GetKeywordKeywords();
            string keywordDocumentation = keyword.GetKeywordDocumentation();
            string keywordArguments = keyword.GetKeywordArguments();
            int index = RobotFileHandler.GetLineAfterLastKeyword(fileName);

            //Add test case to robot file
            FileLineAdd("", fileName, index);
            index++;
            FileLineAdd(keywordName.Trim(), fileName, index);

            //adds documentation
            if (!keywordDocumentation.Replace("[Documentation]", "").Trim().Equals(""))
            {
                index++;
                FileLineAdd(keywordDocumentation, fileName, index);
            }

            if (!keywordArguments.Replace("[Arguments]", "").Trim().Equals(""))
            {
                index++;
                FileLineAdd(keywordArguments, fileName, index);
            }

            if (keywordKeywords != null)
                foreach (Keyword keywordKeyword in keywordKeywords)
                {
                    //adds test steps
                    index++;
                    FileLineAdd(keywordKeyword.GetKeywordName(), fileName, index);
                    AddKeywordToRobot(keywordKeyword);
                }
        }

        public static void FileLineAdd(string newText, string fileName, int line_to_add_after)
        {
            string[] arrLine;
            if (File.Exists(fileName))
                arrLine = File.ReadAllLines(fileName);
            else
            {
                string directory = fileName.Replace(fileName.Split('\\')[fileName.Split('\\').Length-1], "");
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
    }
}
