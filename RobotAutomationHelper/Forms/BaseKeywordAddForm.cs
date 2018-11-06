using RobotAutomationHelper.Forms;
using RobotAutomationHelper.Scripts.Static.Consts;
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

        private bool recursive = false;

        internal BaseKeywordAddForm()
        {

        }

        internal BaseKeywordAddForm(BaseKeywordAddForm parentForm)
        {
            FormParent = parentForm;
        }

        // change the field when the keyword name is changed
        internal void UpdateTheKeywordOnNameChange(object sender, string textChangePassed, string keywordType)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("ChangeTheKeywordFieldAfterSelection " + (sender as TextWithList).Name + " " + this.Name);

            TextWithList textWithList = sender as TextWithList;
            int keywordIndex = int.Parse(textWithList.Name.Replace("Name", "").Replace("DynamicStep", ""));

            if (textChangePassed.Equals(""))
                CheckKeywordTypeAndEvaluateKeywordData(ThisFormKeywords[keywordIndex - 1], textWithList.Text, keywordType);
            else
                CheckKeywordTypeAndEvaluateKeywordData(ThisFormKeywords[keywordIndex - 1], textChangePassed, keywordType);

            UpdateTheKeywordFieldsBasedOnNewData(sender);
        }

        private void UpdateTheKeywordFieldsBasedOnNewData(object sender)
        {
            int settingsLabel = 0;
            if (FormType == FormType.Settings)
                settingsLabel = KeywordFieldConsts.SettingsLabelWidth - KeywordFieldConsts.LabelWidth;

            if (RobotAutomationHelper.Log) Console.WriteLine("UpdateKeywordInThisKeyword " + ((TextWithList)sender).Name + " " + Name);
            int keywordIndex = int.Parse(((TextWithList)sender).Name.Replace("DynamicStep", "").Replace("Name", "")); 
            if (!FormType.Equals(FormType.NameAndOutput))
            {
                if (ThisFormKeywords[keywordIndex - 1].Type.Equals(KeywordType.CUSTOM))
                {
                    string buttonImplementation = "Add Implementation";
                    if (ThisFormKeywords[keywordIndex - 1].Implemented)
                        buttonImplementation = "Edit Implementation";

                    if (RobotAutomationHelper.Log) Console.WriteLine("length: " + Controls.Find("DynamicStep" + keywordIndex + "AddImplementation", false).Length);
                    if (Controls.Find("DynamicStep" + keywordIndex + "AddImplementation", false).Length == 0)
                        FormControls.AddControl("Button", "DynamicStep" + keywordIndex + "AddImplementation",
                        keywordIndex,
                        new Point(settingsLabel + KeywordFieldConsts.AddImplementationX - HorizontalScroll.Value, initialYValue + (keywordIndex - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                        new Size(KeywordFieldConsts.AddImplementationWidth, KeywordFieldConsts.FieldsHeight),
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
                    if (!(ThisFormKeywords[keywordIndex - 1].Params == null)
                        && !(ThisFormKeywords[keywordIndex - 1].Params.Count == 0))
                    {
                        if (Controls.Find("DynamicStep" + keywordIndex + "Params", false).Length == 0)
                        {
                            FormControls.AddControl("Button", "DynamicStep" + keywordIndex + "Params",
                                keywordIndex,
                                new Point(settingsLabel + KeywordFieldConsts.ParamX - HorizontalScroll.Value, initialYValue + (keywordIndex - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                                new Size(KeywordFieldConsts.ParamWidth, KeywordFieldConsts.FieldsHeight),
                                "Params",
                                Color.Black,
                                new EventHandler(InstantiateParamsAddForm),
                                this);
                        }
                    }
                    else
                        FormControls.RemoveControlByKey("DynamicStep" + keywordIndex + "Params", Controls);
                }
                else
                {
                    if (!(ThisFormKeywords[keywordIndex - 1].Params == null)
                        && !(ThisFormKeywords[keywordIndex - 1].Params.Count == 0))
                    {
                        if (Controls.Find("DynamicStep" + keywordIndex + "Params", false).Length == 0)
                            FormControls.AddControl("Button", "DynamicStep" + keywordIndex + "Params",
                                keywordIndex,
                                new Point(settingsLabel + KeywordFieldConsts.ParamX - HorizontalScroll.Value, initialYValue + (keywordIndex - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                                new Size(KeywordFieldConsts.ParamWidth, KeywordFieldConsts.FieldsHeight),
                                "Params",
                                Color.Black,
                                new EventHandler(InstantiateParamsAddForm),
                                this);
                    }
                    else
                        FormControls.RemoveControlByKey("DynamicStep" + keywordIndex + "Params", Controls);
                }
            }

            if (Controls["DynamicStep" + keywordIndex + "Name"] != null)
                Controls["DynamicStep" + keywordIndex + "Name"].Text = ThisFormKeywords[keywordIndex - 1].Name.Trim();
        }

        protected void InstantiateKeywordAddForm(object sender, EventArgs e)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("InstantiateKeywordAddForm " + ((Button)sender).Name);
            int keywordIndex = int.Parse(((Button)sender).Name.Replace("AddImplementation", "").Replace("DynamicStep", ""));
            IndexOfTheKeywordToBeImplemented = keywordIndex;
            Keyword keyword = ThisFormKeywords[keywordIndex - 1];
            keyword.Implemented = true;
            keyword.Name = Controls["DynamicStep" + keywordIndex + "Name"].Text;
            KeywordAddForm addKeywordForm = new KeywordAddForm(ThisFormKeywords, this);
            addKeywordForm.FormClosing += new FormClosingEventHandler(UpdateThisFormAfterImlpementedChildKeyword);
            addKeywordForm.ShowKeywordContent(keyword, keywordIndex - 1);
        }

        protected void InstantiateSettingsAddForm(object sender, EventArgs e)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("InstantiateSettingsAddForm " + ((Button)sender).Name);
            SettingsAddForm AddSettingsForm = new SettingsAddForm(this);
            //AddSettingsForm.FormClosing += new FormClosingEventHandler(UpdateThisFormAfterImlpementedChildKeyword);
            AddSettingsForm.ShowSettingsContent();
        }

        internal void InstantiateParamsAddForm(object sender, EventArgs e)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("InstantiateParamsAddForm " + ((Button)sender).Name);

            AddCurrentKeywordsToKeywordsList(sender, e);

            int keywordIndex = int.Parse(((Button)sender).Name.Replace("Params", "").Replace("DynamicStep", ""));
            // instantiate the new KeywordAddForm with this parent and Keywords argument
            ParamAddForm addParamForm = new ParamAddForm(this);
            ThisFormKeywords[keywordIndex - 1].Name = Controls["DynamicStep" + keywordIndex + "Name"].Text;
            // add closing event
            // addParamForm.FormClosing += new FormClosingEventHandler(UpdateThisFormAfterImlpementedChildKeyword);
            addParamForm.ShowParamContent(ThisFormKeywords[keywordIndex - 1]);
        }

        private object realSender;

        internal void InstantiateNameAndOutputForm(object sender, EventArgs e)
        {
            realSender = sender;
            if (RobotAutomationHelper.Log) Console.WriteLine("InstantiateParamsAddForm " + ((Button)sender).Name);
            FormType formType;
            if (Name.Contains("Robot Automation Helper"))
                formType = FormType.Test;
            else
                formType = FormType.Keyword;

            NameAndOutputForm nameAndOutputForm;
            if (ToString().Contains("TestCaseAddForm"))
                nameAndOutputForm = new NameAndOutputForm(formType, this, null);
            else
                nameAndOutputForm = new NameAndOutputForm(formType, this, (this as KeywordAddForm).FormParent.ThisFormKeywords[ImplementationIndexFromTheParent]);

            nameAndOutputForm.FormClosing += new FormClosingEventHandler(UpdateAfterClosingNameAndOutputForm);
            nameAndOutputForm.ShowKeywordContent();
        }

        private void UpdateAfterClosingNameAndOutputForm(object sender, EventArgs e)
        {
            if (NameAndOutputToTestCaseFormCommunication.Save)
                AddKeywordToThisKeyword(realSender, e);
        }

        // Adds TextBox / Label / Add implementation / Add and remove keyword / Params
        protected void AddKeywordField(Keyword keyword, int keywordsCounter)
        {
            int settingsLabel = 0;
            if (FormType == FormType.Settings)
                settingsLabel = KeywordFieldConsts.SettingsLabelWidth - KeywordFieldConsts.LabelWidth;

            FormControls.AddControl("TextWithList", "DynamicStep" + keywordsCounter + "Name",
                keywordsCounter,
                new Point(settingsLabel + KeywordFieldConsts.NameX - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                new Size(KeywordFieldConsts.NameWidth, KeywordFieldConsts.FieldsHeight),
                keyword.Name.Trim(),
                Color.Black,
                null,
                this);
            if ((Controls.Find("DynamicStep" + keywordsCounter + "Name", false).Length > 0))
                (Controls["DynamicStep" + keywordsCounter + "Name"] as TextWithList).MaxItemsInSuggestionsList = 15;

            FormControls.AddControl("Label", "DynamicStep" + keywordsCounter + "Label",
                keywordsCounter,
                new Point(KeywordFieldConsts.LabelX - HorizontalScroll.Value, initialYValue + 3 + (keywordsCounter - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                new Size(settingsLabel + KeywordFieldConsts.LabelWidth, KeywordFieldConsts.FieldsHeight),
                (FormType == FormType.Settings) ? KeywordFieldConsts.LabelNames[keywordsCounter-1] : keywordsCounter + ".",
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
                    new Point(settingsLabel + KeywordFieldConsts.AddImplementationX - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                    new Size(KeywordFieldConsts.AddImplementationWidth, KeywordFieldConsts.FieldsHeight),
                    buttonImplementation,
                    Color.Black,
                    new EventHandler(InstantiateKeywordAddForm),
                    this);
            }

            FormControls.AddControl("Button", "DynamicStep" + keywordsCounter + "AddKeyword",
                keywordsCounter,
                new Point(settingsLabel + KeywordFieldConsts.AddKeywordX - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                new Size(KeywordFieldConsts.AddKeywordWidth, KeywordFieldConsts.FieldsHeight),
                "+",
                Color.Black,
                new EventHandler(InstantiateNameAndOutputForm),
                this);
            FormControls.AddControl("Button", "DynamicStep" + keywordsCounter + "RemoveKeyword",
                keywordsCounter,
                new Point(settingsLabel + KeywordFieldConsts.RemoveKeywordX - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                new Size(KeywordFieldConsts.RemoveKeywordWidth, KeywordFieldConsts.FieldsHeight),
                "-",
                Color.Black,
                new EventHandler(RemoveKeywordFromThisForm),
                this);

            if (keyword.Params != null && keyword.Params.Count != 0)
                FormControls.AddControl("Button", "DynamicStep" + keywordsCounter + "Params",
                    keywordsCounter,
                    new Point(settingsLabel + KeywordFieldConsts.ParamX - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                    new Size(KeywordFieldConsts.ParamWidth, KeywordFieldConsts.FieldsHeight),
                    "Params",
                    Color.Black,
                    new EventHandler(InstantiateParamsAddForm),
                    this);

            if (NameAndOutputToTestCaseFormCommunication.Value != null)
                (Controls["DynamicStep" + keywordsCounter + "Name"] as TextWithList).TriggerUpdate("", NameAndOutputToTestCaseFormCommunication.Value);
            else
                (Controls["DynamicStep" + keywordsCounter + "Name"] as TextWithList).TriggerUpdate("", keyword.ToString());
            (Controls["DynamicStep" + keywordsCounter + "Name"] as TextWithList).EnableKeywordFields();
        }

        private void UpdateThisFormAfterImlpementedChildKeyword(object sender, EventArgs e)
        {
            int settingsLabel = 0;
            if (FormType == FormType.Settings)
                settingsLabel = KeywordFieldConsts.SettingsLabelWidth - KeywordFieldConsts.LabelWidth;

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
                        FormControls.AddControl("Button", "DynamicStep" + IndexOfTheKeywordToBeImplemented + "Params",
                            IndexOfTheKeywordToBeImplemented,
                            new Point(settingsLabel + KeywordFieldConsts.ParamX - HorizontalScroll.Value, initialYValue + (IndexOfTheKeywordToBeImplemented - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                            new Size(KeywordFieldConsts.ParamWidth, KeywordFieldConsts.FieldsHeight),
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
                if (!(FormType == FormType.Settings))
                {
                    if (FormType == FormType.Keyword || FormType == FormType.Test)
                        UpdateOutputFileSuggestions(Controls["OutputFile"] as ComboBox, FormType);
                }
            }
        }

        private void CheckKeywordTypeAndEvaluateKeywordData(Keyword keyword, string name, string keywordType)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("CheckKeywordTypeAndReturnKeyword " + keyword.Name + " " + name);
            foreach (Lib lib in SuggestionsClass.Suggestions)
                if (lib.ToInclude)
                    foreach (Keyword SuggestedKeyword in lib.LibKeywords)
                    {
                        if (keywordType.StartsWith("["))
                            if (keyword.GetTypeBasedOnSuggestionStart(keywordType) != SuggestedKeyword.Type)
                            {
                                break;
                            }

                        if (SuggestedKeyword.Name.Trim().ToLower().Equals(name.ToLower()))
                        {
                            if (!keyword.Name.Trim().ToLower().Equals(name.Trim().ToLower())
                                || !keyword.Type.Equals(SuggestedKeyword.Type))
                                keyword.CopyKeyword(SuggestedKeyword);
                            else
                                if (!keyword.Implemented)
                                keyword.CopyKeyword(SuggestedKeyword);
                            return;
                        }
                    }

            if (keyword.Type != KeywordType.CUSTOM)
            {
                keyword.CopyKeyword(new Keyword(name, FilesAndFolderStructure.GetFolder(FilesAndFolderStructure.ConvertFormTypeToFolderType(FormType)) + "Auto.robot", keyword.Parent)); //CopyKeyword
                keyword.Type = KeywordType.CUSTOM;
                return;
            }

            foreach (Lib lib in SuggestionsClass.Suggestions)
                if (lib.ToInclude)
                    foreach (Keyword seleniumKeyword in lib.LibKeywords)
                        if (seleniumKeyword.Name.Trim().ToLower().Equals(keyword.Name.Trim().ToLower()))
                        {
                            keyword.CopyKeyword(new Keyword(name, FilesAndFolderStructure.GetFolder(FilesAndFolderStructure.ConvertFormTypeToFolderType(FormType)) + "Auto.robot", keyword.Parent)); //CopyKeyword
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
            string path;
            if (!FormType.Equals(FormType.Params))
                path = FilesAndFolderStructure.ConcatFileNameToFolder(Controls["OutputFile"].Text, FilesAndFolderStructure.ConvertFormTypeToFolderType(FormType));
            else
                path = FormParent.ThisFormKeywords[ImplementationIndexFromTheParent].OutputFilePath;

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
                    ThisFormKeywords.Add(new Keyword(Controls["DynamicStep" + i + "Name"].Text, path, FormParent.ThisFormKeywords[ImplementationIndexFromTheParent].Parent));
            }
            else
            {
                for (int i = ThisFormKeywords.Count + 1; i <= NumberOfKeywordsInThisForm; i++)
                    ThisFormKeywords.Add(new Keyword(Controls["DynamicStep" + i + "Name"].Text, path, FormParent.ThisFormKeywords[ImplementationIndexFromTheParent].Parent));
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
                for (int i = 1; i <= NumberOfKeywordsInThisForm; i++)
                {
                    Controls["DynamicStep" + i + "Name"].Text = ThisFormKeywords[i - 1].Name.Trim();
                }

                for (int i = 1; i <= NumberOfKeywordsInThisForm; i++)
                {
                    (Controls["DynamicStep" + i + "Name"] as TextWithList).TriggerUpdate(ThisFormKeywords[i - 1].Name, ThisFormKeywords[i - 1].ToString());
                    (Controls["DynamicStep" + i + "Name"] as TextWithList).EnableKeywordFields();
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

            string path;
            if (!FormType.Equals(FormType.Params))
                path = FilesAndFolderStructure.ConcatFileNameToFolder(Controls["OutputFile"].Text, FilesAndFolderStructure.ConvertFormTypeToFolderType(FormType));
            else
                path = FormParent.ThisFormKeywords[ImplementationIndexFromTheParent].OutputFilePath;
            
            if (FormParent.ThisFormKeywords != null)
                ThisFormKeywords.Add(new Keyword("New Keyword", path, FormParent.ThisFormKeywords[ImplementationIndexFromTheParent].Parent));
            else
                ThisFormKeywords.Add(new Keyword("New Keyword", path, null));

            for (int i = NumberOfKeywordsInThisForm; i > keywordIndex; i--)
                ThisFormKeywords[i].CopyKeyword(ThisFormKeywords[i - 1]);

            if (FormParent.ThisFormKeywords != null)
                ThisFormKeywords[keywordIndex] = new Keyword(NameAndOutputToTestCaseFormCommunication.Name, NameAndOutputToTestCaseFormCommunication.OutputFile, FormParent.ThisFormKeywords[ImplementationIndexFromTheParent].Parent);
            else
                ThisFormKeywords.Add(new Keyword(NameAndOutputToTestCaseFormCommunication.Name, NameAndOutputToTestCaseFormCommunication.OutputFile, null));

            NumberOfKeywordsInThisForm++;
            AddKeywordField(ThisFormKeywords[NumberOfKeywordsInThisForm - 1], NumberOfKeywordsInThisForm);

            for (int i = 1; i < NumberOfKeywordsInThisForm; i++)
                Controls["DynamicStep" + i + "Name"].Text = ThisFormKeywords[i - 1].Name.Trim();

            for (int i = 1; i <= NumberOfKeywordsInThisForm; i++)
            {
                (Controls["DynamicStep" + i + "Name"] as TextWithList).TriggerUpdate(ThisFormKeywords[i - 1].Name, ThisFormKeywords[i - 1].ToString());
                (Controls["DynamicStep" + i + "Name"] as TextWithList).EnableKeywordFields();
            }
        }

        internal static void UpdateOutputFileSuggestions(ComboBox comboBox, FormType formType)
        {
            FolderType folderType = FilesAndFolderStructure.ConvertFormTypeToFolderType(formType);
            if (RobotAutomationHelper.Log) Console.WriteLine("UpdateOutputFileSuggestions " + comboBox.Name);
            comboBox.Items.Clear();
            comboBox.AutoCompleteCustomSource.Clear();
            comboBox.Items.AddRange(FilesAndFolderStructure.GetShortSavedFiles(folderType).ToArray());
            comboBox.AutoCompleteCustomSource.AddRange(FilesAndFolderStructure.GetShortSavedFiles(folderType).ToArray());
            comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            if (comboBox.DropDownStyle != ComboBoxStyle.DropDownList)
                comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
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
            {
                Controls["DynamicStep" + keywordIndex + "AddImplementation"].Enabled = true;
                if (!IsNameValid(Controls["DynamicStep" + keywordIndex + "Name"].Text))
                    Controls["DynamicStep" + keywordIndex + "AddImplementation"].Enabled = false;
                else
                {
                    // starts with variable
                    string[] checkForVariables = Controls["DynamicStep" + keywordIndex + "Name"].Text.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (checkForVariables != null && checkForVariables.Length != 0)
                        if (StringAndListOperations.StartsWithVariable(checkForVariables[0]))
                            Controls["DynamicStep" + keywordIndex + "AddImplementation"].Enabled = false;

                    recursive = false;
                    if (IsRecursive(ThisFormKeywords[keywordIndex - 1], this))
                    {
                        Controls["DynamicStep" + keywordIndex + "AddImplementation"].Text = "Recursive";
                        Controls["DynamicStep" + keywordIndex + "AddImplementation"].Enabled = false;
                        ThisFormKeywords[keywordIndex - 1].Recursive = true;
                    }
                    else
                    {
                        ThisFormKeywords[keywordIndex - 1].Recursive = false;
                    }
                }
            }
            if (Controls.Find("DynamicStep" + keywordIndex + "AddKeyword", false).Length != 0)
                Controls["DynamicStep" + keywordIndex + "AddKeyword"].Enabled = true;
            if (Controls.Find("DynamicStep" + keywordIndex + "RemoveKeyword", false).Length != 0)
                Controls["DynamicStep" + keywordIndex + "RemoveKeyword"].Enabled = true;
            if (Controls.Find("DynamicStep" + keywordIndex + "Params", false).Length != 0)
                Controls["DynamicStep" + keywordIndex + "Params"].Enabled = true;
        }

        protected bool OutputFileCheck(string OutputFileText)
        {
            if (!OutputFileText.ToLower().EndsWith(".robot"))
                return false;
            else
                return true;
        }

        protected bool IsNameValid(string Name)
        {
            if (Name.Trim().Length == 0)
                return false;
            else
            {
                // the following block allows using variables at the beginning of the name without checking for 2 spaces
                string[] checkForVariables = Name.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (checkForVariables != null && checkForVariables.Length != 0)
                    if (StringAndListOperations.StartsWithVariable(checkForVariables[0]))
                        return true;

                for (int i = 0; i < Name.Trim().Length - 1; i++)
                    if (Name.Trim()[i].Equals(' '))
                        if (Name.Trim()[i + 1].Equals(' '))
                            return false;
                return true;
            }
        }

        private bool IsRecursive(Keyword keyword, BaseKeywordAddForm form)
        {
            if (form.FormParent != null && !form.FormParent.Name.Contains("RobotAutomationHelper"))
            {
                if (form.FormParent.FormType.Equals(FormType.Keyword))
                {
                    if ((form.FormParent as KeywordAddForm).ThisFormKeywords[(form.FormParent as KeywordAddForm).IndexOfTheKeywordToBeImplemented - 1].Name.ToLower().Equals(keyword.Name.Trim().ToLower()))
                    {
                        recursive = true;
                        return true;
                    }
                    else
                        IsRecursive(keyword, form.FormParent);
                }
                else
                {
                    if (form.FormParent.FormType.Equals(FormType.Test))
                    {
                        if ((form.FormParent as TestCaseAddForm).ThisFormKeywords[(form.FormParent as TestCaseAddForm).IndexOfTheKeywordToBeImplemented - 1].Name.ToLower().Equals(keyword.Name.Trim().ToLower()))
                        {
                            recursive = true;
                            return true;
                        }
                        else
                            IsRecursive(keyword, form.FormParent);
                    }
                    else
                    {
                        if (form.FormParent.FormType.Equals(FormType.Settings))
                        {
                            if ((form.FormParent as SettingsAddForm).ThisFormKeywords[(form.FormParent as SettingsAddForm).IndexOfTheKeywordToBeImplemented - 1].Name.ToLower().Equals(keyword.Name.Trim().ToLower()))
                            {
                                recursive = true;
                                return true;
                            }
                            else
                                IsRecursive(keyword, form.FormParent);
                        }
                    }
                }
            }
            return recursive;
        }
    }

    internal enum FormType
    {
        Keyword,
        Test,
        Settings,
        NameAndOutput,
        Params
    }
}
