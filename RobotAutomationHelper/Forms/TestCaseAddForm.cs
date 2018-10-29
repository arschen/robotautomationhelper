using RobotAutomationHelper.Scripts;
using RobotAutomationHelper.Scripts.CustomControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper
{
    internal partial class TestCaseAddForm : BaseKeywordAddForm
    {

        internal TestCaseAddForm()
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("TestCaseAddForm [Constructor]");
            InitializeComponent();
            initialYValue = 140;
            FormType = FormType.Test;
            UpdateOutputFileSuggestions(OutputFile, FormType);
            ActiveControl = TestCaseNameLabel;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (!IsTestCasePresentInFilesOrMemoryTree())
            {
                SaveChangesToTestCases();
                Close();
            }
            else
            {
                if (presentInRobotFile)
                {
                    DialogResult result = MessageBox.Show("Overwrite existing test case in the output file?",
                        "Alert",
                        MessageBoxButtons.YesNo);
                    if (result.Equals(DialogResult.Yes))
                    {
                        SaveChangesToTestCases();
                        RobotAutomationHelper.TestCases[ImplementationIndexFromTheParent].Overwrite = true;
                        Close();
                    }
                    else
                        RobotAutomationHelper.TestCases[ImplementationIndexFromTheParent].Overwrite = false;
                }
                else
                {
                    DialogResult result = MessageBox.Show("Test case with this name has already been implemented in the ouput file.",
                        "Alert",
                        MessageBoxButtons.OK);
                }
            }
        }

        private void Skip_Click(object sender, EventArgs e)
        {
            skip = true;
            Close();
        }

        internal void ShowTestCaseContent(TestCase testCase, int testIndex)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("ShowTestCaseContent " + testCase.Name + " " + testIndex);
            ImplementationIndexFromTheParent = testIndex;
            if (testCase.Name != null) { }
                TestCaseName.Text = testCase.Name;
            if (testCase.Documentation != null)
                TestCaseDocumentation.Text = testCase.Documentation.Replace("[Documentation]", "").Trim();
            if (testCase.Tags != null)
                TestCaseTags.Text = testCase.Tags.Replace("[Tags]","").Trim();
            if (testCase.OutputFilePath != null)
                OutputFile.Text = testCase.OutputFilePath.Replace(FilesAndFolderStructure.GetFolder(FolderType.Tests),"\\");

            IsTestCasePresentInFilesOrMemoryTree();
            ThisFormKeywords = new List<Keyword>();
            ThisFormKeywords = testCase.Steps;

            NumberOfKeywordsInThisForm = 0;
            if (ThisFormKeywords != null && ThisFormKeywords.Count != 0)
                foreach (Keyword testStep in testCase.Steps)
                {
                    AddKeywordField(testStep, NumberOfKeywordsInThisForm + 1);
                    NumberOfKeywordsInThisForm++;
                }
            else
            {
                // add a single keyword field if no keywords are available
                ThisFormKeywords = new List<Keyword>
                {
                    new Keyword("New Keyword", FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot")
                };
                AddKeywordField(ThisFormKeywords[0], NumberOfKeywordsInThisForm + 1);
                NumberOfKeywordsInThisForm++;
            }

            UpdateListNamesAndUpdateStateOfSave();
            StartPosition = FormStartPosition.Manual;
            var dialogResult = ShowDialog();
        }

        private void SaveChangesToTestCases()
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("SaveChangesToTestCases");
            if (RobotAutomationHelper.TestCases[ImplementationIndexFromTheParent].Steps != null && RobotAutomationHelper.TestCases[ImplementationIndexFromTheParent].Steps.Count > 0)
                for (int counter = 1; counter <= RobotAutomationHelper.TestCases[ImplementationIndexFromTheParent].Steps.Count; counter++)
                    ThisFormKeywords[counter-1].Name = ((TextWithList) Controls["DynamicStep" + counter + "Name"]).Text.Trim();
            
            string finalPath = FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Tests);

            RobotAutomationHelper.TestCases[ImplementationIndexFromTheParent] = new TestCase(TestCaseName.Text.Trim(),
                "[Documentation]  " + TestCaseDocumentation.Text.Trim(),
                "[Tags]  " + TestCaseTags.Text.Trim(),
                ThisFormKeywords,
                finalPath,
                true);
        }

        private void TestCaseName_TextChanged(object sender, EventArgs e)
        {
            UpdateListNamesAndUpdateStateOfSave();
            IsTestCasePresentInFilesOrMemoryTree();
        }

        private void TestCaseOutputFile_TextChanged(object sender, EventArgs e)
        {
            UpdateListNamesAndUpdateStateOfSave();
            IsTestCasePresentInFilesOrMemoryTree();
        }

        private bool IsTestCasePresentInFilesOrMemoryTree()
        {
            presentInRobotFile = false;
            memoryPath = TestCasesListOperations.IsPresentInTheTestCasesTree(TestCaseName.Text,
                FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Tests),
                RobotAutomationHelper.TestCases[ImplementationIndexFromTheParent]);

            if (!memoryPath.Equals(""))
                TestCaseName.ForeColor = Color.Red;
            else
            {
                if (RobotFileHandler.LocationOfTestCaseOrKeywordInFile(FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Tests)
                    , TestCaseName.Text, FormType.Test) != -1)
                {
                    TestCaseName.ForeColor = Color.Red;
                    presentInRobotFile = true;
                }
                else
                {
                    TestCaseName.ForeColor = Color.Black;
                    return false;
                }
            }
            return true;
        }

        internal void UpdateListNamesAndUpdateStateOfSave()
        {
            List<string> namesList = new List<string>
            {
                TestCaseName.Text
            };
            for (int i = 1; i <= NumberOfKeywordsInThisForm; i++)
            {
                if (Controls.Find("DynamicStep" + i + "Name", false).Length > 0)
                    namesList.Add(Controls["DynamicStep" + i + "Name"].Text);
            }
            (Save as ButtonWithToolTip).UpdateState(namesList, OutputFile.Text);
        }
    }
}
