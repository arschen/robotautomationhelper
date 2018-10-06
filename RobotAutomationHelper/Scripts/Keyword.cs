using System.Collections.Generic;

namespace RobotAutomationHelper
{
    internal class Keyword
    {
        private readonly List<Keyword> Keywords;
        private readonly string Arguments;
        private List<Param> Params;
        private string Name;
        private readonly string Documentation;
        private readonly string OutputFilePath;
        private readonly bool Implemented = false;
        private readonly bool Saved = false;
        private readonly KeywordType Type;

        internal Keyword(string Name, string Documentation, List<Keyword> Keywords, string Arguments, List<Param> Params, string OutputFilePath, bool Saved, KeywordType Type)
        {
            this.Name = Name;
            this.Documentation = Documentation;
            this.Keywords = Keywords;
            this.Arguments = Arguments;
            this.Params = Params;
            this.OutputFilePath = OutputFilePath;
            Implemented = true;
            this.Saved = Saved;
            this.Type = Type;
        }

        internal Keyword(string Name, string OutputFilePath)
        {
            this.Name = Name;
            this.OutputFilePath = OutputFilePath;
        }

        internal string GetKeywordName()
        {
            return this.Name;
        }

        internal void SetKeywordName(string name)
        {
            this.Name = name;
        }

        internal List<Param> GetKeywordParams()
        {
            return this.Params;
        }

        internal void SetKeywordParams(List<Param> Params)
        {
            this.Params = Params;
        }

        internal string GetKeywordDocumentation()
        {
            return this.Documentation;
        }

        internal string GetKeywordArguments()
        {
            return this.Arguments;
        }

        internal string GetOutputFilePath()
        {
            return this.OutputFilePath;
        }

        internal List<Keyword> GetKeywordKeywords()
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

        internal bool IsSaved()
        {
            return Saved;
        }

        internal bool IsImplemented()
        {
            return Implemented;
        }
    }

    internal enum KeywordType
    {
        SELENIUM, BUILT_IN, CUSTOM
    }
}
