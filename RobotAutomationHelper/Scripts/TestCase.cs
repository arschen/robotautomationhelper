using System.Collections.Generic;

namespace RobotAutomationHelper
{
    internal class TestCase
    {
        internal List<Keyword> Steps { get; }
        internal string Name { get; set; }
        internal string Documentation { get; }
        internal string Tags { get; }
        internal string OutputFilePath { get; }
        internal bool Overwrite { get; set; }

        internal TestCase(string Name, string Documentation, string Tags, List<Keyword> Steps, string outputFilePath)
        {
            this.Name = Name;
            this.Documentation = Documentation;
            this.Tags = Tags;
            this.Steps = Steps;
            OutputFilePath = outputFilePath;
            Overwrite = false;
        }
    }
}
