using RobotAutomationHelper.Scripts.Objects;

namespace RobotAutomationHelper.Scripts.Static
{
    public static class TestCasesListOperations
    {
        public static string IsPresentInTheTestCasesTree(string name, string fileName, TestCase thisTestCase)
        {
            foreach (var test in Forms.RobotAutomationHelper.TestCases)
                if (test.Name.Trim().ToLower().Equals(name.ToLower()))
                    if (!ReferenceEquals(test, thisTestCase))
                        if (test.OutputFilePath.ToLower().Equals(fileName.ToLower()))
                    return fileName + " | " + name;
            return "";
        }

        public static string IsPresentInTheKeywordTree(string name, string fileName, Keyword thisKeyword)
        {
            foreach (var test in Forms.RobotAutomationHelper.TestCases)
                if (test.Steps != null)
                    foreach (var keyword in test.Steps)
                    {
                        if (!ReferenceEquals(keyword, thisKeyword)
                            && keyword.Name.Trim().ToLower().Equals(name.ToLower())
                            && keyword.OutputFilePath.ToLower().Equals(fileName.ToLower()))
                            return test.Name + " | " + keyword.Name.Trim();

                        if (keyword.Keywords != null)
                            foreach (var key in keyword.Keywords)
                            {
                                if (!key.Recursive)
                                {
                                    var temp = IsPresentInChildrenKeywords(name, key, fileName, thisKeyword, test.Name + " | " + keyword.Name.Trim());
                                    if (!temp.Equals(""))
                                        return temp;
                                }
                            }
                    }
            return "";
        }

        private static string IsPresentInChildrenKeywords(string name, Keyword keyword, string fileName, Keyword thisKeyword, string path)
        {
            if (!ReferenceEquals(keyword, thisKeyword) && keyword.Implemented
                && keyword.Name.Trim().ToLower().Equals(name.ToLower())
                && keyword.OutputFilePath.ToLower().Equals(fileName.ToLower()))
                return path + " | " + keyword.Name.Trim();

            if (keyword.Keywords != null)
                foreach (var key in keyword.Keywords)
                {
                    if (!key.Recursive)
                    {
                        var temp = IsPresentInChildrenKeywords(name, key, fileName, thisKeyword, path + " | " + keyword.Name.Trim());
                        if (!temp.Equals(""))
                            return temp;
                    }
                }
            return "";
        }

        public static void OverwriteOccurrencesInKeywordTree(string name, string fileName, Keyword thisKeyword)
        {
            foreach (var test in Forms.RobotAutomationHelper.TestCases)
                if (test.Steps != null)
                    foreach (var keyword in test.Steps)
                    {
                        if (!ReferenceEquals(keyword, thisKeyword)
                            && keyword.Name.Trim().ToLower().Equals(name.ToLower())
                            && keyword.OutputFilePath.ToLower().Equals(fileName.ToLower()))
                            if (keyword.SuggestionIndex == thisKeyword.SuggestionIndex)
                                keyword.CopyKeyword(thisKeyword);

                        if (keyword.Keywords != null)
                            foreach (var key in keyword.Keywords)
                            {
                                if (!key.Recursive)
                                {
                                    OverwriteOccurrencesInChildrenKeywords(name, key, fileName, thisKeyword);
                                }
                            }
                    }
        }

        private static void OverwriteOccurrencesInChildrenKeywords(string name, Keyword keyword, string fileName, Keyword thisKeyword)
        {
            if (!ReferenceEquals(keyword, thisKeyword) && keyword.Implemented
                && keyword.Name.Trim().ToLower().Equals(name.ToLower())
                && keyword.OutputFilePath.ToLower().Equals(fileName.ToLower()))
                if (keyword.SuggestionIndex == thisKeyword.SuggestionIndex)
                    keyword.CopyKeyword(thisKeyword);

            if (keyword.Keywords != null)
                foreach (var key in keyword.Keywords)
                {
                    if (!key.Recursive)
                    {
                        OverwriteOccurrencesInChildrenKeywords(name, key, fileName, thisKeyword);
                    }
                }
        }
    }
}
