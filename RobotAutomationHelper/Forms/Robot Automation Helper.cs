using RobotAutomationHelper.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace RobotAutomationHelper
{
    public partial class ApplicationMain : Form
    {
        public static List<TestCase> TestCases;

        private int implementedTest = 0;

        public ApplicationMain()
        {
            InitializeComponent();
        }

        // open file click
        private void ToolStripMenuOpen_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void ApplicationMain_Load(object sender, EventArgs e)
        {

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
                        Controls.RemoveByKey(tempControl.Name);

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
                    string testCaseName = testCase.GetTestName();

                    FormControls.AddControl("TextBox", "DynamicTest" + testCasesCounter + "Name",
                        new System.Drawing.Point(30 - HorizontalScroll.Value, 50 + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                        new System.Drawing.Size(280, 20),
                        testCaseName.Trim(),
                        System.Drawing.Color.Black,
                        null,
                        this);
                    FormControls.AddControl("Label", "DynamicTest" + testCasesCounter + "Label",
                        new System.Drawing.Point(10 - HorizontalScroll.Value, 53 + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                        new System.Drawing.Size(20, 20),
                        testCasesCounter + ".",
                        System.Drawing.Color.Black,
                        null,
                        this);
                    FormControls.AddControl("CheckBox", "DynamicTest" + testCasesCounter + "CheckBox",
                        new System.Drawing.Point(325 - HorizontalScroll.Value, 50 + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                        new System.Drawing.Size(20, 20),
                        "Add",
                        System.Drawing.Color.Black,
                        null,
                        this);
                    FormControls.AddControl("Button", "DynamicTest" + testCasesCounter + "AddImplementation",
                        new System.Drawing.Point(345 - HorizontalScroll.Value, 50 + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                        new System.Drawing.Size(120, 20),
                        "Add Implementation",
                        System.Drawing.Color.Black,
                        new EventHandler(InstantiateAddTestCaseForm),
                        this);

                    testCasesCounter++;
                }
        }

        private void InstantiateAddTestCaseForm(object sender, EventArgs e)
        {
            int testIndex = int.Parse(((Button)sender).Name.Replace("AddImplementation", "").Replace("DynamicTest", ""));
            implementedTest = testIndex;
            TestCase testCase = TestCases[testIndex - 1];
            testCase.SetTestName(Controls["DynamicTest" + testIndex + "Name"].Text);
            TestCaseAddForm testCaseAddForm = new TestCaseAddForm();
            testCaseAddForm.FormClosing += new FormClosingEventHandler(TestCaseAddFormClosing);
            testCaseAddForm.ShowTestCaseContent(testCase, testIndex - 1);
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

        private void TestCaseAddFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!((TestCaseAddForm) sender).SkipValue())
            {
                Controls["DynamicTest" + implementedTest + "Name"].Text = TestCases[implementedTest - 1].GetTestName();
                Controls["DynamicTest" + implementedTest + "AddImplementation"].Text = "Edit implementation";

                //Adds file path + name to the Files And Folder structure for use in the drop down lists when chosing output file
                FilesAndFolderStructure.AddImplementedTestCasesFilesToSavedFiles(TestCases, implementedTest);
            }
        }

        public void ShowTestCasePanels()
        {
            IndexLabel.Visible = true;
            TestCaseNameLabel.Visible = true;
            AddLabel.Visible = true;
        }

        private void saveToRobotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<int> testCasesToAdd = new List<int>();
            foreach (Control tempControl in Controls)
                if (tempControl.Name.EndsWith("CheckBox") && ((CheckBox)tempControl).Checked)
                    testCasesToAdd.Add(int.Parse(tempControl.Name.Replace("CheckBox", "").Replace("DynamicTest", "")));

            testCasesToAdd.Sort();
            foreach (int index in testCasesToAdd)
            {
                TestCase testCase = TestCases[index - 1];
                WriteToRobot.AddTestCasesToRobot(testCase);
            }
        }
    }
}
