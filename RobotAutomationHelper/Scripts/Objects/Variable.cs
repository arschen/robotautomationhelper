using System;
using System.Collections.Generic;

namespace RobotAutomationHelper.Scripts.Objects
{

    [Serializable]
    internal class Variables
    {
        internal List<string> VariableNames { get; set; }
        internal string OutputFilePath;

        internal Variables(List<string> names, string outputFilePath)
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
