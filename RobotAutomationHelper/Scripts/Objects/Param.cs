using System;

namespace RobotAutomationHelper.Scripts.Objects
{
    [Serializable]
    internal class Param
    {
        internal string Name { get; set; }
        internal string Value { get; set; }

        internal Param(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return Name + " = " + Value;
        }
    }
}