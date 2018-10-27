using RobotAutomationHelper.Forms;
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
        private static int numberOfTestCases;

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

        private void OpenExistingProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                BrowseFolderButtonExistingProject();
            }
        }

        private void NewProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog3.ShowDialog() == DialogResult.OK)
            {
                BrowseFolderButtonNewProject();
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

        private void BrowseFolderButtonNewProject()
        {
            ClearDynamicElements();
            settingsToolStripMenuItem.Visible = true;
            SetStructureFolder(folderBrowserDialog3.SelectedPath);
            AddTestCasesToMainForm();
            ShowTestCasePanels();
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
            if (ProjectTestCases.Count != 0)
                foreach (TestCase tempProj in ProjectTestCases)
                {
                    if (tempProj.Steps != null)
                        foreach (Keyword tempKeyword in tempProj.Steps)
                        {
                            KeywordToSuggestions(tempKeyword);
                        }
                }
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
            if (TestCases.Count != 0)
            {
                foreach (TestCase testCase in TestCases)
                {
                    if (testCase.Steps != null)
                        foreach (Keyword tempKeyword in testCase.Steps)
                        {
                            KeywordToSuggestions(tempKeyword);
                        }
                }
                TestCases.Sort();
                AddTestCasesToMainForm();
                ShowTestCasePanels();
            }
            else
            {
                DialogResult result = MessageBox.Show("No test cases in the selected folder!",
                    "Alert",
                    MessageBoxButtons.OK);
            }
        }

        private void KeywordToSuggestions(Keyword tempKeyword)
        {
            if (tempKeyword.SuggestionIndex == -1 && !tempKeyword.OutputFilePath.Equals(""))
            {
                bool toAdd = true;
                foreach (Keyword suggested in FormControls.Suggestions)
                {
                    if (!suggested.OutputFilePath.Equals(""))
                    {
                        if (suggested.Name.Equals(tempKeyword.Name) && suggested.OutputFilePath.Equals(tempKeyword.OutputFilePath))
                        {
                            toAdd = false;
                            break;
                        }
                    }
                }
                if (toAdd)
                {
                    tempKeyword.SuggestionIndex = FormControls.Suggestions.Count;
                    FormControls.Suggestions.Add(tempKeyword);
                }
            }
            if (tempKeyword.Keywords != null)
                foreach (Keyword nestedKeyword in tempKeyword.Keywords)
                {
                    KeywordToSuggestions(nestedKeyword);
                }
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
            numberOfTestCases = TestCases.Count;
            int testCasesCounter = 1;
            if (TestCases != null && TestCases.Count != 0)
                foreach (TestCase testCase in TestCases)
                {
                    AddTestCaseField(testCase, testCasesCounter);
                    testCasesCounter++;
                }
            else
            {
                TestCases.Add(new TestCase("New Test Case", FilesAndFolderStructure.GetFolder(FolderType.Tests) + "Auto.robot"));
                AddTestCaseField(TestCases[0], testCasesCounter);
                testCasesCounter++;
                numberOfTestCases = 1;
            }
        }

        private void AddTestCaseField(TestCase testCase, int testCasesCounter)
        {
            FormControls.AddControl("TextBox", "DynamicTest" + testCasesCounter + "Name",
                testCasesCounter,
                new Point(30 - HorizontalScroll.Value, 50 + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                new Size(280, 20),
                testCase.Name.Trim(),
                Color.Black,
                null,
                this);
            FormControls.AddControl("Label", "DynamicTest" + testCasesCounter + "Label",
                testCasesCounter,
                new Point(10 - HorizontalScroll.Value, 53 + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                new Size(20, 20),
                testCasesCounter + ".",
                Color.Black,
                null,
                this);
            FormControls.AddControl("CheckBox", "DynamicTest" + testCasesCounter + "CheckBox",
                testCasesCounter,
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
                testCasesCounter,
                new Point(345 - HorizontalScroll.Value, 50 + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                new Size(120, 20),
                ImplementationText,
                Color.Black,
                new EventHandler(InstantiateAddTestCaseForm),
                this);

            FormControls.AddControl("Button", "DynamicTest" + testCasesCounter + "AddTestCase",
                testCasesCounter,
                new Point(470 - HorizontalScroll.Value, 50 + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                new Size(20, 20),
                "+",
                Color.Black,
                new EventHandler(InstantiateNameAndOutputForm),
                this);
            FormControls.AddControl("Button", "DynamicTest" + testCasesCounter + "RemoveTestCase",
                testCasesCounter,
                new Point(490 - HorizontalScroll.Value, 50 + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                new Size(20, 20),
                "-",
                Color.Black,
                new EventHandler(RemoveTestCaseFromProject),
                this);
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
                    WriteToRobot.TestCaseKeywordRemove(testCase.Name, testCase.OutputFilePath, false);

                if (testCase.Steps != null)
                    foreach (Keyword testStep in testCase.Steps)
                    {
                        WriteToRobot.RemoveKeywordForOverwriting(testStep);
                        if (testStep.Overwrite)
                            WriteToRobot.TestCaseKeywordRemove(testStep.Name, testStep.OutputFilePath, true);
                    }
            }

            foreach (int index in testCasesToAdd)
            {
                TestCase testCase = TestCases[index - 1];
                WriteToRobot.AddTestCaseToRobot(testCase);
            }

            WriteToRobot.WriteIncludesToRobotFiles();
            WriteToRobot.WriteSuiteSettingsListToRobot();

            foreach (string fileName in FilesAndFolderStructure.GetShortSavedFiles(FolderType.Root))
                RobotFileHandler.TrimFile(FilesAndFolderStructure.ConcatFileNameToFolder(fileName, FolderType.Root));
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FilesAndFolderStructure.GetShortSavedFiles(FolderType.Root) != null && FilesAndFolderStructure.GetShortSavedFiles(FolderType.Root).Count > 0)
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
            RunCom("cd " + FilesAndFolderStructure.GetFolder(FolderType.Root) + "&robot tests");
        }

        private object realSender;

        internal new void InstantiateNameAndOutputForm(object sender, EventArgs e)
        {
            realSender = sender;
            if (RobotAutomationHelper.Log) Console.WriteLine("InstantiateParamsAddForm " + ((Button)sender).Name);
            FormType formType;
            if (Name.Contains("Robot Automation Helper"))
                formType = FormType.Test;
            else
                formType = FormType.Keyword;

            NameAndOutputForm nameAndOutputForm = new NameAndOutputForm(formType);
            nameAndOutputForm.FormClosing += new FormClosingEventHandler(UpdateAfterClosingNameAndOutputForm);
            nameAndOutputForm.ShowContent();
        }

        private void UpdateAfterClosingNameAndOutputForm(object sender, EventArgs e)
        {
            AddTestCaseToProject(realSender, e);
        }

        internal void AddTestCaseToProject(object sender, EventArgs e)
        {
            int testCaseIndex = int.Parse(((Button)sender).Name.Replace("DynamicTest", "").Replace("AddTestCase", ""));

            AssignThisTestCasesNamesFromTextFields();

            TestCases.Add(new TestCase("New Test Case", FilesAndFolderStructure.GetFolder(FolderType.Tests) + "Auto.robot"));

            for (int i = numberOfTestCases; i > testCaseIndex; i--)
                TestCases[i] = TestCases[i - 1];

            TestCases[testCaseIndex] = new TestCase("New Test Case", FilesAndFolderStructure.GetFolder(FolderType.Tests) + "Auto.robot");

            numberOfTestCases++;
            AddTestCaseField(TestCases[numberOfTestCases - 1], numberOfTestCases);

            for (int i = 1; i < numberOfTestCases; i++)
                Controls["DynamicTest" + i + "Name"].Text = TestCases[i - 1].Name.Trim();
        }

        internal void RemoveTestCaseFromProject(object sender, EventArgs e)
        {
            AssignThisTestCasesNamesFromTextFields();

            if (numberOfTestCases > 1)
            {
                int testCaseIndex = int.Parse(((Button)sender).Name.Replace("DynamicTest", "").Replace("RemoveTestCase", ""));
                RemoveTestCaseField(numberOfTestCases, false);
                TestCases.RemoveAt(testCaseIndex - 1);
                numberOfTestCases--;
                List<string> args = new List<string>();
                for (int i = 1; i <= numberOfTestCases; i++)
                    Controls["DynamicTest" + i + "Name"].Text = TestCases[i - 1].Name.Trim();
            }
        }

        private void AssignThisTestCasesNamesFromTextFields()
        {
            for (int i = 1; i <= numberOfTestCases; i++)
                if (Controls.Find("DynamicTest" + i + "Name", false).Length != 0)
                    TestCases[i - 1].Name = Controls["DynamicTest" + i + "Name"].Text;
        }

        // Removes TextBox / Label / Add implementation / Add and remove keyword / Params
        private void RemoveTestCaseField(int testCaseIndex, bool removeFromList)
        {
            FormControls.RemoveControlByKey("DynamicTest" + testCaseIndex + "Name", Controls);
            FormControls.RemoveControlByKey("DynamicTest" + testCaseIndex + "Label", Controls);
            FormControls.RemoveControlByKey("DynamicTest" + testCaseIndex + "AddImplementation", Controls);
            FormControls.RemoveControlByKey("DynamicTest" + testCaseIndex + "CheckBox", Controls);
            FormControls.RemoveControlByKey("DynamicTest" + testCaseIndex + "AddTestCase", Controls);
            FormControls.RemoveControlByKey("DynamicTest" + testCaseIndex + "RemoveTestCase", Controls);
            if (removeFromList)
                TestCases.RemoveAt(testCaseIndex - 1);
        }
    }
}