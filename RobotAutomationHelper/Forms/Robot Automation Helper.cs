using RobotAutomationHelper.Scripts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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


        private void openExistingProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                BrowseFolderButtonExistingProject();
            }
        }

        // browse folders for output directory after file has been opened
        private void OpenFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                BrowseFolderButtonOpenExcel();
            }
        }

        private void BrowseFolderButtonOpenExcel()
        {
            ClearDynamicElements();
            settingsToolStripMenuItem.Visible = true;
            SetStructureFolder(folderBrowserDialog1.SelectedPath);
            TestCases = ReadExcel.ReadAllTestCasesFromExcel(openFileDialog.FileName);
            TestCases.Sort();
            List<TestCase> ProjectTestCases = new List<TestCase>();
            ProjectTestCases = ReadRobotFiles.ReadAllTests();
            bool showAlert = false;
            foreach (TestCase tempProj in ProjectTestCases)
            {
                foreach (TestCase tempExc in TestCases)
                {
                    if (tempProj.Name.Equals(tempExc.Name))
                    {
                        showAlert = true;
                        break;
                    }
                }
                if (showAlert)
                    break;
            }
            if (showAlert)
            {
                DialogResult result = MessageBox.Show("Use existing Test Cases in project folder?",
                    "Alert",
                    MessageBoxButtons.YesNo);
                if (result.Equals(DialogResult.Yes))
                {
                    foreach (TestCase tempProj in ProjectTestCases)
                    {
                        foreach (TestCase tempExc in TestCases)
                        {
                            if (tempProj.Name.Equals(tempExc.Name))
                            {
                                tempExc.CopyTestCase(tempProj);
                            }
                        }
                    }
                }
            }

            AddTestCasesToMainForm();
            ShowTestCasePanels();
        }

        private void BrowseFolderButtonExistingProject()
        {
            ClearDynamicElements();
            settingsToolStripMenuItem.Visible = true;
            SetStructureFolder(folderBrowserDialog2.SelectedPath);
            TestCases = ReadRobotFiles.ReadAllTests();
            TestCases.Sort();
            AddTestCasesToMainForm();
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

                    string ImplementationText = "Add Implementation";
                    if (TestCases[testCasesCounter - 1].Implemented)
                        ImplementationText = "Edit Implementation";
                    FormControls.AddControl("Button", "DynamicTest" + testCasesCounter + "AddImplementation",
                        new Point(345 - HorizontalScroll.Value, 50 + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                        new Size(120, 20),
                        ImplementationText,
                        Color.Black,
                        new EventHandler(InstantiateAddTestCaseForm),
                        this);

                    testCasesCounter++;
                }
        }

        private void SetStructureFolder(string outputFolder)
        {
            if (!outputFolder.EndsWith("\\"))
                outputFolder = outputFolder + "\\";
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            FilesAndFolderStructure.SetFolder(outputFolder);

            FilesAndFolderStructure.FindAllRobotFilesAndAddToStructure();
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
                if (TestCases[IndexOfTheTestCaseToBeImplemented - 1].Implemented)
                    Controls["DynamicTest" + IndexOfTheTestCaseToBeImplemented + "AddImplementation"].Text = "Edit implementation";
                else
                    Controls["DynamicTest" + IndexOfTheTestCaseToBeImplemented + "AddImplementation"].Text = "Add implementation";
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
            WriteToRobot.WriteSuiteSettingsListToRobot();

            foreach (string fileName in FilesAndFolderStructure.GetShortSavedFiles(""))
                RobotFileHandler.TrimFile(FilesAndFolderStructure.ConcatFileNameToFolder(fileName, ""));
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FilesAndFolderStructure.GetShortSavedFiles("") != null && FilesAndFolderStructure.GetShortSavedFiles("").Count > 0)
                InstantiateSettingsAddForm(sender, e);
            else
            {
                DialogResult result = MessageBox.Show("You haven't saved any keywords or test cases to files yet.",
                "Alert",
                MessageBoxButtons.OK);
            }
        }

        private void RunCom(string command)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c" + command;
            cmd.StartInfo.RedirectStandardInput = false;
            cmd.StartInfo.RedirectStandardOutput = false;
            cmd.StartInfo.CreateNoWindow = false;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
        }

        private void SaveToRobotAndRunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveToRobotToolStripMenuItem_Click(sender, e);
            RunCom("cd " + FilesAndFolderStructure.GetFolder("") + "&robot tests");
        }
    }
}