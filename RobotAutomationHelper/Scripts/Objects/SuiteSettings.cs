using System.Collections.Generic;

namespace RobotAutomationHelper.Scripts.Objects
{
    internal class SuiteSettings
    {
        internal Keyword TestSetup { get; set; }
        internal Keyword TestTeardown { get; set; }
        internal Keyword SuiteSetup { get; set; }
        internal Keyword SuiteTeardown { get; set; }
        internal string Documentation { get; set; }
        internal bool Overwrite { get; set; }
        internal string OutputFilePath { get; }

        internal SuiteSettings(string outputFilePath)
        {
            OutputFilePath = outputFilePath;
            Overwrite = false;
        }

        internal List<Keyword> GetKeywords()
        {
            var listOfKeywords = new List<Keyword>();
            if (TestSetup != null) listOfKeywords.Add(TestSetup);
            if (TestTeardown != null) listOfKeywords.Add(TestTeardown);
            if (SuiteSetup != null) listOfKeywords.Add(SuiteSetup);
            if (SuiteTeardown != null) listOfKeywords.Add(SuiteTeardown);
            return listOfKeywords;
        }

        public override string ToString()
        {
            return OutputFilePath;
        }
    }
}
