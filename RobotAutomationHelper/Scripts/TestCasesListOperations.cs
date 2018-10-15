namespace RobotAutomationHelper.Scripts
{
    internal static class TestCasesListOperations
    {
        internal static bool IsPresentInTheTestCasesTree(string name, string fileName, TestCase thisTestCase)
        {
            foreach (TestCase test in RobotAutomationHelper.TestCases)
                if (test.GetTestName().Trim().ToLower().Equals(name.ToLower())
                    && test != thisTestCase && test.GetOutputFilePath().ToLower().Equals(fileName.ToLower()))
                    return true;
            return false;
        }

        internal static string IsPresentInTheKeywordTree(string name, string fileName, Keyword thisKeyword)
        {
            foreach (TestCase test in RobotAutomationHelper.TestCases)
                if (test.GetTestSteps() != null)
                    foreach (Keyword keyword in test.GetTestSteps())
                    {
                        if (keyword.IsSaved() && keyword != thisKeyword 
                            && keyword.GetKeywordName().Trim().ToLower().Equals(name.ToLower())
                            && keyword.GetOutputFilePath().ToLower().Equals(fileName.ToLower()))
                            return test.GetTestName() + " | " + keyword.GetKeywordName();

                        if (keyword.GetKeywordKeywords() != null)
                            foreach (Keyword key in keyword.GetKeywordKeywords())
                            {
                                string temp = IsPresentInChildrenKeywords(name, key, fileName, thisKeyword, test.GetTestName() + " | " + keyword.GetKeywordName().Trim());
                                if (!temp.Equals(""))
                                return temp;
                            }
                    }
            return "";
        }

        private static string IsPresentInChildrenKeywords(string name, Keyword keyword, string fileName, Keyword thisKeyword, string path)
        {
            if (keyword.IsSaved() && keyword != thisKeyword
                && keyword.GetKeywordName().Trim().ToLower().Equals(name.ToLower())
                && keyword.GetOutputFilePath().ToLower().Equals(fileName.ToLower()))
                return path + " | " + keyword.GetKeywordName().Trim();

            if (keyword.GetKeywordKeywords() != null)
                foreach (Keyword key in keyword.GetKeywordKeywords())
                    IsPresentInChildrenKeywords(name, key, fileName, thisKeyword, path + " | " + keyword.GetKeywordName().Trim());
            return "";
        }
    }
}
