using RobotAutomationHelper.Forms;
using RobotAutomationHelper.Scripts;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RobotAutomationHelper
{
    public partial class TestCaseAddForm : Form
    {

        public int index;
        private bool skip = false;
        private int implementedKeyword = 0;
        public static List<Keyword> Keywords;

        public TestCaseAddForm()
        {
            InitializeComponent();
            TestCaseOutputFile.Items.Clear();
            TestCaseOutputFile.AutoCompleteCustomSource.Clear();
            TestCaseOutputFile.Items.AddRange(FilesAndFolderStructure.GetFilesList().ToArray());
            TestCaseOutputFile.AutoCompleteCustomSource.AddRange(FilesAndFolderStructure.GetFilesList().ToArray());
            TestCaseOutputFile.AutoCompleteSource = AutoCompleteSource.CustomSource;
            TestCaseOutputFile.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        }

        private void TestCaseLabel_Click(object sender, EventArgs e)
        {

        }

        private void Skip_Click(object sender, EventArgs e)
        {
            skip = true;
            this.Close();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            SaveChangesToTestCases();
            this.Close();
        }

        private void TestCaseAddForm_FormClosing(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TestCaseAddForm_Load(object sender, EventArgs e)
        {

        }

        internal bool SkipValue()
        {
            return skip;
        }

        internal void ShowTestCaseContent(TestCase testCase, int testIndex)
        {
            index = testIndex;
            if (testCase.GetTestName() != null)
                TestCaseName.Text = testCase.GetTestName();
            if (testCase.GetTestDocumentation() != null)
                TestCaseDocumentation.Text = testCase.GetTestDocumentation().Replace("[Documentation]", "").Trim();
            if (testCase.GetTestCaseTags() != null)
                TestCaseTags.Text = testCase.GetTestCaseTags().Replace("[Tags]","").Trim();
            if (testCase.GetOutputFilePath() != null)
                TestCaseOutputFile.Text = testCase.GetOutputFilePath().Replace(FilesAndFolderStructure.GetFolder(),"\\");
            Keywords = new List<Keyword>();
            Keywords = testCase.GetTestSteps();

            int testStepsCounter = 1;
            if (Keywords != null && Keywords.Count != 0)
                foreach (Keyword testStep in testCase.GetTestSteps())
                {
                    List<string> args = StringAndListOperations.ReturnListOfArgs(testStep.GetKeywordArguments());

                    FormControls.AddControl("TextBox", "DynamicTestStep" + testStepsCounter + "Name",
                        new System.Drawing.Point(30 - this.HorizontalScroll.Value, 140 + (testStepsCounter - 1) * 30 - this.VerticalScroll.Value),
                        new System.Drawing.Size(280, 20),
                        testStep.GetKeywordName().Trim(),
                        System.Drawing.Color.Black,
                        null,
                        this);
                    FormControls.AddControl("Label", "DynamicTestStep" + testStepsCounter + "Label",
                        new System.Drawing.Point(10 - this.HorizontalScroll.Value, 143 + (testStepsCounter - 1) * 30 - this.VerticalScroll.Value),
                        new System.Drawing.Size(20, 20),
                        testStepsCounter + ".",
                        System.Drawing.Color.Black,
                        null,
                        this);
                    FormControls.AddControl("Button", "DynamicTestStep" + testStepsCounter + "AddImplementation",
                        new System.Drawing.Point(320 - this.HorizontalScroll.Value, 140 + (testStepsCounter - 1) * 30 - this.VerticalScroll.Value),
                        new System.Drawing.Size(120, 20),
                        "Add Implementation",
                        System.Drawing.Color.Black,
                        new EventHandler(InstantiateKeywordAddForm),
                        this);
                    if (args != null && args.Count != 0)
                        FormControls.AddControl("Button", "DynamicTestStep" + testStepsCounter + "Params",
                            new System.Drawing.Point(450 - this.HorizontalScroll.Value, 140 + (testStepsCounter - 1) * 30 - this.VerticalScroll.Value),
                            new System.Drawing.Size(75, 20),
                            "Params",
                            System.Drawing.Color.Black,
                            new EventHandler(InstantiateParamsAddForm),
                            this);
                    testStepsCounter++;
                }

            this.StartPosition = FormStartPosition.Manual;
            var dialogResult = this.ShowDialog();
        }

        private void InstantiateKeywordAddForm(object sender, EventArgs e)
        {
            int keywordIndex = int.Parse(((Button)sender).Name.Replace("AddImplementation", "").Replace("DynamicTestStep", ""));
            implementedKeyword = keywordIndex;
            Keyword keyword = Keywords[keywordIndex - 1];
            KeywordAddForm addKeywordForm = new KeywordAddForm(false, Keywords);
            addKeywordForm.FormClosing += new FormClosingEventHandler(UpdateThisFormAfterImlpementedChildKeyword);
            addKeywordForm.ShowKeywordContent(keyword, keywordIndex - 1);
        }

        private void UpdateThisFormAfterImlpementedChildKeyword(object sender, EventArgs e)
        {
            if ((sender.GetType().FullName.Contains("KeywordAddForm")) && !((KeywordAddForm)sender).SkipValue())
            {
                this.Controls["DynamicTestStep" + implementedKeyword + "Name"].Text = Keywords[implementedKeyword - 1].GetKeywordName().Trim();
                this.Controls["DynamicTestStep" + implementedKeyword + "AddImplementation"].Text = "Edit implementation";
                FormControls.AddControl("Label", "DynamicTestStep" + implementedKeyword + "ImplementationPanel",
                    new System.Drawing.Point(535 - this.HorizontalScroll.Value, 140 + (implementedKeyword - 1) * 30 - this.VerticalScroll.Value),
                    new System.Drawing.Size(20, 20),
                    "✔",
                    System.Drawing.Color.Green,
                    null,
                    this);

                List<string> args = StringAndListOperations.ReturnListOfArgs(Keywords[implementedKeyword - 1].GetKeywordArguments());
                
                if (args != null && args.Count != 0)
                    FormControls.AddControl("Button", "DynamicTestStep" + implementedKeyword + "Params",
                        new System.Drawing.Point(450 - this.HorizontalScroll.Value, 140 + (implementedKeyword - 1) * 30 - this.VerticalScroll.Value),
                        new System.Drawing.Size(75, 20),
                        "Params",
                        System.Drawing.Color.Black,
                        new EventHandler(InstantiateParamsAddForm),
                        this);

                //Adds file path + name to the Files And Folder structure for use in the drop down lists when chosing output file
                FilesAndFolderStructure.AddImplementedKeywordFilesToSavedFiles(Keywords, implementedKeyword);
            }
        }

        private void SaveChangesToTestCases()
        {
            if (ApplicationMain.TestCases[index].GetTestSteps() != null && ApplicationMain.TestCases[index].GetTestSteps().Count > 0)
                for (int counter = 1; counter <= ApplicationMain.TestCases[index].GetTestSteps().Count; counter++)
                    Keywords[counter-1].SetKeywordName("\t" + ((TextBox) Controls["DynamicTestStep" + counter + "Name"]).Text.Trim());

            string finalPath = FilesAndFolderStructure.ConcatFileNameToFolder(TestCaseOutputFile.Text);

            ApplicationMain.TestCases[index] = new TestCase(TestCaseName.Text.Trim(),
                "\t[Documentation]  " + TestCaseDocumentation.Text.Trim(),
                "\t[Tags]  " + TestCaseTags.Text.Trim(),
                Keywords,
                finalPath);
        }

        private void InstantiateParamsAddForm(object sender, EventArgs e)
        {
            int keywordIndex = int.Parse(((Button)sender).Name.Replace("Params", "").Replace("DynamicTestStep", ""));
            // instantiate the new KeywordAddForm with this parent and Keywords argument
            ParamAddForm addParamForm = new ParamAddForm();
            // add closing event
            addParamForm.FormClosing += new FormClosingEventHandler(UpdateThisFormAfterImlpementedChildKeyword);
            addParamForm.ShowParamContent(Keywords[keywordIndex - 1]);
        }
    }
}
