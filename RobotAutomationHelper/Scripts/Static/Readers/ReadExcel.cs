using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using RobotAutomationHelper.Scripts.Objects;

namespace RobotAutomationHelper.Scripts.Static.Readers
{
    internal static class ReadExcel
    {
        private static List<TestCase> _testCases;
        private static List<Keyword> _currentTestCaseTestSteps;
        private static string _currentTestCaseTags;
        private static string _currentTestCaseDocumentation;
        private static string _currentTestCase;
        private static string _outputFilePath;

        internal static List<TestCase> ReadAllTestCasesFromExcel(string filename)
        {
            _testCases = new List<TestCase>();
            _currentTestCaseTestSteps = new List<Keyword>();
            _currentTestCaseTags = "";
            _currentTestCaseDocumentation = "";
            _currentTestCase = "";
            _outputFilePath = FilesAndFolderStructure.GetFolder(FolderType.Tests);

            var package = new ExcelPackage(new FileInfo(filename));
            var workSheet = package.Workbook.Worksheets[1];

            for (var i = workSheet.Dimension.Start.Row;
                                    i <= workSheet.Dimension.End.Row;
                                    i++)
            {
                for (var j = workSheet.Dimension.Start.Column;
                            j <= workSheet.Dimension.End.Column;
                            j++)
                {
                    if (workSheet.Cells[i, j].Value == null) continue;
                    var cellValue = workSheet.Cells[i, j].Value.ToString().Trim();
                    switch (j)
                    {
                        case 1 when !cellValue.Equals(""):
                        {
                            if (!_currentTestCase.Equals(""))
                            {
                                //Setup test creation for previous Test case
                                AddTestCaseAndResetValues();
                                _currentTestCase = !_currentTestCase.Equals(cellValue) ? cellValue : "";
                            }
                            else
                            {
                                _currentTestCase = cellValue;
                            }

                            break;
                        }
                        case 2 when !cellValue.Equals(""):
                            _currentTestCaseDocumentation = "[Documentation]  " + cellValue;
                            break;
                        case 3 when !cellValue.Equals(""):
                            _currentTestCaseTestSteps.Add(new Keyword("" + cellValue,
                                FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot", null));
                            break;
                        case 4 when !cellValue.Equals(""):
                            _outputFilePath =
                                FilesAndFolderStructure.ConcatFileNameToFolder(cellValue, FolderType.Tests);
                            break;
                        case 5 when !cellValue.Equals(""):
                        {
                            var tagsStrings = cellValue.Split(',');
                            _currentTestCaseTags = "[Tags]";
                            foreach (var tag in tagsStrings)
                                _currentTestCaseTags += "  " + tag.Trim();
                            break;
                        }
                    }
                }
            }

            if (!_currentTestCase.Equals(""))
            {
                AddTestCaseAndResetValues();
            }

            return _testCases;
        }

        private static void AddTestCaseAndResetValues()
        {
            if (_outputFilePath.Equals(FilesAndFolderStructure.GetFolder(FolderType.Tests)))
                _outputFilePath = FilesAndFolderStructure.GetFolder(FolderType.Tests) + "Auto.robot";
            _testCases.Add(new TestCase(_currentTestCase, _currentTestCaseDocumentation, _currentTestCaseTags, _currentTestCaseTestSteps, _outputFilePath, false));
            _currentTestCaseTestSteps = new List<Keyword>();
            _currentTestCaseDocumentation = "";
            _currentTestCase = "";
            _currentTestCaseTags = "";
            _outputFilePath = FilesAndFolderStructure.GetFolder(FolderType.Tests);
        }
    }
}
