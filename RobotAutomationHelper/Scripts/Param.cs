namespace RobotAutomationHelper
{
    internal class Param
    {
        internal string Name { get; }
        internal string Value { get; set; }

        internal Param(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
    }
}