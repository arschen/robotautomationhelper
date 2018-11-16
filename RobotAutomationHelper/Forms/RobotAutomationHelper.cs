using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using RobotAutomationHelper.Scripts.Objects;
using RobotAutomationHelper.Scripts.Static;
using RobotAutomationHelper.Scripts.Static.Readers;
using RobotAutomationHelper.Scripts.Static.Writers;

namespace RobotAutomationHelper.Forms
{
    internal partial class RobotAutomationHelper : BaseKeywordAddForm
    {
        internal static List<TestCase> TestCases = new List<TestCase>();
        internal static List<SuiteSettings> SuiteSettingsList = new List<SuiteSettings>();
        internal static List<Variables> GlobalVariables = new List<Variables>();

        internal static bool Log = false;

        private static int _numberOfTestCases = 0;
        private object _realSender;
        // index of the test case to be implemented
        private int _indexOfTheTestCaseToBeImplemented;
        private int _selectedIndex = -1;
        private string _currentFilename;

        internal RobotAutomationHelper(BaseKeywordAddForm parent) : base(parent)
        {
            InitializeComponent();
            ActiveControl = TestCaseNameLabel;
            InitialYValue = 90;
            settingsToolStripMenuItem.Visible = false;
            librariesToolStripMenuItem.Visible = false;
            variablesToolStripMenuItem.Visible = false;
            runOptionsToolStripMenuItem.Visible = false;
            OutputLabel.Visible = false;
            OutputFile.Visible = false;
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
        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                BrowseFolderButtonOpenExcel();
            }
        }

        private void BrowseFolderButtonNewProject()
        {
            FormSetup(folderBrowserDialog3.SelectedPath);
            TestCases = new List<TestCase>();
            CheckForExistingCodeAndShowAlert();
            AddTestCaseToFormAndShow(false);
        }

        private void BrowseFolderButtonOpenExcel()
        {
            FormSetup(folderBrowserDialog1.SelectedPath);
            TestCases = ReadExcel.ReadAllTestCasesFromExcel(openFileDialog.FileName);
            TestCases.Sort();
            CheckForExistingCodeAndShowAlert();
            AddTestCaseToFormAndShow(false);
        }

        private void BrowseFolderButtonExistingProject()
        {
            FormSetup(folderBrowserDialog2.SelectedPath);

            TestCases = ReadRobotFiles.ReadAllTests();
            if (TestCases.Count != 0)
            {
                SuiteSettingsList = ReadRobotFiles.ReadAllSettings();
                GlobalVariables = ReadRobotFiles.ReadAllVariables();

                if (SuiteSettingsList.Count != 0)
                {
                    foreach (var tempKeyword in SuiteSettingsList)
                    {
                        if (tempKeyword.GetKeywords().Count == 0) continue;
                        foreach (var keyword in tempKeyword.GetKeywords())
                            KeywordToSuggestions(keyword);
                    }
                }

                foreach (var testCase in TestCases)
                {
                    if (testCase.Steps == null) continue;
                    foreach (var tempKeyword in testCase.Steps)
                    {
                        KeywordToSuggestions(tempKeyword);
                    }
                }

                TestCases.Sort();

                SuggestionsClass.UpdateSuggestionsToIncludes(TestCases, SuiteSettingsList);
                AddTestCaseToFormAndShow(false);
            }
            else
            {
                MessageBox.Show(@"No test cases in the selected folder!",
                    @"Alert",
                    MessageBoxButtons.OK);
            }
        }

        internal void AddTestCaseToFormAndShow(bool fileValueChanged)
        {
            _selectedIndex = fileValueChanged? OutputFile.Items.IndexOf(_currentFilename) : 0;
            AddTestCasesToMainForm(OutputFile.Items.Count == 0 ? "": OutputFile.Items[_selectedIndex].ToString());
            ShowTestCasePanels();
        }

        internal void FormSetup(string path)
        {
            Cache.ClearCache();
            SuggestionsClass.PopulateSuggestionsList();
            ClearDynamicElements();
            settingsToolStripMenuItem.Visible = true;
            librariesToolStripMenuItem.Visible = true;
            variablesToolStripMenuItem.Visible = true;
            runOptionsToolStripMenuItem.Visible = true;
            OutputLabel.Visible = true;
            OutputFile.Visible = true;
            SetStructureFolder(path);
        }

