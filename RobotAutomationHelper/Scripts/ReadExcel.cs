using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace RobotAutomationHelper.Scripts
{
    public static class ReadExcel
    {

        static List<TestCase> TestCases;
        static List<Keyword> currentTestCaseTestSteps;
        static string currentTestTags;
        static string currentTestCaseDocumentation;
        static string currentTestCase;
        static string outputFilePath;

        public static List<TestCase> ReadAllTestCasesFromExcel(string Filename)
        {
            TestCases = new List<TestCase>();
            currentTestCaseTestSteps = new List<Keyword>();
            currentTestTags = "";
            currentTestCaseDocumentation = "";
            currentTestCase = "";
            outputFilePath = FilesAndFolderStructure.GetFolder();

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
                            currentTestCaseDocumentation = "\t[Documentation]  " + cellValue;
                        }
                        //test cases column equals test steps ( keywords )
                        else if (j == 3 && !cellValue.Equals(""))
                        {
                            currentTestCaseTestSteps.Add(new Keyword("\t" + cellValue, FilesAndFolderStructure.GetFolder() + "Auto.robot"));
                        }
                        else if (j == 4 && !cellValue.Equals(""))
                        {
                            outputFilePath = FilesAndFolderStructure.ConcatFileNameToFolder(cellValue);
                        }
                        // TODO
                        else if (j == 5 && !cellValue.Equals(""))
                        {

                            string[] tagsStrings = cellValue.Split(',');
                            currentTestTags = "\t" + "[Tags]";
                            foreach (string tag in tagsStrings)
                                currentTestTags += "  " + tag.Trim();
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
            if (outputFilePath.Equals(FilesAndFolderStructure.GetFolder()))
                outputFilePath = FilesAndFolderStructure.GetFolder() + "Auto.robot";
            TestCases.Add(new TestCase(currentTestCase, currentTestCaseDocumentation, currentTestTags, currentTestCaseTestSteps, outputFilePath));
            currentTestCaseTestSteps = new List<Keyword>();
            currentTestCaseDocumentation = "";
            currentTestCase = "";
            currentTestTags = "";
            outputFilePath = FilesAndFolderStructure.GetFolder();
        }
    }
}
