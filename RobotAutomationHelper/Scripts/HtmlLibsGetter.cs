using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RobotAutomationHelper.Scripts
{
    internal static class HtmlLibsGetter
    {

        internal static List<Keyword> Selenium = new List<Keyword>();
        internal static List<Keyword> BuiltIn = new List<Keyword>();
        internal static string currentKeywordDocumentation;
        internal static string currentKeywordName;
        internal static List<Param> currentKeywordParams;

        internal static void PopulateSeleniumKeywords()
        {
            Selenium = ReadAllKeywordsFromExcel(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                @"RobotKeywords\Selenium.xlsx")
                , KeywordType.SELENIUM);
            foreach (Keyword key in Selenium)
                FormControls.Suggestions.Add(new Keyword(key.GetKeywordName(), key.GetKeywordDocumentation(),
                    key.GetKeywordKeywords(), key.GetKeywordArguments(), key.GetKeywordParams(),
                    key.GetOutputFilePath(), key.IsSaved(), key.Type));
        }

        internal static void PopulateBuiltInKeywords()
        {
            BuiltIn = ReadAllKeywordsFromExcel(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"RobotKeywords\Built_in.xlsx")
                , KeywordType.BUILT_IN);
            foreach (Keyword key in BuiltIn)
                FormControls.Suggestions.Add(new Keyword(key.GetKeywordName(), key.GetKeywordDocumentation(),
                    key.GetKeywordKeywords(), key.GetKeywordArguments(), key.GetKeywordParams(),
                    key.GetOutputFilePath(), key.IsSaved(), key.Type));
        }

        private static List<Keyword> ReadAllKeywordsFromExcel(string Filename, KeywordType type)
        {
            List<Keyword> listKeys = new List<Keyword>();
            currentKeywordParams = new List<Param>();
            currentKeywordDocumentation = "";
            currentKeywordName = "";

            var package = new ExcelPackage(new FileInfo(Filename));
            ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

            for (int i = workSheet.Dimension.Start.Row;
                                    i <= workSheet.Dimension.End.Row;
                                    i++)
            {
                if (i>1)
                for (int j = workSheet.Dimension.Start.Column;
                            j <= workSheet.Dimension.End.Column;
                            j++)
                {
                    if (workSheet.Cells[i, j].Value != null)
                    {
                        string cellValue = workSheet.Cells[i, j].Value.ToString().Trim();
                        //key column equals test name in robot
                        if (j == 1 && !cellValue.Equals(""))
                        {
                            if (!currentKeywordName.Equals(""))
                            {
                                //Setup test creation for previous Test case
                                AddKeywordAndResetValues(listKeys, type);
                                if (!currentKeywordName.Equals(cellValue))
                                    currentKeywordName = cellValue;
                                else
                                    currentKeywordName = "";
                            }
                            else
                            {
                                currentKeywordName = cellValue;
                            }
                        }
                        //summary column equals documentation in robot
                        else if (j == 2 && !cellValue.Equals(""))
                        {
                            string[] paramString = cellValue.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string occurrence in paramString)
                            {
                                if (occurrence.Contains("="))
                                    currentKeywordParams.Add(new Param(occurrence.Trim().Split('=')[0], occurrence.Trim().Split('=')[1]));
                                else
                                    currentKeywordParams.Add(new Param(occurrence.Trim(), ""));
                            }
                        }
                        //summary column equals documentation in robot
                        else if (j == 3 && !cellValue.Equals(""))
                        {
                            currentKeywordDocumentation = cellValue;
                        }
                    }
                }
            }

            if (!currentKeywordName.Equals(""))
            {
                AddKeywordAndResetValues(listKeys, type);
            }

            return listKeys;
        }

        private static void AddKeywordAndResetValues(List<Keyword> keywordsList, KeywordType type)
        {
            keywordsList.Add(new Keyword(currentKeywordName, currentKeywordDocumentation, null, "", currentKeywordParams, "", false, type));
            currentKeywordDocumentation = "";
            currentKeywordName = "";
            currentKeywordParams = new List<Param>();
        }
    }
}
