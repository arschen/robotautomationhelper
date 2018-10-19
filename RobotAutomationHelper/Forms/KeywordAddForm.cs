using RobotAutomationHelper.Forms;
using RobotAutomationHelper.Scripts;
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
        internal int NumberOfKeywordsInThisKeyword { get; set; }
        //keywords in this form
        private List<Param> ThisKeywordParams = new List<Param>();

        internal KeywordAddForm(List<Keyword> parentKeywords)
        {
            InitializeComponent();
            initialYValue = 185;
            ParentKeywords = parentKeywords;
            FormControls.UpdateOutputFileSuggestions(OutputFile);
            ActiveControl = KeywordNameLabel;
            IsKeyword = true;
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
                    DialogResult result = MessageBox.Show("Overwrite existing keyword in the output file?",
                        "Alert",
                        MessageBoxButtons.YesNo);
                    if (result.Equals(DialogResult.Yes))
                    {
                        AddCurrentKeywordsToKeywordsList(sender, e);
                        SaveChangesToKeyword(true);
                        ParentKeywords[ImplementationIndexFromTheParent].Overwrite = true;
                        Close();
                    }
                    else
                        ParentKeywords[ImplementationIndexFromTheParent].Overwrite = false;
                }
                else
                {
                    DialogResult result = MessageBox.Show("Keyword with this name has already been implemented in the output file. \n" + memoryPath,
                        "Alert",
                        MessageBoxButtons.OK);
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
                OutputFile.Text = keyword.OutputFilePath.Replace(FilesAndFolderStructure.GetFolder(), "\\");
            if (keyword.Arguments != null)
                KeywordArguments.Text = keyword.Arguments.Replace("[Arguments]", "").Trim();
            IsKeywordPresentInFilesOrMemoryTree();
            ThisFormKeywords = keyword.Keywords;

            int keywordsCounter = 1;
            NumberOfKeywordsInThisKeyword = 0;

            if (ThisFormKeywords != null && ThisFormKeywords.Count != 0)
            {
                // adds the keywords in the form
                foreach (Keyword testStep in ThisFormKeywords)
                {
                    AddKeywordField(testStep, keywordsCounter);
                    keywordsCounter++;
                    NumberOfKeywordsInThisKeyword++;
                }
            }
            else
            {
                // add a single keyword field if no keywords are available
                ThisFormKeywords = new List<Keyword>
                {
                    new Keyword("New Keyword", ParentKeywords[ImplementationIndexFromTheParent].OutputFilePath)
                };
                AddKeywordField(ThisFormKeywords[0], keywordsCounter);
                NumberOfKeywordsInThisKeyword++;
            }

            // show the form dialog
            StartPosition = FormStartPosition.Manual;
            var dialogResult = ShowDialog();
        }

        // adds all field data to parentKeyword or testcaseaddform if not nested
        private void SaveChangesToKeyword(bool save)
        {
            string finalPath = FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text);

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
            ParentKeywords[ImplementationIndexFromTheParent].SuggestionIndex);

            if (addToSuggestions)
            {
                ParentKeywords[ImplementationIndexFromTheParent].SuggestionIndex = FormControls.Suggestions.Count;
                Keyword temp = new Keyword();
                temp.CopyKeyword(ParentKeywords[ImplementationIndexFromTheParent]); //CopyKeyword
                FormControls.Suggestions.Add(temp);
            }
            else
                FormControls.Suggestions[ParentKeywords[ImplementationIndexFromTheParent].SuggestionIndex].CopyKeyword(ParentKeywords[ImplementationIndexFromTheParent]); //CopyKeyword
        }

        //adds the list of keywords ( + unimplemented ones ) to a Keyword and returns it
        internal Keyword AddCurrentKeywordsToKeywordsList(object sender, EventArgs e)
        {
            string path = FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text);

            // if AddImplementation is pressed a new form should be opened which requires the keyword that it represents
            int keywordIndex = 0;
            if (((Button)sender).Name.Contains("DynamicStep") 
                && !((Button)sender).Name.Contains("Params")
                && !((Button)sender).Name.Contains("AddKeyword")
                && !((Button)sender).Name.Contains("RemoveKeyword"))
                keywordIndex = int.Parse(((Button)sender).Name.Replace("AddImplementation", "").Replace("DynamicStep", ""));
            else
                if (((Button)sender).Name.Contains("DynamicStep")
                && ((Button)sender).Name.Contains("Params"))
                keywordIndex = int.Parse(((Button)sender).Name.Replace("Params", "").Replace("DynamicStep", ""));

            // add to the global variable for the form that matches the index of the keyword to implement
            IndexOfTheKeywordToBeImplemented = keywordIndex;
            if (keywordIndex <= 0) keywordIndex = 1;

            if (ThisFormKeywords == null)
            {
                ThisFormKeywords = new List<Keyword>();
                for (int i = 1; i <= NumberOfKeywordsInThisKeyword; i++)
                    ThisFormKeywords.Add(new Keyword(Controls["DynamicStep" + i + "Name"].Text, path));
            }
            else
            {
                for (int i = ThisFormKeywords.Count + 1; i <= NumberOfKeywordsInThisKeyword; i++)
                    ThisFormKeywords.Add(new Keyword(Controls["DynamicStep" + i + "Name"].Text, path));
            }

            AssignThisKeywordNamesFromTextFields();

            return ThisFormKeywords[keywordIndex - 1];
        }

        private void AssignThisKeywordNamesFromTextFields()
        {
            for (int i = 1; i <= NumberOfKeywordsInThisKeyword; i++)
                if (Controls.Find("DynamicStep" + i + "Name", false).Length != 0)
                    ThisFormKeywords[i - 1].Name = Controls["DynamicStep" + i + "Name"].Text;
        }

        internal void RemoveKeywordFromThisKeyword(object sender, EventArgs e)
        {
            AssignThisKeywordNamesFromTextFields();

            if (NumberOfKeywordsInThisKeyword > 1)
            {
                int keywordIndex = int.Parse(((Button)sender).Name.Replace("DynamicStep", "").Replace("RemoveKeyword", ""));
                RemoveKeywordField(NumberOfKeywordsInThisKeyword, false);
                ThisFormKeywords.RemoveAt(keywordIndex - 1);
                NumberOfKeywordsInThisKeyword--;
                List<string> args = new List<string>();
                for (int i = 1; i <= NumberOfKeywordsInThisKeyword; i++)
                {
                    args = StringAndListOperations.ReturnListOfArgs(ThisFormKeywords[i - 1].Arguments);
                    if (Controls.Find("DynamicStep" + i + "Params", false).Length != 0)
                        FormControls.RemoveControlByKey("DynamicStep" + i + "Params", Controls);
                    if (args != null && args.Count != 0)
                        FormControls.AddControl("Button", "DynamicStep" + i + "Params",
                            new Point(500 - HorizontalScroll.Value, initialYValue + (i - 1) * 30 - VerticalScroll.Value),
                            new Size(75, 20),
                            "Params",
                            Color.Black,
                            new EventHandler(InstantiateParamsAddForm),
                            this);
                    Controls["DynamicStep" + i + "Name"].Text = ThisFormKeywords[i - 1].Name.Trim();
                }
            }
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

        internal void AddKeywordToThisKeyword(object sender, EventArgs e)
        {
            int keywordIndex = int.Parse(((Button)sender).Name.Replace("DynamicStep", "").Replace("AddKeyword", ""));

            AssignThisKeywordNamesFromTextFields();

            if (ThisFormKeywords == null)
            {
                var checkNull = AddCurrentKeywordsToKeywordsList(sender, e);
                if (checkNull == null) ThisFormKeywords = new List<Keyword>();
            }
                
            ThisFormKeywords.Add(new Keyword("New Keyword", FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text)));

            for (int i = NumberOfKeywordsInThisKeyword; i > keywordIndex; i--)
                ThisFormKeywords[i] = ThisFormKeywords[i - 1];

            ThisFormKeywords[keywordIndex] = new Keyword("New Keyword", FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text));

            NumberOfKeywordsInThisKeyword++;
            AddKeywordField(ThisFormKeywords[NumberOfKeywordsInThisKeyword-1], NumberOfKeywordsInThisKeyword);

            for (int i = 1; i < NumberOfKeywordsInThisKeyword; i++)
                Controls["DynamicStep" + i + "Name"].Text = ThisFormKeywords[i - 1].Name.Trim();

            for (int i = NumberOfKeywordsInThisKeyword; i > keywordIndex; i--)
                if (Controls.Find("DynamicStep" + i + "Params", false).Length != 0)
                {
                    if (i == NumberOfKeywordsInThisKeyword + 1)
                        FormControls.RemoveControlByKey("DynamicStep" + i + "Params", Controls);
                    else
                    {
                        Controls["DynamicStep" + i + "Params"].Location = new Point(
                            Controls["DynamicStep" + i + "Params"].Location.X,
                            Controls["DynamicStep" + i + "Params"].Location.Y + 30);
                        Controls["DynamicStep" + i + "Params"].Name = "DynamicStep" + (i + 1) + "Params";
                    }
                }
        }

        private void KeywordName_TextChanged(object sender, EventArgs e)
        {
            IsKeywordPresentInFilesOrMemoryTree();
        }

        private void KeywordOutputFile_TextChanged(object sender, EventArgs e)
        {
            IsKeywordPresentInFilesOrMemoryTree();
        }

        private bool IsKeywordPresentInFilesOrMemoryTree()
        {
            presentInRobotFile = false;
            memoryPath = TestCasesListOperations.IsPresentInTheKeywordTree(KeywordName.Text,
                FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text),
                ParentKeywords[ImplementationIndexFromTheParent]);

            if (!memoryPath.Equals(""))
                KeywordName.ForeColor = Color.Red;
            else
            {
                if (RobotFileHandler.LocationOfTestCaseOrKeywordInFile(FilesAndFolderStructure.ConcatFileNameToFolder(OutputFile.Text)
                    , KeywordName.Text, "keyword") != -1)
                {
                    KeywordName.ForeColor = Color.Red;
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
    }
}
