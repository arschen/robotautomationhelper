using RobotAutomationHelper.Scripts;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RobotAutomationHelper.Forms
{
    internal partial class SettingsAddForm : BaseKeywordAddForm
    {

        private SuiteSettings CurrentSuiteSettings;
        private int SelectedIndex = 0;

        internal SettingsAddForm()
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("SettingsAddForm Constructor");
            InitializeComponent();
            initialYValue = 100;
            SetupsSettingsAddForm();
        }

        private void SetupsSettingsAddForm()
        {
            FormControls.UpdateOutputFileSuggestions(OutputFile, "");
            OutputFile.SelectedIndex = SelectedIndex;
            bool add;
            foreach (string fileName in FilesAndFolderStructure.GetShortSavedFiles(""))
            {
                add = true;
                if (RobotAutomationHelper.SuiteSettingsList.Count != 0)
                    foreach (SuiteSettings temp in RobotAutomationHelper.SuiteSettingsList)
                    {
                        if (temp.ToString().Equals(fileName))
                        {
                            add = false;
                            break;
                        }
                    }
                if (add)
                    RobotAutomationHelper.SuiteSettingsList.Add(new SuiteSettings(fileName));
            }

            ActiveControl = AddSettingsLabel;

            foreach (SuiteSettings temp in RobotAutomationHelper.SuiteSettingsList)
                if (temp.ToString().Equals(OutputFile.Items[OutputFile.SelectedIndex]))
                {
                    CurrentSuiteSettings = temp;
                    break;
                }

            if (!CurrentSuiteSettings.Overwrite)
            {
                string type = "";
                if (CurrentSuiteSettings.OutputFilePath.Contains("Resources"))
                    type = "Resources";
                if (CurrentSuiteSettings.OutputFilePath.Contains("Tests"))
                    type = "Tests";
                CurrentSuiteSettings.Documentation = RobotFileHandler.OccuranceInSettings(FilesAndFolderStructure.ConcatFileNameToFolder(CurrentSuiteSettings.OutputFilePath, type), 
                    "Documentation").Replace("Documentation", "").Trim();
                CurrentSuiteSettings.TestSetup = new Keyword(RobotFileHandler.OccuranceInSettings(FilesAndFolderStructure.ConcatFileNameToFolder(CurrentSuiteSettings.OutputFilePath, type), "Test Setup").Replace("Test Setup", "").Trim(),
                    CurrentSuiteSettings.OutputFilePath, true);
                CurrentSuiteSettings.TestTeardown = new Keyword(RobotFileHandler.OccuranceInSettings(FilesAndFolderStructure.ConcatFileNameToFolder(CurrentSuiteSettings.OutputFilePath, type), "Test Teardown").Replace("Test Teardown", "").Trim(), 
                    CurrentSuiteSettings.OutputFilePath, true);
                CurrentSuiteSettings.SuiteSetup = new Keyword(RobotFileHandler.OccuranceInSettings(FilesAndFolderStructure.ConcatFileNameToFolder(CurrentSuiteSettings.OutputFilePath, type), "Suite Setup").Replace("Suite Setup", "").Trim(), 
                    CurrentSuiteSettings.OutputFilePath, true);
                CurrentSuiteSettings.SuiteTeardown = new Keyword(RobotFileHandler.OccuranceInSettings(FilesAndFolderStructure.ConcatFileNameToFolder(CurrentSuiteSettings.OutputFilePath, type), "Suite Teardown").Replace("Suite Teardown", "").Trim(), 
                    CurrentSuiteSettings.OutputFilePath, true);
            }

            ThisFormKeywords = new List<Keyword>();
            if (CurrentSuiteSettings.TestSetup == null)
                CurrentSuiteSettings.TestSetup = new Keyword("", CurrentSuiteSettings.OutputFilePath);
            ThisFormKeywords.Add(CurrentSuiteSettings.TestSetup);

            if (CurrentSuiteSettings.TestTeardown == null)
                CurrentSuiteSettings.TestTeardown = new Keyword("", CurrentSuiteSettings.OutputFilePath);
            ThisFormKeywords.Add(CurrentSuiteSettings.TestTeardown);

            if (CurrentSuiteSettings.SuiteSetup == null)
                CurrentSuiteSettings.SuiteSetup = new Keyword("", CurrentSuiteSettings.OutputFilePath);
            ThisFormKeywords.Add(CurrentSuiteSettings.SuiteSetup);

            if (CurrentSuiteSettings.SuiteTeardown == null)
                CurrentSuiteSettings.SuiteTeardown = new Keyword("", CurrentSuiteSettings.OutputFilePath);
            ThisFormKeywords.Add(CurrentSuiteSettings.SuiteTeardown);

            SuiteDocumentation.Text = CurrentSuiteSettings.Documentation;

            for (int i = 1; i <=4; i++)
                if (Controls["DynamicStep" + i + "Name"] != null)
                    Controls["DynamicStep" + i + "Name"].Text = ThisFormKeywords[i-1].Name;
                else
                    AddKeywordField(ThisFormKeywords[i-1], i);
        }

        internal void ShowSettingsContent()
        {
            StartPosition = FormStartPosition.Manual;
            var dialogResult = ShowDialog();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            foreach (SuiteSettings temp in RobotAutomationHelper.SuiteSettingsList)
            {
                if (temp.ToString().Equals(OutputFile.Text))
                {
                    temp.Documentation = SuiteDocumentation.Text;
                    temp.Overwrite = true;
                    temp.TestSetup.CopyKeyword(ThisFormKeywords[0]);
                    temp.TestTeardown.CopyKeyword(ThisFormKeywords[1]);
                    temp.SuiteSetup.CopyKeyword(ThisFormKeywords[2]);
                    temp.SuiteTeardown.CopyKeyword(ThisFormKeywords[3]);
                    break;
                }
            }
        }

        private void OutputFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OutputFile.SelectedIndex != -1 && SelectedIndex != OutputFile.SelectedIndex)
            {
                SelectedIndex = OutputFile.SelectedIndex;
                for (int i = 1; i <= 4; i++)
                    RemoveKeywordField(i, false);
                SetupsSettingsAddForm();
            }
        }
    }
}
