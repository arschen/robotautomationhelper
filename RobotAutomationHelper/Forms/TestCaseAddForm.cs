using RobotAutomationHelper.Forms;
using RobotAutomationHelper.Scripts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper
{
    internal partial class TestCaseAddForm : Form
    {
        // index of the implemented test case
        private int IndexOfTheParentTestCase;
        private bool skip = false;

        //index of the keyword that will be implemented after Add/Edit implementation
        private int IndexOfTheKeywordToBeImplemented = 0;

        //current keywords in this test case
        internal List<Keyword> Keywords { get; set; }

        //y value for dynamic buttons
        internal int initialYValue;

        internal TestCaseAddForm()
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("TestCaseAddForm [Constructor]");
            InitializeComponent();
            initialYValue = 140;
            FormControls.UpdateOutputFileSuggestions(TestCaseOutputFile);
            ActiveControl = TestCaseNameLabel;
        }

        private void TestCaseLabel_Click(object sender, EventArgs e)
        {

        }

        private void Skip_Click(object sender, EventArgs e)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("Skip_Click");
            skip = true;
            Close();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("Save_Click");
            if (!IsTestCasePresentInFilesOrMemoryTree())
            {
                SaveChangesToTestCases();
                Close();
            }
            else
            {
                if (presentInRobotFile)
                {
                    DialogResult result = MessageBox.Show("Overwrite existing test case in the output file?",
                        "Alert",
                        MessageBoxButtons.YesNo);
                    if (result.Equals(DialogResult.Yes))
                    {
                        SaveChangesToTestCases();
                        RobotAutomationHelper.TestCases[IndexOfTheParentTestCase].Overwrite = true;
                        Close();
                    }
                    else
                        RobotAutomationHelper.TestCases[IndexOfTheParentTestCase].Overwrite = false;
                }
                else
                {
                    DialogResult result = MessageBox.Show("Test case with this name has already been implemented in the ouput file.",
                        "Alert",
                        MessageBoxButtons.OK);
                }
            }
        }

        private void TestCaseAddForm_FormClosing(object sender, EventArgs e)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("TestCaseAddForm_FormClosing");
            Close();
        }

        private void TestCaseAddForm_Load(object sender, EventArgs e)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("TestCaseAddForm_Load");
        }

        internal bool SkipValue()
        {
            return skip;
        }

        internal void ShowTestCaseContent(TestCase testCase, int testIndex)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("ShowTestCaseContent " + testCase.Name + " " + testIndex);
            IndexOfTheParentTestCase = testIndex;
            if (testCase.Name != null) { }
                TestCaseName.Text = testCase.Name;
            if (testCase.Documentation != null)
                TestCaseDocumentation.Text = testCase.Documentation.Replace("[Documentation]", "").Trim();
            if (testCase.Tags != null)
                TestCaseTags.Text = testCase.Tags.Replace("[Tags]","").Trim();
            if (testCase.OutputFilePath != null)
                TestCaseOutputFile.Text = testCase.OutputFilePath.Replace(FilesAndFolderStructure.GetFolder(),"\\");
            IsTestCasePresentInFilesOrMemoryTree();
            Keywords = new List<Keyword>();
            Keywords = testCase.Steps;

            int testStepsCounter = 1;
            if (Keywords != null && Keywords.Count != 0)
                foreach (Keyword testStep in testCase.Steps)
                {
                    AddKeywordField(testStep, testStepsCounter);
                    testStepsCounter++;
                }

            StartPosition = FormStartPosition.Manual;
            var dialogResult = ShowDialog();
        }

        private void AddKeywordField(Keyword step, int testStepsCounter)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("AddKeywordField " + step.Name + " " + testStepsCounter);
            //List<string> args = StringAndListOperations.ReturnListOfArgs(testStep.Arguments);
            
            FormControls.AddControl("TextWithList", "DynamicTestStep" + testStepsCounter + "Name",
                new Point(30 - HorizontalScroll.Value, initialYValue + (testStepsCounter - 1) * 30 - VerticalScroll.Value),
                new Size(280, 20),
                step.Name.Trim(),
                Color.Black,
                null,
                this);

            FormControls.AddControl("Label", "DynamicTestStep" + testStepsCounter + "Label",
                new Point(10 - HorizontalScroll.Value, initialYValue + 3 + (testStepsCounter - 1) * 30 - VerticalScroll.Value),
                new Size(20, 20),
                testStepsCounter + ".",
                Color.Black,
                null,
                this);
            if (step.Type.Equals(KeywordType.CUSTOM))
            {
                string buttonImplementation = "Add Implementation";
                if (step.Implemented)
                    buttonImplementation = "Edit Implementation";
                FormControls.AddControl("Button", "DynamicTestStep" + testStepsCounter + "AddImplementation",
                    new Point(320 - HorizontalScroll.Value, initialYValue + (testStepsCounter - 1) * 30 - VerticalScroll.Value),
                    new Size(120, 20),
                    buttonImplementation,
                    Color.Black,
                    new EventHandler(InstantiateKeywordAddForm),
                    this);
            }
            if (step.Params != null && step.Params.Count != 0)
                FormControls.AddControl("Button", "DynamicTestStep" + testStepsCounter + "Params",
                    new Point(450 - HorizontalScroll.Value, initialYValue + (testStepsCounter - 1) * 30 - VerticalScroll.Value),
                    new Size(75, 20),
                    "Params",
                    Color.Black,
                    new EventHandler(InstantiateParamsAddForm),
                    this);
        }

        internal void InstantiateKeywordAddForm(object sender, EventArgs e)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("InstantiateKeywordAddForm " + ((Button)sender).Name);
            int keywordIndex = int.Parse(((Button)sender).Name.Replace("AddImplementation", "").Replace("DynamicTestStep", ""));
            IndexOfTheKeywordToBeImplemented = keywordIndex;
            Keyword keyword = Keywords[keywordIndex - 1];
            keyword.Implemented = true;
            keyword.Name = Controls["DynamicTestStep" + keywordIndex + "Name"].Text;
            KeywordAddForm addKeywordForm = new KeywordAddForm(false, Keywords);
            addKeywordForm.FormClosing += new FormClosingEventHandler(UpdateThisFormAfterImlpementedChildKeyword);
            addKeywordForm.ShowKeywordContent(keyword, keywordIndex - 1);
        }

        private void UpdateThisFormAfterImlpementedChildKeyword(object sender, EventArgs e)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("UpdateThisFormAfterImlpementedChildKeyword");
            if ((sender.GetType().FullName.Contains("KeywordAddForm")) && !((KeywordAddForm)sender).SkipValue())
            {
                Controls["DynamicTestStep" + IndexOfTheKeywordToBeImplemented + "Name"].Text = Keywords[IndexOfTheKeywordToBeImplemented - 1].Name.Trim();
                Controls["DynamicTestStep" + IndexOfTheKeywordToBeImplemented + "AddImplementation"].Text = "Edit implementation";

                //List<string> args = StringAndListOperations.ReturnListOfArgs(Keywords[IndexOfTheKeywordToBeImplemented - 1].Arguments);
                
                if (Keywords[IndexOfTheKeywordToBeImplemented - 1].Params != null &&
                    Keywords[IndexOfTheKeywordToBeImplemented - 1].Params.Count != 0)
                    FormControls.AddControl("Button", "DynamicTestStep" + IndexOfTheKeywordToBeImplemented + "Params",
                        new Point(450 - HorizontalScroll.Value, initialYValue + (IndexOfTheKeywordToBeImplemented - 1) * 30 - VerticalScroll.Value),
                        new Size(75, 20),
                        "Params",
                        Color.Black,
                        new EventHandler(InstantiateParamsAddForm),
                        this);
                else
                    if (Controls.Find("DynamicTestStep" + IndexOfTheKeywordToBeImplemented + "Params", false).Length != 0)
                        FormControls.RemoveControlByKey("DynamicTestStep" + IndexOfTheKeywordToBeImplemented + "Params", Controls);

                //Adds file path + name to the Files And Folder structure for use in the drop down lists when chosing output file
                FilesAndFolderStructure.AddImplementedKeywordFilesToSavedFiles(Keywords, IndexOfTheKeywordToBeImplemented);
                FormControls.UpdateOutputFileSuggestions(TestCaseOutputFile);
            }
        }

        private void SaveChangesToTestCases()
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("SaveChangesToTestCases");
            if (RobotAutomationHelper.TestCases[IndexOfTheParentTestCase].Steps != null && RobotAutomationHelper.TestCases[IndexOfTheParentTestCase].Steps.Count > 0)
                for (int counter = 1; counter <= RobotAutomationHelper.TestCases[IndexOfTheParentTestCase].Steps.Count; counter++)
                    Keywords[counter-1].Name = ((TextWithList) Controls["DynamicTestStep" + counter + "Name"]).Text.Trim();
            
            string finalPath = FilesAndFolderStructure.ConcatFileNameToFolder(TestCaseOutputFile.Text);

            RobotAutomationHelper.TestCases[IndexOfTheParentTestCase] = new TestCase(TestCaseName.Text.Trim(),
                "[Documentation]  " + TestCaseDocumentation.Text.Trim(),
                "[Tags]  " + TestCaseTags.Text.Trim(),
                Keywords,
                finalPath);
        }

        internal void InstantiateParamsAddForm(object sender, EventArgs e)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("InstantiateParamsAddForm " + ((Button)sender).Name);
            int keywordIndex = int.Parse(((Button)sender).Name.Replace("Params", "").Replace("DynamicTestStep", ""));
            // instantiate the new KeywordAddForm with this parent and Keywords argument
            ParamAddForm addParamForm = new ParamAddForm();
            Keywords[keywordIndex - 1].Name = Controls["DynamicTestStep" + keywordIndex + "Name"].Text;
            // add closing event
            addParamForm.FormClosing += new FormClosingEventHandler(UpdateThisFormAfterImlpementedChildKeyword);
            addParamForm.ShowParamContent(Keywords[keywordIndex - 1]);
        }

        private void TestCaseName_TextChanged(object sender, EventArgs e)
        {
            IsTestCasePresentInFilesOrMemoryTree();
        }

        private void TestCaseOutputFile_TextChanged(object sender, EventArgs e)
        {
            IsTestCasePresentInFilesOrMemoryTree();
        }

        private bool presentInRobotFile;

        private bool IsTestCasePresentInFilesOrMemoryTree()
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("IsTestCasePresentInFilesOrMemoryTree");
            presentInRobotFile = false;
            if (TestCasesListOperations.IsPresentInTheTestCasesTree(TestCaseName.Text,
                FilesAndFolderStructure.ConcatFileNameToFolder(TestCaseOutputFile.Text),
                RobotAutomationHelper.TestCases[IndexOfTheParentTestCase]))
                TestCaseName.ForeColor = Color.Red;
            else
            {
                if (RobotFileHandler.ContainsTestCaseOrKeyword(FilesAndFolderStructure.ConcatFileNameToFolder(TestCaseOutputFile.Text)
                    , TestCaseName.Text, "test cases") != -1)
                {
                    TestCaseName.ForeColor = Color.Red;
                    presentInRobotFile = true;
                }
                else
                {
                    TestCaseName.ForeColor = Color.Black;
                    return false;
                }
            }
            return true;
        }
    }
}
