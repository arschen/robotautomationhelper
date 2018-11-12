using System;
using System.Collections.Generic;

namespace RobotAutomationHelper.Scripts.Objects
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

        internal TestCase(string name, string documentation, string tags, List<Keyword> steps, string outputFilePath, bool implemented)
        {
            Name = name;
            Documentation = documentation;
            Tags = tags;
            Steps = steps;
            OutputFilePath = outputFilePath;
            Overwrite = false;
            Implemented = implemented;
        }

        internal TestCase(string name, string outputFilePath)
        {
            Name = name;
            OutputFilePath = outputFilePath;
            Documentation = "";
            Tags = "";
        }

        public int CompareTo(TestCase other)
        {
            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
