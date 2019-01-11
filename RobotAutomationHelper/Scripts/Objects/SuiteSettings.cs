using System.Collections.Generic;

namespace RobotAutomationHelper.Scripts.Objects
{
    public class SuiteSettings
    {
        public Keyword TestSetup { get; set; }
        public Keyword TestTeardown { get; set; }
        public Keyword SuiteSetup { get; set; }
        public Keyword SuiteTeardown { get; set; }
        public string Documentation { get; set; }
        public bool Overwrite { get; set; }
        public string OutputFilePath { get; }

        public SuiteSettings(string outputFilePath)
        {
            OutputFilePath = outputFilePath;
            Overwrite = false;
        }

        public List<Keyword> GetKeywords()
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
