using RobotAutomationHelper.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts
{
    internal class BaseKeywordAddForm : Form
    {
        Form Parent;
        protected bool IsKeyword;
        // index and keywords of the parent
        protected int ImplementationIndexFromTheParent;
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
                        FormControls.AddControl("Button", "DynamicStep" + keywordIndex + "Params",
                        new Point(500 - HorizontalScroll.Value, initialYValue + (keywordIndex - 1) * 30 - VerticalScroll.Value),
                        new Size(75, 20),
                        "Params",
                        Color.Black,
                        new EventHandler(InstantiateParamsAddForm),
                        this);
                }else
                    FormControls.RemoveControlByKey("DynamicStep" + keywordIndex + "Params", Controls);
            }
            else
            {
                if (!(ThisFormKeywords[keywordIndex - 1].Params == null)
                    && !(ThisFormKeywords[keywordIndex - 1].Params.Count == 0))
                {
                    if (Controls.Find("DynamicStep" + keywordIndex + "Params", false).Length == 0)
                        FormControls.AddControl("Button", "DynamicStep" + keywordIndex + "Params",
                            new Point(500 - HorizontalScroll.Value, initialYValue + (keywordIndex - 1) * 30 - VerticalScroll.Value),
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
            Parent = this;
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
            Parent = this;
            if (RobotAutomationHelper.Log) Console.WriteLine("InstantiateSettingsAddForm " + ((Button)sender).Name);
            SettingsAddForm AddSettingsForm = new SettingsAddForm();
            //AddSettingsForm.FormClosing += new FormClosingEventHandler(UpdateThisFormAfterImlpementedChildKeyword);
            AddSettingsForm.ShowSettingsContent();
        }

        internal void InstantiateParamsAddForm(object sender, EventArgs e)
        {
            Parent = this;
            if (RobotAutomationHelper.Log) Console.WriteLine("InstantiateParamsAddForm " + ((Button)sender).Name);
            if (IsKeyword)
                (this as KeywordAddForm).AddCurrentKeywordsToKeywordsList(sender, e);

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
                new Point(30 - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - VerticalScroll.Value),
                new Size(280, 20),
                keyword.Name.Trim(),
                Color.Black,
                null,
                this);

            FormControls.AddControl("Label", "DynamicStep" + keywordsCounter + "Label",
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
                    new Point(320 - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - VerticalScroll.Value),
                    new Size(120, 20),
                    buttonImplementation,
                    Color.Black,
                    new EventHandler(InstantiateKeywordAddForm),
                    this);
            }
            if (IsKeyword)
            {
                FormControls.AddControl("Button", "DynamicStep" + keywordsCounter + "AddKeyword",
                    new Point(450 - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - VerticalScroll.Value),
                    new Size(20, 20),
                    "+",
                    Color.Black,
                    new EventHandler((this as KeywordAddForm).AddKeywordToThisKeyword),
                    this);
                FormControls.AddControl("Button", "DynamicStep" + keywordsCounter + "RemoveKeyword",
                    new Point(470 - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - VerticalScroll.Value),
                    new Size(20, 20),
                    "-",
                    Color.Black,
                    new EventHandler((this as KeywordAddForm).RemoveKeywordFromThisKeyword),
                    this);
            }

            int ParamsButtonX = 450;
            if (IsKeyword) ParamsButtonX = 500;

            if (keyword.Params != null && keyword.Params.Count != 0)
                FormControls.AddControl("Button", "DynamicStep" + keywordsCounter + "Params",
                    new Point(ParamsButtonX - HorizontalScroll.Value, initialYValue + (keywordsCounter - 1) * 30 - VerticalScroll.Value),
                    new Size(75, 20),
                    "Params",
                    Color.Black,
                    new EventHandler(InstantiateParamsAddForm),
                    this);
        }

        private void UpdateThisFormAfterImlpementedChildKeyword(object sender, EventArgs e)
        {
            if (sender.GetType().FullName.Contains("KeywordAddForm") && !((KeywordAddForm)sender).skip)
            {
                Controls["DynamicStep" + IndexOfTheKeywordToBeImplemented + "Name"].Text = ThisFormKeywords[IndexOfTheKeywordToBeImplemented - 1].Name.Trim();
                Controls["DynamicStep" + IndexOfTheKeywordToBeImplemented + "AddImplementation"].Text = "Edit implementation";

                //List<string> args = StringAndListOperations.ReturnListOfArgs(Keywords[IndexOfTheKeywordToBeImplemented - 1].Arguments);

                if (ThisFormKeywords[IndexOfTheKeywordToBeImplemented - 1].Params != null &&
                    ThisFormKeywords[IndexOfTheKeywordToBeImplemented - 1].Params.Count != 0)
                    FormControls.AddControl("Button", "DynamicStep" + IndexOfTheKeywordToBeImplemented + "Params",
                        new Point(500 - HorizontalScroll.Value, initialYValue + (IndexOfTheKeywordToBeImplemented - 1) * 30 - VerticalScroll.Value),
                        new Size(75, 20),
                        "Params",
                        Color.Black,
                        new EventHandler(InstantiateParamsAddForm),
                        this);
                else
                    if (Controls.Find("DynamicStep" + IndexOfTheKeywordToBeImplemented + "Params", false).Length != 0)
                    FormControls.RemoveControlByKey("DynamicStep" + IndexOfTheKeywordToBeImplemented + "Params", Controls);

                //Adds file path + name to the Files And Folder structure for use in the drop down lists when chosing output file
                FilesAndFolderStructure.AddImplementedKeywordFilesToSavedFiles(ThisFormKeywords, IndexOfTheKeywordToBeImplemented);
                //update suggestion when not navigating to "Settings" form
                if (!Parent.Name.Contains("Settings"))
                    FormControls.UpdateOutputFileSuggestions(Controls["OutputFile"] as ComboBox);
            }
        }

        private void CheckKeywordTypeAndEvaluateKeywordData(Keyword keyword, string name)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("CheckKeywordTypeAndReturnKeyword " + keyword.Name + " " + name);
            foreach (Keyword seleniumKeyword in FormControls.Suggestions)
                if (seleniumKeyword.Name.Trim().ToLower().Equals(name.ToLower()))
                {
                    keyword.CopyKeyword(seleniumKeyword); //CopyKeyword
                    return;
                }

            if (keyword.Type != KeywordType.CUSTOM)
            {
                keyword.CopyKeyword(new Keyword(name, FilesAndFolderStructure.GetFolder() + "Auto.robot")); //CopyKeyword
                keyword.Type = KeywordType.CUSTOM;
                return;
            }

            foreach (Keyword seleniumKeyword in FormControls.Suggestions)
                if (seleniumKeyword.Name.Trim().ToLower().Equals(keyword.Name.Trim().ToLower()))
                {
                    keyword.CopyKeyword(new Keyword(name, FilesAndFolderStructure.GetFolder() + "Auto.robot")); //CopyKeyword
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
    }
}
