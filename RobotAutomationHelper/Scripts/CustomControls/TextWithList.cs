using RobotAutomationHelper.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts
{
    internal class TextWithList : TextBox
    {

        // Fields and Properties ===================================================
        private SuggestionsList SuggestionsList;
        private BaseKeywordAddForm ParentForm;
        private readonly int IndexOf;
        private bool JustGotFocused = false;
        private bool ChangedImmediatelyAfterSelection = false;
        internal int MaxItemsInSuggestionsList { get; set; }
        private bool updateNeeded = false;
        internal string updateValue = "";

        // Methods =================================================================
        internal TextWithList(BaseKeywordAddForm Parent, int IndexOf)
        {
            ParentForm = Parent;
            SuggestionsList = new SuggestionsList(this);
            this.IndexOf = IndexOf;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            UpdateListNamesAndUpdateStateOfSave();
            updateNeeded = true;
            if (Focused || SuggestionsList.SelectionPerformed)
            {
                base.OnTextChanged(e);
                string txt = Text;

                // if an item is selected from the list, then TriggerUpdate
                if (SuggestionsList.SelectionPerformed)
                {
                    string realName = ((SuggestionsListObjects)SuggestionsList.Items[SuggestionsList.SelectedIndex]).ValueMember;
                    SuggestionsList.SelectionPerformed = false;
                    ChangedImmediatelyAfterSelection = true;
                    updateNeeded = false;
                    //Console.WriteLine("OnTextChanged.SelectionPerformed Trigger Update: " + ((SuggestionsListObjects)SuggestionsList.Items[SuggestionsList.SelectedIndex]).Text);
                    updateValue = ((SuggestionsListObjects)SuggestionsList.Items[SuggestionsList.SelectedIndex]).Text;
                    TriggerUpdate(realName, ((SuggestionsListObjects)SuggestionsList.Items[SuggestionsList.SelectedIndex]).Text);
                    HideSuggestionsList();
                    EnableKeywordFields();
                }
                else
                {
                    if (!JustGotFocused)
                        DisableKeywordFields();
                    else
                        JustGotFocused = false;

                    if (ChangedImmediatelyAfterSelection)
                    {
                        ChangedImmediatelyAfterSelection = false;
                    }
                    else
                    {
                        ShowSuggestions(txt);
                    }
                }
            }
        }

        //on Enter triggers update and hides suggestions
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Enter && e.KeyCode == Keys.Return)
            {
                HideSuggestionsList();
                //Console.WriteLine("OnKeyDown Trigger Update " + updateNeeded.ToString() + " / " + updateValue);
                if (updateNeeded)
                    TriggerUpdate("", "");
                else
                    TriggerUpdate("", updateValue);
                EnableKeywordFields();
                SelectionStart = Text.Length;
            }
            else
            {
                if (e.KeyCode == Keys.Down)
                {
                    ShowSuggestions(Text);
                    SuggestionsList.SelectedIndex = 0;
                    Parent.Controls["SuggestionsList"].Focus();
                }
                else
                {
                    if (e.KeyCode == Keys.Up)
                    {
                        ShowSuggestions(Text);
                        SuggestionsList.SelectedIndex = SuggestionsList.Items.Count - 1;
                        Parent.Controls["SuggestionsList"].Focus();
                    }
                }
            }
        }
        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            if (Parent.Controls.Find("SuggestionsList", false).Length > 0)
            {
                if (!Parent.Controls["SuggestionsList"].Focused)
                {
                    HideSuggestionsList();
                    //Console.WriteLine("OnLeave Trigger Update " + updateNeeded.ToString() + " / " + updateValue);
                    if (updateNeeded)
                        TriggerUpdate("", "");
                    else
                        TriggerUpdate("", updateValue);
                    EnableKeywordFields();
                }
            }
            else
            {
                HideSuggestionsList();
                //Console.WriteLine("OnLeave Trigger Update " + updateNeeded.ToString() + " / " + updateValue);
                if (updateNeeded)
                    TriggerUpdate("", "");
                else
                    TriggerUpdate("", updateValue);
                EnableKeywordFields();
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            JustGotFocused = true;
            OnTextChanged(e);
            updateNeeded = false;
        }

        internal void TriggerUpdate(string textChangedPassed, string keywordType)
        {
            if (ParentForm.FormType == FormType.Keyword)
                (ParentForm as KeywordAddForm).UpdateTheKeywordOnNameChange(this, textChangedPassed, keywordType);
            else
            {
                if (ParentForm.FormType == FormType.Settings)
                {
                    (ParentForm as SettingsAddForm).UpdateTheKeywordOnNameChange(this, textChangedPassed, keywordType);
                }
                else
                {
                    if (ParentForm.FormType == FormType.Params)
                    {
                        (ParentForm as ParamAddForm).UpdateTheKeywordOnNameChange(this, textChangedPassed, keywordType);
                    }
                    else
                    {
                        if (ParentForm.FormType == FormType.Test)
                        {
                            (ParentForm as TestCaseAddForm).UpdateTheKeywordOnNameChange(this, textChangedPassed, keywordType);
                        }
                        else
                        {
                            if (ParentForm.FormType == FormType.NameAndOutput)
                            {
                                (ParentForm as NameAndOutputForm).UpdateTheKeywordOnNameChange(this, textChangedPassed, keywordType);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateListNamesAndUpdateStateOfSave()
        {
            if (ParentForm.FormType == FormType.Keyword)
                (ParentForm as KeywordAddForm).UpdateNamesListAndUpdateStateOfSave();
            else
            {
                if (ParentForm.FormType == FormType.Settings)
                {
                    (ParentForm as SettingsAddForm).UpdateNamesListAndUpdateStateOfSave();
                }
                else
                {
                    if (ParentForm.FormType == FormType.Params)
                    {
                        (ParentForm as ParamAddForm).UpdateListNamesAndUpdateStateOfSave();
                    }
                    else
                    {
                        if (ParentForm.FormType == FormType.Test)
                        {
                            (ParentForm as TestCaseAddForm).UpdateListNamesAndUpdateStateOfSave();
                        }
                        else
                        {
                            if (ParentForm.FormType == FormType.NameAndOutput)
                            {
                                //(ParentForm as NameAndOutputForm).UpdateTheKeywordOnNameChange(this, textChangedPassed);
                            }
                        }
                    }
                }
            }
        }

        private List<SuggestionsListObjects> ReturnSuggestionsMatches(string txt)
        {
            List<SuggestionsListObjects> foundItems = new List<SuggestionsListObjects>();
            foreach (Lib lib in SuggestionsClass.Suggestions)
                if (lib.ToInclude)
                    foreach (Keyword keyword in lib.LibKeywords)
                    //if (!keyword.Name.ToLower().Trim().Equals(txt.ToLower().Trim()))
                    {
                        bool containsAll = true;
                        foreach (string temp in txt.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (RobotAutomationHelper.Log) Console.WriteLine(keyword.ToString());
                            if (!keyword.Name.ToLower().Contains(temp))
                            {
                                containsAll = false;
                                break;
                            }
                        }
                        if (containsAll)
                        {
                            if (keyword.Type != KeywordType.CUSTOM)
                                foundItems.Add(new SuggestionsListObjects { Text = keyword.ToString(), ValueMember = keyword.Name, Documentation = keyword.Documentation });
                            else
                                foundItems.Add(new SuggestionsListObjects { Text = keyword.ToString().Trim(), ValueMember = keyword.Name.Trim(), Documentation = keyword.OutputFilePath + "\n" + keyword.Documentation.Trim() });
                        }
                    }
                    return foundItems;
        }

        private void ShowSuggestions(string textInTheField)
        {
            // find all the items in suggestion that match the current text
            List<SuggestionsListObjects> foundItems = ReturnSuggestionsMatches(textInTheField);

            if (foundItems.Count > 0)
            {
                //show suggestions list
                SuggestionsList.Items.Clear();
                SuggestionsList.Items.AddRange(foundItems.ToArray());
                SuggestionsList.Visible = true;
                SuggestionsList.Location = new Point(Location.X, Location.Y + Height);
                SuggestionsList.Size = new Size(Size.Width, foundItems.Count >= MaxItemsInSuggestionsList ? (MaxItemsInSuggestionsList + 1) * SuggestionsList.ItemHeight : (foundItems.Count + 1) * SuggestionsList.ItemHeight);
                SuggestionsList.IntegralHeight = true;
                FormControls.RemoveControlByKey(SuggestionsList.Name, ParentForm.Controls);
                ParentForm.Controls.Add(SuggestionsList);
                SuggestionsList.BringToFront();
            }
            else
            {
                //hide suggestions list
                HideSuggestionsList();
            }
        }
        internal void HideSuggestionsList()
        {
            SuggestionsList.Visible = false;
            SuggestionsList.HideToolTip();
            ParentForm.Controls.Remove(SuggestionsList);
        }

        internal void DisableKeywordFields()
        {
            if (ParentForm.FormType == FormType.Keyword)
                (ParentForm as KeywordAddForm).DisableKeywordFields(IndexOf);
            else
            {
                if (ParentForm.FormType == FormType.Params)
                {
                    (ParentForm as ParamAddForm).DisableKeywordFields(IndexOf);
                }
                else
                {
                    if (ParentForm.FormType == FormType.Settings)
                    {
                        (ParentForm as SettingsAddForm).DisableKeywordFields(IndexOf);
                    }
                    else
                    if (ParentForm.FormType == FormType.Test)
                        (ParentForm as TestCaseAddForm).DisableKeywordFields(IndexOf);
                }
            }
        }
        internal void EnableKeywordFields()
        {
            if (ParentForm.FormType == FormType.Keyword)
                (ParentForm as KeywordAddForm).EnableKeywordFields(IndexOf);
            else
            {
                if (ParentForm.FormType == FormType.Params)
                {
                    (ParentForm as ParamAddForm).EnableKeywordFields(IndexOf);
                }
                else
                {
                    if (ParentForm.FormType == FormType.Settings)
                    {
                        (ParentForm as SettingsAddForm).EnableKeywordFields(IndexOf);
                    }
                    else
                    if (ParentForm.FormType == FormType.Test)
                        (ParentForm as TestCaseAddForm).EnableKeywordFields(IndexOf);
                }
            }
        }
    }
}
