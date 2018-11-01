using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RobotAutomationHelper.Scripts
{
    internal static class SuggestionsClass
    {
        internal static List<Keyword> Suggestions = new List<Keyword>();
        private static List<Keyword> Selenium = new List<Keyword>();
        private static List<Keyword> BuiltIn = new List<Keyword>();

        internal static void CleanUp()
        {
            Suggestions = new List<Keyword>();
            Selenium = new List<Keyword>();
            BuiltIn = new List<Keyword>();
        }

        internal static bool IsInSuggestionsList(string name)
        {
            foreach (Keyword SuggestedKeyword in Suggestions)
                if (SuggestedKeyword.Name.Trim().ToLower().Equals(name.Trim().ToLower()))
                {
                    return true;
                }
            return false;
        }

        internal static void PopulateSuggestionsList(bool Selenium, bool BuiltIn)
        {
            PopulateForLoops();
            if (Selenium)
                PopulateSeleniumKeywords();
            if (BuiltIn)
                PopulateBuiltInKeywords();
        }

        private static void PopulateSeleniumKeywords()
        {
            Selenium = ExcelLibsGetter.ReadAllKeywordsFromExcel(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"RobotKeywords\Selenium.xlsx")
                , KeywordType.SELENIUM);
            foreach (Keyword key in Selenium)
                SuggestionsClass.Suggestions.Add(new Keyword(key.Name, key.Documentation,
                    key.Keywords, key.Arguments, key.Params,
                    key.OutputFilePath, key.Saved, key.Type, key.SuggestionIndex));
        }

        private static void PopulateBuiltInKeywords()
        {
            BuiltIn = ExcelLibsGetter.ReadAllKeywordsFromExcel(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"RobotKeywords\Built_in.xlsx")
                , KeywordType.BUILT_IN);
            foreach (Keyword key in BuiltIn)
                SuggestionsClass.Suggestions.Add(new Keyword(key.Name, key.Documentation,
                    key.Keywords, key.Arguments, key.Params,
                    key.OutputFilePath, key.Saved, key.Type, key.SuggestionIndex));
        }

        private static void PopulateForLoops()
        {
            List<Param> currentKeywordParams = new List<Param>();
            currentKeywordParams.Add(new Param("Param name", ""));
            currentKeywordParams.Add(new Param("Initial value", ""));
            currentKeywordParams.Add(new Param("End value", ""));
            Keyword ForLoopInRange = new Keyword("ForLoopInRange", 
                "For loop from the initial value to the end value ( excluding ) using the param.", null, "", 
                currentKeywordParams, "", false, 
                KeywordType.FOR_LOOP_IN_RANGE, -1);

            Suggestions.Add(ForLoopInRange);

            currentKeywordParams = new List<Param>();
            currentKeywordParams.Add(new Param("Param name", ""));
            currentKeywordParams.Add(new Param("List", ""));
            Keyword ForLoopElements = new Keyword("ForLoopElements",
                "Loops through all the values in the provided list.", null, "",
                currentKeywordParams, "", false,
                KeywordType.FOR_LOOP_ELEMENTS, -1);

            Suggestions.Add(ForLoopElements);
        }
    }
}