using System.Collections.Generic;

namespace RobotAutomationHelper
{
    internal class Keyword
    {
        private List<Keyword> Keywords;
        private string Arguments;
        private List<Param> Params;
        private string Name;
        private string Documentation;
        private string OutputFilePath;
        private bool Implemented = false;
        private bool Saved = false;
        internal KeywordType Type { get; set; }

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

        internal void CopyKeyword(Keyword keyword)
        {
            this.Name = keyword.Name;
            this.Documentation = keyword.Documentation;
            this.Keywords = keyword.Keywords;
            this.Arguments = keyword.Arguments;
            this.Params = keyword.Params;
            this.OutputFilePath = keyword.OutputFilePath;
            Implemented = true;
            this.Saved = keyword.Saved;
            this.Type = keyword.Type;
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

        public override string ToString()
        {
            switch (Type)
            {
                case KeywordType.SELENIUM: return "[S] " + Name;
                case KeywordType.BUILT_IN: return "[B] " + Name;
            }
            return Name;
        }
    }

    internal enum KeywordType
    {
        SELENIUM, BUILT_IN, CUSTOM
    }
}
