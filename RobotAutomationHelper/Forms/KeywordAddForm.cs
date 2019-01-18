using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RobotAutomationHelper.Scripts.CustomControls;
using RobotAutomationHelper.Scripts.Objects;
using RobotAutomationHelper.Scripts.Static;

namespace RobotAutomationHelper.Forms
{
    public partial class KeywordAddForm : BaseKeywordAddForm
    {
        // keywords of the parent
        private readonly List<Keyword> _parentKeywords;
        //keywords in this form
        private readonly List<Param> _thisKeywordParams = new List<Param>();

        public KeywordAddForm(List<Keyword> parentKeywords, BaseKeywordAddForm parent) : base(parent)
        {
            InitializeComponent();
            InitialYValue = 185;
            FormType = FormType.Keyword;
            _parentKeywords = parentKeywords;
            UpdateOutputFileSuggestions(OutputFile, FormType);
            ActiveControl = KeywordNameLabel;
        }

        // when save is pressed add all currently listed keywords to the Keywords List and then AddChangesToKeyword
        private void Save_Click(object sender, EventArgs e)
        {
            if (!IsKeywordPresentInFilesOrMemoryTree())
            {
                AddCurrentKeywordsToKeywordsList(sender, e);
                SaveChangesToKeyword();
                Close();
            }
            else
            {
                var result = MessageBox.Show(@"This action will affect other test cases / keywords that are using this one " + MemoryPath,
                    @"Alert",
                    MessageBoxButtons.YesNo);
                if (!result.Equals(DialogResult.Yes)) return;
                AddCurrentKeywordsToKeywordsList(sender, e);
                SaveChangesToKeyword();
                TestCasesListOperations.OverwriteOccurrencesInKeywordTree(KeywordName.Text,
                    FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Resources),
                    _parentKeywords[ImplementationIndexFromTheParent]);
                Close();
            }
        }

        private void Skip_Click(object sender, EventArgs e)
        {
            SkipForm = true;
            Close();
        }

        public void ShowKeywordContent(Keyword keyword, int keywordIndex)
        {
            //index variable gets the keyword index coming from the parent form
            ImplementationIndexFromTheParent = keywordIndex;

            if (keyword.Name != null)
                KeywordName.Text = keyword.Name.Trim();
            if (keyword.Documentation != null)
                KeywordDocumentation.Text = keyword.Documentation.Replace("[Documentation]", "").Trim();
            if (keyword.OutputFilePath != null)
                if (!keyword.OutputFilePath.Equals(""))
                    OutputFile.Text = keyword.OutputFilePath.Replace(FilesAndFolderStructure.GetFolder(FolderType.Resources), "\\");
            if (keyword.Arguments != null)
                KeywordArguments.Text = keyword.Arguments.Replace("[Arguments]", "").Trim();

            IsKeywordPresentInFilesOrMemoryTree();
            ThisFormKeywords = keyword.Keywords;

            NumberOfKeywordsInThisForm = 0;

            if (ThisFormKeywords != null && ThisFormKeywords.Count != 0)
            {
                // adds the keywords in the form
                foreach (var testStep in ThisFormKeywords)
                {
                    AddKeywordField(testStep, NumberOfKeywordsInThisForm + 1, false, true);
                    NumberOfKeywordsInThisForm++;
                }
            }
            else
            {
                // add a single keyword field if no keywords are available
                ThisFormKeywords = new List<Keyword>
                {
                    new Keyword("New Keyword", _parentKeywords[ImplementationIndexFromTheParent].OutputFilePath, keyword)
                };
                AddKeywordField(ThisFormKeywords[0], NumberOfKeywordsInThisForm + 1, true, true);
                NumberOfKeywordsInThisForm++;
                FilesAndFolderStructure.AddFileToSavedFiles(ThisFormKeywords[0].OutputFilePath);
            }

            UpdateNamesListAndUpdateStateOfSave();

            // show the form dialog
            StartPosition = FormStartPosition.Manual;
            ShowDialog();
        }

        // adds all field data to parentKeyword or testCaseAddForm if not nested
        private void SaveChangesToKeyword()
        {
            var finalPath = FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Resources);

            var args = StringAndListOperations.ReturnListOfArgs(KeywordArguments.Text);

            if (args != null)
                foreach (var arg in args)
                    if (_parentKeywords[ImplementationIndexFromTheParent].Params != null
                        && (_parentKeywords[ImplementationIndexFromTheParent].Params.Count > 0))
                    {
                        var foundMatch = false;
                        foreach (var parentParam in _parentKeywords[ImplementationIndexFromTheParent].Params)
                            if (parentParam.Name.Equals(arg))
                            {
                                _thisKeywordParams.Add(parentParam);
                                foundMatch = true;
                                break;
                            }
                        if (!foundMatch)
                            _thisKeywordParams.Add(new Param(arg, ""));
                    }
                    else
                        _thisKeywordParams.Add(new Param(arg, ""));

