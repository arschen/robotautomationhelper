﻿using RobotAutomationHelper.Scripts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RobotAutomationHelper
{
    internal partial class RobotAutomationHelper : BaseKeywordAddForm
    {
        internal static List<TestCase> TestCases = new List<TestCase>();
        internal static List<SuiteSettings> SuiteSettingsList = new List<SuiteSettings>();

        internal static bool Log = false;
        // index of the test case to be implemented
        private int IndexOfTheTestCaseToBeImplemented = 0;

        internal RobotAutomationHelper()
        {
            InitializeComponent();
            ActiveControl = TestCaseNameLabel;
            HtmlLibsGetter.PopulateSeleniumKeywords();
            HtmlLibsGetter.PopulateBuiltInKeywords();
        }

        private void ApplicationMain_Load(object sender, EventArgs e)
        {

        }

        // open file click
        private void ToolStripMenuOpen_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        // browse folders for output directory after file has been opened
        private void OpenFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                BrowseFolderButton_Click();
            }
        }

        private void BrowseFolderButton_Click()
        {
            ClearDynamicElements();
            settingsToolStripMenuItem.Visible = true;
            SetStructureFolder();
            ShowTestCasePanels();
        }

        //Clear dynamic elements when new file is opened
        private void ClearDynamicElements()
        {
            bool cleared = false;
            while (!cleared)
            {
                foreach (Control tempControl in Controls)
                    if (tempControl.Name.ToLower().StartsWith("dynamictest"))
                        FormControls.RemoveControlByKey(tempControl.Name, Controls);

                cleared = true;
                foreach (Control tempControl in Controls)
                {
                    if (tempControl.Name.ToLower().StartsWith("dynamictest"))
                    {
                        cleared = false;
                        break;
                    }
                }
            }
        }

        private void AddTestCasesToMainForm()
        {
            int testCasesCounter = 1;
            if (TestCases != null && TestCases.Count != 0)
                foreach (TestCase testCase in TestCases)
                {
                    string testCaseName = testCase.Name;

                    FormControls.AddControl("TextBox", "DynamicTest" + testCasesCounter + "Name",
                        new Point(30 - HorizontalScroll.Value, 50 + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                        new Size(280, 20),
                        testCaseName.Trim(),
                        Color.Black,
                        null,
                        this);
                    FormControls.AddControl("Label", "DynamicTest" + testCasesCounter + "Label",
                        new Point(10 - HorizontalScroll.Value, 53 + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                        new Size(20, 20),
                        testCasesCounter + ".",
                        Color.Black,
                        null,
                        this);
                    FormControls.AddControl("CheckBox", "DynamicTest" + testCasesCounter + "CheckBox",
                        new Point(325 - HorizontalScroll.Value, 50 + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                        new Size(20, 20),
                        "Add",
                        Color.Black,
                        null,
                        this);
                    FormControls.AddControl("Button", "DynamicTest" + testCasesCounter + "AddImplementation",
                        new Point(345 - HorizontalScroll.Value, 50 + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                        new Size(120, 20),
                        "Add Implementation",
                        Color.Black,
                        new EventHandler(InstantiateAddTestCaseForm),
                        this);

                    testCasesCounter++;
                }
        }

        private void SetStructureFolder()
        {
            
            string outputFolder = folderBrowserDialog1.SelectedPath;

            if (!outputFolder.EndsWith("\\"))
                outputFolder = outputFolder + "\\";
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            FilesAndFolderStructure.SetFolder(outputFolder);

            FilesAndFolderStructure.FindAllRobotFilesAndAddToStructure();

            TestCases = ReadExcel.ReadAllTestCasesFromExcel(openFileDialog.FileName);
            AddTestCasesToMainForm();
        }

        protected void InstantiateAddTestCaseForm(object sender, EventArgs e)
        {
            int testIndex = int.Parse(((Button)sender).Name.Replace("AddImplementation", "").Replace("DynamicTest", ""));
            IndexOfTheTestCaseToBeImplemented = testIndex;
            TestCase testCase = TestCases[testIndex - 1];
            testCase.Name = Controls["DynamicTest" + testIndex + "Name"].Text;
            TestCaseAddForm testCaseAddForm = new TestCaseAddForm();
            testCaseAddForm.FormClosing += new FormClosingEventHandler(UpdateThisFormTestCaseAddFormClosing);
            testCaseAddForm.ShowTestCaseContent(testCase, testIndex - 1);
        }

        private void UpdateThisFormTestCaseAddFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!((TestCaseAddForm) sender).skip)
            {
                Controls["DynamicTest" + IndexOfTheTestCaseToBeImplemented + "Name"].Text = TestCases[IndexOfTheTestCaseToBeImplemented - 1].Name;
                Controls["DynamicTest" + IndexOfTheTestCaseToBeImplemented + "AddImplementation"].Text = "Edit implementation";
            }

            //Adds file path + name to the Files And Folder structure for use in the drop down lists when chosing output file
            FilesAndFolderStructure.AddImplementedTestCasesFilesToSavedFiles(TestCases, IndexOfTheTestCaseToBeImplemented);
        }

        internal void ShowTestCasePanels()
        {
            IndexLabel.Visible = true;
            TestCaseNameLabel.Visible = true;
            AddLabel.Visible = true;
        }

        private void SaveToRobotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WriteToRobot.includes = new List<Includes>();
            List<int> testCasesToAdd = new List<int>();
            foreach (Control tempControl in Controls)
                if (tempControl.Name.EndsWith("CheckBox") && ((CheckBox)tempControl).Checked)
                    testCasesToAdd.Add(int.Parse(tempControl.Name.Replace("CheckBox", "").Replace("DynamicTest", "")));

            testCasesToAdd.Sort();
            foreach (int index in testCasesToAdd)
            {
                TestCase testCase = TestCases[index - 1];
                if (testCase.Overwrite)
                    RobotFileHandler.TestCaseKeywordRemove(testCase.Name, testCase.OutputFilePath, false);

                if (testCase.Steps != null)
                    foreach (Keyword testStep in testCase.Steps)
                    {
                        WriteToRobot.RemoveKeyword(testStep);
                        if (testStep.Overwrite)
                            RobotFileHandler.TestCaseKeywordRemove(testStep.Name, testStep.OutputFilePath, true);
                    }
            }

            foreach (int index in testCasesToAdd)
            {
                TestCase testCase = TestCases[index - 1];
                WriteToRobot.AddTestCaseToRobot(testCase);
            }

            WriteToRobot.WriteIncludesToRobotFiles();
            //TODO
            //WriteToRobot.WriteSuiteSettingsListToRobot();

            foreach (string fileName in FilesAndFolderStructure.SavedFiles)
                RobotFileHandler.TrimFile(FilesAndFolderStructure.ConcatFileNameToFolder(fileName));
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FilesAndFolderStructure.SavedFiles != null && FilesAndFolderStructure.SavedFiles.Count > 0)
                InstantiateSettingsAddForm(sender, e);
            else
            {
                DialogResult result = MessageBox.Show("You haven't saved any keywords or test cases to files yet.",
                "Alert",
                MessageBoxButtons.OK);
            }
        }
    }
}