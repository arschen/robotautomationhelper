using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RobotAutomationHelper.Scripts;
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
                    RobotAutomationHelper.GlobalVariables.Add(new Variables(new List<string>(), FilesAndFolderStructure.ConcatFileNameToFolder(fileName, FolderType.Root)));
            }

            ActiveControl = AddVariablesLabel;
            CurrentVariables = new Variables(new List<string>(),"");

            foreach (var temp in RobotAutomationHelper.GlobalVariables)
                if (temp.ToString().Equals(FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Items[OutputFile.SelectedIndex].ToString(), FolderType.Root)))
                {
                    CurrentVariables = temp.DeepClone();
                    break;
                }

            _variablesCounter = 0;
            if (CurrentVariables.VariableNames.Count == 0)
            {
                FormControls.AddControl("Button", "DynamicStep" + _variablesCounter + "AddVariable",
                    _variablesCounter,
                    new Point(KeywordFieldConsts.LabelX - HorizontalScroll.Value, InitialYValue + (_variablesCounter - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                    new Size(KeywordFieldConsts.AddKeywordWidth, KeywordFieldConsts.FieldsHeight),
                    "+",
                    Color.Black,
                    AddVariableToTheForm,
                    this);
                return;
            }
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
            TextFieldsToCurrentVariablesNames();
            foreach (var temp in RobotAutomationHelper.GlobalVariables)
            {
                if (!temp.ToString().Equals(FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Root))) continue;
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
            if (_variablesCounter == 0)
            {
                FormControls.RemoveControlByKey("DynamicStep" + 0 + "AddVariable", Controls);
            }
            if (OutputFile.SelectedIndex == -1 || _selectedIndex == OutputFile.SelectedIndex) return;
            _selectedIndex = OutputFile.SelectedIndex;
            for (var i = _variablesCounter; i > 0; i--)
                RemoveVariableField(i, true, false);
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

            FormControls.AddControl("Button", "DynamicStep" + _variablesCounter + "AddVariable",
                _variablesCounter,
                new Point(settingsLabel + KeywordFieldConsts.AddKeywordX - HorizontalScroll.Value, InitialYValue + (_variablesCounter - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                new Size(KeywordFieldConsts.AddKeywordWidth, KeywordFieldConsts.FieldsHeight),
                "+",
                Color.Black,
                AddVariableToTheForm,
                this);
            FormControls.AddControl("Button", "DynamicStep" + _variablesCounter + "RemoveVariable",
                _variablesCounter,
                new Point(settingsLabel + KeywordFieldConsts.RemoveKeywordX - HorizontalScroll.Value, InitialYValue + (_variablesCounter - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                new Size(KeywordFieldConsts.RemoveKeywordWidth, KeywordFieldConsts.FieldsHeight),
                "-",
                Color.Black,
                RemoveVariableFromThisForm,
                this);
        }

        protected void RemoveVariableField(int variableIndex, bool removeFromList, bool removeButton)
        {
            FormControls.RemoveControlByKey("DynamicStep" + variableIndex + "Name", Controls);
            FormControls.RemoveControlByKey("DynamicStep" + variableIndex + "Label", Controls);
            FormControls.RemoveControlByKey("DynamicStep" + variableIndex + "AddVariable", Controls);
            FormControls.RemoveControlByKey("DynamicStep" + variableIndex + "RemoveVariable", Controls);
            if (removeFromList)
                CurrentVariables.VariableNames.RemoveAt(variableIndex - 1);
            if (!removeButton || CurrentVariables.VariableNames.Count > 1) return;
            FormControls.AddControl("Button", "DynamicStep" + 0 + "AddVariable",
                _variablesCounter,
                new Point(KeywordFieldConsts.LabelX - HorizontalScroll.Value, InitialYValue + (0 - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                new Size(KeywordFieldConsts.AddKeywordWidth, KeywordFieldConsts.FieldsHeight),
                "+",
                Color.Black,
                AddVariableToTheForm,
                this);
        }

        internal void RemoveVariableFromThisForm(object sender, EventArgs e)
        {
            TextFieldsToCurrentVariablesNames();
            var variableIndex = int.Parse(((Button)sender).Name.Replace("DynamicStep", "").Replace("RemoveVariable", ""));
            RemoveVariableField(_variablesCounter, false, true);
            CurrentVariables.VariableNames.RemoveAt(variableIndex - 1);
            _variablesCounter--;
            AssignNamesFromCurrentVariablesToTextFields();
        }

        internal void AddVariableToTheForm(object sender, EventArgs e)
        {
            TextFieldsToCurrentVariablesNames();
            if (_variablesCounter == 0)
            {
                FormControls.RemoveControlByKey("DynamicStep" + 0 + "AddVariable", Controls);
            }
            _variablesCounter++;
            var variableIndex = int.Parse(((Button)sender).Name.Replace("DynamicStep", "").Replace("AddVariable", ""));
            if (CurrentVariables.VariableNames.Count != 0)
                CurrentVariables.VariableNames.Insert(variableIndex, "");
            else
                CurrentVariables.VariableNames.Add("");
            AddVariableField();
            AssignNamesFromCurrentVariablesToTextFields();
        }

        private void AssignNamesFromCurrentVariablesToTextFields()
        {
            for (var i = 1; i <= _variablesCounter; i++)
            {
                Controls["DynamicStep" + i + "Name"].Text = CurrentVariables.VariableNames[i - 1].Trim();
            }
        }

        private void TextFieldsToCurrentVariablesNames()
        {
            for (var i = 0; i < _variablesCounter; i++)
                CurrentVariables.VariableNames[i] = Controls["DynamicStep" + (i + 1) + "Name"].Text;
        }
    }
}
