using RobotAutomationHelper.Scripts;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RobotAutomationHelper
{
    public partial class KeywordAddForm : Form
    {
        private bool skip = false;
        private int index;
        private int numberOfKeywords;
        private List<Keyword> parentKeywords;
        private List<Keyword> Keywords;
        private int implementedKeyword;
        private bool nestedKeyword = false;

        public KeywordAddForm(bool nested, List<Keyword> parentKeywords)
        {
            InitializeComponent();
            nestedKeyword = nested;
            this.parentKeywords = parentKeywords;
            KeywordOutputFile.Items.Clear();
            KeywordOutputFile.AutoCompleteCustomSource.Clear();
            KeywordOutputFile.Items.AddRange(FilesAndFolderStructure.GetFilesList().ToArray());
            KeywordOutputFile.AutoCompleteCustomSource.AddRange(FilesAndFolderStructure.GetFilesList().ToArray());
            KeywordOutputFile.AutoCompleteSource = AutoCompleteSource.CustomSource;
            KeywordOutputFile.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        }

        internal bool SkipValue()
        {
            return skip;
        }

        internal void ShowKeywordContent(Keyword keyword, int keywordIndex)
        {
            //y value for dynamic buttons
            int initialYValue = 165;

            //index variable gets the keyword index coming from the parent form
            index = keywordIndex;

            if (keyword.GetKeywordName() != null)
                KeywordName.Text = keyword.GetKeywordName().Trim();
            if (keyword.GetKeywordDocumentation() != null)
                KeywordDocumentation.Text = keyword.GetKeywordDocumentation().Replace("[Documentation]", "").Trim();
            if (keyword.GetOutputFilePath() != null)
                KeywordOutputFile.Text = keyword.GetOutputFilePath().Replace(FilesAndFolderStructure.GetFolder(), "\\");
            if (keyword.GetKeywordArguments() != null)
                KeywordArguments.Text = keyword.GetKeywordArguments().Replace("[Arguments]", "").Trim();

            Keywords = new List<Keyword>();
            Keywords = keyword.GetKeywordKeywords();

            int keywordsCounter = 1;
            numberOfKeywords = 0;

            if (Keywords != null && Keywords.Count != 0)
            {
                // adds the keywords in the form
                foreach (Keyword testStep in Keywords)
                {
                    AddKeywordField(keywordsCounter, testStep.GetKeywordArguments(), initialYValue);
                    keywordsCounter++;
                    numberOfKeywords++;
                }
            }
            else
            {
                // add a single keyword field if no keywords are available
                AddKeywordField(keywordsCounter, "", initialYValue);
                numberOfKeywords++;
            }

            // show the form dialog
            this.StartPosition = FormStartPosition.Manual;
            var dialogResult = this.ShowDialog();
        }

        private void ShowAddKeywordForm(object sender, EventArgs e)
        {
            // get the keyword that will be implemented
            Keyword keyword = AddCurrentKeywordsToKeywordsList(sender, e);
            // instantiate the new KeywordAddForm with this parent and Keywords argument
            KeywordAddForm addKeywordForm = new KeywordAddForm(true, Keywords);
            // add closing event
            addKeywordForm.FormClosing += new FormClosingEventHandler(KeywordAddFormClosing);
            addKeywordForm.ShowKeywordContent(keyword, implementedKeyword - 1);
        }

        //adds the list of keywords ( + unimplemented ones ) to a Keyword and returns it
        private Keyword AddCurrentKeywordsToKeywordsList(object sender, EventArgs e)
        {
            string path = FilesAndFolderStructure.GetFolder();
            if (!KeywordOutputFile.Text.StartsWith("\\"))
                path = path + KeywordOutputFile.Text;
            else
                path = path.Trim('\\') + KeywordOutputFile.Text;

            // if AddImplementation is pressed a new form should be opened which requires the keyword that it represents
            int keywordIndex = 0;
            if (((Button)sender).Name.Contains("DynamicTestStep"))
                keywordIndex = int.Parse(((Button)sender).Name.Replace("AddImplementation", "").Replace("DynamicTestStep", ""));

            // add to the global variable for the form that matches the index of the keyword to implement
            implementedKeyword = keywordIndex;
            if (keywordIndex <= 0) keywordIndex = 1;

            if (Keywords == null)
            {
                Keywords = new List<Keyword>();
                for (int i = 1; i <= numberOfKeywords; i++)
                    Keywords.Add(new Keyword("\t" + this.Controls["DynamicTestStep" + i + "Name"].Text, path));
            }
            else
            {
                for (int i = 1; i <= numberOfKeywords; i++)
                    if (i > Keywords.Count)
                        Keywords.Add(new Keyword("\t" + this.Controls["DynamicTestStep" + i + "Name"].Text, path));
                    else
                        Keywords[i - 1].SetKeywordName("\t" + this.Controls["DynamicTestStep" + i + "Name"].Text);
            }
            return Keywords[keywordIndex - 1];
        }

        private void KeywordAddFormClosing(object sender, EventArgs e)
        {
            if (!((KeywordAddForm)sender).SkipValue())
            {
                this.Controls["DynamicTestStep" + implementedKeyword + "Name"].Text = Keywords[implementedKeyword - 1].GetKeywordName().Trim();
                this.Controls["DynamicTestStep" + implementedKeyword + "AddImplementation"].Text = "Edit implementation";
                FormControls.AddControl("Label", "DynamicTestStep" + implementedKeyword + "ImplementationPanel",
                    new System.Drawing.Point(448 - this.HorizontalScroll.Value, 53 + (implementedKeyword - 1) * 25 - this.VerticalScroll.Value),
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

        private void Skip_Click(object sender, EventArgs e)
        {
            skip = true;
            this.Close();
        }

        // when save is pressed add all currently listed keywords to the Keywords List and then AddChangesToKeyword
        private void Save_Click(object sender, EventArgs e)
        {
            AddCurrentKeywordsToKeywordsList(sender, e);
            AddChangesToKeyword();
            this.Close();
        }

        // adds all field data to parentKeyword or testcaseaddform if not nested
        private void AddChangesToKeyword()
        {
            string finalPath = FilesAndFolderStructure.GetFolder();
            if (!KeywordOutputFile.Text.StartsWith("\\"))
                finalPath = finalPath + KeywordOutputFile.Text;
            else
                finalPath = finalPath.Trim('\\') + KeywordOutputFile.Text;

            if (!nestedKeyword)
            {
                TestCaseAddForm.Keywords[index] = new Keyword("\t" + KeywordName.Text.Trim(),
                "\t[Documentation]  " + KeywordDocumentation.Text.Trim(),
                Keywords,
                "\t[Arguments]  " + KeywordArguments.Text.Trim(),
                finalPath);
            }
            else
            {
                parentKeywords[index] = new Keyword("\t" + KeywordName.Text.Trim(),
                "\t[Documentation]  " + KeywordDocumentation.Text.Trim(),
                Keywords,
                "\t[Arguments]  " + KeywordArguments.Text.Trim(),
                finalPath);
            }
        }

        // Adds TextBox / Label / Add implementation / Add and remove keyword
        private void AddKeywordField(int keywordsCounter, string arguments, int initialYValue)
        {
            string[] args;
            if (arguments != null)
                args = arguments.Replace("[Arguments]", "").Trim().Split(' ');
            else
                args = new string[0];

            FormControls.AddControl("TextBox", "DynamicTestStep" + keywordsCounter + "Name",
                new System.Drawing.Point(30 - this.HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - this.VerticalScroll.Value),
                new System.Drawing.Size(280, 20),
                "test keyword",
                System.Drawing.Color.Black,
                null,
                this);
            FormControls.AddControl("Label", "DynamicTestStep" + keywordsCounter + "Label",
                new System.Drawing.Point(10 - this.HorizontalScroll.Value, initialYValue + 3 + (keywordsCounter - 1) * 30 - this.VerticalScroll.Value),
                new System.Drawing.Size(20, 20),
                keywordsCounter + ".",
                System.Drawing.Color.Black,
                null,
                this);
            int i = 0;
            if (args != null && args.Length != 0)
                foreach (string arg in args)
                    if (!arg.Trim().Equals(""))
                    {
                        FormControls.AddControl("TextBox", "DynamicTestStep" + keywordsCounter + "NameArg",
                            new System.Drawing.Point(580 + i * 160 - this.HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - this.VerticalScroll.Value),
                            new System.Drawing.Size(75, 20),
                            "test name",
                            System.Drawing.Color.Black,
                            null,
                            this);
                        FormControls.AddControl("Label", "DynamicTestStep" + keywordsCounter + "LabelArg",
                            new System.Drawing.Point(500 + i * 160 - this.HorizontalScroll.Value, initialYValue + 3 + (keywordsCounter - 1) * 30 - this.VerticalScroll.Value),
                            new System.Drawing.Size(75, 20),
                            arg + ":",
                            System.Drawing.Color.Black,
                            null,
                            this);
                        i++;
                    }

            FormControls.AddControl("Button", "DynamicTestStep" + keywordsCounter + "AddImplementation",
                new System.Drawing.Point(320 - this.HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - this.VerticalScroll.Value),
                new System.Drawing.Size(120, 20),
                "Add Implementation",
                System.Drawing.Color.Black,
                new EventHandler(ShowAddKeywordForm),
                this);
            FormControls.AddControl("Button", "DynamicTestStep" + keywordsCounter + "AddKeyword",
                new System.Drawing.Point(450 - this.HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - this.VerticalScroll.Value),
                new System.Drawing.Size(20, 20),
                "+",
                System.Drawing.Color.Black,
                new EventHandler(ShowAddKeywordForm),
                this);
            FormControls.AddControl("Button", "DynamicTestStep" + keywordsCounter + "RemoveKeyword",
                new System.Drawing.Point(470 - this.HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - this.VerticalScroll.Value),
                new System.Drawing.Size(20, 20),
                "X",
                System.Drawing.Color.Black,
                new EventHandler(ShowAddKeywordForm),
                this);
        }
    }
}
