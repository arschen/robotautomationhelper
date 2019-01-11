using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RobotAutomationHelper.Scripts.CustomControls;
using RobotAutomationHelper.Scripts.Objects;
using RobotAutomationHelper.Scripts.Static;
using RobotAutomationHelper.Scripts.Static.Consts;

namespace RobotAutomationHelper.Forms
{
    public partial class NameAndOutputForm : BaseKeywordAddForm
    {

        private readonly FormType _parentType;
        private readonly ToolTip _toolTip = new ToolTip();

        public NameAndOutputForm(FormType type, BaseKeywordAddForm parent, Keyword keyword) : base(parent)
        {
            InitializeComponent();
            NameAndOutputToTestCaseFormCommunication.Save = false;
            UpdateOutputFileSuggestions(OutputFile, type);
            _parentType = type;
            UpdateSaveButtonState();
            FormType = FormType.NameAndOutput;
            ThisFormKeywords = new List<Keyword>
                {
                    new Keyword("", "", keyword)
                };
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (_parentType.Equals(FormType.Test))
                TestCaseSave();
            else
                if (_parentType.Equals(FormType.Keyword))
                    KeywordSave();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            NameAndOutputToTestCaseFormCommunication.Save = false;
            Close();
        }

        public void ShowKeywordContent()
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
            ((TextWithList) Controls["DynamicStep1Name"]).MaxItemsInSuggestionsList = 5;
            ShowDialog();
        }

        public void ShowTestCaseContent()
        {
            OutputLabel.Text = OutputLabel.Text.Replace("Keyword", "Test Case");
            NameLabel.Text = NameLabel.Text.Replace("Keyword", "Test Case");
            ShowDialog();
        }

        private void TestCaseSave()
        {
            if (!IsTestCasePresentInFilesOrMemoryTree())
            {
                NameAndOutputToTestCaseFormCommunication.Save = true;
                NameAndOutputToTestCaseFormCommunication.Name = ContentName.Text;
                NameAndOutputToTestCaseFormCommunication.OutputFile = FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Tests);
                Close();
            }
            else
            {
                MessageBox.Show(@"Test case with this name has already been implemented in the output file.",
                    @"Alert",
                    MessageBoxButtons.OK);
            }
        }

        private void KeywordSave()
        {
            NameAndOutputToTestCaseFormCommunication.Save = true;
            NameAndOutputToTestCaseFormCommunication.Name = Controls["DynamicStep1Name"].Text;
            NameAndOutputToTestCaseFormCommunication.Value = ((TextWithList)Controls["DynamicStep1Name"]).UpdateValue;
            NameAndOutputToTestCaseFormCommunication.OutputFile = FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Resources);
            Close();
        }

        private void ContentName_TextChanged(object sender, EventArgs e)
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
            if (_parentType.Equals(FormType.Test))
                IsTestCasePresentInFilesOrMemoryTree();
        }

        private void UpdateSaveButtonState()
        {
            string name;
            if (_parentType.Equals(FormType.Test))
                name = ContentName.Text;
            else
            {
                name = Controls.Find("DynamicStep1Name", false).Length > 0 ? Controls["DynamicStep1Name"].Text : "";
            }

            if (_parentType.Equals(FormType.Keyword))
            {
                if (Controls.Find("DynamicStep1Name", false).Length > 0)
                {
                    if (SuggestionsClass.IsInSuggestionsList(Controls["DynamicStep1Name"].Text))
                    {
                        OutputFile.Enabled = false;
                        if (IsNameValid(name))
                        {
                            _toolTip.Hide(this);
                            Save.Enabled = true;
                        }
                    }
                    else
                    {
                        OutputFile.Enabled = true;
                        if (IsNameValid(name) && OutputFileCheck(OutputFile.Text))
                        {
                            _toolTip.Hide(this);
                            Save.Enabled = true;
                        }
                        else
                        {
                            _toolTip.Show("Invalid Name or OutputFile", Save, 0, Save.Size.Height);
                            Save.Enabled = false;
                        }
                    }  
                }
                else
                {
                    OutputFile.Enabled = true;
                    if (IsNameValid(name) && OutputFileCheck(OutputFile.Text))
                    {
                        _toolTip.Hide(this);
                        Save.Enabled = true;
                    }
                    else
                    {
                        _toolTip.Show("Invalid Name or OutputFile", Save, 0, Save.Size.Height);
                        Save.Enabled = false;
                    }
                }
            }
            else
            {
                if (IsNameValid(name) && OutputFileCheck(OutputFile.Text))
                {
                    _toolTip.Hide(this);
                    Save.Enabled = true;
                }
                else
                {
                    _toolTip.Show("Invalid Name or OutputFile", Save, 0, Save.Size.Height);
                    Save.Enabled = false;
                }
            }
        }

        private bool IsTestCasePresentInFilesOrMemoryTree()
        {
            MemoryPath = TestCasesListOperations.IsPresentInTheTestCasesTree(ContentName.Text,
                FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Tests),
                null);

            if (!MemoryPath.Equals(""))
            {
                ContentName.ForeColor = Color.Red;
                return true;
            }
            return false;
        }
    }

    public static class NameAndOutputToTestCaseFormCommunication
    {
        public static string Name { get; set; }
        public static string Value { get; set; }
        public static string OutputFile { get; set; }
        public static bool Save { get; set; }
    }
}
