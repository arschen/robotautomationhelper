using System;
using System.Collections.Generic;

namespace RobotAutomationHelper.Scripts.Objects
{

    [Serializable]
    internal class Variables
    {
        internal List<string> VariableNames { get; set; }
        private readonly string _outputFilePath;

        internal Variables(List<string> names, string outputFilePath)
        {
            VariableNames = names;
            _outputFilePath = outputFilePath;
        }

        public override string ToString()
        {
            return _outputFilePath;
        }
    }
}
