using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RobotAutomationHelper.Scripts
{
    internal static class ExcelLibsGetter
    {
        private static string currentKeywordDocumentation;
        private static string currentKeywordName;
        private static List<Param> currentKeywordParams;

        internal static List<Keyword> ReadAllKeywordsFromExcel(string Filename, KeywordType type)
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

        internal static List<Keyword> ReadAllKeywordsFromExcelSecondType(string Filename, KeywordType type)
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
                if (i > 1)
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
                                    AddKeywordAndResetValuesSecondType(listKeys, type);
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
                                string[] paramString = cellValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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
                AddKeywordAndResetValuesSecondType(listKeys, type);
            }

            return listKeys;
        }

        private static void AddKeywordAndResetValues(List<Keyword> keywordsList, KeywordType type)
        {
            keywordsList.Add(new Keyword(currentKeywordName, currentKeywordDocumentation, null, "", currentKeywordParams, "", false, type, -1));
            currentKeywordDocumentation = "";
            currentKeywordName = "";
            currentKeywordParams = new List<Param>();
        }

        private static void AddKeywordAndResetValuesSecondType(List<Keyword> keywordsList, KeywordType type)
        {
            bool removeTheKeyword = false;
            foreach (Keyword temp in keywordsList)
            {
                if ((temp.Name == currentKeywordName) && (temp.Documentation == currentKeywordDocumentation)
                    && (temp.Type == type) && (temp.Params.Count == currentKeywordParams.Count))
                {
                    bool check = true;
                    for (int i = 0; i < temp.Params.Count; i++)
                    {
                        if (!temp.Params[i].Name.Equals(currentKeywordParams[i].Name))
                        {
                            check = false;
                            break;
                        }
                        else
                        {
                            if (!temp.Params[i].Value.Equals(currentKeywordParams[i].Value))
                            {
                                check = false;
                                break;
                            }
                        }
                    }

                    if (check)
                        removeTheKeyword = true;

                    if (removeTheKeyword)
                    {
                        keywordsList.Remove(temp);
                        break;
                    }
                }
            }

            if (!removeTheKeyword)
                keywordsList.Add(new Keyword(currentKeywordName, currentKeywordDocumentation, null, "", currentKeywordParams, "", false, type, -1));

            currentKeywordDocumentation = "";
            currentKeywordName = "";
            currentKeywordParams = new List<Param>();
        }
    }
}