        internal void CheckForExistingCodeAndShowAlert()
        {
            var projectTestCases = ReadRobotFiles.ReadAllTests();
            SuiteSettingsList = ReadRobotFiles.ReadAllSettings();
            GlobalVariables = ReadRobotFiles.ReadAllVariables();

            if (projectTestCases.Count == 0) return;
            var result = MessageBox.Show(@"Use existing Test Cases in project folder?",
                @"Alert",
                MessageBoxButtons.YesNo);
            if (!result.Equals(DialogResult.Yes)) return;
            TestCases = projectTestCases;

            foreach (var testCase in projectTestCases)
            {
                if (testCase.Steps == null) continue;
                foreach (var keyword in testCase.Steps)
                    KeywordToSuggestions(keyword);
            }

            if (SuiteSettingsList.Count != 0) return;
            foreach (var suiteSetting in SuiteSettingsList)
            {
                if (suiteSetting.GetKeywords().Count == 0) continue;
                foreach (Keyword keyword in suiteSetting.GetKeywords())
                    KeywordToSuggestions(keyword);
            }

            SuggestionsClass.UpdateSuggestionsToIncludes(TestCases, SuiteSettingsList);
        }

        private static void KeywordToSuggestions(Keyword tempKeyword)
        {
            if (tempKeyword.SuggestionIndex == -1 && !tempKeyword.OutputFilePath.Equals("") && !StringAndListOperations.StartsWithVariable(tempKeyword.Name))
            {
                var toAdd = true;
                foreach (var lib in SuggestionsClass.Suggestions)
                    if (lib.ToInclude)
                        foreach (var suggested in lib.LibKeywords)
                        {
                            if (suggested.OutputFilePath.Equals("")) continue;
                            if (!suggested.Name.Equals(tempKeyword.Name) ||
                                !suggested.OutputFilePath.Equals(tempKeyword.OutputFilePath)) continue;
                            toAdd = false;
                            break;
                        }
                        if (toAdd)
                        {
                            tempKeyword.SuggestionIndex = SuggestionsClass.GetLibKeywordsByName("Custom").Count;
                            Keyword temp = new Keyword(null);
                            temp.CopyKeyword(tempKeyword);
                            if (temp.Params != null && temp.Params.Count > 0)
                                foreach (Param param in temp.Params)
                                    if (!temp.Arguments.ToLower().Contains(param.Name + @"=" + param.Value))
                                        param.Value = "";

                            SuggestionsClass.GetLibKeywordsByName("Custom").Add(temp);
                        }
            }
            if (tempKeyword.Keywords != null)
                foreach (var nestedKeyword in tempKeyword.Keywords)
                {
                    KeywordToSuggestions(nestedKeyword);
                }

            if (tempKeyword.ForLoopKeywords == null) return;
            {
                foreach (var nestedKeyword in tempKeyword.ForLoopKeywords)
                {
                    KeywordToSuggestions(nestedKeyword);
                }
            }
        }

        //Clear dynamic elements when new file is opened
        private void ClearDynamicElements()
        {
            var cleared = false;
            while (!cleared)
            {
                foreach (Control tempControl in Controls)
                    if (tempControl.Name.ToLower().StartsWith("dynamictest"))
                        FormControls.RemoveControlByKey(tempControl.Name, Controls);

                cleared = true;
                foreach (Control tempControl in Controls)
                {
                    if (!tempControl.Name.ToLower().StartsWith("dynamictest")) continue;
                    cleared = false;
                    break;
                }
            }
        }

        private void AddTestCasesToMainForm(string fileName)
        {
            Console.WriteLine("AddTestCasesToMainForm: " + fileName);
            UpdateOutputFileSuggestions(OutputFile, FormType.Test);
            OutputFile.SelectedIndex = _selectedIndex;
            var testCasesCounter = 1;
            _numberOfTestCases = 0;
            if (TestCases != null && TestCases.Count != 0)
                for (var i = 0; i < TestCases.Count; i++)
                {
                    if (!FilesAndFolderStructure.ConcatFileNameToFolder(fileName, FolderType.Tests).ToLower().Equals(TestCases[i].OutputFilePath.ToLower())) continue;
                    AddTestCaseField(TestCases[i], testCasesCounter, i + 1);
                    testCasesCounter++;
                    _numberOfTestCases++;
                }
            else
            {
                TestCases.Add(new TestCase("New Test Case", FilesAndFolderStructure.GetFolder(FolderType.Tests) + "Auto.robot"));
                AddTestCaseField(TestCases[0], testCasesCounter, 1);
                _numberOfTestCases = 1;
                FilesAndFolderStructure.AddFileToSavedFiles(TestCases[0].OutputFilePath);
            }
        }

