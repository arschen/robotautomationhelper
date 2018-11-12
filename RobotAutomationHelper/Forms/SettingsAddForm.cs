using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RobotAutomationHelper.Scripts.Objects;
using RobotAutomationHelper.Scripts.Static;
using RobotAutomationHelper.Scripts.Static.Readers;
using RobotAutomationHelper.Scripts.Static.Writers;

namespace RobotAutomationHelper.Forms
{
    internal partial class SettingsAddForm : BaseKeywordAddForm
    {

        private SuiteSettings _currentSuiteSettings;
        private int _selectedIndex;

        internal SettingsAddForm(BaseKeywordAddForm parent) : base(parent)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine(@"SettingsAddForm Constructor");
            InitializeComponent();
            InitialYValue = 120;
            FormType = FormType.Settings;
            SetupsSettingsAddForm();
        }

        private void SetupsSettingsAddForm()
        {
            UpdateOutputFileSuggestions(OutputFile, FormType);
            OutputFile.SelectedIndex = _selectedIndex;
            foreach (var fileName in FilesAndFolderStructure.GetShortSavedFiles(FolderType.Root))
            {
                var add = true;
                if (RobotAutomationHelper.SuiteSettingsList.Count != 0)
                    foreach (var temp in RobotAutomationHelper.SuiteSettingsList)
                    {
                        if (!temp.ToString().Equals(fileName)) continue;
                        add = false;
                        break;
                    }
                if (add)
                    RobotAutomationHelper.SuiteSettingsList.Add(new SuiteSettings(fileName));
            }

            ActiveControl = AddSettingsLabel;

            foreach (var temp in RobotAutomationHelper.SuiteSettingsList)
                if (temp.ToString().Equals(OutputFile.Items[OutputFile.SelectedIndex]))
                {
                    _currentSuiteSettings = temp;
                    break;
                }


            var folderType = FolderType.Root;
            if (_currentSuiteSettings.OutputFilePath.Contains(FilesAndFolderStructure.Resources))
                folderType = FolderType.Resources;
            if (_currentSuiteSettings.OutputFilePath.Contains(FilesAndFolderStructure.Tests))
                folderType = FolderType.Tests;

            if (!_currentSuiteSettings.Overwrite)
            {
                var realOutput = _currentSuiteSettings.OutputFilePath;
                switch (folderType)
                {
                    case FolderType.Tests:
                        realOutput = realOutput.Remove(0, 5);
                        break;
                    case FolderType.Resources:
                        realOutput = realOutput.Remove(0, 9);
                        break;
                    case FolderType.Root:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _currentSuiteSettings.Documentation = RobotFileHandler.OccurenceInSettings(FilesAndFolderStructure.ConcatFileNameToFolder(_currentSuiteSettings.OutputFilePath, FolderType.Root), 
                    "Documentation").Replace("Documentation", "").Trim();
                _currentSuiteSettings.TestSetup = new Keyword(RobotFileHandler.OccurenceInSettings(FilesAndFolderStructure.ConcatFileNameToFolder(_currentSuiteSettings.OutputFilePath, FolderType.Root), "Test Setup").Replace("Test Setup", "").Trim(),
                    realOutput, ReadRobotFiles.GetLibs(FilesAndFolderStructure.ConcatFileNameToFolder(_currentSuiteSettings.OutputFilePath, FolderType.Root)), null);
                _currentSuiteSettings.TestTeardown = new Keyword(RobotFileHandler.OccurenceInSettings(FilesAndFolderStructure.ConcatFileNameToFolder(_currentSuiteSettings.OutputFilePath, FolderType.Root), "Test Teardown").Replace("Test Teardown", "").Trim(),
                    realOutput, ReadRobotFiles.GetLibs(FilesAndFolderStructure.ConcatFileNameToFolder(_currentSuiteSettings.OutputFilePath, FolderType.Root)), null);
                _currentSuiteSettings.SuiteSetup = new Keyword(RobotFileHandler.OccurenceInSettings(FilesAndFolderStructure.ConcatFileNameToFolder(_currentSuiteSettings.OutputFilePath, FolderType.Root), "Suite Setup").Replace("Suite Setup", "").Trim(),
                    realOutput, ReadRobotFiles.GetLibs(FilesAndFolderStructure.ConcatFileNameToFolder(_currentSuiteSettings.OutputFilePath, FolderType.Root)), null);
                _currentSuiteSettings.SuiteTeardown = new Keyword(RobotFileHandler.OccurenceInSettings(FilesAndFolderStructure.ConcatFileNameToFolder(_currentSuiteSettings.OutputFilePath, FolderType.Root), "Suite Teardown").Replace("Suite Teardown", "").Trim(),
                    realOutput, ReadRobotFiles.GetLibs(FilesAndFolderStructure.ConcatFileNameToFolder(_currentSuiteSettings.OutputFilePath, FolderType.Root)), null);
            }

            ThisFormKeywords = new List<Keyword>();
            if (_currentSuiteSettings.TestSetup == null)
                _currentSuiteSettings.TestSetup = new Keyword("", "Auto.robot", null);
            ThisFormKeywords.Add(_currentSuiteSettings.TestSetup);

            if (_currentSuiteSettings.TestTeardown == null)
                _currentSuiteSettings.TestTeardown = new Keyword("", "Auto.robot", null);
            ThisFormKeywords.Add(_currentSuiteSettings.TestTeardown);

            if (_currentSuiteSettings.SuiteSetup == null)
                _currentSuiteSettings.SuiteSetup = new Keyword("", "Auto.robot", null);
            ThisFormKeywords.Add(_currentSuiteSettings.SuiteSetup);

            if (_currentSuiteSettings.SuiteTeardown == null)
                _currentSuiteSettings.SuiteTeardown = new Keyword("", "Auto.robot", null);
            ThisFormKeywords.Add(_currentSuiteSettings.SuiteTeardown);

            SuiteDocumentation.Text = _currentSuiteSettings.Documentation;

            if (folderType != FolderType.Tests) return;
            for (var i = 1; i <=4; i++)
                if (Controls["DynamicStep" + i + "Name"] != null)
                    Controls["DynamicStep" + i + "Name"].Text = ThisFormKeywords[i-1].Name;
                else
                    AddKeywordField(ThisFormKeywords[i-1], i, false);
        }

        internal void ShowSettingsContent()
        {
            StartPosition = FormStartPosition.Manual;
            ShowDialog();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            foreach (var temp in RobotAutomationHelper.SuiteSettingsList)
            {
                if (!temp.ToString().Equals(OutputFile.Text)) continue;
                temp.Documentation = SuiteDocumentation.Text;
                temp.Overwrite = true;
                temp.TestSetup.CopyKeyword(ThisFormKeywords[0]);
                temp.TestTeardown.CopyKeyword(ThisFormKeywords[1]);
                temp.SuiteSetup.CopyKeyword(ThisFormKeywords[2]);
                temp.SuiteTeardown.CopyKeyword(ThisFormKeywords[3]);
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
            SetupsSettingsAddForm();
        }

        internal void UpdateNamesListAndUpdateStateOfSave()
        {
            var namesList = new List<string>();
            for (var i = 1; i <= NumberOfKeywordsInThisForm; i++)
            {
                if (Controls.Find("DynamicStep" + i + "Name", false).Length > 0)
                    namesList.Add(Controls["DynamicStep" + i + "Name"].Text);
            }
            Save.UpdateState(namesList, OutputFile.Text);
        }
    }
}
