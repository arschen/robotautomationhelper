using RobotAutomationHelper.Forms;
using RobotAutomationHelper.Scripts;
using RobotAutomationHelper.Scripts.CustomControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper
{
    internal partial class KeywordAddForm : BaseKeywordAddForm
    {
        // keywords of the parent
        private List<Keyword> ParentKeywords;
        //keywords in this form
        private List<Param> ThisKeywordParams = new List<Param>();

        internal KeywordAddForm(List<Keyword> parentKeywords, BaseKeywordAddForm parent) : base(parent)
        {
            InitializeComponent();
            initialYValue = 185;
            FormType = FormType.Keyword;
            ParentKeywords = parentKeywords;
            UpdateOutputFileSuggestions(OutputFile, FormType);
            ActiveControl = KeywordNameLabel;
        }

        // when save is pressed add all currently listed keywords to the Keywords List and then AddChangesToKeyword
        private void Save_Click(object sender, EventArgs e)
        {
            if (!IsKeywordPresentInFilesOrMemoryTree())
            {
                AddCurrentKeywordsToKeywordsList(sender, e);
                SaveChangesToKeyword(true);
                Close();
            }
            else
            {
                if (presentInRobotFile)
                {
                    if (!ParentKeywords[ImplementationIndexFromTheParent].ToWrite)
                    {
                        DialogResult result = MessageBox.Show("Overwrite existing keyword in the output file?",
                            "Alert",
                            MessageBoxButtons.YesNo);
                        if (result.Equals(DialogResult.Yes))
                        {
                            AddCurrentKeywordsToKeywordsList(sender, e);
                            SaveChangesToKeyword(true);
                            ParentKeywords[ImplementationIndexFromTheParent].ToWrite = true;
                            Close();
                        }
                        else
                            ParentKeywords[ImplementationIndexFromTheParent].ToWrite = false;
                    }
                    else
                    {
                        AddCurrentKeywordsToKeywordsList(sender, e);
                        SaveChangesToKeyword(true);
                        ParentKeywords[ImplementationIndexFromTheParent].ToWrite = true;
                        Close();
                    }
                }
                else
                {
                    DialogResult result = MessageBox.Show("This action will affect other test cases / keywords that are using this one \n" + memoryPath,
                        "Alert",
                        MessageBoxButtons.YesNo);
                    if (result.Equals(DialogResult.Yes))
                    {
                        AddCurrentKeywordsToKeywordsList(sender, e);
                        SaveChangesToKeyword(true);
                        ParentKeywords[ImplementationIndexFromTheParent].ToWrite = true;
                        TestCasesListOperations.OverwriteOccurrencesInKeywordTree(KeywordName.Text,
                            FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Resources),
                            ParentKeywords[ImplementationIndexFromTheParent]);
                        Close();
                    }
                    else
                        ParentKeywords[ImplementationIndexFromTheParent].ToWrite = false;
                }
            }
        }

        private void Skip_Click(object sender, EventArgs e)
        {
            skip = true;
            Close();
        }

        internal void ShowKeywordContent(Keyword keyword, int keywordIndex)
        {
            //index variable gets the keyword index coming from the parent form
            ImplementationIndexFromTheParent = keywordIndex;

            if (keyword.Name != null)
                KeywordName.Text = keyword.Name.Trim();
            if (keyword.Documentation != null)
                KeywordDocumentation.Text = keyword.Documentation.Replace("[Documentation]", "").Trim();
            if (keyword.OutputFilePath != null || !keyword.OutputFilePath.Equals(""))
                OutputFile.Text = keyword.OutputFilePath.Replace(FilesAndFolderStructure.GetFolder(FolderType.Resources), "\\");
            if (keyword.Arguments != null)
                KeywordArguments.Text = keyword.Arguments.Replace("[Arguments]", "").Trim();

            IsKeywordPresentInFilesOrMemoryTree();
            ThisFormKeywords = keyword.Keywords;

            NumberOfKeywordsInThisForm = 0;

            if (ThisFormKeywords != null && ThisFormKeywords.Count != 0)
            {
                // adds the keywords in the form
                foreach (Keyword testStep in ThisFormKeywords)
                {
                    AddKeywordField(testStep, NumberOfKeywordsInThisForm + 1, false);
                    NumberOfKeywordsInThisForm++;
                }
            }
            else
            {
                // add a single keyword field if no keywords are available
                ThisFormKeywords = new List<Keyword>
                {
                    new Keyword("New Keyword", ParentKeywords[ImplementationIndexFromTheParent].OutputFilePath, keyword, true)
                };
                AddKeywordField(ThisFormKeywords[0], NumberOfKeywordsInThisForm + 1, false);
                NumberOfKeywordsInThisForm++;
                FilesAndFolderStructure.AddFileToSavedFiles(ThisFormKeywords[0].OutputFilePath);
            }

            UpdateNamesListAndUpdateStateOfSave();

            // show the form dialog
            StartPosition = FormStartPosition.Manual;
            var dialogResult = ShowDialog();
        }

        // adds all field data to parentKeyword or testcaseaddform if not nested
        private void SaveChangesToKeyword(bool save)
        {
            string finalPath = FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Resources);

            List<string> args = StringAndListOperations.ReturnListOfArgs(KeywordArguments.Text);

            if (args != null)
                for (int i = 0; i < args.Count; i++)
                    if (ParentKeywords[ImplementationIndexFromTheParent].Params != null
                        && (ParentKeywords[ImplementationIndexFromTheParent].Params.Count > 0))
                    {
                        bool foundMatch = false;
                        foreach (Param parentParam in ParentKeywords[ImplementationIndexFromTheParent].Params)
                            if (parentParam.Name.Equals(args[i]))
                            {
                                ThisKeywordParams.Add(parentParam);
                                foundMatch = true;
                                break;
                            }
                        if (!foundMatch)
                            ThisKeywordParams.Add(new Param(args[i], ""));
                    }
                    else
                        ThisKeywordParams.Add(new Param(args[i], ""));

            bool addToSuggestions = false;

            if (ParentKeywords[ImplementationIndexFromTheParent].SuggestionIndex == -1)
                addToSuggestions = true;

            ParentKeywords[ImplementationIndexFromTheParent] = new Keyword(KeywordName.Text.Trim(),
            "[Documentation]  " + KeywordDocumentation.Text.Trim(),
            ThisFormKeywords,
            "[Arguments]  " + KeywordArguments.Text.Trim(),
            ThisKeywordParams,
            finalPath,
            save,
            KeywordType.CUSTOM,
            ParentKeywords[ImplementationIndexFromTheParent].SuggestionIndex,
            "Custom",
            ParentKeywords[ImplementationIndexFromTheParent].Parent,
            true);

            if (addToSuggestions)
            {
                ParentKeywords[ImplementationIndexFromTheParent].SuggestionIndex = SuggestionsClass.GetCustomLibKeywords().Count;
                Keyword temp = new Keyword(ParentKeywords[ImplementationIndexFromTheParent].Parent);
                temp.CopyKeyword(ParentKeywords[ImplementationIndexFromTheParent]); //CopyKeyword
                SuggestionsClass.GetCustomLibKeywords().Add(temp);
            }
            else
                SuggestionsClass.GetCustomLibKeywords()[ParentKeywords[ImplementationIndexFromTheParent].SuggestionIndex].CopyKeyword(ParentKeywords[ImplementationIndexFromTheParent]); //CopyKeyword
        }

        // adding argument to the KeywordArguments.Text
        private void AddArgument_Click(object sender, EventArgs e)
        {
            string arg = "";
            bool add = false;
            switch (ArgumentType.Text)
            {
                case "Scalar": arg = "${"; break;
                case "Dictionary": arg = "&{"; break;
                case "List": arg = "@{"; break;
                default: break;
            }
            if (!arg.Equals(""))
                if (!ArgumentName.Text.Trim().Equals(""))
                {
                    arg += ArgumentName.Text + "}";
                    add = true;
                }
            if (add && !KeywordArguments.Text.Contains(arg))
                if (KeywordArguments.Text.Trim().Equals(""))
                    KeywordArguments.Text += arg;
                else
                    KeywordArguments.Text += "  " + arg;
        }

        // removing argument from the KeywordArguments.Text
        private void RemoveArgument_Click(object sender, EventArgs e)
        {
            string arg = "";
            bool remove = false;
            switch (ArgumentType.Text)
            {
                case "Scalar": arg = "${"; break;
                case "Dictionary": arg = "&{"; break;
                case "List": arg = "@{"; break;
                default: break;
            }
            if (!arg.Equals(""))
                if (!ArgumentName.Text.Trim().Equals(""))
                {
                    arg += ArgumentName.Text + "}";
                    remove = true;
                }
            if (remove && KeywordArguments.Text.Contains(arg))
            {
                if (KeywordArguments.Text.IndexOf(arg) != 0)
                    arg = "  " + arg;
                KeywordArguments.Text = KeywordArguments.Text.Replace(arg, "").Trim();
            } 
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
            presentInRobotFile = false;
            memoryPath = TestCasesListOperations.IsPresentInTheKeywordTree(KeywordName.Text,
                FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Resources),
                ParentKeywords[ImplementationIndexFromTheParent]);

            if (!memoryPath.Equals(""))
                KeywordName.ForeColor = Color.DarkOrange;
            else
            {
                if (RobotFileHandler.LocationOfTestCaseOrKeywordInFile(FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text, FolderType.Resources)
                    , KeywordName.Text, FormType.Keyword) != -1)
                {
                    KeywordName.ForeColor = Color.DarkOrange;
                    presentInRobotFile = true;
                }
                else
                {
                    KeywordName.ForeColor = Color.Black;
                    return false;
                }
            }
            return true;
        }

        internal void UpdateNamesListAndUpdateStateOfSave()
        {
            List<string> namesList = new List<string>
            {
                KeywordName.Text
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
