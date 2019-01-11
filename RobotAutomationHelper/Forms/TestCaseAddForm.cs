using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RobotAutomationHelper.Scripts.CustomControls;
using RobotAutomationHelper.Scripts.Objects;
using RobotAutomationHelper.Scripts.Static;

namespace RobotAutomationHelper.Forms
{
    public partial class TestCaseAddForm : BaseKeywordAddForm
    {

        public TestCaseAddForm(BaseKeywordAddForm parent) : base(parent)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine(@"TestCaseAddForm [Constructor]");
            InitializeComponent();
            InitialYValue = 140;
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
                MessageBox.Show(@"Test case with this name has already been implemented in the output file.",
                    @"Alert",
                    MessageBoxButtons.OK);
            }
        }

        private void Skip_Click(object sender, EventArgs e)
        {
            SkipForm = true;
            Close();
        }

        public void ShowTestCaseContent(TestCase testCase, int testIndex)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine(@"ShowTestCaseContent " + testCase.Name + @" " + testIndex);
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
                foreach (var testStep in testCase.Steps)
                {
                    AddKeywordField(testStep, NumberOfKeywordsInThisForm + 1, false);
                    NumberOfKeywordsInThisForm++;
                }
            else
            {
                // add a single keyword field if no keywords are available
                ThisFormKeywords = new List<Keyword>
                {
                    new Keyword("New Keyword", FilesAndFolderStructure.GetFolder(FolderType.Resources) + "Auto.robot", null)
                };
                AddKeywordField(ThisFormKeywords[0], NumberOfKeywordsInThisForm + 1, true);
                NumberOfKeywordsInThisForm++;
            }

            UpdateListNamesAndUpdateStateOfSave();
            StartPosition = FormStartPosition.Manual;
            ShowDialog();
        }

        private void SaveChangesToTestCases()
        {
            if (RobotAutomationHelper.Log) Console.WriteLine(@"SaveChangesToTestCases");
            if (RobotAutomationHelper.TestCases[ImplementationIndexFromTheParent].Steps != null && RobotAutomationHelper.TestCases[ImplementationIndexFromTheParent].Steps.Count > 0)
                for (var counter = 1; counter <= RobotAutomationHelper.TestCases[ImplementationIndexFromTheParent].Steps.Count; counter++)
                    ThisFormKeywords[counter-1].Name = ((TextWithList) Controls["DynamicStep" + counter + "Name"]).Text.Trim();
            
            var finalPath = FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Tests);

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
            MemoryPath = TestCasesListOperations.IsPresentInTheTestCasesTree(TestCaseName.Text,
                FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Tests),
                RobotAutomationHelper.TestCases[ImplementationIndexFromTheParent]);

            if (!MemoryPath.Equals(""))
            {
                TestCaseName.ForeColor = Color.Red;
                return true;
            }
            return false;
        }

        public void UpdateListNamesAndUpdateStateOfSave()
        {
            var namesList = new List<string>
            {
                TestCaseName.Text
            };
            for (var i = 1; i <= NumberOfKeywordsInThisForm; i++)
            {
                if (Controls.Find("DynamicStep" + i + "Name", false).Length > 0)
                    namesList.Add(Controls["DynamicStep" + i + "Name"].Text);
            }
            ((ButtonWithToolTip) Save).UpdateState(namesList, OutputFile.Text);
        }
    }
}
