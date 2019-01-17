using System;
using System.Collections.Generic;

namespace RobotAutomationHelper.Scripts.Objects
{

    [Serializable]
    public class TestCase: IComparable<TestCase>
    {
        public List<Keyword> Steps { get; set; }
        public string Name { get; set; }
        public string Documentation { get; set;  }
        public string Tags { get; set; }
        public string OutputFilePath { get; set;  }
        public bool Overwrite { get; set; }
        public bool Implemented { get; set; }

        public TestCase(string name, string documentation, string tags, List<Keyword> steps, string outputFilePath, bool implemented)
        {
            Name = name;
            Documentation = documentation;
            Tags = tags;
            Steps = steps;
            OutputFilePath = outputFilePath;
            Overwrite = false;
            Implemented = implemented;
        }

        public void CopyTestCase(TestCase testCase)
        {
            Name = testCase.Name;
            Documentation = testCase.Documentation;
            Tags = testCase.Tags;
            Steps = testCase.Steps.DeepClone();
            OutputFilePath = testCase.OutputFilePath;
            Overwrite = false;
            Implemented = testCase.Implemented;
        }

        public TestCase(string name, string outputFilePath)
        {
            Name = name;
            OutputFilePath = outputFilePath;
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
