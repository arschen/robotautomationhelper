using System;
using System.Collections.Generic;

namespace RobotAutomationHelper
{
    internal class TestCase: IComparable<TestCase>
    {
        internal List<Keyword> Steps { get; }
        internal string Name { get; set; }
        internal string Documentation { get; }
        internal string Tags { get; }
        internal string OutputFilePath { get; }
        internal bool Overwrite { get; set; }
        internal bool Implemented { get; set; }

        internal TestCase(string Name, string Documentation, string Tags, List<Keyword> Steps, string outputFilePath, bool implemented)
        {
            this.Name = Name;
            this.Documentation = Documentation;
            this.Tags = Tags;
            this.Steps = Steps;
            OutputFilePath = outputFilePath;
            Overwrite = false;
            Implemented = implemented;
        }

        public int CompareTo(TestCase other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
