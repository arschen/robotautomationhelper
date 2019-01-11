using System;

namespace RobotAutomationHelper.Scripts.Objects
{
    [Serializable]
    public class Param
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Param(string name, string value)
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