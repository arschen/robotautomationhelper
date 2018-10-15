using System.Collections.Generic;

namespace RobotAutomationHelper
{
    internal class TestCase
    {
        private List<Keyword> Steps;
        private string Name;
        private string Documentation;
        private string Tags;
        private string OutputFilePath;
        internal bool Overwrite { get; set; }

        internal TestCase(string Name, string Documentation, string Tags, List<Keyword> Steps, string outputFilePath)
        {
            this.Name = Name;
            this.Documentation = Documentation;
            this.Tags = Tags;
            this.Steps = Steps;
            this.OutputFilePath = outputFilePath;
            Overwrite = false;
        }

        internal string GetTestName()
        {
            return this.Name;
        }

        internal void SetTestName(string Name)
        {
            this.Name = Name;
        }

        internal string GetTestDocumentation()
        {
            return this.Documentation;
        }

        internal List<Keyword> GetTestSteps()
        {
            return this.Steps;
        }

        internal string GetTestCaseTags()
        {
            return this.Tags;
        }

        internal string GetOutputFilePath()
        {
            return this.OutputFilePath;
        }
    }
}
