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
            Documentation = "";
            Overwrite = false;
        }

        public override string ToString()
        {
            return OutputFilePath;
        }
    }
}
