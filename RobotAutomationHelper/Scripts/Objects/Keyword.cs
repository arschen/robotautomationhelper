using System;
using System.Collections.Generic;
using RobotAutomationHelper.Scripts.Static;

namespace RobotAutomationHelper.Scripts.Objects
{
    [Serializable]
    internal class Keyword
    {
        private bool _includeImportFile;
        private string _name;
        private string _outputFilePath;
        private bool _recursive;
        private List<Keyword> _keywords;
        private List<Keyword> _forLoopKeywords;
        private string _arguments;
        private List<Param> _params;
        private string _documentation;
        private bool _implemented;
        private KeywordType _type;
        private string _keywordString;
        private int _suggestionIndex;
        private bool _recursive1;
        private string _comments;
        private string _importFileName;

        internal Keyword Parent;
        internal List<Keyword> Keywords { get => _keywords; set => _keywords = value; }
        internal List<Keyword> ForLoopKeywords { get => _forLoopKeywords; set => _forLoopKeywords = value; }
        internal string Arguments { get => _arguments; set => _arguments = value; }
        internal List<Param> Params { get => _params; set => _params = value; }
        internal string Name
        {
            get
            {
                if (!IncludeImportFile) return _name;
                var temp = _name.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (temp.Length == 2)
                    return temp[1];
                var concat = temp[1];
                for (var i = 2; i < temp.Length; i++)
                    concat += "." + temp[i];
                return concat;
            }
            set
            {
                if (!IncludeImportFile) _name = value;
                else
                {
                    if (OutputFilePath != null && !OutputFilePath.Equals(""))
                    {
                        var temp = OutputFilePath.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                        if (_name != null)
                            _name = temp[temp.Length - 1].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0] + "." + _name.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[1];
                        else
                            _name = temp[temp.Length - 1].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0] + "." + value;
                    }
                    else
                        _name = value;
                }
            }
        }
        internal string Documentation { get => _documentation; set => _documentation = value; }
        internal string OutputFilePath
        {
            get => _outputFilePath;
            set
            {
                if (IncludeImportFile && _outputFilePath != null)
                {
                    var tempOutputFileSplit = _outputFilePath.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                    ImportFileName = tempOutputFileSplit[tempOutputFileSplit.Length - 1].Replace(".robot", "");
                }
                _outputFilePath = value;
            }
        }
        internal bool Implemented { get => _implemented; set => _implemented = value; }
        internal KeywordType Type { get => _type; set => _type = value; }
        internal string KeywordString { get => _keywordString; set => _keywordString = value; }
        internal int SuggestionIndex { get => _suggestionIndex; set => _suggestionIndex = value; }
        internal bool Recursive { get => _recursive1; set => _recursive1 = value; }
        internal string Comments { get => _comments; set => _comments = value; }
        internal string ImportFileName { get => _importFileName; set => _importFileName = value; }
        internal bool IncludeImportFile { get => _includeImportFile; set => _includeImportFile = value; }

        internal Keyword(string name, string documentation, List<Keyword> keywords, string arguments, List<Param> paramsList, string outputFilePath, KeywordType type, int suggestionIndex, string keywordString, Keyword parent, bool includeImportFile)
        {
            OutputFilePath = outputFilePath;
            IncludeImportFile = includeImportFile;
            Name = name;
            Documentation = documentation;
            Keywords = keywords != null ? keywords.DeepClone() : new List<Keyword>();
            ForLoopKeywords = ForLoopKeywords != null ? ForLoopKeywords.DeepClone() : new List<Keyword>();
            Params = paramsList != null ? paramsList.DeepClone() : new List<Param>();
            Arguments = arguments;
            Implemented = true;
            Type = type;
            SuggestionIndex = suggestionIndex;
            Recursive = false;
            KeywordString = keywordString;
            Parent = parent;
        }

        internal Keyword(Keyword parent)
        {
            Parent = parent;
        }

        internal void CopyKeyword(Keyword keyword)
        {
            Name = keyword.Name;
            Documentation = keyword.Documentation;

            Keywords = keyword.Keywords != null ? keyword.Keywords.DeepClone() : new List<Keyword>();

            ForLoopKeywords = keyword.ForLoopKeywords != null ? keyword.ForLoopKeywords.DeepClone() : new List<Keyword>();

            Params = keyword.Params != null ? keyword.Params.DeepClone() : new List<Param>();

            Arguments = keyword.Arguments;
            OutputFilePath = keyword.OutputFilePath;
            Implemented = keyword.Implemented;
            Type = keyword.Type;
            SuggestionIndex = keyword.SuggestionIndex;
            KeywordString = keyword.KeywordString;
        }

        internal Keyword(string name, string outputFilePath, Keyword parent)
        {
            Name = name;
            OutputFilePath = outputFilePath;
            SuggestionIndex = -1;
            Recursive = false;
            Parent = parent;
        }

        // convert keyword string taken from file into keyword
        internal Keyword(string keywordString, string outputFilePath, ICollection<string> libsToCheck, Keyword parent)
        {
            Parent = parent;
            if (!keywordString.Equals(""))
            {
                if (keywordString.Trim().StartsWith("#"))
                {
                    Comments = keywordString;
                    OutputFilePath = outputFilePath;
                    SuggestionIndex = -1;
                    Type = KeywordType.Comment;
                }
                else
                {
                    Implemented = true;
                    keywordString = keywordString.Trim();
                    var splitKeyword = !StringAndListOperations.StartsWithVariable(keywordString) ? keywordString.Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries) : new[] { keywordString };

                    if (!splitKeyword[0].StartsWith("{") && splitKeyword[0].Contains("."))
                    {
                        IncludeImportFile = true;
                        ImportFileName = splitKeyword[0].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
                    }

                    var found = false;
                    foreach (var lib in SuggestionsClass.Suggestions)
                        if (libsToCheck.Contains(lib.Name))
                            foreach (var key in lib.LibKeywords)
                            {
                                if (splitKeyword[0].ToLower().Trim().Equals(key.Name.ToLower().Trim()))
                                {
                                    CopyKeyword(key);
                                    found = true;
                                    break;
                                }
                            }

                    if (found)
                    {
                        for (var i = 1; i < splitKeyword.Length; i++)
                        {
                            if (!splitKeyword[i].Contains("="))
                            {
                                if (i - 1 < Params.Count)
                                    Params[i - 1].Value = splitKeyword[i];
                                else
                                    Params[Params.Count - 1].Value += "  " + splitKeyword[i];
                            }
                            else
                            {
                                // check if after splitting the first string matches any param name
                                var temp = splitKeyword[i].Split('=');
                                foreach (var tempParam in Params)
                                {
                                    if (tempParam.Name.ToLower().Trim().Equals(temp[0].ToLower().Trim()))
                                        tempParam.Value = splitKeyword[i].Replace(temp[0] + "=", "");
                                }
                            }
                        }
                    }
                    else
                    {
                        Name = splitKeyword[0];
                        if (splitKeyword.Length > 1)
                            Params = new List<Param>();
                        for (var i = 1; i < splitKeyword.Length; i++)
                        {
                            if (!splitKeyword[i].Contains("="))
                                Params.Add(new Param("", splitKeyword[i]));
                            else
                            {
                                // check if after splitting the first string matches any param name
                                var temp = splitKeyword[i].Split('=');
                                Params.Add(new Param(temp[0], temp[1]));
                            }
                        }
                        OutputFilePath = outputFilePath;
                        SuggestionIndex = -1;
                    }
                }
            }
            else
            {
                Name = "";
                OutputFilePath = outputFilePath;
                SuggestionIndex = -1;
            }
        }

        internal string ParamsToString()
        {
            var paramsString = "";
            if (Params == null) return paramsString;
            foreach (var key in SuggestionsClass.GetLibKeywordsByName(KeywordString))
                if (key.Name.Equals((Name)))
                {
                    for (var i = 0; i < Params.Count; i++)
                    {
                        if (Params[i].Value.Equals(key.Params[i].Value)) continue;
                        paramsString += "  " + Params[i].Value;
                    }
                    return paramsString;
                }
            foreach (var tempParam in Params)
                paramsString += "  " + tempParam.Value;
            return paramsString;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case KeywordType.SeleniumLibrary: return "[SEL] " + Name;
                case KeywordType.Standard: return "[STD] " + Name;
                case KeywordType.ForLoopElements: return "[FORE] " + Name;
                case KeywordType.ForLoopInRange: return "[FORR] " + Name;
                case KeywordType.AppiumLibrary: return "[APPIUM] " + Name;
                case KeywordType.Rest: return "[REST] " + Name;
                case KeywordType.FakerLibrary: return "[FAKE] " + Name;
                case KeywordType.Custom:
                    return Name;
                case KeywordType.Comment:
                    return Name;
                case KeywordType.Empty:
                    return Name;
                default: return Name;
            }
        }

        public KeywordType GetTypeBasedOnSuggestionStart(string suggestionStartsWith)
        {
            suggestionStartsWith = suggestionStartsWith.Split(' ')[0];
            switch (suggestionStartsWith)
            {
                case "[SEL]": return KeywordType.SeleniumLibrary;
                case "[STD]": return KeywordType.Standard;
                case "[FORE]": return KeywordType.ForLoopElements;
                case "[FORR]": return KeywordType.ForLoopInRange;
                case "[APPIUM]": return KeywordType.AppiumLibrary;
                case "[REST]": return KeywordType.Rest;
                case "[FAKE]": return KeywordType.FakerLibrary;
                default: return KeywordType.Custom;
            }
        }

        internal bool IsRecursive(Keyword keyword)
        {
            if (keyword.Parent != null)
                if (Name.ToLower().Equals(keyword.Parent.Name.Trim().ToLower()))
                {
                    _recursive = true;
                    return true;
                }
                else
                {
                    IsRecursive(keyword.Parent);
                }

            if (!_recursive) return false;
            _recursive = false;
            return true;

        }

        internal string GetName()
        {
            return _name;
        }
    }

    internal enum KeywordType
    {
        Custom, SeleniumLibrary, AppiumLibrary, FakerLibrary, Rest, Standard, ForLoopInRange, ForLoopElements, Comment, Empty
    }
}
