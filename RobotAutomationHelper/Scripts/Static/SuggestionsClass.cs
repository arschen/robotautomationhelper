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
    }
}