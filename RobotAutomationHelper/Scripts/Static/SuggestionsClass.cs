using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using RobotAutomationHelper.Scripts.Objects;

namespace RobotAutomationHelper.Scripts.Static
{
    internal static class SuggestionsClass
    {
        internal static List<Lib> Suggestions = new List<Lib>();

        internal static void CleanUp()
        {
            Suggestions = new List<Lib>();
        }

        internal static bool IsInSuggestionsList(string name)
        {
            foreach (var lib in Suggestions)
                if (lib.ToInclude)
                    foreach (var suggestedKeyword in lib.LibKeywords)
                    if (suggestedKeyword.Name.Trim().ToLower().Equals(name.Trim().ToLower()))
                    {
                        return true;
                    }
            return false;
        }

        internal static bool ContainsName(string name, bool toInclude, bool isPopulating)
        {
            var fileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "Preferences.txt";
            if (File.Exists(fileName))
            {
                var arrLine = File.ReadAllLines(fileName);
                for (var i = 0; i < arrLine.Length; i++)
                {
                    if (!arrLine[i].StartsWith(name + "=")) continue;
                    var tempString = arrLine[i].Split(new []{ "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempString[tempString.Length - 1].ToLower().Equals(toInclude.ToString().ToLower()))
                        return toInclude;
                    if (isPopulating) return toInclude;
                    tempString[tempString.Length - 1] = toInclude.ToString().ToLower();
                    arrLine[i] = tempString[0] + "=" + tempString[tempString.Length - 1];
                    File.WriteAllLines(fileName, arrLine);
                    return toInclude;
                }

                var tempList = new List<string>();
                tempList.AddRange(arrLine);
                tempList.Add(name + "=" + toInclude.ToString().ToLower());
                File.WriteAllLines(fileName, tempList);
                return toInclude;
            }

            var myFile = File.Create(fileName);
            myFile.Close();
            File.WriteAllLines(fileName, new[] { name + @"=" + toInclude.ToString().ToLower() });
            return toInclude;
        }

        internal static void PopulateSuggestionsList()
        {
            var lib = new Lib
            {
                Name = "CUSTOM",
                LibKeywords = new List<Keyword>(),
                ToInclude = ContainsName("CUSTOM", true, true),
                KeyType = KeywordType.Custom
            };
            Suggestions.Add(lib);

            var d = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(),
                @"RobotKeywords\Standard libraries\"));
            foreach (var file in d.GetFiles("*.xlsx", SearchOption.AllDirectories))
            {
                lib = new Lib
                {
                    Name = file.Name.Replace(".xlsx", ""),
                    ToInclude = ContainsName(file.Name.Replace(".xlsx", ""), true, true),
                    LibKeywords = ExcelLibsGetter.ReadAllKeywordsFromExcelSecondType(file.FullName, KeywordType.Standard),
                    KeyType = KeywordType.Standard
                };
                Suggestions.Add(lib);
            }

            d = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(),
                @"RobotKeywords\External libraries\"));
            foreach (var file in d.GetFiles("*.xlsx", SearchOption.AllDirectories))
            {
                var type = KeywordType.Standard;
                foreach (KeywordType temp in Enum.GetValues(typeof(KeywordType)))
                {
                    if (!temp.ToString().ToLower().Equals(file.Name.Replace(".xlsx", "").ToLower())) continue;
                    type = temp;
                    break;
                }

                lib = new Lib
                {
                    Name = file.Name.Replace(".xlsx", ""),
                    ToInclude = ContainsName(file.Name.Replace(".xlsx", ""), true, true),
                    LibKeywords = ExcelLibsGetter.ReadAllKeywordsFromExcelSecondType(file.FullName, type),
                    KeyType = type
                };
                Suggestions.Add(lib);
            }

            PopulateForLoops();
        }

        private static void PopulateForLoops()
        {
            var currentKeywordParams = new List<Param>
            {
                new Param("Param name", ""),
                new Param("Initial value", ""),
                new Param("End value", "")
            };
            var forLoopInRange = new Keyword("ForLoopInRange",
                "For loop from the initial value to the end value ( excluding ) using the param.", null, "",
                currentKeywordParams, "",
                KeywordType.ForLoopInRange, -1, "ForLoop", null, false);

            var lib = new Lib
            {
                Name = "FOR_LOOP_IN_RANGE",
                LibKeywords = new List<Keyword>(),
                ToInclude = true,
                KeyType = KeywordType.ForLoopInRange
            };
            lib.LibKeywords.Add(forLoopInRange);
            Suggestions.Add(lib);

            currentKeywordParams = new List<Param>
            {
                new Param("Param name", ""),
                new Param("List", "")
            };

            var forLoopElements = new Keyword("ForLoopElements",
                "Loops through all the values in the provided list.", null, "",
                currentKeywordParams, "",
                KeywordType.ForLoopElements, -1, "ForLoop", null, false);

            var lib1 = new Lib
            {
                Name = "FOR_LOOP_ELEMENTS",
                LibKeywords = new List<Keyword>(),
                ToInclude = true,
                KeyType = KeywordType.ForLoopElements
            };
            lib1.LibKeywords.Add(forLoopElements);
            Suggestions.Add(lib1);
        }

        internal static Keyword GetForLoop(KeywordType keywordType)
        {
            foreach (var lib in Suggestions)
                if (lib.ToInclude)
                    foreach (var temp in lib.LibKeywords)
                        if (temp.Type.Equals(keywordType))
                            return temp;
            return null;
        }

        internal static List<Keyword> GetLibKeywordsByName(string Name)
        {
            foreach (var lib in Suggestions)
                if (lib.Name.Equals(Name))
                    return lib.LibKeywords;
            return null;
        }

        private static List<string> _suggestionsToInclude;
        internal static List<string> UpdateSuggestionsToIncludes(List<TestCase> testCases, List<SuiteSettings> suiteSettingsKeywordsList)
        {
            _suggestionsToInclude = new List<string>
            {
                "BuiltIn"
            };

            if (testCases != null)
                foreach (var test in testCases)
                    if (test.Steps != null)
                        foreach (var keyword in test.Steps)
                            UpdateSuggestionsKeywordToIncludes(keyword);

            foreach (var suiteKeyword in suiteSettingsKeywordsList)
            {
                if (suiteKeyword.GetKeywords().Count == 0) continue;
                foreach (Keyword keyword in suiteKeyword.GetKeywords())
                    UpdateSuggestionsKeywordToIncludes(keyword);
            }

            return _suggestionsToInclude;
        }

        private static void UpdateSuggestionsKeywordToIncludes(Keyword keyword)
        {
            foreach (var lib in Suggestions)
                if (lib.Name.Equals(keyword.KeywordString))
                {
                    if (_suggestionsToInclude.Contains(lib.Name)) continue;
                    lib.ToInclude = true;
                    _suggestionsToInclude.Add(lib.Name);
                }

            if (keyword.Keywords != null)
                foreach (var key in keyword.Keywords)
                    if (!key.Recursive)
                        UpdateSuggestionsKeywordToIncludes(key);

            if (keyword.ForLoopKeywords == null) return;
            {
                foreach (var key in keyword.ForLoopKeywords)
                    if (!key.Recursive)
                        UpdateSuggestionsKeywordToIncludes(key);
            }
        }
    }

    internal class Lib
    {
        internal List<Keyword> LibKeywords;
        internal string Name { get; set; }
        internal bool ToInclude;
        internal KeywordType KeyType;

        public override string ToString()
        {
            return Name;
        }
    }
}