using System;
using System.Collections.Generic;

namespace RobotAutomationHelper
{
    internal class TestCase: IComparable<TestCase>
    {
        internal List<Keyword> Steps { get; set; }
        internal string Name { get; set; }
        internal string Documentation { get; set;  }
        internal string Tags { get; set; }
        internal string OutputFilePath { get; set;  }
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

        internal void CopyTestCase(TestCase testCase)
        {
            Steps = testCase.Steps;
            Name = testCase.Name;
            Documentation = testCase.Documentation;
            Tags = testCase.Tags;
            OutputFilePath = testCase.OutputFilePath;
            Overwrite = testCase.Overwrite;
            Implemented = testCase.Implemented;
        }

        public int CompareTo(TestCase other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
