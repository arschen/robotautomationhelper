using System;

namespace RobotAutomationHelper
{
    [Serializable]
    internal class Param
    {
        internal string Name { get; set; }
        internal string Value { get; set; }

        internal Param(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
    }
}