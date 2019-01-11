using System;
using System.Collections.Generic;

namespace RobotAutomationHelper.Scripts.Objects
{

    [Serializable]
    public class Variables
    {
        public List<string> VariableNames { get; set; }
        public string OutputFilePath;

        public Variables(List<string> names, string outputFilePath)
        {
            VariableNames = names;
            OutputFilePath = outputFilePath;
        }

        public override string ToString()
        {
            return OutputFilePath;
        }
    }
}