            var addToSuggestions = _parentKeywords[ImplementationIndexFromTheParent].SuggestionIndex == -1;

            _parentKeywords[ImplementationIndexFromTheParent] = new Keyword(KeywordName.Text.Trim(),
            "[Documentation]  " + KeywordDocumentation.Text.Trim(),
            ThisFormKeywords,
            "[Arguments]  " + KeywordArguments.Text.Trim(),
            _thisKeywordParams,
            finalPath,
            KeywordType.Custom,
            _parentKeywords[ImplementationIndexFromTheParent].SuggestionIndex,
            "Custom",
            _parentKeywords[ImplementationIndexFromTheParent].Parent,
            _parentKeywords[ImplementationIndexFromTheParent].IncludeImportFile);

            if (addToSuggestions)
            {
                _parentKeywords[ImplementationIndexFromTheParent].SuggestionIndex = SuggestionsClass.GetLibKeywordsByName("Custom").Count;
                var temp = new Keyword(_parentKeywords[ImplementationIndexFromTheParent].Parent);
                temp.CopyKeyword(_parentKeywords[ImplementationIndexFromTheParent]); //CopyKeyword
                SuggestionsClass.GetLibKeywordsByName("Custom").Add(temp);
            }
            else
                SuggestionsClass.GetLibKeywordsByName("Custom")[_parentKeywords[ImplementationIndexFromTheParent].SuggestionIndex].CopyKeyword(_parentKeywords[ImplementationIndexFromTheParent]); //CopyKeyword
        }

        // adding argument to the KeywordArguments.Text
        private void AddArgument_Click(object sender, EventArgs e)
        {
            var arg = "";
            var add = false;
            switch (ArgumentType.Text)
            {
                case "Scalar": arg = "${"; break;
                case "Dictionary": arg = "&{"; break;
                case "List": arg = "@{"; break;
            }
            if (!arg.Equals(""))
                if (!ArgumentName.Text.Trim().Equals(""))
                {
                    arg += ArgumentName.Text + "}";
                    add = true;
                }

            if (!add || KeywordArguments.Text.Contains(arg)) return;
            if (KeywordArguments.Text.Trim().Equals(""))
                KeywordArguments.Text += arg;
            else
                KeywordArguments.Text += @"  " + arg;
        }

        // removing argument from the KeywordArguments.Text
        private void RemoveArgument_Click(object sender, EventArgs e)
        {
            var arg = "";
            var remove = false;
            switch (ArgumentType.Text)
            {
                case "Scalar": arg = "${"; break;
                case "Dictionary": arg = "&{"; break;
                case "List": arg = "@{"; break;
            }
            if (!arg.Equals(""))
                if (!ArgumentName.Text.Trim().Equals(""))
                {
                    arg += ArgumentName.Text + "}";
                    remove = true;
                }

            if (!remove || !KeywordArguments.Text.Contains(arg)) return;
            if (KeywordArguments.Text.IndexOf(arg, StringComparison.Ordinal) != 0)
                arg = "  " + arg;
            KeywordArguments.Text = KeywordArguments.Text.Replace(arg, "").Trim();
        }

        private void KeywordName_TextChanged(object sender, EventArgs e)
        {
            UpdateNamesListAndUpdateStateOfSave();
            IsKeywordPresentInFilesOrMemoryTree();
        }

        private void KeywordOutputFile_TextChanged(object sender, EventArgs e)
        {
            UpdateNamesListAndUpdateStateOfSave();
            IsKeywordPresentInFilesOrMemoryTree();
        }

        private bool IsKeywordPresentInFilesOrMemoryTree()
        {
            MemoryPath = TestCasesListOperations.IsPresentInTheKeywordTree(KeywordName.Text,
                FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Resources),
                _parentKeywords[ImplementationIndexFromTheParent]);

            if (MemoryPath.Equals("")) return false;
            KeywordName.ForeColor = Color.DarkOrange;
            return true;
        }

        public void UpdateNamesListAndUpdateStateOfSave()
        {
            var namesList = new List<string>
            {
                KeywordName.Text
            };
            for (var i = 1; i <= NumberOfKeywordsInThisForm; i++)
            {
                if (Controls.Find("DynamicStep" + i + "Name", false).Length > 0)
                    namesList.Add(Controls["DynamicStep" + i + "Name"].Text);
            }
            ((ButtonWithToolTip) Save).UpdateState(namesList, OutputFile.Text);
        }
    }
}
