using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RobotAutomationHelper.Scripts.CustomControls;
using RobotAutomationHelper.Scripts.Objects;
using RobotAutomationHelper.Scripts.Static;
using RobotAutomationHelper.Scripts.Static.Consts;

namespace RobotAutomationHelper.Forms
{
    internal class BaseKeywordAddForm : Form
    {
        private readonly BaseKeywordAddForm _formParent;
        // index and keywords of the parent
        protected int ImplementationIndexFromTheParent;
        internal int NumberOfKeywordsInThisForm { get; set; }
        //y value for dynamic buttons
        protected int InitialYValue;
        internal bool SkipForm = false;
        //index of the keyword to be implemented after Add/Edit Implementation
        protected int IndexOfTheKeywordToBeImplemented;
        //Keywords in the form
        protected List<Keyword> ThisFormKeywords { get; set; }

        //Present in memory and robot files
        protected string MemoryPath;
        internal FormType FormType { get; set; }

        internal BaseKeywordAddForm()
        {

        }

        internal BaseKeywordAddForm(BaseKeywordAddForm parentForm)
        {
            _formParent = parentForm;
        }

        // change the field when the keyword name is changed
        internal void UpdateTheKeywordOnNameChange(object sender, string textChangePassed, string keywordType)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine(@"ChangeTheKeywordFieldAfterSelection " + ((TextWithList) sender).Name + @" " + Name);

            if (sender is TextWithList textWithList)
            {
                var keywordIndex = int.Parse(textWithList.Name.Replace("Name", "").Replace("DynamicStep", ""));

                CheckKeywordTypeAndEvaluateKeywordData(ThisFormKeywords[keywordIndex - 1],
                    textChangePassed.Equals("") ? textWithList.Text : textChangePassed, keywordType);
            }

            UpdateTheKeywordFieldsBasedOnNewData(sender);
        }

