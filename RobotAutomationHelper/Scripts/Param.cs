namespace RobotAutomationHelper
{
    public class Param
    {
        private string ArgName;
        private string ParamValue;

        public Param(string ArgName, string ParamValue)
        {
            this.ArgName = ArgName;
            this.ParamValue = ParamValue;
        }

        public string GetParamValue()
        {
            return ParamValue;
        }

        public void SetParamValue(string ParamValue)
        {
            this.ParamValue = ParamValue;
        }

        public string GetArgName()
        {
            return ArgName;
        }
    }
}