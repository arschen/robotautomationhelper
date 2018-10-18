using RobotAutomationHelper.Forms;
using RobotAutomationHelper.Scripts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper
{
    internal partial class KeywordAddForm : Form
    {
        private bool skip = false;

        // index and keywords of the parent
        private int IndexOfTheParentKeyword;
        private List<Keyword> ParentKeywords;

        internal int NumberOfKeywordsInThisKeyword { get; set; }

        //keywords in this form
        internal List<Keyword> ThisFormKeywords;
        private List<Param> ThisKeywordParams = new List<Param>();

        //index of the keyword to be implemented after Add/Edit Implementation
        private int IndexOfTheKeywordToBeImplemented = 0;
        private bool nestedKeyword = false;

        //y value for dynamic buttons
        internal int initialYValue;

        internal KeywordAddForm(bool nested, List<Keyword> parentKeywords)
        {
            InitializeComponent();
            initialYValue = 165;
            nestedKeyword = nested;
            this.ParentKeywords = parentKeywords;
            FormControls.UpdateOutputFileSuggestions(KeywordOutputFile);
            ActiveControl = KeywordNameLabel;
        }

        internal bool SkipValue()
        {
            return skip;
        }

        internal void ShowKeywordContent(Keyword keyword, int keywordIndex)
        {
            //index variable gets the keyword index coming from the parent form
            IndexOfTheParentKeyword = keywordIndex;

            if (keyword.Name != null)
                KeywordName.Text = keyword.Name.Trim();
            if (keyword.Documentation != null)
                KeywordDocumentation.Text = keyword.Documentation.Replace("[Documentation]", "").Trim();
            if (keyword.OutputFilePath != null || !keyword.OutputFilePath.Equals(""))
                KeywordOutputFile.Text = keyword.OutputFilePath.Replace(FilesAndFolderStructure.GetFolder(), "\\");
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
                    new Keyword("New Keyword", ParentKeywords[IndexOfTheParentKeyword].OutputFilePath)
                };
                AddKeywordField(ThisFormKeywords[0], keywordsCounter);
                NumberOfKeywordsInThisKeyword++;
            }

            // show the form dialog
            StartPosition = FormStartPosition.Manual;
            var dialogResult = ShowDialog();
        }

        internal void InstantiateKeywordAddForm(object sender, EventArgs e)
        {
            // get the keyword that will be implemented
            int keywordIndex = int.Parse(((Button)sender).Name.Replace("AddImplementation", "").Replace("DynamicTestStep", ""));
            Keyword keyword = AddCurrentKeywordsToKeywordsList(sender, e);
            keyword.Implemented = true;
            keyword.Name = Controls["DynamicTestStep" + keywordIndex + "Name"].Text;
            // instantiate the new KeywordAddForm with this parent and Keywords argument
            KeywordAddForm addKeywordForm = new KeywordAddForm(true, ThisFormKeywords);
            // add closing event
            addKeywordForm.FormClosing += new FormClosingEventHandler(UpdateParentFormAfterClosing);
            addKeywordForm.ShowKeywordContent(keyword, IndexOfTheKeywordToBeImplemented - 1);
        }

        //adds the list of keywords ( + unimplemented ones ) to a Keyword and returns it
        private Keyword AddCurrentKeywordsToKeywordsList(object sender, EventArgs e)
        {
            string path = FilesAndFolderStructure.ConcatFileNameToFolder(KeywordOutputFile.Text);

            // if AddImplementation is pressed a new form should be opened which requires the keyword that it represents
            int keywordIndex = 0;
            if (((Button)sender).Name.Contains("DynamicTestStep") 
                && !((Button)sender).Name.Contains("Params")
                && !((Button)sender).Name.Contains("AddKeyword")
                && !((Button)sender).Name.Contains("RemoveKeyword"))
                keywordIndex = int.Parse(((Button)sender).Name.Replace("AddImplementation", "").Replace("DynamicTestStep", ""));
            else
                if (((Button)sender).Name.Contains("DynamicTestStep")
                && ((Button)sender).Name.Contains("Params"))
                keywordIndex = int.Parse(((Button)sender).Name.Replace("Params", "").Replace("DynamicTestStep", ""));

            // add to the global variable for the form that matches the index of the keyword to implement
            IndexOfTheKeywordToBeImplemented = keywordIndex;
            if (keywordIndex <= 0) keywordIndex = 1;

            if (ThisFormKeywords == null)
            {
                ThisFormKeywords = new List<Keyword>();
                for (int i = 1; i <= NumberOfKeywordsInThisKeyword; i++)
                    ThisFormKeywords.Add(new Keyword(Controls["DynamicTestStep" + i + "Name"].Text, path));
            }
            else
            {
                for (int i = ThisFormKeywords.Count + 1; i <= NumberOfKeywordsInThisKeyword; i++)
                    ThisFormKeywords.Add(new Keyword(Controls["DynamicTestStep" + i + "Name"].Text, path));
            }

            AssignThisKeywordNamesFromTextFields();

            return ThisFormKeywords[keywordIndex - 1];
        }

        private void AssignThisKeywordNamesFromTextFields()
        {
            for (int i = 1; i <= NumberOfKeywordsInThisKeyword; i++)
                if (Controls.Find("DynamicTestStep" + i + "Name", false).Length != 0)
                    ThisFormKeywords[i - 1].Name = Controls["DynamicTestStep" + i + "Name"].Text;
        }

        private void UpdateParentFormAfterClosing(object sender, EventArgs e)
        {
            if ((sender.GetType().FullName.Contains("KeywordAddForm")) && !((KeywordAddForm)sender).SkipValue())
            {
                Controls["DynamicTestStep" + IndexOfTheKeywordToBeImplemented + "Name"].Text = ThisFormKeywords[IndexOfTheKeywordToBeImplemented - 1].Name.Trim();
                Controls["DynamicTestStep" + IndexOfTheKeywordToBeImplemented + "AddImplementation"].Text = "Edit implementation";

                List<string> args = StringAndListOperations.ReturnListOfArgs(ThisFormKeywords[IndexOfTheKeywordToBeImplemented - 1].Arguments);

                if (args != null && args.Count != 0)
                    FormControls.AddControl("Button", "DynamicTestStep" + IndexOfTheKeywordToBeImplemented + "Params",
                        new Point(500 - HorizontalScroll.Value, initialYValue + (IndexOfTheKeywordToBeImplemented - 1) * 30 - VerticalScroll.Value),
                        new Size(75, 20),
                        "Params",
                        Color.Black,
                        new EventHandler(InstantiateParamsAddForm),
                        this);
                else
                    if (Controls.Find("DynamicTestStep" + IndexOfTheKeywordToBeImplemented + "Params", false).Length != 0)
                        FormControls.RemoveControlByKey("DynamicTestStep" + IndexOfTheKeywordToBeImplemented + "Params", Controls);

                //Adds file path + name to the Files And Folder structure for use in the drop down lists when chosing output file
                FilesAndFolderStructure.AddImplementedKeywordFilesToSavedFiles(ThisFormKeywords, IndexOfTheKeywordToBeImplemented);
                FormControls.UpdateOutputFileSuggestions(KeywordOutputFile);
            }
        }

        private void Skip_Click(object sender, EventArgs e)
        {
            skip = true;
            Close();
        }

        // when save is pressed add all currently listed keywords to the Keywords List and then AddChangesToKeyword
        private void Save_Click(object sender, EventArgs e)
        {
            if (!IsKeywordPresentInFilesOrMemoryTree())
                {
                    AddCurrentKeywordsToKeywordsList(sender, e);
                    AddChangesToKeyword(true);
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
                        AddChangesToKeyword(true);
                        ParentKeywords[IndexOfTheParentKeyword].Overwrite = true;
                        Close();
                    }
                    else
                        ParentKeywords[IndexOfTheParentKeyword].Overwrite = false;
                }
                else
                {
                    DialogResult result = MessageBox.Show("Keyword with this name has already been implemented in the output file. \n" + memoryPath,
                        "Alert",
                        MessageBoxButtons.OK);
                }
            }
        }

        // adds all field data to parentKeyword or testcaseaddform if not nested
        private void AddChangesToKeyword(bool save)
        {
            string finalPath = FilesAndFolderStructure.ConcatFileNameToFolder(KeywordOutputFile.Text);

            List<string> args = StringAndListOperations.ReturnListOfArgs(KeywordArguments.Text);

            if (args != null)
                for (int i = 0; i < args.Count; i++)
                    if (ParentKeywords[IndexOfTheParentKeyword].Params != null
                        && (ParentKeywords[IndexOfTheParentKeyword].Params.Count > 0))
                        {
                            bool foundMatch = false;
                            foreach (Param parentParam in ParentKeywords[IndexOfTheParentKeyword].Params)
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

            if (ParentKeywords[IndexOfTheParentKeyword].SuggestionIndex == -1)
                addToSuggestions = true;

            ParentKeywords[IndexOfTheParentKeyword] = new Keyword(KeywordName.Text.Trim(),
            "[Documentation]  " + KeywordDocumentation.Text.Trim(),
            ThisFormKeywords,
            "[Arguments]  " + KeywordArguments.Text.Trim(),
            ThisKeywordParams,
            finalPath,
            save,
            KeywordType.CUSTOM,
            ParentKeywords[IndexOfTheParentKeyword].SuggestionIndex);

            if (addToSuggestions)
            {
                ParentKeywords[IndexOfTheParentKeyword].SuggestionIndex = FormControls.Suggestions.Count;
                Keyword temp = new Keyword();
                temp.CopyKeyword(ParentKeywords[IndexOfTheParentKeyword]); //CopyKeyword
                FormControls.Suggestions.Add(temp);
            }
            else
                FormControls.Suggestions[ParentKeywords[IndexOfTheParentKeyword].SuggestionIndex].CopyKeyword(ParentKeywords[IndexOfTheParentKeyword]); //CopyKeyword
        }

        // Removes TextBox / Label / Add implementation / Add and remove keyword / Params
        internal void RemoveKeywordField(int keywordIndex, bool removeFromList)
        {
            FormControls.RemoveControlByKey("DynamicTestStep" + keywordIndex + "Name", Controls);
            FormControls.RemoveControlByKey("DynamicTestStep" + keywordIndex + "Label", Controls);
            FormControls.RemoveControlByKey("DynamicTestStep" + keywordIndex + "AddImplementation", Controls);
            FormControls.RemoveControlByKey("DynamicTestStep" + keywordIndex + "AddKeyword", Controls);
            FormControls.RemoveControlByKey("DynamicTestStep" + keywordIndex + "RemoveKeyword", Controls);
            FormControls.RemoveControlByKey("DynamicTestStep" + keywordIndex + "Params", Controls);
            if (removeFromList)
                ThisFormKeywords.RemoveAt(keywordIndex - 1);
        }

        // Adds TextBox / Label / Add implementation / Add and remove keyword / Params
        private void AddKeywordField(Keyword keyword, int keywordsCounter)
        {
            //List<string> args = StringAndListOperations.ReturnListOfArgs(keyword.Arguments);

            FormControls.AddControl("TextWithList", "DynamicTestStep" + keywordsCounter + "Name",
                new Point(30 - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - VerticalScroll.Value),
                new Size(280, 20),
                keyword.Name.Trim(),
                Color.Black,
                null,
                this);

            FormControls.AddControl("Label", "DynamicTestStep" + keywordsCounter + "Label",
                new Point(10 - HorizontalScroll.Value, initialYValue + 3 + (keywordsCounter - 1) * 30 - VerticalScroll.Value),
                new Size(20, 20),
                keywordsCounter + ".",
                Color.Black,
                null,
                this);

            if (keyword.Type.Equals(KeywordType.CUSTOM))
            {
                string buttonImplementation = "Add Implementation";
                if (ThisFormKeywords[keywordsCounter - 1].Implemented)
                    buttonImplementation = "Edit Implementation";
                FormControls.AddControl("Button", "DynamicTestStep" + keywordsCounter + "AddImplementation",
                    new Point(320 - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - VerticalScroll.Value),
                    new Size(120, 20),
                    buttonImplementation,
                    Color.Black,
                    new EventHandler(InstantiateKeywordAddForm),
                    this);
            }
            FormControls.AddControl("Button", "DynamicTestStep" + keywordsCounter + "AddKeyword",
                new Point(450 - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - VerticalScroll.Value),
                new Size(20, 20),
                "+",
                Color.Black,
                new EventHandler(AddKeywordToThisKeyword),
                this);
            FormControls.AddControl("Button", "DynamicTestStep" + keywordsCounter + "RemoveKeyword",
                new Point(470 - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - VerticalScroll.Value),
                new Size(20, 20),
                "-",
                Color.Black,
                new EventHandler(RemoveKeywordFromThisKeyword),
                this);
            if (keyword.Params != null && keyword.Params.Count != 0)
                FormControls.AddControl("Button", "DynamicTestStep" + keywordsCounter + "Params",
                    new Point(500 - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - VerticalScroll.Value),
                    new Size(75, 20),
                    "Params",
                    Color.Black,
                    new EventHandler(InstantiateParamsAddForm),
                    this);
        }

        internal void RemoveKeywordFromThisKeyword(object sender, EventArgs e)
        {
            AssignThisKeywordNamesFromTextFields();

            if (NumberOfKeywordsInThisKeyword > 1)
            {
                int keywordIndex = int.Parse(((Button)sender).Name.Replace("DynamicTestStep", "").Replace("RemoveKeyword", ""));
                RemoveKeywordField(NumberOfKeywordsInThisKeyword, false);
                ThisFormKeywords.RemoveAt(keywordIndex - 1);
                NumberOfKeywordsInThisKeyword--;
                List<string> args = new List<string>();
                for (int i = 1; i <= NumberOfKeywordsInThisKeyword; i++)
                {
                    args = StringAndListOperations.ReturnListOfArgs(ThisFormKeywords[i - 1].Arguments);
                    if (Controls.Find("DynamicTestStep" + i + "Params", false).Length != 0)
                        FormControls.RemoveControlByKey("DynamicTestStep" + i + "Params", Controls);
                    if (args != null && args.Count != 0)
                        FormControls.AddControl("Button", "DynamicTestStep" + i + "Params",
                            new Point(500 - HorizontalScroll.Value, initialYValue + (i - 1) * 30 - VerticalScroll.Value),
                            new Size(75, 20),
                            "Params",
                            Color.Black,
                            new EventHandler(InstantiateParamsAddForm),
                            this);
                    Controls["DynamicTestStep" + i + "Name"].Text = ThisFormKeywords[i - 1].Name.Trim();
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

        private void AddKeywordToThisKeyword(object sender, EventArgs e)
        {
            int keywordIndex = int.Parse(((Button)sender).Name.Replace("DynamicTestStep", "").Replace("AddKeyword", ""));

            AssignThisKeywordNamesFromTextFields();

            if (ThisFormKeywords == null)
            {
                var checkNull = AddCurrentKeywordsToKeywordsList(sender, e);
                if (checkNull == null) ThisFormKeywords = new List<Keyword>();
            }
                
            ThisFormKeywords.Add(new Keyword("New Keyword", FilesAndFolderStructure.ConcatFileNameToFolder(KeywordOutputFile.Text)));

            for (int i = NumberOfKeywordsInThisKeyword; i > keywordIndex; i--)
                ThisFormKeywords[i] = ThisFormKeywords[i - 1];

            ThisFormKeywords[keywordIndex] = new Keyword("New Keyword", FilesAndFolderStructure.ConcatFileNameToFolder(KeywordOutputFile.Text));

            NumberOfKeywordsInThisKeyword++;
            AddKeywordField(ThisFormKeywords[NumberOfKeywordsInThisKeyword-1], NumberOfKeywordsInThisKeyword);

            for (int i = 1; i < NumberOfKeywordsInThisKeyword; i++)
                Controls["DynamicTestStep" + i + "Name"].Text = ThisFormKeywords[i - 1].Name.Trim();

            for (int i = NumberOfKeywordsInThisKeyword; i > keywordIndex; i--)
                if (Controls.Find("DynamicTestStep" + i + "Params", false).Length != 0)
                {
                    if (i == NumberOfKeywordsInThisKeyword + 1)
                        FormControls.RemoveControlByKey("DynamicTestStep" + i + "Params", Controls);
                    else
                    {
                        Controls["DynamicTestStep" + i + "Params"].Location = new Point(
                            Controls["DynamicTestStep" + i + "Params"].Location.X,
                            Controls["DynamicTestStep" + i + "Params"].Location.Y + 30);
                        Controls["DynamicTestStep" + i + "Params"].Name = "DynamicTestStep" + (i + 1) + "Params";
                    }
                }
            
        }

        internal void InstantiateParamsAddForm(object sender, EventArgs e)
        {
            AddCurrentKeywordsToKeywordsList(sender, e);
            int keywordIndex = int.Parse(((Button)sender).Name.Replace("Params", "").Replace("DynamicTestStep", ""));
            // instantiate the new KeywordAddForm with this parent and Keywords argument
            ParamAddForm addParamForm = new ParamAddForm();
            ThisFormKeywords[keywordIndex - 1].Name = Controls["DynamicTestStep" + keywordIndex + "Name"].Text;
            // add closing event
            addParamForm.FormClosing += new FormClosingEventHandler(UpdateParentFormAfterClosing);
            addParamForm.ShowParamContent(ThisFormKeywords[keywordIndex - 1]);
        }

        private void KeywordName_TextChanged(object sender, EventArgs e)
        {
            IsKeywordPresentInFilesOrMemoryTree();
        }

        private void KeywordOutputFile_TextChanged(object sender, EventArgs e)
        {
            IsKeywordPresentInFilesOrMemoryTree();
        }

        private bool presentInRobotFile;
        private string memoryPath;

        private bool IsKeywordPresentInFilesOrMemoryTree()
        {
            memoryPath = TestCasesListOperations.IsPresentInTheKeywordTree(KeywordName.Text,
                FilesAndFolderStructure.ConcatFileNameToFolder(KeywordOutputFile.Text),
                ParentKeywords[IndexOfTheParentKeyword]);
            presentInRobotFile = false;
            if (!memoryPath.Equals(""))
                KeywordName.ForeColor = Color.Red;
            else
            {
                if (RobotFileHandler.ContainsTestCaseOrKeyword(FilesAndFolderStructure.ConcatFileNameToFolder(KeywordOutputFile.Text)
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
