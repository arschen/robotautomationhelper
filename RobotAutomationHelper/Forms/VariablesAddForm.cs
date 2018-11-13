using System;
using System.Drawing;
using System.Windows.Forms;
using RobotAutomationHelper.Scripts.Objects;
using RobotAutomationHelper.Scripts.Static;
using RobotAutomationHelper.Scripts.Static.Consts;

namespace RobotAutomationHelper.Forms
{
    internal partial class VariablesAddForm : BaseKeywordAddForm
    {
        private int _selectedIndex;
        internal Variables CurrentVariables { get; set; }
        private int _variablesCounter;

        internal VariablesAddForm(BaseKeywordAddForm parent) : base(parent)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine(@"VariablesAddForm Constructor");
            InitializeComponent();
            InitialYValue = 120;
            FormType = FormType.Variable;
            SetupsVariablesAddForm();
        }

        private void SetupsVariablesAddForm()
        {
            UpdateOutputFileSuggestions(OutputFile, FormType);
            OutputFile.SelectedIndex = _selectedIndex;
            foreach (var fileName in FilesAndFolderStructure.GetShortSavedFiles(FolderType.Root))
            {
                var add = true;
                if (RobotAutomationHelper.GlobalVariables.Count != 0)
                    foreach (var temp in RobotAutomationHelper.GlobalVariables)
                    {
                        if (!temp.ToString().Equals(FilesAndFolderStructure.ConcatFileNameToFolder(fileName, FolderType.Root))) continue;
                        add = false;
                        break;
                    }
                if (add)
                    RobotAutomationHelper.GlobalVariables.Add(new Variables(null, fileName));
            }

            ActiveControl = AddVariablesLabel;

            foreach (var temp in RobotAutomationHelper.GlobalVariables)
                if (temp.ToString().Equals(FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Items[OutputFile.SelectedIndex].ToString(), FolderType.Root)))
                {
                    CurrentVariables = temp;
                    break;
                }

            _variablesCounter = 0;
            if (CurrentVariables != null && CurrentVariables.VariableNames == null) return;
            if (CurrentVariables == null) return;
            foreach (var unused in CurrentVariables.VariableNames)
            {
                _variablesCounter++;
                AddVariableField();
            }
        }

        internal void ShowVariablesContent()
        {
            StartPosition = FormStartPosition.Manual;
            ShowDialog();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            foreach (var temp in RobotAutomationHelper.GlobalVariables)
            {
                if (!temp.ToString().Equals(OutputFile.Text)) continue;
                temp.VariableNames = CurrentVariables.VariableNames;
                break;
            }
        }

        private void SaveAndExit_Click(object sender, EventArgs e)
        {
            Save_Click(sender, e);
            Close();
        }

        private void OutputFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OutputFile.SelectedIndex == -1 || _selectedIndex == OutputFile.SelectedIndex) return;
            _selectedIndex = OutputFile.SelectedIndex;
            for (var i = 1; i <= 4; i++)
                RemoveKeywordField(i, false);
            SetupsVariablesAddForm();
        }

        protected void AddVariableField()
        {
            var settingsLabel = 0;
            if (FormType == FormType.Settings)
                settingsLabel = KeywordFieldConsts.SettingsLabelWidth - KeywordFieldConsts.LabelWidth;

            FormControls.AddControl("TextBox", "DynamicStep" + _variablesCounter + "Name",
                _variablesCounter,
                new Point(settingsLabel + KeywordFieldConsts.NameX - HorizontalScroll.Value, InitialYValue + (_variablesCounter - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                new Size(KeywordFieldConsts.NameWidth, KeywordFieldConsts.FieldsHeight),
                CurrentVariables.VariableNames[_variablesCounter - 1].Trim(),
                Color.Black,
                null,
                this);

            FormControls.AddControl("Label", "DynamicStep" + _variablesCounter + "Label",
                _variablesCounter,
                new Point(KeywordFieldConsts.LabelX - HorizontalScroll.Value, InitialYValue + 3 + (_variablesCounter - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                new Size(settingsLabel + KeywordFieldConsts.LabelWidth, KeywordFieldConsts.FieldsHeight),
                (FormType == FormType.Settings) ? KeywordFieldConsts.LabelNames[_variablesCounter - 1] : _variablesCounter + ".",
                Color.Black,
                null,
                this);

            FormControls.AddControl("Button", "DynamicStep" + _variablesCounter + "AddKeyword",
                _variablesCounter,
                new Point(settingsLabel + KeywordFieldConsts.AddKeywordX - HorizontalScroll.Value, InitialYValue + (_variablesCounter - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                new Size(KeywordFieldConsts.AddKeywordWidth, KeywordFieldConsts.FieldsHeight),
                "+",
                Color.Black,
                InstantiateNameAndOutputForm,
                this);
            FormControls.AddControl("Button", "DynamicStep" + _variablesCounter + "RemoveKeyword",
                _variablesCounter,
                new Point(settingsLabel + KeywordFieldConsts.RemoveKeywordX - HorizontalScroll.Value, InitialYValue + (_variablesCounter - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                new Size(KeywordFieldConsts.RemoveKeywordWidth, KeywordFieldConsts.FieldsHeight),
                "-",
                Color.Black,
                RemoveKeywordFromThisForm,
                this);
        }
    }
}
