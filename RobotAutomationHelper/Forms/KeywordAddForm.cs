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
        private int index;
        private int numberOfKeywords;
        private List<Keyword> parentKeywords;
        private List<Keyword> Keywords;
        private List<Param> Params = new List<Param>();
        private int implementedKeyword;
        private bool nestedKeyword = false;

        //y value for dynamic buttons
        private int initialYValue;

        internal KeywordAddForm(bool nested, List<Keyword> parentKeywords)
        {
            InitializeComponent();
            initialYValue = 165;
            nestedKeyword = nested;
            this.parentKeywords = parentKeywords;
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
            index = keywordIndex;

            if (keyword.GetKeywordName() != null)
                KeywordName.Text = keyword.GetKeywordName().Trim();
            if (keyword.GetKeywordDocumentation() != null)
                KeywordDocumentation.Text = keyword.GetKeywordDocumentation().Replace("[Documentation]", "").Trim();
            if (keyword.GetOutputFilePath() != null || !keyword.GetOutputFilePath().Equals(""))
                KeywordOutputFile.Text = keyword.GetOutputFilePath().Replace(FilesAndFolderStructure.GetFolder(), "\\");
            if (keyword.GetKeywordArguments() != null)
                KeywordArguments.Text = keyword.GetKeywordArguments().Replace("[Arguments]", "").Trim();
            IsKeywordPresentInFilesOrMemoryTree();
            Keywords = keyword.GetKeywordKeywords();

            int keywordsCounter = 1;
            numberOfKeywords = 0;

            if (Keywords != null && Keywords.Count != 0)
            {
                // adds the keywords in the form
                foreach (Keyword testStep in Keywords)
                {
                    AddKeywordField(keywordsCounter, testStep.GetKeywordArguments());
                    keywordsCounter++;
                    numberOfKeywords++;
                }
            }
            else
            {
                // add a single keyword field if no keywords are available
                Keywords = new List<Keyword>();
                Keywords.Add(new Keyword("New Keyword", parentKeywords[index].GetOutputFilePath()));
                AddKeywordField(keywordsCounter, "");
                numberOfKeywords++;
            }

            // show the form dialog
            StartPosition = FormStartPosition.Manual;
            var dialogResult = ShowDialog();
        }

        private void InstantiateKeywordAddForm(object sender, EventArgs e)
        {
            // get the keyword that will be implemented
            Keyword keyword = AddCurrentKeywordsToKeywordsList(sender, e);
            // instantiate the new KeywordAddForm with this parent and Keywords argument
            KeywordAddForm addKeywordForm = new KeywordAddForm(true, Keywords);
            // add closing event
            addKeywordForm.FormClosing += new FormClosingEventHandler(UpdateParentFormAfterClosing);
            addKeywordForm.ShowKeywordContent(keyword, implementedKeyword - 1);
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

            // add to the global variable for the form that matches the index of the keyword to implement
            implementedKeyword = keywordIndex;
            if (keywordIndex <= 0) keywordIndex = 1;

            if (Keywords == null)
            {
                Keywords = new List<Keyword>();
                for (int i = 1; i <= numberOfKeywords; i++)
                    Keywords.Add(new Keyword("\t" + Controls["DynamicTestStep" + i + "Name"].Text, path));
            }
            else
            {
                for (int i = 1; i <= numberOfKeywords; i++)
                    if (i > Keywords.Count)
                        Keywords.Add(new Keyword("\t" + Controls["DynamicTestStep" + i + "Name"].Text, path));
                    else
                        Keywords[i - 1].SetKeywordName("\t" + Controls["DynamicTestStep" + i + "Name"].Text);
            }

            return Keywords[keywordIndex - 1];
        }

        private void UpdateParentFormAfterClosing(object sender, EventArgs e)
        {
            if ((sender.GetType().FullName.Contains("KeywordAddForm")) && !((KeywordAddForm)sender).SkipValue())
            {
                Controls["DynamicTestStep" + implementedKeyword + "Name"].Text = Keywords[implementedKeyword - 1].GetKeywordName().Trim();
                Controls["DynamicTestStep" + implementedKeyword + "AddImplementation"].Text = "Edit implementation";

                List<string> args = StringAndListOperations.ReturnListOfArgs(Keywords[implementedKeyword - 1].GetKeywordArguments());

                if (args != null && args.Count != 0)
                    FormControls.AddControl("Button", "DynamicTestStep" + implementedKeyword + "Params",
                        new Point(500 - HorizontalScroll.Value, initialYValue + (implementedKeyword - 1) * 30 - VerticalScroll.Value),
                        new Size(75, 20),
                        "Params",
                        Color.Black,
                        new EventHandler(InstantiateParamsAddForm),
                        this);
                else
                    if (Controls.Find("DynamicTestStep" + implementedKeyword + "Params", false).Length != 0)
                        Controls.RemoveByKey("DynamicTestStep" + implementedKeyword + "Params");
            }

            //Adds file path + name to the Files And Folder structure for use in the drop down lists when chosing output file
            FilesAndFolderStructure.AddImplementedKeywordFilesToSavedFiles(Keywords, implementedKeyword);

            FormControls.UpdateOutputFileSuggestions(KeywordOutputFile);
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
        }

        // adds all field data to parentKeyword or testcaseaddform if not nested
        private void AddChangesToKeyword(bool save)
        {
            string finalPath = FilesAndFolderStructure.ConcatFileNameToFolder(KeywordOutputFile.Text);

            List<string> args = StringAndListOperations.ReturnListOfArgs(KeywordArguments.Text);

            if (args != null)
                for (int i = 0; i < args.Count; i++)
                    if (parentKeywords[index].GetKeywordParams() != null
                        && (parentKeywords[index].GetKeywordParams().Count > 0))
                        {
                            bool foundMatch = false;
                            foreach (Param parentParam in parentKeywords[index].GetKeywordParams())
                                if (parentParam.GetArgName().Equals(args[i]))
                                {
                                    Params.Add(parentParam);
                                    foundMatch = true;
                                    break;
                                }
                            if (!foundMatch)
                                Params.Add(new Param(args[i], ""));
                        }
                        else
                            Params.Add(new Param(args[i], ""));

            if (!nestedKeyword)
            {
                TestCaseAddForm.Keywords[index] = new Keyword("\t" + KeywordName.Text.Trim(),
                "\t[Documentation]  " + KeywordDocumentation.Text.Trim(),
                Keywords,
                "\t[Arguments]  " + KeywordArguments.Text.Trim(),
                Params,
                finalPath, 
                save,
                KeywordType.CUSTOM);
            }
            else
            {
                parentKeywords[index] = new Keyword("\t" + KeywordName.Text.Trim(),
                "\t[Documentation]  " + KeywordDocumentation.Text.Trim(),
                Keywords,
                "\t[Arguments]  " + KeywordArguments.Text.Trim(),
                Params,
                finalPath,
                save,
                KeywordType.CUSTOM);
            }
        }

        // Removes TextBox / Label / Add implementation / Add and remove keyword / Params
        private void RemoveKeywordField(int keywordIndex, bool removeFromList)
        {
            Controls.RemoveByKey("DynamicTestStep" + keywordIndex + "Name");
            Controls.RemoveByKey("DynamicTestStep" + keywordIndex + "Label");
            Controls.RemoveByKey("DynamicTestStep" + keywordIndex + "AddImplementation");
            Controls.RemoveByKey("DynamicTestStep" + keywordIndex + "AddKeyword");
            Controls.RemoveByKey("DynamicTestStep" + keywordIndex + "RemoveKeyword");
            Controls.RemoveByKey("DynamicTestStep" + keywordIndex + "Params");
            if (removeFromList)
                Keywords.RemoveAt(keywordIndex - 1);
        }

        // Adds TextBox / Label / Add implementation / Add and remove keyword / Params
        private void AddKeywordField(int keywordsCounter, string arguments)
        {
            List<string> args = StringAndListOperations.ReturnListOfArgs(arguments);

            FormControls.AddControl("TextBox", "DynamicTestStep" + keywordsCounter + "Name",
                new Point(30 - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - VerticalScroll.Value),
                new Size(280, 20),
                Keywords[keywordsCounter - 1].GetKeywordName().Trim(),
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
            string buttonImplementation = "Add Implementation";
            if (Keywords[keywordsCounter - 1].IsImplemented())
                buttonImplementation = "Edit Implementation";
            FormControls.AddControl("Button", "DynamicTestStep" + keywordsCounter + "AddImplementation",
                new Point(320 - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - VerticalScroll.Value),
                new Size(120, 20),
                buttonImplementation,
                Color.Black,
                new EventHandler(InstantiateKeywordAddForm),
                this);
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
            if (args != null && args.Count != 0)
                FormControls.AddControl("Button", "DynamicTestStep" + keywordsCounter + "Params",
                    new Point(500 - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - VerticalScroll.Value),
                    new Size(75, 20),
                    "Params",
                    Color.Black,
                    new EventHandler(InstantiateParamsAddForm),
                    this);
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

        private void InstantiateParamsAddForm(object sender, EventArgs e)
        {
            AddCurrentKeywordsToKeywordsList(sender, e);
            int keywordIndex = int.Parse(((Button)sender).Name.Replace("Params", "").Replace("DynamicTestStep", ""));
            // instantiate the new KeywordAddForm with this parent and Keywords argument
            ParamAddForm addParamForm = new ParamAddForm();
            // add closing event
            addParamForm.FormClosing += new FormClosingEventHandler(UpdateParentFormAfterClosing);
            addParamForm.ShowParamContent(Keywords[keywordIndex - 1]);
        }

        private void AddKeywordToThisKeyword(object sender, EventArgs e)
        {
            int keywordIndex = int.Parse(((Button)sender).Name.Replace("DynamicTestStep", "").Replace("AddKeyword", ""));

            if (Keywords == null)
            {
                var checkNull = AddCurrentKeywordsToKeywordsList(sender, e);
                if (checkNull == null) Keywords = new List<Keyword>();
            }
                
            Keywords.Add(new Keyword("New Keyword", FilesAndFolderStructure.ConcatFileNameToFolder(KeywordOutputFile.Text)));

            for (int i = numberOfKeywords; i > keywordIndex; i--)
                Keywords[i] = Keywords[i - 1];

            Keywords[keywordIndex] = new Keyword("New Keyword", FilesAndFolderStructure.ConcatFileNameToFolder(KeywordOutputFile.Text));

            numberOfKeywords++;
            AddKeywordField(numberOfKeywords, "");

            for (int i = 1; i < numberOfKeywords; i++)
                Controls["DynamicTestStep" + i + "Name"].Text = Keywords[i - 1].GetKeywordName().Trim();

            for (int i = numberOfKeywords; i > keywordIndex; i--)
                if (Controls.Find("DynamicTestStep" + i + "Params", false).Length != 0)
                {
                    if (i == numberOfKeywords + 1)
                        Controls.RemoveByKey("DynamicTestStep" + i + "Params");
                    else
                    {
                        Controls["DynamicTestStep" + i + "Params"].Location = new Point(
                            Controls["DynamicTestStep" + i + "Params"].Location.X,
                            Controls["DynamicTestStep" + i + "Params"].Location.Y + 30);
                        Controls["DynamicTestStep" + i + "Params"].Name = "DynamicTestStep" + (i + 1) + "Params";
                    }
                }
            
        }

        private void RemoveKeywordFromThisKeyword(object sender, EventArgs e)
        {
            if (numberOfKeywords > 1)
            {
                int keywordIndex = int.Parse(((Button)sender).Name.Replace("DynamicTestStep", "").Replace("RemoveKeyword", ""));
                RemoveKeywordField(numberOfKeywords, false);
                Keywords.RemoveAt(keywordIndex - 1);
                numberOfKeywords--;
                List<string> args = new List<string>();
                for (int i = 1; i <= numberOfKeywords; i++)
                {
                    args = StringAndListOperations.ReturnListOfArgs(Keywords[i-1].GetKeywordArguments());
                    if (Controls.Find("DynamicTestStep" + i + "Params", false).Length != 0)
                        Controls.RemoveByKey("DynamicTestStep" + i + "Params");
                    if (args != null && args.Count != 0)
                        FormControls.AddControl("Button", "DynamicTestStep" + i + "Params",
                            new Point(500 - HorizontalScroll.Value, initialYValue + (i - 1) * 30 - VerticalScroll.Value),
                            new Size(75, 20),
                            "Params",
                            Color.Black,
                            new EventHandler(InstantiateParamsAddForm),
                            this);
                    Controls["DynamicTestStep" + i + "Name"].Text = Keywords[i - 1].GetKeywordName().Trim();
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
            if (TestCasesListOperations.IsPresentInTheKeywordTree(KeywordName.Text,
                FilesAndFolderStructure.ConcatFileNameToFolder(KeywordOutputFile.Text), parentKeywords[index]))
                KeywordName.ForeColor = Color.Red;
            else
            {
                if (RobotFileHandler.ContainsTestCaseOrKeyword(FilesAndFolderStructure.ConcatFileNameToFolder(KeywordOutputFile.Text)
                    , KeywordName.Text, "keyword") != -1)
                    KeywordName.ForeColor = Color.Red;
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
