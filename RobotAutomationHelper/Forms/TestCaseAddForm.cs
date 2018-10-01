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
            AddChangesToTestCases();
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
                        new EventHandler(ShowAddKeywordForm),
                        this);
                    testStepsCounter++;
                }

            this.StartPosition = FormStartPosition.Manual;
            var dialogResult = this.ShowDialog();
        }

        private void ShowAddKeywordForm(object sender, EventArgs e)
        {
            int keywordIndex = int.Parse(((Button)sender).Name.Replace("AddImplementation", "").Replace("DynamicTestStep", ""));
            implementedKeyword = keywordIndex;
            Keyword keyword = Keywords[keywordIndex - 1];
            KeywordAddForm addKeywordForm = new KeywordAddForm(false, Keywords);
            addKeywordForm.FormClosing += new FormClosingEventHandler(KeywordAddFormClosing);
            addKeywordForm.ShowKeywordContent(keyword, keywordIndex - 1);
        }

        private void KeywordAddFormClosing(object sender, EventArgs e)
        {
            if (!((KeywordAddForm)sender).SkipValue())
            {
                this.Controls["DynamicTestStep" + implementedKeyword + "Name"].Text = Keywords[implementedKeyword - 1].GetKeywordName().Trim();
                this.Controls["DynamicTestStep" + implementedKeyword + "AddImplementation"].Text = "Edit implementation";
                FormControls.AddControl("Label", "DynamicTestStep" + implementedKeyword + "ImplementationPanel",
                    new System.Drawing.Point(448 - this.HorizontalScroll.Value, 140 + (implementedKeyword - 1) * 30 - this.VerticalScroll.Value),
                    new System.Drawing.Size(20, 20),
                    "✔",
                    System.Drawing.Color.Green,
                    null,
                    this);
            }

            //Adds file path + name to the Files And Folder structure for use in the drop down lists when chosing output file
            FilesAndFolderStructure.AddFile(Keywords[implementedKeyword - 1].GetOutputFilePath());
            if (Keywords[implementedKeyword - 1].GetKeywordKeywords() != null)
                foreach (Keyword key in Keywords[implementedKeyword - 1].GetKeywordKeywords())
                    RobotFileHandler.AddFilesFromKeywords(key);
        }

        private void AddChangesToTestCases()
        {
            if (ApplicationMain.TestCases[index].GetTestSteps() != null && ApplicationMain.TestCases[index].GetTestSteps().Count > 0)
                for (int counter = 1; counter <= ApplicationMain.TestCases[index].GetTestSteps().Count; counter++)
                    Keywords[counter-1].SetKeywordName("\t" + ((TextBox) Controls["DynamicTestStep" + counter + "Name"]).Text.Trim());

            string finalPath = FilesAndFolderStructure.GetFolder();
            if (!TestCaseOutputFile.Text.StartsWith("\\"))
                finalPath = finalPath + TestCaseOutputFile.Text;
            else
                finalPath = finalPath.Trim('\\') + TestCaseOutputFile.Text;

            ApplicationMain.TestCases[index] = new TestCase(TestCaseName.Text.Trim(),
                "\t[Documentation]  " + TestCaseDocumentation.Text.Trim(),
                "\t[Tags]  " + TestCaseTags.Text.Trim(),
                Keywords,
                finalPath);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
