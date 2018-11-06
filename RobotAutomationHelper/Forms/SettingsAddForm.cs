using RobotAutomationHelper.Scripts;
using RobotAutomationHelper.Scripts.CustomControls;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RobotAutomationHelper.Forms
{
    internal partial class SettingsAddForm : BaseKeywordAddForm
    {

        private SuiteSettings CurrentSuiteSettings;
        private int SelectedIndex = 0;

        internal SettingsAddForm(BaseKeywordAddForm parent) : base(parent)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("SettingsAddForm Constructor");
            InitializeComponent();
            initialYValue = 120;
            FormType = FormType.Settings;
            SetupsSettingsAddForm();
        }

        private void SetupsSettingsAddForm()
        {
            UpdateOutputFileSuggestions(OutputFile, FormType);
            OutputFile.SelectedIndex = SelectedIndex;
            bool add;
            foreach (string fileName in FilesAndFolderStructure.GetShortSavedFiles(FolderType.Root))
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


            FolderType folderType = FolderType.Root;
            if (CurrentSuiteSettings.OutputFilePath.Contains("Resources"))
                folderType = FolderType.Resources;
            if (CurrentSuiteSettings.OutputFilePath.Contains("Tests"))
                folderType = FolderType.Tests;

            if (!CurrentSuiteSettings.Overwrite)
            {
                string realOutput = CurrentSuiteSettings.OutputFilePath;
                if (folderType.Equals(FolderType.Tests))
                    realOutput = realOutput.Remove(0, 5);
                else
                    if (folderType.Equals(FolderType.Resources))
                    realOutput = realOutput.Remove(0, 9);

                CurrentSuiteSettings.Documentation = RobotFileHandler.OccuranceInSettings(FilesAndFolderStructure.ConcatFileNameToFolder(CurrentSuiteSettings.OutputFilePath, FolderType.Root), 
                    "Documentation").Replace("Documentation", "").Trim();
                CurrentSuiteSettings.TestSetup = new Keyword(RobotFileHandler.OccuranceInSettings(FilesAndFolderStructure.ConcatFileNameToFolder(CurrentSuiteSettings.OutputFilePath, FolderType.Root), "Test Setup").Replace("Test Setup", "").Trim(),
                    realOutput, true, ReadRobotFiles.GetLibs(FilesAndFolderStructure.ConcatFileNameToFolder(CurrentSuiteSettings.OutputFilePath, FolderType.Root)), null);
                CurrentSuiteSettings.TestTeardown = new Keyword(RobotFileHandler.OccuranceInSettings(FilesAndFolderStructure.ConcatFileNameToFolder(CurrentSuiteSettings.OutputFilePath, FolderType.Root), "Test Teardown").Replace("Test Teardown", "").Trim(),
                    realOutput, true, ReadRobotFiles.GetLibs(FilesAndFolderStructure.ConcatFileNameToFolder(CurrentSuiteSettings.OutputFilePath, FolderType.Root)), null);
                CurrentSuiteSettings.SuiteSetup = new Keyword(RobotFileHandler.OccuranceInSettings(FilesAndFolderStructure.ConcatFileNameToFolder(CurrentSuiteSettings.OutputFilePath, FolderType.Root), "Suite Setup").Replace("Suite Setup", "").Trim(),
                    realOutput, true, ReadRobotFiles.GetLibs(FilesAndFolderStructure.ConcatFileNameToFolder(CurrentSuiteSettings.OutputFilePath, FolderType.Root)), null);
                CurrentSuiteSettings.SuiteTeardown = new Keyword(RobotFileHandler.OccuranceInSettings(FilesAndFolderStructure.ConcatFileNameToFolder(CurrentSuiteSettings.OutputFilePath, FolderType.Root), "Suite Teardown").Replace("Suite Teardown", "").Trim(),
                    realOutput, true, ReadRobotFiles.GetLibs(FilesAndFolderStructure.ConcatFileNameToFolder(CurrentSuiteSettings.OutputFilePath, FolderType.Root)), null);
            }

            ThisFormKeywords = new List<Keyword>();
            if (CurrentSuiteSettings.TestSetup == null)
                CurrentSuiteSettings.TestSetup = new Keyword("", "Auto.robot", null);
            ThisFormKeywords.Add(CurrentSuiteSettings.TestSetup);

            if (CurrentSuiteSettings.TestTeardown == null)
                CurrentSuiteSettings.TestTeardown = new Keyword("", "Auto.robot", null);
            ThisFormKeywords.Add(CurrentSuiteSettings.TestTeardown);

            if (CurrentSuiteSettings.SuiteSetup == null)
                CurrentSuiteSettings.SuiteSetup = new Keyword("", "Auto.robot", null);
            ThisFormKeywords.Add(CurrentSuiteSettings.SuiteSetup);

            if (CurrentSuiteSettings.SuiteTeardown == null)
                CurrentSuiteSettings.SuiteTeardown = new Keyword("", "Auto.robot", null);
            ThisFormKeywords.Add(CurrentSuiteSettings.SuiteTeardown);

            SuiteDocumentation.Text = CurrentSuiteSettings.Documentation;

            if (folderType == FolderType.Tests)
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

        private void SaveAndExit_Click(object sender, EventArgs e)
        {
            Save_Click(sender, e);
            Close();
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

        internal void UpdateNamesListAndUpdateStateOfSave()
        {
            List<string> namesList = new List<string>
            {
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
