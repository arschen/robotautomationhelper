using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RobotAutomationHelper.Scripts;

namespace RobotAutomationHelper.Forms
{
    internal partial class NameAndOutputForm : BaseKeywordAddForm
    {
        internal NameAndOutputForm(FormType type)
        {
            InitializeComponent();
            NameAndOutputToTestCaseFormCommunication.Save = false;
            FormControls.UpdateOutputFileSuggestions(OutputFile, type);
            UpdateSaveButtonState();
            FormType = FormType.NameAndOutput;
            ThisFormKeywords = new List<Keyword>
                {
                    new Keyword("", "")
                };
        }

        private void Save_Click(object sender, System.EventArgs e)
        {
            TestCaseSave(sender, e);
        }

        private void Cancel_Click(object sender, System.EventArgs e)
        {
            NameAndOutputToTestCaseFormCommunication.Save = false;
            Close();
        }

        internal void ShowKeywordContent()
        {
            FormControls.RemoveControlByKey(ContentName.Name, Controls);
            FormControls.AddControl("TextWithList", "DynamicStep" + 1 + "Name",
                1,
                new Point(32 - HorizontalScroll.Value, 24 - VerticalScroll.Value),
                new Size(280, 20),
                "",
                Color.Black,
                null,
                this);
            var dialogResult = ShowDialog();
        }

        internal void ShowTestCaseContent()
        {
            OutputLabel.Text = OutputLabel.Text.Replace("Keyword", "Test Case");
            NameLabel.Text = NameLabel.Text.Replace("Keyword", "Test Case");
            var dialogResult = ShowDialog();
        }

        private void TestCaseSave(object sender, EventArgs e)
        {
            if (!IsTestCasePresentInFilesOrMemoryTree())
            {
                NameAndOutputToTestCaseFormCommunication.Save = true;
                NameAndOutputToTestCaseFormCommunication.Name = ContentName.Text;
                NameAndOutputToTestCaseFormCommunication.OutputFile = FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Tests);
                NameAndOutputToTestCaseFormCommunication.Overwrite = false;
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
                        NameAndOutputToTestCaseFormCommunication.Save = true;
                        NameAndOutputToTestCaseFormCommunication.Name = ContentName.Text;
                        NameAndOutputToTestCaseFormCommunication.OutputFile = FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Tests);
                        NameAndOutputToTestCaseFormCommunication.Overwrite = true;
                        Close();
                    }
                }
                else
                {
                    DialogResult result = MessageBox.Show("Test case with this name has already been implemented in the ouput file.",
                        "Alert",
                        MessageBoxButtons.OK);
                }
            }
        }

        private void ContentName_TextChanged(object sender, System.EventArgs e)
        {
            UpdateSaveButtonState();
            IsTestCasePresentInFilesOrMemoryTree();
        }

        private void OutputFile_TextChanged(object sender, EventArgs e)
        {
            UpdateSaveButtonState();
            IsTestCasePresentInFilesOrMemoryTree();
        }

        private void UpdateSaveButtonState()
        {
            if (NameCheck(ContentName.Text) && OutputFileCheck(OutputFile.Text))
                Save.Enabled = true;
            else
                Save.Enabled = false;
        }

        private bool IsTestCasePresentInFilesOrMemoryTree()
        {
            presentInRobotFile = false;
            memoryPath = TestCasesListOperations.IsPresentInTheTestCasesTree(ContentName.Text,
                FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Tests),
                null);

            if (!memoryPath.Equals(""))
                ContentName.ForeColor = Color.Red;
            else
            {
                if (RobotFileHandler.LocationOfTestCaseOrKeywordInFile(FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Tests)
                    , ContentName.Text, FormType.Test) != -1)
                {
                    ContentName.ForeColor = Color.Red;
                    presentInRobotFile = true;
                }
                else
                {
                    ContentName.ForeColor = Color.Black;
                    return false;
                }
            }
            return true;
        }
    }

    internal static class NameAndOutputToTestCaseFormCommunication
    {
        internal static string Name { get; set; }
        internal static string OutputFile { get; set; }
        internal static bool Save { get; set; }
        internal static bool Overwrite { get; set; }
    }
}
