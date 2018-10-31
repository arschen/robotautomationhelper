using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RobotAutomationHelper.Scripts;
using RobotAutomationHelper.Scripts.Static.Consts;

namespace RobotAutomationHelper.Forms
{
    internal partial class NameAndOutputForm : BaseKeywordAddForm
    {

        private FormType parentType;
        private ToolTip toolTip = new ToolTip();

        internal NameAndOutputForm(FormType type, BaseKeywordAddForm parent) : base(parent)
        {
            InitializeComponent();
            NameAndOutputToTestCaseFormCommunication.Save = false;
            UpdateOutputFileSuggestions(OutputFile, type);
            parentType = type;
            UpdateSaveButtonState();
            FormType = FormType.NameAndOutput;
            ThisFormKeywords = new List<Keyword>
                {
                    new Keyword("", "")
                };
        }

        private void Save_Click(object sender, System.EventArgs e)
        {
            if (parentType.Equals(FormType.Test))
                TestCaseSave(sender, e);
            else
                if (parentType.Equals(FormType.Keyword))
                    KeywordSave(sender, e);
        }

        private void Cancel_Click(object sender, System.EventArgs e)
        {
            NameAndOutputToTestCaseFormCommunication.Save = false;
            Close();
        }

        internal void ShowKeywordContent()
        {
            FormControls.RemoveControlByKey(ContentName.Name, Controls);
            FormControls.AddControl("TextWithList", "DynamicStep1Name",
                1,
                new Point(32 - HorizontalScroll.Value, 24 - VerticalScroll.Value),
                new Size(KeywordFieldConsts.NameWidth, KeywordFieldConsts.FieldsHeight),
                "",
                Color.Black,
                null,
                this);
            Controls["DynamicStep1Name"].TextChanged += KeywordName_TextChanged;
            (Controls["DynamicStep1Name"] as TextWithList).MaxItemsInSuggestionsList = 5;
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

        private void KeywordSave(object sender, EventArgs e)
        {
            NameAndOutputToTestCaseFormCommunication.Save = true;
            NameAndOutputToTestCaseFormCommunication.Name = Controls["DynamicStep1Name"].Text;
            NameAndOutputToTestCaseFormCommunication.OutputFile = FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Resources);
            NameAndOutputToTestCaseFormCommunication.Overwrite = false;
            Close();
        }

        private void ContentName_TextChanged(object sender, System.EventArgs e)
        {
            UpdateSaveButtonState();
            IsTestCasePresentInFilesOrMemoryTree();
        }

        private void KeywordName_TextChanged(object sender, EventArgs e)
        {
            UpdateSaveButtonState();
        }

        private void OutputFile_TextChanged(object sender, EventArgs e)
        {
            UpdateSaveButtonState();
            if (parentType.Equals(FormType.Test))
                IsTestCasePresentInFilesOrMemoryTree();
        }

        private void UpdateSaveButtonState()
        {
            string name;
            if (parentType.Equals(FormType.Test))
                name = ContentName.Text;
            else
            {
                if (Controls.Find("DynamicStep1Name", false).Length > 0)
                    name = Controls["DynamicStep1Name"].Text;
                else
                    name = "";
            }

            if (parentType.Equals(FormType.Keyword))
            {
                if (Controls.Find("DynamicStep1Name", false).Length > 0)
                {
                    if (SuggestionsClass.IsInSuggestionsList(Controls["DynamicStep1Name"].Text))
                    {
                        OutputFile.Enabled = false;
                        if (IsNameValid(name))
                        {
                            toolTip.Hide(this);
                            Save.Enabled = true;
                        }
                    }
                    else
                    {
                        OutputFile.Enabled = true;
                        if (IsNameValid(name) && OutputFileCheck(OutputFile.Text))
                        {
                            toolTip.Hide(this);
                            Save.Enabled = true;
                        }
                        else
                        {
                            toolTip.Show("Invalid Name or OutputFile", Save, 0, Save.Size.Height);
                            Save.Enabled = false;
                        }
                    }  
                }
                else
                {
                    OutputFile.Enabled = true;
                    if (IsNameValid(name) && OutputFileCheck(OutputFile.Text))
                    {
                        toolTip.Hide(this);
                        Save.Enabled = true;
                    }
                    else
                    {
                        toolTip.Show("Invalid Name or OutputFile", Save, 0, Save.Size.Height);
                        Save.Enabled = false;
                    }
                }
            }
            else
            {
                if (IsNameValid(name) && OutputFileCheck(OutputFile.Text))
                {
                    toolTip.Hide(this);
                    Save.Enabled = true;
                }
                else
                {
                    toolTip.Show("Invalid Name or OutputFile", Save, 0, Save.Size.Height);
                    Save.Enabled = false;
                }
            }
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