        private void AddTestCaseField(TestCase testCase, int testCasesCounter, int index)
        {
            FormControls.AddControl("TextBox", "DynamicTest" + index + "Name",
                testCasesCounter,
                new Point(30 - HorizontalScroll.Value, InitialYValue + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                new Size(280, 20),
                testCase.Name.Trim(),
                Color.Black,
                null,
                this);
            FormControls.AddControl("Label", "DynamicTest" + index + "Label",
                testCasesCounter,
                new Point(10 - HorizontalScroll.Value, InitialYValue + 3 + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                new Size(20, 20),
                testCasesCounter + ".",
                Color.Black,
                null,
                this);
            FormControls.AddControl("CheckBox", "DynamicTest" + index + "CheckBox",
                testCasesCounter,
                new Point(325 - HorizontalScroll.Value, InitialYValue + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                new Size(20, 20),
                "Add",
                Color.Black,
                null,
                this);

            var implementationText = TestCases[testCasesCounter - 1].Implemented? "Edit Implementation" : "Add Implementation";
            FormControls.AddControl("Button", "DynamicTest" + index + "AddImplementation",
                testCasesCounter,
                new Point(345 - HorizontalScroll.Value, InitialYValue + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                new Size(120, 20),
                implementationText,
                Color.Black,
                InstantiateAddTestCaseForm,
                this);

            FormControls.AddControl("Button", "DynamicTest" + index + "AddTestCase",
                testCasesCounter,
                new Point(470 - HorizontalScroll.Value, InitialYValue + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                new Size(20, 20),
                "+",
                Color.Black,
                InstantiateNameAndOutputForm,
                this);
            FormControls.AddControl("Button", "DynamicTest" + index + "RemoveTestCase",
                testCasesCounter,
                new Point(490 - HorizontalScroll.Value, InitialYValue + (testCasesCounter - 1) * 25 - VerticalScroll.Value),
                new Size(20, 20),
                "-",
                Color.Black,
                RemoveTestCaseFromProject,
                this);
        }

        private static void SetStructureFolder(string outputFolder)
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
            var testIndex = int.Parse(((Button)sender).Name.Replace("AddImplementation", "").Replace("DynamicTest", ""));
            _indexOfTheTestCaseToBeImplemented = testIndex;
            var testCase = TestCases[testIndex - 1];
            testCase.Name = Controls["DynamicTest" + testIndex + "Name"].Text;
            var testCaseAddForm = new TestCaseAddForm(this);
            testCaseAddForm.FormClosing += UpdateThisFormTestCaseAddFormClosing;
            testCaseAddForm.ShowTestCaseContent(testCase, testIndex - 1);
        }

        private void UpdateThisFormTestCaseAddFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!((TestCaseAddForm)sender).SkipForm)
            {
                FilesAndFolderStructure.AddImplementedTestCasesFilesToSavedFiles(TestCases, _indexOfTheTestCaseToBeImplemented);
                _currentFilename = TestCases[_indexOfTheTestCaseToBeImplemented - 1].OutputFilePath.Replace(FilesAndFolderStructure.GetFolder(FolderType.Tests), "");
                ClearDynamicElements();
                AddTestCaseToFormAndShow(false);
            }
        }

        internal void ShowTestCasePanels()
        {
            IndexLabel.Visible = true;
            TestCaseNameLabel.Visible = true;
            AddLabel.Visible = true;
        }

        private void SaveToRobotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WriteToRobot.Includes = new List<Includes>();
            //Cleanup
            FilesAndFolderStructure.DeleteAllFiles();

            TestCases.Sort();
            foreach (var testCase in TestCases)
                WriteToRobot.AddTestCaseToRobot(testCase);

            Console.WriteLine(@"WriteSuiteSettingsListToRobot ===============================");
            WriteToRobot.WriteSuiteSettingsListToRobot();
            Console.WriteLine(@"WriteIncludesToRobotFiles ===================================");
            WriteToRobot.WriteIncludesToRobotFiles();
            Console.WriteLine(@"WriteVariablesToRobotFiles ===================================");
            WriteToRobot.WriteVariablesToRobotFiles();

            foreach (var fileName in FilesAndFolderStructure.GetShortSavedFiles(FolderType.Root))
                RobotFileHandler.TrimFile(FilesAndFolderStructure.ConcatFileNameToFolder(fileName, FolderType.Root));
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FilesAndFolderStructure.GetShortSavedFiles(FolderType.Root) != null && FilesAndFolderStructure.GetShortSavedFiles(FolderType.Root).Count > 0)
                InstantiateSettingsAddForm(sender, e);
            else
            {
                MessageBox.Show(@"You haven't saved any keywords or test cases to files yet.",
                    @"Alert",
                    MessageBoxButtons.OK);
            }
        }

