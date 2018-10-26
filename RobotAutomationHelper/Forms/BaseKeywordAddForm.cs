using RobotAutomationHelper.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts
{
    internal class BaseKeywordAddForm : Form
    {
        private BaseKeywordAddForm FormParent;
        // index and keywords of the parent
        protected int ImplementationIndexFromTheParent;
        internal int NumberOfKeywordsInThisForm { get; set; }
        //y value for dynamic buttons
        protected int initialYValue;
        internal bool skip = false;
        //index of the keyword to be implemented after Add/Edit Implementation
        protected int IndexOfTheKeywordToBeImplemented = 0;
        //Keywords in the form
        protected List<Keyword> ThisFormKeywords { get; set; }

        //Present in memory and robot files
        protected bool presentInRobotFile;
        protected string memoryPath;
        internal FormType FormType { get; set; }

        // change the field when the keyword name is changed
        internal void UpdateTheKeywordOnNameChange(object sender, string textChangePassed)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("ChangeTheKeywordFieldAfterSelection " + (sender as TextWithList).Name + " " + this.Name);

            TextWithList textWithList = sender as TextWithList;
            int keywordIndex = int.Parse(textWithList.Name.Replace("Name", "").Replace("DynamicStep", ""));

            if (textChangePassed.Equals(""))
                CheckKeywordTypeAndEvaluateKeywordData(ThisFormKeywords[keywordIndex - 1], textWithList.Text);
            else
                CheckKeywordTypeAndEvaluateKeywordData(ThisFormKeywords[keywordIndex - 1], textChangePassed);

            UpdateTheKeywordFieldsBasedOnNewData(sender);
        }

        private void UpdateTheKeywordFieldsBasedOnNewData(object sender)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("UpdateKeywordInThisKeyword " + ((TextWithList)sender).Name + " " + Name);
            int keywordIndex = int.Parse(((TextWithList)sender).Name.Replace("DynamicStep", "").Replace("Name", "")); 

            if (ThisFormKeywords[keywordIndex - 1].Type.Equals(KeywordType.CUSTOM))
            {
                string buttonImplementation = "Add Implementation";
                if (ThisFormKeywords[keywordIndex - 1].Implemented)
                    buttonImplementation = "Edit Implementation";

                if (RobotAutomationHelper.Log) Console.WriteLine("length: " + Controls.Find("DynamicStep" + keywordIndex + "AddImplementation", false).Length);
                if (Controls.Find("DynamicStep" + keywordIndex + "AddImplementation", false).Length == 0)
                    FormControls.AddControl("Button", "DynamicStep" + keywordIndex + "AddImplementation",
                    keywordIndex,
                    new Point(320 - HorizontalScroll.Value, initialYValue + (keywordIndex - 1) * 30 - VerticalScroll.Value),
                    new Size(120, 20),
                    buttonImplementation,
                    Color.Black,
                    new EventHandler(InstantiateKeywordAddForm),
                    this);
                else
                    (Controls.Find("DynamicStep" + keywordIndex + "AddImplementation", false)[0] as Button).Text = buttonImplementation;
            }
            else
                FormControls.RemoveControlByKey("DynamicStep" + keywordIndex + "AddImplementation", Controls);

            if (ThisFormKeywords[keywordIndex - 1].Type.Equals(KeywordType.CUSTOM))
            {
                List<string> args = new List<string>();
                args = StringAndListOperations.ReturnListOfArgs(ThisFormKeywords[keywordIndex - 1].Arguments);

                if (args != null && args.Count != 0)
                {
                    if (Controls.Find("DynamicStep" + keywordIndex + "Params", false).Length == 0)
                    {
                        int ParamsButtonX = 500;

                        FormControls.AddControl("Button", "DynamicStep" + keywordIndex + "Params",
                            keywordIndex,
                            new Point(ParamsButtonX - HorizontalScroll.Value, initialYValue + (keywordIndex - 1) * 30 - VerticalScroll.Value),
                            new Size(75, 20),
                            "Params",
                            Color.Black,
                            new EventHandler(InstantiateParamsAddForm),
                            this);
                    }
                }else
                    FormControls.RemoveControlByKey("DynamicStep" + keywordIndex + "Params", Controls);
            }
            else
            {
                if (!(ThisFormKeywords[keywordIndex - 1].Params == null)
                    && !(ThisFormKeywords[keywordIndex - 1].Params.Count == 0))
                {
                    int ParamsButtonX = 500;

                    if (Controls.Find("DynamicStep" + keywordIndex + "Params", false).Length == 0)
                        FormControls.AddControl("Button", "DynamicStep" + keywordIndex + "Params",
                            keywordIndex,
                            new Point(ParamsButtonX - HorizontalScroll.Value, initialYValue + (keywordIndex - 1) * 30 - VerticalScroll.Value),
                            new Size(75, 20),
                            "Params",
                            Color.Black,
                            new EventHandler(InstantiateParamsAddForm),
                            this);
                }else
                    FormControls.RemoveControlByKey("DynamicStep" + keywordIndex + "Params", Controls);
            }

            if (Controls["DynamicStep" + keywordIndex + "Name"] != null)
                Controls["DynamicStep" + keywordIndex + "Name"].Text = ThisFormKeywords[keywordIndex - 1].Name.Trim();
        }

        protected void InstantiateKeywordAddForm(object sender, EventArgs e)
        {
            FormParent = this;
            if (RobotAutomationHelper.Log) Console.WriteLine("InstantiateKeywordAddForm " + ((Button)sender).Name);
            int keywordIndex = int.Parse(((Button)sender).Name.Replace("AddImplementation", "").Replace("DynamicStep", ""));
            IndexOfTheKeywordToBeImplemented = keywordIndex;
            Keyword keyword = ThisFormKeywords[keywordIndex - 1];
            keyword.Implemented = true;
            keyword.Name = Controls["DynamicStep" + keywordIndex + "Name"].Text;
            KeywordAddForm addKeywordForm = new KeywordAddForm(ThisFormKeywords);
            addKeywordForm.FormClosing += new FormClosingEventHandler(UpdateThisFormAfterImlpementedChildKeyword);
            addKeywordForm.ShowKeywordContent(keyword, keywordIndex - 1);
        }

        protected void InstantiateSettingsAddForm(object sender, EventArgs e)
        {
            FormParent = this;
            if (RobotAutomationHelper.Log) Console.WriteLine("InstantiateSettingsAddForm " + ((Button)sender).Name);
            SettingsAddForm AddSettingsForm = new SettingsAddForm();
            //AddSettingsForm.FormClosing += new FormClosingEventHandler(UpdateThisFormAfterImlpementedChildKeyword);
            AddSettingsForm.ShowSettingsContent();
        }

        internal void InstantiateParamsAddForm(object sender, EventArgs e)
        {
            FormParent = this;
            if (RobotAutomationHelper.Log) Console.WriteLine("InstantiateParamsAddForm " + ((Button)sender).Name);

            AddCurrentKeywordsToKeywordsList(sender, e);

            int keywordIndex = int.Parse(((Button)sender).Name.Replace("Params", "").Replace("DynamicStep", ""));
            // instantiate the new KeywordAddForm with this parent and Keywords argument
            ParamAddForm addParamForm = new ParamAddForm();
            ThisFormKeywords[keywordIndex - 1].Name = Controls["DynamicStep" + keywordIndex + "Name"].Text;
            // add closing event
            addParamForm.FormClosing += new FormClosingEventHandler(UpdateThisFormAfterImlpementedChildKeyword);
            addParamForm.ShowParamContent(ThisFormKeywords[keywordIndex - 1]);
        }

        // Adds TextBox / Label / Add implementation / Add and remove keyword / Params
        protected void AddKeywordField(Keyword keyword, int keywordsCounter)
        {
            FormControls.AddControl("TextWithList", "DynamicStep" + keywordsCounter + "Name",
                keywordsCounter,
                new Point(30 - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - VerticalScroll.Value),
                new Size(280, 20),
                keyword.Name.Trim(),
                Color.Black,
                null,
                this);

            FormControls.AddControl("Label", "DynamicStep" + keywordsCounter + "Label",
                keywordsCounter,
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
                FormControls.AddControl("Button", "DynamicStep" + keywordsCounter + "AddImplementation",
                    keywordsCounter,
                    new Point(320 - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - VerticalScroll.Value),
                    new Size(120, 20),
                    buttonImplementation,
                    Color.Black,
                    new EventHandler(InstantiateKeywordAddForm),
                    this);
            }

            FormControls.AddControl("Button", "DynamicStep" + keywordsCounter + "AddKeyword",
                keywordsCounter,
                new Point(450 - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - VerticalScroll.Value),
                new Size(20, 20),
                "+",
                Color.Black,
                new EventHandler(AddKeywordToThisKeyword),
                this);
            FormControls.AddControl("Button", "DynamicStep" + keywordsCounter + "RemoveKeyword",
                keywordsCounter,
                new Point(470 - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - VerticalScroll.Value),
                new Size(20, 20),
                "-",
                Color.Black,
                new EventHandler(RemoveKeywordFromThisForm),
                this);

            int ParamsButtonX = 500;

            if (keyword.Params != null && keyword.Params.Count != 0)
                FormControls.AddControl("Button", "DynamicStep" + keywordsCounter + "Params",
                    keywordsCounter,
                    new Point(ParamsButtonX - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - VerticalScroll.Value),
                    new Size(75, 20),
                    "Params",
                    Color.Black,
                    new EventHandler(InstantiateParamsAddForm),
                    this);
        }

        private void UpdateThisFormAfterImlpementedChildKeyword(object sender, EventArgs e)
        {
            if (((sender as BaseKeywordAddForm).FormType == FormType.Keyword) && !(sender as BaseKeywordAddForm).skip)
            {
                Controls["DynamicStep" + IndexOfTheKeywordToBeImplemented + "Name"].Text = ThisFormKeywords[IndexOfTheKeywordToBeImplemented - 1].Name.Trim();
                Controls["DynamicStep" + IndexOfTheKeywordToBeImplemented + "AddImplementation"].Text = "Edit implementation";

                //List<string> args = StringAndListOperations.ReturnListOfArgs(Keywords[IndexOfTheKeywordToBeImplemented - 1].Arguments);
                if (ThisFormKeywords[IndexOfTheKeywordToBeImplemented - 1].Params != null &&
                    ThisFormKeywords[IndexOfTheKeywordToBeImplemented - 1].Params.Count != 0)
                {
                    if (Controls.Find("DynamicStep" + IndexOfTheKeywordToBeImplemented + "Params", false).Length == 0)
                    {
                        int ParamsButtonX = 500;

                        FormControls.AddControl("Button", "DynamicStep" + IndexOfTheKeywordToBeImplemented + "Params",
                            IndexOfTheKeywordToBeImplemented,
                            new Point(ParamsButtonX - HorizontalScroll.Value, initialYValue + (IndexOfTheKeywordToBeImplemented - 1) * 30 - VerticalScroll.Value),
                            new Size(75, 20),
                            "Params",
                            Color.Black,
                            new EventHandler(InstantiateParamsAddForm),
                            this);
                    }
                }
                else
                    if (Controls.Find("DynamicStep" + IndexOfTheKeywordToBeImplemented + "Params", false).Length != 0)
                    FormControls.RemoveControlByKey("DynamicStep" + IndexOfTheKeywordToBeImplemented + "Params", Controls);

                //Adds file path + name to the Files And Folder structure for use in the drop down lists when chosing output file
                FilesAndFolderStructure.AddImplementedKeywordFilesToSavedFiles(ThisFormKeywords, IndexOfTheKeywordToBeImplemented);
                //update suggestion when not navigating to "Settings" form
                if (!(FormParent.FormType == FormType.Settings))
                {
                    if (!(FormParent.FormType == FormType.Keyword))
                        FormControls.UpdateOutputFileSuggestions(Controls["OutputFile"] as ComboBox, FormParent.FormType);
                    else
                        if (!(FormParent.FormType == FormType.Test))
                        FormControls.UpdateOutputFileSuggestions(Controls["OutputFile"] as ComboBox, FormParent.FormType);
                }
            }
        }

        private void CheckKeywordTypeAndEvaluateKeywordData(Keyword keyword, string name)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("CheckKeywordTypeAndReturnKeyword " + keyword.Name + " " + name);
            foreach (Keyword SuggestedKeyword in FormControls.Suggestions)
                if (SuggestedKeyword.Name.Trim().ToLower().Equals(name.ToLower()))
                {
                    keyword.CopyKeyword(SuggestedKeyword); //CopyKeyword
                    return;
                }

            if (keyword.Type != KeywordType.CUSTOM)
            {
                keyword.CopyKeyword(new Keyword(name, FilesAndFolderStructure.GetFolder(FilesAndFolderStructure.ConvertFormTypeToFolderType(FormType)) + "Auto.robot")); //CopyKeyword
                keyword.Type = KeywordType.CUSTOM;
                return;
            }

            foreach (Keyword seleniumKeyword in FormControls.Suggestions)
                if (seleniumKeyword.Name.Trim().ToLower().Equals(keyword.Name.Trim().ToLower()))
                {
                    keyword.CopyKeyword(new Keyword(name, FilesAndFolderStructure.GetFolder(FilesAndFolderStructure.ConvertFormTypeToFolderType(FormType)) + "Auto.robot")); //CopyKeyword
                    keyword.Type = KeywordType.CUSTOM;
                    return;
                }

            keyword.Name = name;
            keyword.Type = KeywordType.CUSTOM;
        }

        // Removes TextBox / Label / Add implementation / Add and remove keyword / Params
        protected void RemoveKeywordField(int keywordIndex, bool removeFromList)
        {
            FormControls.RemoveControlByKey("DynamicStep" + keywordIndex + "Name", Controls);
            FormControls.RemoveControlByKey("DynamicStep" + keywordIndex + "Label", Controls);
            FormControls.RemoveControlByKey("DynamicStep" + keywordIndex + "AddImplementation", Controls);
            FormControls.RemoveControlByKey("DynamicStep" + keywordIndex + "AddKeyword", Controls);
            FormControls.RemoveControlByKey("DynamicStep" + keywordIndex + "RemoveKeyword", Controls);
            FormControls.RemoveControlByKey("DynamicStep" + keywordIndex + "Params", Controls);
            if (removeFromList)
                ThisFormKeywords.RemoveAt(keywordIndex - 1);
        }

        //adds the list of keywords ( + unimplemented ones ) to a Keyword and returns it
        protected Keyword AddCurrentKeywordsToKeywordsList(object sender, EventArgs e)
        {
            string path = FilesAndFolderStructure.ConcatFileNameToFolder(Controls["OutputFile"].Text, FilesAndFolderStructure.ConvertFormTypeToFolderType(FormType));

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
                for (int i = 1; i <= NumberOfKeywordsInThisForm; i++)
                    ThisFormKeywords.Add(new Keyword(Controls["DynamicStep" + i + "Name"].Text, path));
            }
            else
            {
                for (int i = ThisFormKeywords.Count + 1; i <= NumberOfKeywordsInThisForm; i++)
                    ThisFormKeywords.Add(new Keyword(Controls["DynamicStep" + i + "Name"].Text, path));
            }

            AssignThisKeywordNamesFromTextFields();

            return ThisFormKeywords[keywordIndex - 1];
        }

        protected void AssignThisKeywordNamesFromTextFields()
        {
            for (int i = 1; i <= NumberOfKeywordsInThisForm; i++)
                if (Controls.Find("DynamicStep" + i + "Name", false).Length != 0)
                    ThisFormKeywords[i - 1].Name = Controls["DynamicStep" + i + "Name"].Text;
        }

        internal void RemoveKeywordFromThisForm(object sender, EventArgs e)
        {
            AssignThisKeywordNamesFromTextFields();

            if (NumberOfKeywordsInThisForm > 1)
            {
                int keywordIndex = int.Parse(((Button)sender).Name.Replace("DynamicStep", "").Replace("RemoveKeyword", ""));
                RemoveKeywordField(NumberOfKeywordsInThisForm, false);
                ThisFormKeywords.RemoveAt(keywordIndex - 1);
                NumberOfKeywordsInThisForm--;
                List<string> args = new List<string>();
                for (int i = 1; i <= NumberOfKeywordsInThisForm; i++)
                {
                    args = StringAndListOperations.ReturnListOfArgs(ThisFormKeywords[i - 1].Arguments);
                    if (Controls.Find("DynamicStep" + i + "Params", false).Length != 0)
                        FormControls.RemoveControlByKey("DynamicStep" + i + "Params", Controls);
                    if (args != null && args.Count != 0)
                        FormControls.AddControl("Button", "DynamicStep" + i + "Params",
                            i,
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

        protected void AddKeywordToThisKeyword(object sender, EventArgs e)
        {
            int keywordIndex = int.Parse(((Button)sender).Name.Replace("DynamicStep", "").Replace("AddKeyword", ""));

            AssignThisKeywordNamesFromTextFields();

            if (ThisFormKeywords == null)
            {
                var checkNull = AddCurrentKeywordsToKeywordsList(sender, e);
                if (checkNull == null) ThisFormKeywords = new List<Keyword>();
            }

            ThisFormKeywords.Add(new Keyword("New Keyword", FilesAndFolderStructure.ConcatFileNameToFolder(Controls["OutputFile"].Text, FilesAndFolderStructure.ConvertFormTypeToFolderType(FormType))));

            for (int i = NumberOfKeywordsInThisForm; i > keywordIndex; i--)
                ThisFormKeywords[i] = ThisFormKeywords[i - 1];

            ThisFormKeywords[keywordIndex] = new Keyword("New Keyword", FilesAndFolderStructure.ConcatFileNameToFolder(Controls["OutputFile"].Text, FilesAndFolderStructure.ConvertFormTypeToFolderType(FormType)));

            NumberOfKeywordsInThisForm++;
            AddKeywordField(ThisFormKeywords[NumberOfKeywordsInThisForm - 1], NumberOfKeywordsInThisForm);

            for (int i = 1; i < NumberOfKeywordsInThisForm; i++)
                Controls["DynamicStep" + i + "Name"].Text = ThisFormKeywords[i - 1].Name.Trim();

            for (int i = NumberOfKeywordsInThisForm; i > keywordIndex; i--)
                if (Controls.Find("DynamicStep" + i + "Params", false).Length != 0)
                {
                    if (i == NumberOfKeywordsInThisForm + 1)
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

        // Block fields
        internal void DisableKeywordFields(int keywordIndex)
        {
            if (Controls.Find("DynamicStep" + keywordIndex + "Label", false).Length != 0)
                Controls["DynamicStep" + keywordIndex + "Label"].Enabled = false;
            if (Controls.Find("DynamicStep" + keywordIndex + "AddImplementation", false).Length != 0)
                Controls["DynamicStep" + keywordIndex + "AddImplementation"].Enabled = false;
            if (Controls.Find("DynamicStep" + keywordIndex + "AddKeyword", false).Length != 0)
                Controls["DynamicStep" + keywordIndex + "AddKeyword"].Enabled = false;
            if (Controls.Find("DynamicStep" + keywordIndex + "RemoveKeyword", false).Length != 0)
                Controls["DynamicStep" + keywordIndex + "RemoveKeyword"].Enabled = false;
            if (Controls.Find("DynamicStep" + keywordIndex + "Params", false).Length != 0)
                Controls["DynamicStep" + keywordIndex + "Params"].Enabled = false;
        }

        // Enable fields
        internal void EnableKeywordFields(int keywordIndex)
        {
            if (Controls.Find("DynamicStep" + keywordIndex + "Label", false).Length != 0)
                Controls["DynamicStep" + keywordIndex + "Label"].Enabled = true;
            if (Controls.Find("DynamicStep" + keywordIndex + "AddImplementation", false).Length != 0)
                Controls["DynamicStep" + keywordIndex + "AddImplementation"].Enabled = true;
            if (Controls.Find("DynamicStep" + keywordIndex + "AddKeyword", false).Length != 0)
                Controls["DynamicStep" + keywordIndex + "AddKeyword"].Enabled = true;
            if (Controls.Find("DynamicStep" + keywordIndex + "RemoveKeyword", false).Length != 0)
                Controls["DynamicStep" + keywordIndex + "RemoveKeyword"].Enabled = true;
            if (Controls.Find("DynamicStep" + keywordIndex + "Params", false).Length != 0)
                Controls["DynamicStep" + keywordIndex + "Params"].Enabled = true;
        }
    }

    internal enum FormType
    {
        Keyword,
        Test,
        Settings
    }
}
