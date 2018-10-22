using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace RobotAutomationHelper.Scripts
{
    internal static class ReadExcel
    {

        private static List<TestCase> TestCases;
        private static List<Keyword> currentTestCaseTestSteps;
        private static string currentTestCaseTags;
        private static string currentTestCaseDocumentation;
        private static string currentTestCase;
        private static string outputFilePath;

        internal static List<TestCase> ReadAllTestCasesFromExcel(string Filename)
        {
            TestCases = new List<TestCase>();
            currentTestCaseTestSteps = new List<Keyword>();
            currentTestCaseTags = "";
            currentTestCaseDocumentation = "";
            currentTestCase = "";
            outputFilePath = FilesAndFolderStructure.GetFolder("Tests");

            var package = new ExcelPackage(new FileInfo(Filename));
            ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

            for (int i = workSheet.Dimension.Start.Row;
                                    i <= workSheet.Dimension.End.Row;
                                    i++)
            {
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
                            if (!currentTestCase.Equals(""))
                            {
                                //Setup test creation for previous Test case
                                AddTestCaseAndResetValues();
                                if (!currentTestCase.Equals(cellValue))
                                    currentTestCase = cellValue;
                                else
                                    currentTestCase = "";
                            }
                            else
                            {
                                currentTestCase = cellValue;
                            }
                        }
                        //summary column equals documentation in robot
                        else if (j == 2 && !cellValue.Equals(""))
                        {
                            currentTestCaseDocumentation = "[Documentation]  " + cellValue;
                        }
                        //test cases column equals test steps ( keywords )
                        else if (j == 3 && !cellValue.Equals(""))
                        {
                            currentTestCaseTestSteps.Add(new Keyword("" + cellValue, FilesAndFolderStructure.GetFolder("Keywords") + "Auto.robot"));
                        }
                        else if (j == 4 && !cellValue.Equals(""))
                        {
                            outputFilePath = FilesAndFolderStructure.ConcatFileNameToFolder(cellValue, "Tests");
                        }
                        // TODO
                        else if (j == 5 && !cellValue.Equals(""))
                        {

                            string[] tagsStrings = cellValue.Split(',');
                            currentTestCaseTags = "[Tags]";
                            foreach (string tag in tagsStrings)
                                currentTestCaseTags += "  " + tag.Trim();
                        }
                    }
                }
            }

            if (!currentTestCase.Equals(""))
            {
                AddTestCaseAndResetValues();
            }

            return TestCases;
        }

        private static void AddTestCaseAndResetValues()
        {
            if (outputFilePath.Equals(FilesAndFolderStructure.GetFolder("Tests")))
                outputFilePath = FilesAndFolderStructure.GetFolder("Tests") + "Auto.robot";
            TestCases.Add(new TestCase(currentTestCase, currentTestCaseDocumentation, currentTestCaseTags, currentTestCaseTestSteps, outputFilePath));
            currentTestCaseTestSteps = new List<Keyword>();
            currentTestCaseDocumentation = "";
            currentTestCase = "";
            currentTestCaseTags = "";
            outputFilePath = FilesAndFolderStructure.GetFolder("Tests");
        }
    }
}