        private void VariablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FilesAndFolderStructure.GetShortSavedFiles(FolderType.Root) != null && FilesAndFolderStructure.GetShortSavedFiles(FolderType.Root).Count > 0)
                InstantiateVariablesAddForm(sender, e);
            else
            {
                MessageBox.Show(@"You haven't saved any keywords or test cases to files yet.",
                    @"Alert",
                    MessageBoxButtons.OK);
            }
        }

        private void RunOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InstantiateRunOptionsForm(sender, e);
        }

        private void SuggestionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SuggestionsClass.Suggestions != null && SuggestionsClass.Suggestions.Count > 0)
                InstantiateLibrariesAddForm(sender, e);
            else
            {
                MessageBox.Show(@"No libraries loaded.",
                    @"Alert",
                    MessageBoxButtons.OK);
            }
        }

        private static void RunCom(string command)
        {
            var cmd = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    Arguments = "/c" + command,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false,
                    CreateNoWindow = false,
                    UseShellExecute = false
                }
            };
            cmd.Start();
        }

        private void SaveToRobotAndRunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveToRobotToolStripMenuItem_Click(sender, e);
            RunCom("cd " + FilesAndFolderStructure.GetFolder(FolderType.Root) + "&robot " + RunOptionsForm.RunOptionsString + " tests");
        }

        internal new void InstantiateNameAndOutputForm(object sender, EventArgs e)
        {
            _realSender = sender;
            if (Log) Console.WriteLine(@"InstantiateParamsAddForm " + ((Button)sender).Name);
            var formType = Name.Contains("RobotAutomationHelper") ? FormType.Test : FormType.Keyword;

            var nameAndOutputForm = new NameAndOutputForm(formType, this, null);
            nameAndOutputForm.FormClosing += UpdateAfterClosingNameAndOutputForm;
            nameAndOutputForm.ShowTestCaseContent();
        }

        private void UpdateAfterClosingNameAndOutputForm(object sender, EventArgs e)
        {
            if (NameAndOutputToTestCaseFormCommunication.Save)
                AddTestCaseToProject(_realSender, e);
        }

        internal void AddTestCaseToProject(object sender, EventArgs e)
        {
            TestCases.Add(new TestCase(NameAndOutputToTestCaseFormCommunication.Name, NameAndOutputToTestCaseFormCommunication.OutputFile));
            _currentFilename = TestCases[TestCases.Count - 1].OutputFilePath.Replace(FilesAndFolderStructure.GetFolder(FolderType.Tests), "");
            Console.WriteLine("After Name and output form: " + _currentFilename);
            TestCases.Sort();
            ClearDynamicElements();
            AddTestCaseToFormAndShow(false);
        }

        internal void RemoveTestCaseFromProject(object sender, EventArgs e)
        {
            if (_numberOfTestCases <= 1) return;
            var testCaseIndex = int.Parse(((Button)sender).Name.Replace("DynamicTest", "").Replace("RemoveTestCase", ""));

            TestCases.RemoveAt(testCaseIndex - 1);
            ClearDynamicElements();
            AddTestCaseToFormAndShow(true);
        }

        private void OutputFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OutputFile.SelectedIndex == -1 || _selectedIndex == OutputFile.SelectedIndex) return;
            _selectedIndex = OutputFile.SelectedIndex;
            _currentFilename = OutputFile.Text;
            Console.WriteLine("Index changed: " + _currentFilename);
            ClearDynamicElements();
            AddTestCaseToFormAndShow(true);
        }
    }
}