        private void UpdateTheKeywordFieldsBasedOnNewData(object sender)
        {
            var settingsLabel = 0;
            if (FormType == FormType.Settings)
                settingsLabel = KeywordFieldConsts.SettingsLabelWidth - KeywordFieldConsts.LabelWidth;

            if (RobotAutomationHelper.Log) Console.WriteLine(@"UpdateKeywordInThisKeyword " + ((TextWithList)sender).Name + @" " + Name);
            var keywordIndex = int.Parse(((TextWithList)sender).Name.Replace("DynamicStep", "").Replace("Name", "")); 
            if (!FormType.Equals(FormType.NameAndOutput))
            {
                if (ThisFormKeywords[keywordIndex - 1].Type.Equals(KeywordType.Custom))
                {
                    var buttonImplementation = "Add Implementation";
                    if (ThisFormKeywords[keywordIndex - 1].Implemented)
                        buttonImplementation = "Edit Implementation";

                    if (RobotAutomationHelper.Log) Console.WriteLine(@"length: " + Controls.Find("DynamicStep" + keywordIndex + "AddImplementation", false).Length);
                    if (Controls.Find("DynamicStep" + keywordIndex + "AddImplementation", false).Length == 0)
                        FormControls.AddControl("Button", "DynamicStep" + keywordIndex + "AddImplementation",
                        keywordIndex,
                        new Point(settingsLabel + KeywordFieldConsts.AddImplementationX - HorizontalScroll.Value, InitialYValue + (keywordIndex - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                        new Size(KeywordFieldConsts.AddImplementationWidth, KeywordFieldConsts.FieldsHeight),
                        buttonImplementation,
                        Color.Black,
                        InstantiateKeywordAddForm,
                        this);
                    else
                        ((Button) Controls.Find("DynamicStep" + keywordIndex + "AddImplementation", false)[0]).Text = buttonImplementation;
                }
                else
                    FormControls.RemoveControlByKey("DynamicStep" + keywordIndex + "AddImplementation", Controls);

                if (ThisFormKeywords[keywordIndex - 1].Type.Equals(KeywordType.Custom))
                {
                    if (ThisFormKeywords[keywordIndex - 1].Params != null
                        && ThisFormKeywords[keywordIndex - 1].Params.Count != 0)
                    {
                        if (Controls.Find("DynamicStep" + keywordIndex + "Params", false).Length == 0)
                        {
                            FormControls.AddControl("Button", "DynamicStep" + keywordIndex + "Params",
                                keywordIndex,
                                new Point(settingsLabel + KeywordFieldConsts.ParamX - HorizontalScroll.Value, InitialYValue + (keywordIndex - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                                new Size(KeywordFieldConsts.ParamWidth, KeywordFieldConsts.FieldsHeight),
                                "Params",
                                Color.Black,
                                InstantiateParamsAddForm,
                                this);
                        }
                    }
                    else
                        FormControls.RemoveControlByKey("DynamicStep" + keywordIndex + "Params", Controls);
                }
                else
                {
                    if (ThisFormKeywords[keywordIndex - 1].Params != null
                        && ThisFormKeywords[keywordIndex - 1].Params.Count != 0)
                    {
                        if (Controls.Find("DynamicStep" + keywordIndex + "Params", false).Length == 0)
                            FormControls.AddControl("Button", "DynamicStep" + keywordIndex + "Params",
                                keywordIndex,
                                new Point(settingsLabel + KeywordFieldConsts.ParamX - HorizontalScroll.Value, InitialYValue + (keywordIndex - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                                new Size(KeywordFieldConsts.ParamWidth, KeywordFieldConsts.FieldsHeight),
                                "Params",
                                Color.Black,
                                InstantiateParamsAddForm,
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
            if (RobotAutomationHelper.Log) Console.WriteLine(@"InstantiateKeywordAddForm " + ((Button)sender).Name);
            var keywordIndex = int.Parse(((Button)sender).Name.Replace("AddImplementation", "").Replace("DynamicStep", ""));
            IndexOfTheKeywordToBeImplemented = keywordIndex;
            var keyword = ThisFormKeywords[keywordIndex - 1];
            keyword.Implemented = true;
            keyword.Name = Controls["DynamicStep" + keywordIndex + "Name"].Text;
            var addKeywordForm = new KeywordAddForm(ThisFormKeywords, this);
            addKeywordForm.FormClosing += UpdateThisFormAfterImplementedChildKeyword;
            addKeywordForm.ShowKeywordContent(keyword, keywordIndex - 1);
        }

        protected void InstantiateSettingsAddForm(object sender, EventArgs e)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine(@"InstantiateSettingsAddForm " + ((Button)sender).Name);
            var addSettingsForm = new SettingsAddForm(this);
            //AddSettingsForm.FormClosing += new FormClosingEventHandler(UpdateThisFormAfterImplementedChildKeyword);
            addSettingsForm.ShowSettingsContent();
        }

        protected void InstantiateLibrariesAddForm(object sender, EventArgs e)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine(@"InstantiateLibrariesAddForm " + ((Button)sender).Name);
            var librariesForm = new LibrariesForm();
            librariesForm.ShowLibrariesContent();
        }

        internal void InstantiateParamsAddForm(object sender, EventArgs e)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine(@"InstantiateParamsAddForm " + ((Button)sender).Name);

            AddCurrentKeywordsToKeywordsList(sender, e);

            var keywordIndex = int.Parse(((Button)sender).Name.Replace("Params", "").Replace("DynamicStep", ""));
            // instantiate the new KeywordAddForm with this parent and Keywords argument
            var addParamForm = new ParamAddForm(this);
            ThisFormKeywords[keywordIndex - 1].Name = Controls["DynamicStep" + keywordIndex + "Name"].Text;
            // add closing event
            // addParamForm.FormClosing += new FormClosingEventHandler(UpdateThisFormAfterImplementedChildKeyword);
            addParamForm.ShowParamContent(ThisFormKeywords[keywordIndex - 1]);
        }

        private object _realSender;

        internal void InstantiateNameAndOutputForm(object sender, EventArgs e)
        {
            _realSender = sender;
            if (RobotAutomationHelper.Log) Console.WriteLine(@"InstantiateParamsAddForm " + ((Button)sender).Name);
            var formType = Name.Contains("Robot Automation Helper") ? FormType.Test : FormType.Keyword;

            var nameAndOutputForm = ToString().Contains("TestCaseAddForm") ? new NameAndOutputForm(formType, this, null) : new NameAndOutputForm(formType, this, ((KeywordAddForm) this)._formParent.ThisFormKeywords[ImplementationIndexFromTheParent]);

            nameAndOutputForm.FormClosing += UpdateAfterClosingNameAndOutputForm;
            nameAndOutputForm.ShowKeywordContent();
        }

        private void UpdateAfterClosingNameAndOutputForm(object sender, EventArgs e)
        {
            if (NameAndOutputToTestCaseFormCommunication.Save)
                AddKeywordToThisKeyword(_realSender, e);
        }

        // Adds TextBox / Label / Add implementation / Add and remove keyword / Params
        protected void AddKeywordField(Keyword keyword, int keywordsCounter, bool newKeyword)
        {
            var settingsLabel = 0;
            if (FormType == FormType.Settings)
                settingsLabel = KeywordFieldConsts.SettingsLabelWidth - KeywordFieldConsts.LabelWidth;

            FormControls.AddControl("TextWithList", "DynamicStep" + keywordsCounter + "Name",
                keywordsCounter,
                new Point(settingsLabel + KeywordFieldConsts.NameX - HorizontalScroll.Value, InitialYValue + (keywordsCounter - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                new Size(KeywordFieldConsts.NameWidth, KeywordFieldConsts.FieldsHeight),
                keyword.Name.Trim(),
                Color.Black,
                null,
                this);
            if ((Controls.Find("DynamicStep" + keywordsCounter + "Name", false).Length > 0))
                ((TextWithList) Controls["DynamicStep" + keywordsCounter + "Name"]).MaxItemsInSuggestionsList = 15;

            FormControls.AddControl("Label", "DynamicStep" + keywordsCounter + "Label",
                keywordsCounter,
                new Point(KeywordFieldConsts.LabelX - HorizontalScroll.Value, InitialYValue + 3 + (keywordsCounter - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                new Size(settingsLabel + KeywordFieldConsts.LabelWidth, KeywordFieldConsts.FieldsHeight),
                (FormType == FormType.Settings) ? KeywordFieldConsts.LabelNames[keywordsCounter-1] : keywordsCounter + ".",
                Color.Black,
                null,
                this);

            if (keyword.Type.Equals(KeywordType.Custom))
            {
                var buttonImplementation = "Add Implementation";
                if (ThisFormKeywords[keywordsCounter - 1].Implemented)
                    buttonImplementation = "Edit Implementation";
                FormControls.AddControl("Button", "DynamicStep" + keywordsCounter + "AddImplementation",
                    keywordsCounter,
                    new Point(settingsLabel + KeywordFieldConsts.AddImplementationX - HorizontalScroll.Value, InitialYValue + (keywordsCounter - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                    new Size(KeywordFieldConsts.AddImplementationWidth, KeywordFieldConsts.FieldsHeight),
                    buttonImplementation,
                    Color.Black,
                    InstantiateKeywordAddForm,
                    this);
            }

            FormControls.AddControl("Button", "DynamicStep" + keywordsCounter + "AddKeyword",
                keywordsCounter,
                new Point(settingsLabel + KeywordFieldConsts.AddKeywordX - HorizontalScroll.Value, InitialYValue + (keywordsCounter - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                new Size(KeywordFieldConsts.AddKeywordWidth, KeywordFieldConsts.FieldsHeight),
                "+",
                Color.Black,
                InstantiateNameAndOutputForm,
                this);
            FormControls.AddControl("Button", "DynamicStep" + keywordsCounter + "RemoveKeyword",
                keywordsCounter,
                new Point(settingsLabel + KeywordFieldConsts.RemoveKeywordX - HorizontalScroll.Value, InitialYValue + (keywordsCounter - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                new Size(KeywordFieldConsts.RemoveKeywordWidth, KeywordFieldConsts.FieldsHeight),
                "-",
                Color.Black,
                RemoveKeywordFromThisForm,
                this);

            if (keyword.Params != null && keyword.Params.Count != 0)
                FormControls.AddControl("Button", "DynamicStep" + keywordsCounter + "Params",
                    keywordsCounter,
                    new Point(settingsLabel + KeywordFieldConsts.ParamX - HorizontalScroll.Value, InitialYValue + (keywordsCounter - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                    new Size(KeywordFieldConsts.ParamWidth, KeywordFieldConsts.FieldsHeight),
                    "Params",
                    Color.Black,
                    InstantiateParamsAddForm,
                    this);

                if (newKeyword && NameAndOutputToTestCaseFormCommunication.Value != null)
                    ((TextWithList) Controls["DynamicStep" + keywordsCounter + "Name"]).TriggerUpdate("", NameAndOutputToTestCaseFormCommunication.Value);
                else
                    if (newKeyword)
                    ((TextWithList) Controls["DynamicStep" + keywordsCounter + "Name"]).TriggerUpdate("", keyword.KeywordString);
            ((TextWithList) Controls["DynamicStep" + keywordsCounter + "Name"]).EnableKeywordFields();
        }

        private void UpdateThisFormAfterImplementedChildKeyword(object sender, EventArgs e)
        {
            var settingsLabel = 0;
            if (FormType == FormType.Settings)
                settingsLabel = KeywordFieldConsts.SettingsLabelWidth - KeywordFieldConsts.LabelWidth;

            if ((((BaseKeywordAddForm) sender).FormType != FormType.Keyword) ||
                ((BaseKeywordAddForm) sender).SkipForm) return;
            Controls["DynamicStep" + IndexOfTheKeywordToBeImplemented + "Name"].Text = ThisFormKeywords[IndexOfTheKeywordToBeImplemented - 1].Name.Trim();
            Controls["DynamicStep" + IndexOfTheKeywordToBeImplemented + "AddImplementation"].Text = @"Edit implementation";

            //List<string> args = StringAndListOperations.ReturnListOfArgs(Keywords[IndexOfTheKeywordToBeImplemented - 1].Arguments);
            if (ThisFormKeywords[IndexOfTheKeywordToBeImplemented - 1].Params != null &&
                ThisFormKeywords[IndexOfTheKeywordToBeImplemented - 1].Params.Count != 0)
            {
                if (Controls.Find("DynamicStep" + IndexOfTheKeywordToBeImplemented + "Params", false).Length == 0)
                {
                    FormControls.AddControl("Button", "DynamicStep" + IndexOfTheKeywordToBeImplemented + "Params",
                        IndexOfTheKeywordToBeImplemented,
                        new Point(settingsLabel + KeywordFieldConsts.ParamX - HorizontalScroll.Value, InitialYValue + (IndexOfTheKeywordToBeImplemented - 1) * KeywordFieldConsts.VerticalDistanceBetweenKeywords - VerticalScroll.Value),
                        new Size(KeywordFieldConsts.ParamWidth, KeywordFieldConsts.FieldsHeight),
                        "Params",
                        Color.Black,
                        InstantiateParamsAddForm,
                        this);
                }
            }
            else
            if (Controls.Find("DynamicStep" + IndexOfTheKeywordToBeImplemented + "Params", false).Length != 0)
                FormControls.RemoveControlByKey("DynamicStep" + IndexOfTheKeywordToBeImplemented + "Params", Controls);

            //Adds file path + name to the Files And Folder structure for use in the drop down lists when choosing output file
            FilesAndFolderStructure.AddImplementedKeywordFilesToSavedFiles(ThisFormKeywords, IndexOfTheKeywordToBeImplemented);
            switch (FormType)
            {
                //update suggestion when not navigating to "Settings" form
                case FormType.Settings:
                    return;
                case FormType.Keyword:
                case FormType.Test:
                    UpdateOutputFileSuggestions(Controls["OutputFile"] as ComboBox, FormType);
                    break;
                case FormType.NameAndOutput:
                    break;
                case FormType.Params:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CheckKeywordTypeAndEvaluateKeywordData(Keyword keyword, string name, string keywordType)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine(@"CheckKeywordTypeAndReturnKeyword " + keyword.Name + @" " + name);
            foreach (var lib in SuggestionsClass.Suggestions)
                if (lib.ToInclude)
                    foreach (var suggestedKeyword in lib.LibKeywords)
                    {
                        if (keywordType != null && keywordType.StartsWith("["))
                            if (keyword.GetTypeBasedOnSuggestionStart(keywordType) != suggestedKeyword.Type)
                            {
                                break;
                            }

                        if (!suggestedKeyword.Name.Trim().ToLower().Equals(name.ToLower())) continue;
                        if (!keyword.Name.Trim().ToLower().Equals(name.Trim().ToLower())
                            || !keyword.Type.Equals(suggestedKeyword.Type))
                            keyword.CopyKeyword(suggestedKeyword);
                        else
                        if (!keyword.Implemented)
                            keyword.CopyKeyword(suggestedKeyword);
                        return;
                    }

            if (keyword.Type != KeywordType.Custom)
            {
                keyword.CopyKeyword(new Keyword(name, FilesAndFolderStructure.GetFolder(FilesAndFolderStructure.ConvertFormTypeToFolderType(FormType)) + "Auto.robot", keyword.Parent)); //CopyKeyword
                keyword.Type = KeywordType.Custom;
                return;
            }

            foreach (var lib in SuggestionsClass.Suggestions)
                if (lib.ToInclude)
                    foreach (var seleniumKeyword in lib.LibKeywords)
                        if (seleniumKeyword.Name.Trim().ToLower().Equals(keyword.Name.Trim().ToLower()))
                        {
                            keyword.CopyKeyword(new Keyword(name, FilesAndFolderStructure.GetFolder(FilesAndFolderStructure.ConvertFormTypeToFolderType(FormType)) + "Auto.robot", keyword.Parent)); //CopyKeyword
                            keyword.Type = KeywordType.Custom;
                            return;
                        }

            keyword.Name = name;
            keyword.Type = KeywordType.Custom;
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
            var path = !FormType.Equals(FormType.Params) ? FilesAndFolderStructure.ConcatFileNameToFolder(Controls["OutputFile"].Text, FilesAndFolderStructure.ConvertFormTypeToFolderType(FormType)) : _formParent.ThisFormKeywords[ImplementationIndexFromTheParent].OutputFilePath;

            // if AddImplementation is pressed a new form should be opened which requires the keyword that it represents
            var keywordIndex = 0;
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
                for (var i = 1; i <= NumberOfKeywordsInThisForm; i++)
                    ThisFormKeywords.Add(new Keyword(Controls["DynamicStep" + i + "Name"].Text, path, _formParent.ThisFormKeywords[ImplementationIndexFromTheParent].Parent));
            }
            else
            {
                for (var i = ThisFormKeywords.Count + 1; i <= NumberOfKeywordsInThisForm; i++)
                    ThisFormKeywords.Add(new Keyword(Controls["DynamicStep" + i + "Name"].Text, path, _formParent.ThisFormKeywords[ImplementationIndexFromTheParent].Parent));
            }

            AssignThisKeywordNamesFromTextFields();

            return ThisFormKeywords[keywordIndex - 1];
        }

        protected void AssignThisKeywordNamesFromTextFields()
        {
            for (var i = 1; i <= NumberOfKeywordsInThisForm; i++)
                if (Controls.Find("DynamicStep" + i + "Name", false).Length != 0)
                    ThisFormKeywords[i - 1].Name = Controls["DynamicStep" + i + "Name"].Text;
        }

        internal void RemoveKeywordFromThisForm(object sender, EventArgs e)
        {
            AssignThisKeywordNamesFromTextFields();

            if (NumberOfKeywordsInThisForm <= 1) return;
            var keywordIndex = int.Parse(((Button)sender).Name.Replace("DynamicStep", "").Replace("RemoveKeyword", ""));
            RemoveKeywordField(NumberOfKeywordsInThisForm, false);
            ThisFormKeywords.RemoveAt(keywordIndex - 1);
            NumberOfKeywordsInThisForm--;
            for (var i = 1; i <= NumberOfKeywordsInThisForm; i++)
            {
                Controls["DynamicStep" + i + "Name"].Text = ThisFormKeywords[i - 1].Name.Trim();
            }

            for (var i = 1; i <= NumberOfKeywordsInThisForm; i++)
            {
                ((TextWithList) Controls["DynamicStep" + i + "Name"]).TriggerUpdate(ThisFormKeywords[i - 1].Name, ThisFormKeywords[i - 1].ToString());
                ((TextWithList) Controls["DynamicStep" + i + "Name"]).EnableKeywordFields();
            }
        }

        protected void AddKeywordToThisKeyword(object sender, EventArgs e)
        {
            var keywordIndex = int.Parse(((Button)sender).Name.Replace("DynamicStep", "").Replace("AddKeyword", ""));

            AssignThisKeywordNamesFromTextFields();

            if (ThisFormKeywords == null)
            {
                var checkNull = AddCurrentKeywordsToKeywordsList(sender, e);
                if (checkNull == null) ThisFormKeywords = new List<Keyword>();
            }

            var path = !FormType.Equals(FormType.Params) ? FilesAndFolderStructure.ConcatFileNameToFolder(Controls["OutputFile"].Text, FilesAndFolderStructure.ConvertFormTypeToFolderType(FormType)) : _formParent.ThisFormKeywords[ImplementationIndexFromTheParent].OutputFilePath;

            ThisFormKeywords.Add(_formParent.ThisFormKeywords != null
                ? new Keyword("New Keyword", path,
                    _formParent.ThisFormKeywords[ImplementationIndexFromTheParent].Parent)
                : new Keyword("New Keyword", path, null));

            for (var i = NumberOfKeywordsInThisForm; i > keywordIndex; i--)
                ThisFormKeywords[i].CopyKeyword(ThisFormKeywords[i - 1]);

            if (_formParent.ThisFormKeywords != null)
                ThisFormKeywords[keywordIndex] = new Keyword(NameAndOutputToTestCaseFormCommunication.Name, NameAndOutputToTestCaseFormCommunication.OutputFile, _formParent.ThisFormKeywords[ImplementationIndexFromTheParent].Parent);
            else
                ThisFormKeywords[keywordIndex] = new Keyword(NameAndOutputToTestCaseFormCommunication.Name, NameAndOutputToTestCaseFormCommunication.OutputFile, null);

            NumberOfKeywordsInThisForm++;
            AddKeywordField(ThisFormKeywords[NumberOfKeywordsInThisForm - 1], NumberOfKeywordsInThisForm, false);

            for (var i = 1; i < NumberOfKeywordsInThisForm; i++)
                Controls["DynamicStep" + i + "Name"].Text = ThisFormKeywords[i - 1].Name.Trim();

            for (var i = 1; i <= NumberOfKeywordsInThisForm; i++)
            {
                ((TextWithList) Controls["DynamicStep" + i + "Name"]).TriggerUpdate(ThisFormKeywords[i - 1].Name, ThisFormKeywords[i - 1].ToString());
                ((TextWithList) Controls["DynamicStep" + i + "Name"]).EnableKeywordFields();
            }
        }

        internal static void UpdateOutputFileSuggestions(ComboBox comboBox, FormType formType)
        {
            var folderType = FilesAndFolderStructure.ConvertFormTypeToFolderType(formType);
            if (RobotAutomationHelper.Log) Console.WriteLine(@"UpdateOutputFileSuggestions " + comboBox.Name);
            comboBox.Items.Clear();
            comboBox.AutoCompleteCustomSource.Clear();
            // ReSharper disable once CoVariantArrayConversion
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
                    var checkForVariables = Controls["DynamicStep" + keywordIndex + "Name"].Text.Trim().Split(new [] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (checkForVariables.Length != 0)
                        if (StringAndListOperations.StartsWithVariable(checkForVariables[0]))
                            Controls["DynamicStep" + keywordIndex + "AddImplementation"].Enabled = false;

                    if (ThisFormKeywords[keywordIndex - 1].IsRecursive(ThisFormKeywords[keywordIndex - 1]))
                    {
                        Controls["DynamicStep" + keywordIndex + "AddImplementation"].Text = @"Recursive";
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

        protected bool OutputFileCheck(string outputFileText)
        {
            return outputFileText.ToLower().EndsWith(".robot");
        }

        protected bool IsNameValid(string name)
        {
            if (name.Trim().Length == 0)
                return false;
            // the following block allows using variables at the beginning of the name without checking for 2 spaces
            var checkForVariables = name.Trim().Split(new [] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (checkForVariables.Length != 0)
                if (StringAndListOperations.StartsWithVariable(checkForVariables[0]))
                    return true;

            for (var i = 0; i < name.Trim().Length - 1; i++)
                if (name.Trim()[i].Equals(' '))
                    if (name.Trim()[i + 1].Equals(' '))
                        return false;
            return true;
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
