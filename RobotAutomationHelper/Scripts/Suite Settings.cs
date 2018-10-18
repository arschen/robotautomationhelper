namespace RobotAutomationHelper.Scripts
{
    internal class Suite_Settings
    {
        internal Keyword TestSetup { get; }
        internal Keyword TestTeardown { get; }
        internal Keyword SuiteSetup { get; }
        internal Keyword SuiteTeardown { get; }
        internal string Documentation { get; set; }
        internal bool Overwrite { get; set; }
        internal string OutputFilePath { get; }

        internal Suite_Settings(string OutputFilePath)
        {
            this.OutputFilePath = OutputFilePath;
            TestSetup = new Keyword();
            TestTeardown = new Keyword();
            SuiteSetup = new Keyword();
            SuiteTeardown = new Keyword();
            Documentation = "";
            Overwrite = false;
        }
    }
}
