namespace RobotAutomationHelper
{
    internal class Param
    {
        private string ArgName;
        private string ParamValue;

        internal Param(string ArgName, string ParamValue)
        {
            this.ArgName = ArgName;
            this.ParamValue = ParamValue;
        }

        internal string GetParamValue()
        {
            return ParamValue;
        }

        internal void SetParamValue(string ParamValue)
        {
            this.ParamValue = ParamValue;
        }

        internal string GetArgName()
        {
            return ArgName;
        }
    }
}