using System;
using System.Collections.Generic;

namespace RobotAutomationHelper
{
    public class Keyword
    {
        private readonly List<Keyword> Keywords;
        private readonly string Arguments;
        private List<Param> Params;
        private string Name;
        private readonly string Documentation;
        private readonly string OutputFilePath;
        private readonly bool implemented = false;

        public Keyword(string Name, string Documentation, List<Keyword> Keywords, string Arguments, List<Param> Params, string OutputFilePath)
        {
            this.Name = Name;
            this.Documentation = Documentation;
            this.Keywords = Keywords;
            this.Arguments = Arguments;
            this.Params = Params;
            this.OutputFilePath = OutputFilePath;
            implemented = true;
        }

        public Keyword(string Name, string OutputFilePath)
        {
            this.Name = Name;
            this.OutputFilePath = OutputFilePath;
        }

        public string GetKeywordName()
        {
            return this.Name;
        }

        public void SetKeywordName(string name)
        {
            this.Name = name;
        }

        public List<Param> GetKeywordParams()
        {
            return this.Params;
        }

        public void SetKeywordParams(List<Param> Params)
        {
            this.Params = Params;
        }

        public string GetKeywordDocumentation()
        {
            return this.Documentation;
        }

        public string GetKeywordArguments()
        {
            return this.Arguments;
        }

        public string GetOutputFilePath()
        {
            return this.OutputFilePath;
        }

        public List<Keyword> GetKeywordKeywords()
        {
            return this.Keywords;
        }

        internal string ParamsToString()
        {
            string paramsString = "";
            if (Params != null)
                foreach (Param tempParam in Params)
                    paramsString += "  " + tempParam.GetParamValue();
            return paramsString;
        }
    }
}
