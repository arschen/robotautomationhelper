using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using RobotAutomationHelper.Scripts.Objects;

namespace RobotAutomationHelper.Scripts.Static
{
    internal static class ExcelLibsGetter
    {
        private static string _currentKeywordDocumentation;
        private static string _currentKeywordName;
        private static List<Param> _currentKeywordParams;

        internal static List<Keyword> ReadAllKeywordsFromExcelSecondType(string filename, KeywordType type)
        {
            var listKeys = new List<Keyword>();
            _currentKeywordParams = new List<Param>();
            _currentKeywordDocumentation = "";
            _currentKeywordName = "";

            var package = new ExcelPackage(new FileInfo(filename));
            var workSheet = package.Workbook.Worksheets[1];
            var splitFileName = filename.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            filename = splitFileName[splitFileName.Length - 1].Replace(".xlsx", "");

            for (var i = workSheet.Dimension.Start.Row;
                                    i <= workSheet.Dimension.End.Row;
                                    i++)
            {
                if (i > 1)
                    for (var j = workSheet.Dimension.Start.Column;
                                j <= workSheet.Dimension.End.Column;
                                j++)
                    {
                        if (workSheet.Cells[i, j].Value != null)
                        {
                            var cellValue = workSheet.Cells[i, j].Value.ToString().Trim();
                            //key column equals test name in robot
                            if (j == 1 && !cellValue.Equals(""))
                            {
                                if (!_currentKeywordName.Equals(""))
                                {
                                    //Setup test creation for previous Test case
                                    AddKeywordAndResetValuesSecondType(listKeys, type, filename);
                                    _currentKeywordName = !_currentKeywordName.Equals(cellValue) ? cellValue : "";
                                }
                                else
                                {
                                    _currentKeywordName = cellValue;
                                }
                            }
                            //summary column equals documentation in robot
                            else if (j == 2 && !cellValue.Equals(""))
                            {
                                var paramString = cellValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var occurrence in paramString)
                                {
                                    _currentKeywordParams.Add(occurrence.Contains("=")
                                        ? new Param(occurrence.Trim().Split('=')[0], occurrence.Trim().Split('=')[1])
                                        : new Param(occurrence.Trim(), ""));
                                }
                            }
                            //summary column equals documentation in robot
                            else if (j == 3 && !cellValue.Equals(""))
                            {
                                _currentKeywordDocumentation = cellValue;
                            }
                        }
                    }
            }

            if (!_currentKeywordName.Equals(""))
            {
                AddKeywordAndResetValuesSecondType(listKeys, type, filename);
            }

            return listKeys;
        }

        private static void AddKeywordAndResetValuesSecondType(List<Keyword> keywordsList, KeywordType type, string filename)
        {
            var removeTheKeyword = false;
            foreach (var temp in keywordsList)
            {
                if ((temp.Name != _currentKeywordName) || (temp.Documentation != _currentKeywordDocumentation) ||
                    (temp.Type != type) || (temp.Params.Count != _currentKeywordParams.Count)) continue;
                var check = true;
                for (var i = 0; i < temp.Params.Count; i++)
                {
                    if (!temp.Params[i].Name.Equals(_currentKeywordParams[i].Name))
                    {
                        check = false;
                        break;
                    }

                    if (!temp.Params[i].Value.Equals(_currentKeywordParams[i].Value))
                    {
                        check = false;
                        break;
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

            if (!removeTheKeyword)
                keywordsList.Add(new Keyword(_currentKeywordName, _currentKeywordDocumentation, null, "", _currentKeywordParams, "", type, -1, filename, null, false));

            _currentKeywordDocumentation = "";
            _currentKeywordName = "";
            _currentKeywordParams = new List<Param>();
        }
    }
}
