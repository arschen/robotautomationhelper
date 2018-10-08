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

        internal static bool IsPresentInTheKeywordTree(string name, string fileName, Keyword thisKeyword)
        {
            foreach (TestCase test in RobotAutomationHelper.TestCases)
                foreach (Keyword keyword in test.GetTestSteps())
                {
                    if (keyword.IsSaved() && keyword != thisKeyword 
                        && keyword.GetKeywordName().Trim().ToLower().Equals(name.ToLower())
                        && keyword.GetOutputFilePath().ToLower().Equals(fileName.ToLower()))
                        return true;

                    if (test.GetTestSteps() != null)
                        foreach (Keyword key in test.GetTestSteps())
                            IsPresentInChildrenKeywords(name, key, fileName, thisKeyword);
                }
            return false;
        }

        private static bool IsPresentInChildrenKeywords(string name, Keyword keyword, string fileName, Keyword thisKeyword)
        {
            if (keyword.IsSaved() && keyword.GetKeywordName().Trim().ToLower().Equals(name.ToLower())
                && keyword.GetOutputFilePath().ToLower().Equals(fileName.ToLower()))
                return true;

            if (keyword.GetKeywordKeywords() != null)
                foreach (Keyword key in keyword.GetKeywordKeywords())
                    IsPresentInChildrenKeywords(name, key, fileName, thisKeyword);
            return false;
        }
    }
}
