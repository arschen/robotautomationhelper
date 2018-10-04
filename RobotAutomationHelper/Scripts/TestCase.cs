using System.Collections.Generic;

namespace RobotAutomationHelper
{
    public class TestCase
    {
        private List<Keyword> Steps;
        private string Name;
        private string Documentation;
        private string Tags;
        private string outputFilePath;

        public TestCase(string Name, string Documentation, string Tags, List<Keyword> Steps, string outputFilePath)
        {
            this.Name = Name;
            this.Documentation = Documentation;
            this.Tags = Tags;
            this.Steps = Steps;
            this.outputFilePath = outputFilePath;
        }

        public string GetTestName()
        {
            return this.Name;
        }

        public void SetTestName(string Name)
        {
            this.Name = Name;
        }

        public string GetTestDocumentation()
        {
            return this.Documentation;
        }

        public List<Keyword> GetTestSteps()
        {
            return this.Steps;
        }

        public string GetTestCaseTags()
        {
            return this.Tags;
        }

        public string GetOutputFilePath()
        {
            return this.outputFilePath;
        }
    }
}
